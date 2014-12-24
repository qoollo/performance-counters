using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters.Counters
{
    /// <summary>
    /// Счётчик среднего времени для NullCounters
    /// </summary>
    public sealed class NullAverageTimeCounter : AverageTimeCounter
    {
        /// <summary>
        /// Конструктор NullAverageTimeCounter
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="description">Описание</param>
        public NullAverageTimeCounter(string name, string description)
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
