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
    /// Счётчик числа операций в секунду для InternalCounters
    /// </summary>
    public sealed class InternalOperationsPerSecondCounter : OperationsPerSecondCounter
    {
        /// <summary>
        /// Дескриптор для счётчика InternalOperationsPerSecondCounter
        /// </summary>
        private class Descriptor : InternalCounterDescriptor
        {
            /// <summary>
            /// Конструктор Descriptor
            /// </summary>
            /// <param name="name">Имя счётчика</param>
            /// <param name="description">Описание счётчика</param>
            public Descriptor(string name, string description)
                : base(name, description, CounterTypes.OperationsPerSecond)
            {
            }

            /// <summary>
            /// Метод создания счётчика из дескриптора
            /// </summary>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter()
            {
                return new InternalOperationsPerSecondCounter(this.Name, this.Description);
            }
        }

        /// <summary>
        /// Метод создания дескриптора для InternalOperationsPerSecondCounter
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
        /// Период аккумулирования для определения среднего числа операций в секунду
        /// </summary>
        private const int MeasurePeriodMs = 4000;
        /// <summary>
        /// Степень сокращения измерений при достижении MeasurePeriodMs.
        /// Аккумулированное значение делятся на это значение, время отсчёта также сдвигается пропорционально.
        /// Обеспечивает постепенное затухание старых измерений
        /// </summary>
        private const int ReductionFactor = 8;
        /// <summary>
        /// Множитель числа операций в секунду для уменьшения ошибок округления.
        /// Должен быть кратен 2 и >= 2
        /// </summary>
        private const int OperationCountMultiplier = 64;

        private volatile uint _fastTimeStamp;
        private long _preciseTimeStamp;
        private long _accumulatedValue;

        private int _syncFlag;


        /// <summary>
        /// Конструктор InternalOperationsPerSecondCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        public InternalOperationsPerSecondCounter(string name, string description)
            : base(name, description)
        {
            _syncFlag = 0;
            _accumulatedValue = 0;
            _preciseTimeStamp = GetPreciseTimestamp();
            _fastTimeStamp = GetFastTimestamp();
        }

        // Работа со временем

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint GetFastTimestamp()
        {
            return (uint)Environment.TickCount;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetFastElapsedMilliseconds()
        {
            var result = GetFastTimestamp() - _fastTimeStamp;
            if (result > int.MaxValue)
                return int.MaxValue;
            return (int)result;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long GetPreciseTimestamp()
        {
            return Stopwatch.GetTimestamp();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double RawPreciseElapsedToDoubleMs(long rawElapsed)
        {
            return (rawElapsed * 1000.0) / (double)Stopwatch.Frequency;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long RawPreciseElapsedToLongMs(long rawElapsed)
        {
            return (rawElapsed * 1000) / Stopwatch.Frequency;
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
        /// Получить текущие значения числа операций и времени
        /// </summary>
        /// <param name="curVal">Число операций</param>
        /// <param name="timeStamp">Срез времени</param>
        /// <returns>Удалось ли прочитать атомарно</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetValues(out long curVal, out long timeStamp)
        {
            curVal = Interlocked.Read(ref _accumulatedValue);
            timeStamp = Interlocked.Read(ref _preciseTimeStamp);
            return curVal == Interlocked.Read(ref _accumulatedValue) && timeStamp == Interlocked.Read(ref _preciseTimeStamp);
        }
        /// <summary>
        /// Получить атомарно значения числа операций и времени
        /// </summary>
        /// <param name="curVal">Число операций</param>
        /// <param name="timeStamp">Срез времени</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetValuesAtomic(out long curVal, out long timeStamp)
        {
            bool isOk = false;
            do
            {
                isOk = TryGetValues(out curVal, out timeStamp);
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
                long curVal = 0;
                long preciseTimeStamp = 0;
                GetValuesAtomic(out curVal, out preciseTimeStamp);
                var doubleElapsed = RawPreciseElapsedToDoubleMs(GetPreciseTimestamp() - preciseTimeStamp);

                if (doubleElapsed > 2 * MeasurePeriodMs || doubleElapsed < 0)
                    return 0;

                return ((double)(curVal * 1000)) / doubleElapsed / OperationCountMultiplier;
            }
        }



        /// <summary>
        /// Внутренний метод учёта операций
        /// </summary>
        /// <param name="opCnt">Число завершённых операций</param>
        private void OpertionFinishedInternal(int opCnt)
        {
            bool wasProcessedInLock = false;

            if (GetFastElapsedMilliseconds() > MeasurePeriodMs)
            {
                try { }
                finally
                {
                    if (TryTakeLock())
                    {
                        if (GetFastElapsedMilliseconds() > MeasurePeriodMs)
                        {
                            _fastTimeStamp = GetFastTimestamp();
                            wasProcessedInLock = true;
                            Interlocked.Add(ref _accumulatedValue, opCnt * OperationCountMultiplier / 2);

                            long currentValue = Interlocked.Read(ref _accumulatedValue);
                            long currentPreciseTimestamp = GetPreciseTimestamp();
                            long rawElapsed = currentPreciseTimestamp - Interlocked.Read(ref _preciseTimeStamp);

                            double expectedUpdate = ((double)currentValue * (ReductionFactor - 1)) / ReductionFactor;
                            long realUpdate = (long)expectedUpdate;
                            double updateError = realUpdate / expectedUpdate;
                            long correctedRawElapsed = (long)(rawElapsed * updateError);

                            Interlocked.Add(ref _accumulatedValue, -realUpdate + opCnt * OperationCountMultiplier / 2);
                            Interlocked.Exchange(ref _preciseTimeStamp, currentPreciseTimestamp - correctedRawElapsed / ReductionFactor);
                        }

                        ReleaseLock();
                    }
                }
            }

            if (!wasProcessedInLock)
                Interlocked.Add(ref _accumulatedValue, opCnt * OperationCountMultiplier);
        }

        /// <summary>
        /// Закончилась 1 операция
        /// </summary>
        public override void OperationFinished()
        {
            OpertionFinishedInternal(1);
        }

        /// <summary>
        /// Закончилось несколько операций
        /// </summary>
        /// <param name="operationCount">Число операций</param>
        public override void OperationFinished(int operationCount)
        {
            OpertionFinishedInternal(operationCount);
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
                Interlocked.Exchange(ref _preciseTimeStamp, GetPreciseTimestamp());
                _fastTimeStamp = GetFastTimestamp();

                ReleaseLock();
            }
        } 
    }
}
