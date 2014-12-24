using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CompositeCounters.Counters
{
    /// <summary>
    /// Счётчик замера среднего числа значений для CompositeCounters
    /// </summary>
    public sealed class CompositeAverageCountCounter : AverageCountCounter
    {
        /// <summary>
        /// Дескриптор для счётчика CompositeAverageCountCounter
        /// </summary>
        private class Descriptor : CompositeCounterDescriptor
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
            /// <param name="counters">Оборачиваемые счётчики</param>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter(IEnumerable<Counter> counters)
            {
                return new CompositeAverageCountCounter(this.Name, this.Description, counters.Cast<AverageCountCounter>());
            }
        }

        /// <summary>
        /// Метод создания дескриптора для CompositeAverageCountCounter
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
        private readonly AverageCountCounter[] _wrappedCounters;

        /// <summary>
        /// Конструктор CompositeAverageCountCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="counters">Оборачиваемые счётчики</param>
        public CompositeAverageCountCounter(string name, string description, IEnumerable<AverageCountCounter> counters)
            : base(name, description)
        {
            if (counters == null)
                throw new ArgumentNullException("counters");

            _wrappedCounters = counters.ToArray();

            if (_wrappedCounters.Length == 0)
                _wrappedCounters = new AverageCountCounter[] { new NullCounters.Counters.NullAverageCountCounter(name, description) };
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public override double CurrentValue
        {
            get { return _wrappedCounters[0].CurrentValue; }
        }


        /// <summary>
        /// Зарегистрировать измерение
        /// </summary>
        /// <param name="value">Новое значение</param>
        public override void RegisterValue(long value)
        {
            for (int i = 0; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].RegisterValue(value);
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
