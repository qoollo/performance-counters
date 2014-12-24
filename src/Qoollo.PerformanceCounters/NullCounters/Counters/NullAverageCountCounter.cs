using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters.Counters
{
    /// <summary>
    /// Счётчик замера среднего числа значений для NullCounters
    /// </summary> 
    public sealed class NullAverageCountCounter: AverageCountCounter
    {
        /// <summary>
        /// Конструктор NullAverageCountCounter
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="description">Описание</param>
        public NullAverageCountCounter(string name, string description)
            : base(name, description)
        {
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public override double CurrentValue
        {
            get { return 0; }
        }

        /// <summary>
        /// Зарегистрировать измерение
        /// </summary>
        /// <param name="value">Новое значение</param>
        public override void RegisterValue(long value)
        {
        }
    }
}
