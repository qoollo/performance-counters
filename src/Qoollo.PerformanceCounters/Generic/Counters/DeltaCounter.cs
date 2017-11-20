using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Abstract Counter for the difference between the raw value at the beginning and the end of the measured time frame
    /// </summary>
    public abstract class DeltaCounter : Counter
    {
        /// <summary>
        /// Конструктор NumberOfItemsCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        protected DeltaCounter(string name, string description)
            : base(name, description, CounterTypes.Delta)
        {
        }

        /// <summary>
        /// Уменьшить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public abstract long Decrement();

        /// <summary>
        /// Уменьшить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое уменьшаем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public abstract long DecrementBy(long value);

        /// <summary>
        /// Увеличить значение на 1
        /// </summary>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public abstract long Increment();

        /// <summary>
        /// Увеличить значение на "value"
        /// </summary>
        /// <param name="value">Значение, на которое увеличиваем</param>
        /// <returns>Текущее значение или Counter.FailureNum</returns>
        public abstract long IncrementBy(long value);

        /// <summary>
        /// Задать значение
        /// </summary>
        /// <param name="value">Новое значение</param>
        public abstract void SetValue(long value);

        /// <summary>
        /// Получает текущее значение счетчика производительности
        /// </summary>
        public abstract long CurrentValue { get; }

        /// <summary>
        /// Сброс значения счётчика
        /// </summary>
        public override void Reset()
        {
            SetValue(0);
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
