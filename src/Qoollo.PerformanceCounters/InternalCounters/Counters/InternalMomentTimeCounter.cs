using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.InternalCounters.Counters
{
    /// <summary>
    /// Счётчик моментального времени для InternalCounters
    /// </summary> 
    public sealed class InternalMomentTimeCounter : MomentTimeCounter
    {
        /// <summary>
        /// Дескриптор для счётчика InternalMomentTimeCounter
        /// </summary>
        private class Descriptor : InternalCounterDescriptor
        {
            /// <summary>
            /// Конструктор Descriptor
            /// </summary>
            /// <param name="name">Имя счётчика</param>
            /// <param name="description">Описание счётчика</param>
            public Descriptor(string name, string description)
                : base(name, description, CounterTypes.MomentTime)
            {
            }

            /// <summary>
            /// Метод создания счётчика из дескриптора
            /// </summary>
            /// <returns>Созданный счётчик</returns>
            public override Counter CreateCounter()
            {
                return new InternalMomentTimeCounter(this.Name, this.Description);
            }
        }

        /// <summary>
        /// Метод создания дескриптора для InternalMomentTimeCounter
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
        /// Конструктор InternalMomentTimeCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        public InternalMomentTimeCounter(string name, string description)
            : base(name, description)
        {
            _value = 0;
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public override TimeSpan CurrentValue
        {
            get { return new TimeSpan(Interlocked.Read(ref _value)); }
        }

        /// <summary>
        /// Зарегистрировать измерение
        /// </summary>
        /// <param name="period">Период</param>
        public override void RegisterMeasurement(TimeSpan period)
        {
            Interlocked.Exchange(ref _value, period.Ticks);
        }


        /// <summary>
        /// Сброс значения счётчика
        /// </summary>
        public override void Reset()
        {
            Interlocked.Exchange(ref _value, 0);
        } 
    }
}
