using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Счётчик замера среднего числа значений
    /// </summary>
    public abstract class AverageCountCounter : Counter
    {
        /// <summary>
        /// Конструктор AverageCountCounter
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="description">Описание</param>
        protected AverageCountCounter(string name, string description)
            : base(name, description, CounterTypes.AverageCount)
        {
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public abstract double CurrentValue { get; }

        /// <summary>
        /// Зарегистрировать измерение
        /// </summary>
        /// <param name="value">Новое значение</param>
        public abstract void RegisterValue(long value);

        /// <summary>
        /// Сброс значений счётчика
        /// </summary>
        public override void Reset()
        {
        }


        /// <summary>
        /// Преобразование в строку с информацией
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[Value: {0}, Name: {1}, CounterType: {2}]", CurrentValue, Name, Type);
        }
    }
}
