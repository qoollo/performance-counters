using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.InternalCounters.Counters
{
    /// <summary>
    /// Internal Counter for the difference between the raw value at the beginning and the end of the measured time frame
    /// </summary>
    public sealed class InternalDeltaCounter : DeltaCounter
    {
        private int _syncFlag;
        private long _currentValue;

        /// <summary>
        /// Дескриптор для счётчика InternalNumberOfItemsCounter
        /// </summary>
        private class Descriptor : InternalCounterDescriptor
        {
            /// <summary>
            /// Конструктор Descriptor
            /// </summary>
            /// <param name="name">Имя счётчика</param>
            /// <param name="description">Описание счётчика</param>
            public Descriptor(string name, string description)
                : base(name, description, CounterTypes.Delta)
            {
            }

            /// <summary>
            /// Метод создания счётчика из дескриптора
            /// </summary>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter()
            {
                return new InternalNumberOfItemsCounter(this.Name, this.Description);
            }
        }

        /// <summary>
        /// Метод создания дескриптора для InternalNumberOfItemsCounter
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
        /// Конструктор InternalNumberOfItemsCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        public InternalDeltaCounter(string name, string description)
            : base(name, description)
        {
            _currentValue = 0;
        }

        /// <summary>
        /// Уменьшить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long Decrement()
        {
            return Interlocked.Decrement(ref _currentValue);
        }

        /// <summary>
        /// Уменьшить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое уменьшаем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long DecrementBy(long value)
        {
            return Interlocked.Add(ref _currentValue, -value);
        }

        /// <summary>
        /// Увеличить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long Increment()
        {
            return Interlocked.Increment(ref _currentValue);
        }

        /// <summary>
        /// Увеличить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое увеличиваем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long IncrementBy(long value)
        {
            return Interlocked.Add(ref _currentValue, value);
        }

        /// <summary>
        /// Задать значение
        /// </summary>
        /// <param name="value">Новое значение</param>
        public override void SetValue(long value)
        {
            Interlocked.Exchange(ref _currentValue, value);
        }

        /// <summary>
        /// Получает текущее значение счетчика производительности
        /// </summary>
        public override long CurrentValue => Interlocked.Read(ref _currentValue);

        /// <summary>
        /// Take next sample interval and return its value
        /// </summary>
        /// <returns>Difference between counter raw value during sample interval</returns>
        public override long Measure()
        {
            long currentValue = 0;
            try { }
            finally
            {
                var sw = new SpinWait();
                while (!TryTakeLock())
                    sw.SpinOnce();

                GetValueAtomic(out currentValue);
                Interlocked.Exchange(ref _currentValue, 0);

                ReleaseLock();
            }
            return currentValue;
        }

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
        /// Try atomically read _currentValue field
        /// </summary>
        /// <param name="curVal">current value</param>
        /// <returns>Success or failure</returns>
        private bool TryGetValue(out long curVal)
        {
            curVal = Interlocked.Read(ref _currentValue);
            return curVal == Interlocked.Read(ref _currentValue);
        }

        /// <summary>
        /// Get value atomically
        /// </summary>
        /// <param name="curVal">Current value</param>
        private void GetValueAtomic(out long curVal)
        {
            bool isOk = false;
            do
            {
                isOk = TryGetValue(out curVal);
            }
            while (!isOk);
        }
    }
}
