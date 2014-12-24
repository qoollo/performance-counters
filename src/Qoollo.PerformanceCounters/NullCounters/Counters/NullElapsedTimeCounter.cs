using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters.Counters
{
    /// <summary>
    /// Счётчик, который показывает полное время от начала работы компонента или процесса, для NullCounters
    /// </summary>
    public sealed class NullElapsedTimeCounter: ElapsedTimeCounter
    {
        /// <summary>
        /// Конструктор NullElapsedTimeCounter
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="description">Описание</param>
        public NullElapsedTimeCounter(string name, string description)
            : base(name, description)
        {
        }

        /// <summary>
        /// Текущее время, которое прошло с момента запуска
        /// </summary>
        public override TimeSpan CurrentValue
        {
            get { return TimeSpan.Zero; }
        }

        /// <summary>
        /// Сбросить прошедшее время
        /// </summary>
        public override void Reset()
        {
        }
    }
}
