using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CompositeCounters.Counters
{
    /// <summary>
    /// Счетчик мгновенного значения, показывающий последнее наблюдавшееся значение для CompositeCounters
    /// </summary>
    public sealed class CompositeDeltaCountCounter : DeltaCountCounter
    {
        /// <summary>
        /// Дескриптор для счётчика CompositeNumberOfItemsCounter
        /// </summary>
        private class Descriptor : CompositeCounterDescriptor
        {
            /// <summary>
            /// Конструктор Descriptor
            /// </summary>
            /// <param name="name">Имя счётчика</param>
            /// <param name="description">Описание счётчика</param>
            public Descriptor(string name, string description)
                : base(name, description, CounterTypes.DeltaCount)
            {
            }

            /// <summary>
            /// Метод создания счётчика из дескриптора
            /// </summary>
            /// <param name="counters">Оборачиваемые счётчики</param>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter(IEnumerable<Counter> counters)
            {
                return new CompositeDeltaCountCounter(this.Name, this.Description, counters.Cast<DeltaCountCounter>());
            }
        }

        /// <summary>
        /// Метод создания дескриптора для CompositeNumberOfItemsCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <returns>Дескриптор</returns>
        internal static CompositeCounterDescriptor CreateDescriptor(string name, string description)
        {
            return new Descriptor(name, description);
        }

        // ===================


        /// <summary>
        /// Оборачиваемые счётчики
        /// </summary>
        private readonly DeltaCountCounter[] _wrappedCounters;


        /// <summary>
        /// Конструктор CompositeNumberOfItemsCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="counters">Оборачиваемые счётчики</param>
        public CompositeDeltaCountCounter(string name, string description, IEnumerable<DeltaCountCounter> counters)
            : base(name, description)
        {
            if (counters == null)
                throw new ArgumentNullException("counters");

            _wrappedCounters = counters.ToArray();

            if (_wrappedCounters.Length == 0)
                _wrappedCounters = new DeltaCountCounter[] { new NullCounters.Counters.NullDeltaCountCounter(name, description),  };
        }

        /// <summary>
        /// Уменьшить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long Decrement()
        {
            long result = _wrappedCounters[0].Decrement();
            for (int i = 1; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].Decrement();
            return result;
        }

        /// <summary>
        /// Уменьшить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое уменьшаем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long DecrementBy(long value)
        {
            long result = _wrappedCounters[0].DecrementBy(value);
            for (int i = 1; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].DecrementBy(value);
            return result;
        }

        /// <summary>
        /// Увеличить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long Increment()
        {
            long result = _wrappedCounters[0].Increment();
            for (int i = 1; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].Increment();
            return result;
        }

        /// <summary>
        /// Увеличить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое увеличиваем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public override long IncrementBy(long value)
        {
            long result = _wrappedCounters[0].IncrementBy(value);
            for (int i = 1; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].IncrementBy(value);
            return result;
        }

        /// <summary>
        /// Задать значение
        /// </summary>
        /// <param name="value">Новое значение</param>
        public override void SetValue(long value)
        {
            for (int i = 0; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].SetValue(value);
        }

        /// <summary>
        /// Получает текущее значение счетчика производительности
        /// </summary>
        public override long CurrentValue
        {
            get { return _wrappedCounters[0].CurrentValue; }
        }
    }
}
