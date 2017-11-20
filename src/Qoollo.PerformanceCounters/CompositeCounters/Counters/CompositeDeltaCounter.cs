using System;
using System.Collections.Generic;
using System.Linq;

namespace Qoollo.PerformanceCounters.CompositeCounters.Counters
{
    /// <summary>
    /// Composite Counter for the difference between the raw value at the beginning and the end of the measured time frame (for CompositeCounters)
    /// </summary>
    public sealed class CompositeDeltaCounter : DeltaCounter
    {
        /// <summary>
        /// Descriptor of CompositeDeltaCountCounter
        /// </summary>
        private class Descriptor : CompositeCounterDescriptor
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
            /// <param name="counters">Оборачиваемые счётчики</param>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter(IEnumerable<Counter> counters)
            {
                return new CompositeDeltaCounter(this.Name, this.Description, counters.Cast<DeltaCounter>());
            }
        }

        /// <summary>
        /// Метод создания дескриптора для CompositeDeltaCounter
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
        private readonly DeltaCounter[] _wrappedCounters;


        /// <summary>
        /// Конструктор CompositeDeltaCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="counters">Оборачиваемые счётчики</param>
        public CompositeDeltaCounter(string name, string description, IEnumerable<DeltaCounter> counters)
            : base(name, description)
        {
            if (counters == null)
                throw new ArgumentNullException("counters");

            _wrappedCounters = counters.ToArray();

            if (_wrappedCounters.Length == 0)
                _wrappedCounters = new DeltaCounter[] { new NullCounters.Counters.NullDeltaCounter(name, description),  };
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

        public override long Measure()
        {
            long result = _wrappedCounters[0].Measure();
            for (int i = 1; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].Measure();
            return result;
        }
    }
}
