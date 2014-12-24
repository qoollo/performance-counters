using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CompositeCounters.Counters
{
    /// <summary>
    /// Счётчик среднего времени для CompositeCounters
    /// </summary> 
    public sealed class CompositeAverageTimeCounter : AverageTimeCounter
    {
        /// <summary>
        /// Дескриптор для счётчика CompositeAverageTimeCounter
        /// </summary>
        private class Descriptor : CompositeCounterDescriptor
        {
            /// <summary>
            /// Конструктор Descriptor
            /// </summary>
            /// <param name="name">Имя счётчика</param>
            /// <param name="description">Описание счётчика</param>
            public Descriptor(string name, string description)
                : base(name, description, CounterTypes.AverageTime)
            {
            }

            /// <summary>
            /// Метод создания счётчика из дескриптора
            /// </summary>
            /// <param name="counters">Оборачиваемые счётчики</param>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter(IEnumerable<Counter> counters)
            {
                return new CompositeAverageTimeCounter(this.Name, this.Description, counters.Cast<AverageTimeCounter>());
            }
        }

        /// <summary>
        /// Метод создания дескриптора для CompositeAverageTimeCounter
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
        private readonly AverageTimeCounter[] _wrappedCounters;


        /// <summary>
        /// Конструктор CompositeAverageTimeCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="counters">Оборачиваемые счётчики</param>
        public CompositeAverageTimeCounter(string name, string description, IEnumerable<AverageTimeCounter> counters)
            : base(name, description)
        {
            if (counters == null)
                throw new ArgumentNullException("counters");

            _wrappedCounters = counters.ToArray();

            if (_wrappedCounters.Length == 0)
                _wrappedCounters = new AverageTimeCounter[] { new NullCounters.Counters.NullAverageTimeCounter(name, description) };
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public override TimeSpan CurrentValue
        {
            get { return _wrappedCounters[0].CurrentValue; }
        }


        /// <summary>
        /// Зарегистрировать измерение
        /// </summary>
        /// <param name="period">Период</param>
        public override void RegisterMeasurement(TimeSpan period)
        {
            for (int i = 0; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].RegisterMeasurement(period);
        }


        /// <summary>
        /// Сброс значения счётчика
        /// </summary>
        public override void Reset()
        {
            for (int i = 0; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].Reset();
        }
    }
}
