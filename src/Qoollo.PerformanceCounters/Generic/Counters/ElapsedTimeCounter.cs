using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Счётчик, который показывает полное время от начала работы компонента или процесса
    /// </summary>
    public abstract class ElapsedTimeCounter: Counter
    {
        /// <summary>
        /// Конструктор ElapsedTimeCounter
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="description">Описание</param>
        protected ElapsedTimeCounter(string name, string description)
            : base(name, description, CounterTypes.ElapsedTime)
        {
        }

        /// <summary>
        /// Текущее время, которое прошло с момента запуска
        /// </summary>
        public abstract TimeSpan CurrentValue { get; }


        /// <summary>
        /// Преобразование в строку с информацией
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[Time: {0}, Name: {1}, CounterType: {2}]", CurrentValue, Name, Type);
        }
    }
}
