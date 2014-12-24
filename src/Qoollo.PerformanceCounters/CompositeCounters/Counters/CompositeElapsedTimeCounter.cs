using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CompositeCounters.Counters
{
    /// <summary>
    /// Счётчик, который показывает полное время от начала работы компонента или процесса, для CompositeCounters
    /// </summary>
    public sealed class CompositeElapsedTimeCounter : ElapsedTimeCounter
    {
        /// <summary>
        /// Дескриптор для счётчика CompositeElapsedTimeCounter
        /// </summary>
        private class Descriptor : CompositeCounterDescriptor
        {
            /// <summary>
            /// Конструктор Descriptor
            /// </summary>
            /// <param name="name">Имя счётчика</param>
            /// <param name="description">Описание счётчика</param>
            public Descriptor(string name, string description)
                : base(name, description, CounterTypes.ElapsedTime)
            {
            }

            /// <summary>
            /// Метод создания счётчика из дескриптора
            /// </summary>
            /// <param name="counters">Оборачиваемые счётчики</param>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter(IEnumerable<Counter> counters)
            {
                return new CompositeElapsedTimeCounter(this.Name, this.Description, counters.Cast<ElapsedTimeCounter>());
            }
        }

        /// <summary>
        /// Метод создания дескриптора для CompositeElapsedTimeCounter
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
        private readonly ElapsedTimeCounter[] _wrappedCounters;

        /// <summary>
        /// Конструктор CompositeElapsedTimeCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <param name="counters">Оборачиваемые счётчики</param>
        public CompositeElapsedTimeCounter(string name, string description, IEnumerable<ElapsedTimeCounter> counters)
            : base(name, description)
        {
            if (counters == null)
                throw new ArgumentNullException("counters");

            _wrappedCounters = counters.ToArray();

            if (_wrappedCounters.Length == 0)
                _wrappedCounters = new ElapsedTimeCounter[] { new NullCounters.Counters.NullElapsedTimeCounter(name, description) };
        }


        /// <summary>
        /// Текущее время, которое прошло с момента запуска
        /// </summary>
        public override TimeSpan CurrentValue
        {
            get { return _wrappedCounters[0].CurrentValue; }
        }

        /// <summary>
        /// Сбросить прошедшее время
        /// </summary>
        public override void Reset()
        {
            for (int i = 0; i < _wrappedCounters.Length; i++)
                _wrappedCounters[i].Reset();
        }
    }
}
