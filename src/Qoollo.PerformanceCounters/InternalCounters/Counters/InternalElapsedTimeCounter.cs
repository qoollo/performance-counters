using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.InternalCounters.Counters
{
    /// <summary>
    /// Счётчик, который показывает полное время от начала работы компонента или процесса, для InternalCounters
    /// </summary>
    public sealed class InternalElapsedTimeCounter : ElapsedTimeCounter
    {
        /// <summary>
        /// Дескриптор для счётчика InternalElapsedTimeCounter
        /// </summary>
        private class Descriptor : InternalCounterDescriptor
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
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter()
            {
                return new InternalElapsedTimeCounter(this.Name, this.Description);
            }
        }

        /// <summary>
        /// Метод создания дескриптора для InternalElapsedTimeCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        /// <returns>Дескриптор</returns>
        internal static InternalCounterDescriptor CreateDescriptor(string name, string description)
        {
            return new Descriptor(name, description);
        }

        // ===================

        private DateTime _startTime;

        /// <summary>
        /// Конструктор InternalElapsedTimeCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        public InternalElapsedTimeCounter(string name, string description)
            : base(name, description)
        {
            _startTime = DateTime.UtcNow;
        }


        /// <summary>
        /// Текущее время, которое прошло с момента запуска
        /// </summary>
        public override TimeSpan CurrentValue
        {
            get { return DateTime.UtcNow - _startTime; }
        }

        /// <summary>
        /// Сбросить прошедшее время
        /// </summary>
        public override void Reset()
        {
            _startTime = DateTime.UtcNow;
        }
    }
}
