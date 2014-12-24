using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.InternalCounters.Counters
{
    /// <summary>
    /// Счётчик замера среднего числа значений для InternalCounters
    /// </summary>
    public sealed class InternalAverageCountCounter : AverageCountCounter
    {
        /// <summary>
        /// Дескриптор для счётчика InternalAverageCountCounter
        /// </summary>
        private class Descriptor : InternalCounterDescriptor
        {
            /// <summary>
            /// Конструктор Descriptor
            /// </summary>
            /// <param name="name">Имя счётчика</param>
            /// <param name="description">Описание счётчика</param>
            public Descriptor(string name, string description)
                : base(name, description, CounterTypes.AverageCount)
            {
            }

            /// <summary>
            /// Метод создания счётчика из дескриптора
            /// </summary>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter()
            {
                return new InternalAverageCountCounter(this.Name, this.Description);
            }
        }

        /// <summary>
        /// Метод создания дескриптора для InternalAverageCountCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <returns>Дескриптор</returns>
        internal static InternalCounterDescriptor CreateDescriptor(string name, string description)
        {
            return new Descriptor(name, description);
        }

        // ===================

        /// <summary>
        /// Время с последнего обновления, после истечения которого счётчик сбрасывается
        /// </summary>
        private const int MonitoringPeriodMs = 100000;
        /// <summary>
        /// Число аккумулируемых значений для определения среднего
        /// </summary>
        private const int AccumulatingSize = 256;
        /// <summary>
        /// Степень сокращения при достижении AccumulatingSize.
        /// Аккумулированное значение и число элементов делятся на это значение.
        /// Обеспечивает постепенное затухание старых измерений
        /// </summary>
        private const int ReductionFactor = 8;

        private volatile uint _timeStamp;
        private long _accumulatedValue;
        private int _count;

        private int _syncFlag;
     

        /// <summary>
        /// Конструктор InternalAverageCountCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        public InternalAverageCountCounter(string name, string description)
            : base(name, description)
        {
            _accumulatedValue = 0;
            _count = 0;
            _syncFlag = 0;
            _timeStamp = GetTimestamp();
        }


        // Работа со временем

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint GetTimestamp()
        {
            return (uint)Environment.TickCount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetElapsedMilliseconds()
        {
            var result = GetTimestamp() - _timeStamp;
            if (result > int.MaxValue)
                return int.MaxValue;
            return (int)result;
        }


        // Блокировки

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryTakeLock()
        {
            return Interlocked.CompareExchange(ref _syncFlag, 1, 0) == 0;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReleaseLock()
        {
            Interlocked.Exchange(ref _syncFlag, 0);
        }


        /// <summary>
        /// Попробовать атомарно считать аккумулированное значение и число срабатываний
        /// </summary>
        /// <param name="curVal">Аккумулированное значение</param>
        /// <param name="curCnt">Число срабатываний</param>
        /// <returns>Удалось ли их считать атомарно</returns>
        private bool TryGetValues(out long curVal, out int curCnt)
        {
            curVal = Interlocked.Read(ref _accumulatedValue);
            curCnt = Volatile.Read(ref _count);
            return curVal == Interlocked.Read(ref _accumulatedValue) && curCnt == Volatile.Read(ref _count);
        }

        /// <summary>
        /// Атомарно считать аккумулированное значение и число срабатываний
        /// </summary>
        /// <param name="curVal">Аккумулированное значение</param>
        /// <param name="curCnt">Число срабатываний</param>
        private void GetValuesAtomic(out long curVal, out int curCnt)
        {
            bool isOk = false;
            do
            {
                isOk = TryGetValues(out curVal, out curCnt);
            }
            while (!isOk);
        }


        /// <summary>
        /// Текущее значение
        /// </summary>
        public override double CurrentValue
        {
            get
            {
                long currentValue = 0;
                int currentCount = 0;
                GetValuesAtomic(out currentValue, out currentCount);

                if (currentCount == 0 || GetElapsedMilliseconds() > MonitoringPeriodMs)
                    return 0;

                return (double)currentValue / (double)currentCount;
            }
        }


        /// <summary>
        /// Зарегистрировать измерение
        /// </summary>
        /// <param name="value">Новое значение</param>
        public override void RegisterValue(long value)
        {
            var currentValue = Interlocked.Add(ref _accumulatedValue, value);
            var currentCount = Interlocked.Increment(ref _count);
            if ((currentCount & 3) == 0)
                _timeStamp = GetTimestamp();

            if (currentCount >= AccumulatingSize)
            {
                try { }
                finally
                {
                    if (TryTakeLock())
                    {
                        if (currentCount >= AccumulatingSize)
                        {
                            GetValuesAtomic(out currentValue, out currentCount);

                            Interlocked.Add(ref _accumulatedValue, (-currentValue * (ReductionFactor - 1)) / ReductionFactor);
                            Interlocked.Add(ref _count, (-currentCount * (ReductionFactor - 1)) / ReductionFactor);                           
                        }

                        ReleaseLock();
                    }
                }
            }
        }


        /// <summary>
        /// Сброс значения счётчика
        /// </summary>
        public override void Reset()
        {
            try { }
            finally
            {
                SpinWait sw = new SpinWait();
                while (!TryTakeLock())
                    sw.SpinOnce();

                Interlocked.Exchange(ref _accumulatedValue, 0);
                Interlocked.Exchange(ref _count, 0);
                _timeStamp = GetTimestamp();

                ReleaseLock();
            }
        }   
    }
}
