using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters.Counters
{
    /// <summary>
    /// Счётчик мгновенного времени для NullCounters
    /// </summary>
    public sealed class NullMomentTimeCounter: MomentTimeCounter
    {
        /// <summary>
        /// Конструктор NullMomentTimeCounter
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="description">Описание</param>
        public NullMomentTimeCounter(string name, string description)
            : base(name, description)
        {
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public override TimeSpan CurrentValue
        {
            get { return TimeSpan.Zero; }
        }

        /// <summary>
        /// Зарегистрировать измерение
        /// </summary>
        /// <param name="period">Период</param>
        public override void RegisterMeasurement(TimeSpan period)
        {
        }
    }
}
