using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.InternalCounters.Counters
{
    /// <summary>
    /// Счетчик мгновенного значения, показывающий последнее наблюдавшееся значение для InternalCounters
    /// </summary>
    public sealed class InternalNumberOfItemsCounter : NumberOfItemsCounter
    {
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
                : base(name, description, CounterTypes.NumberOfItems)
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

        private long _value;


        /// <summary>
        /// Конструктор InternalNumberOfItemsCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        public InternalNumberOfItemsCounter(string name, string description)
            : base(name, description)
        {
            _value = 0;
        }

        /// <summary>
        /// Уменьшить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long Decrement()
        {
            return Interlocked.Decrement(ref _value);
        }

        /// <summary>
        /// Уменьшить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое уменьшаем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long DecrementBy(long value)
        {
            return Interlocked.Add(ref _value, -value);
        }

        /// <summary>
        /// Увеличить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long Increment()
        {
            return Interlocked.Increment(ref _value);
        }

        /// <summary>
        /// Увеличить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое увеличиваем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long IncrementBy(long value)
        {
            return Interlocked.Add(ref _value, value);
        }

        /// <summary>
        /// Задать значение
        /// </summary>
        /// <param name="value">Новое значение</param>
        public override void SetValue(long value)
        {
            Interlocked.Exchange(ref _value, value);
        }

        /// <summary>
        /// Получает текущее значение счетчика производительности
        /// </summary>
        public override long CurrentValue
        {
            get { return Interlocked.Read(ref _value); }
        }
    }
}
