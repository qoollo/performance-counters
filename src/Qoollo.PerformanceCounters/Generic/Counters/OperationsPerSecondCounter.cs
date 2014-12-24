using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Счётчик числа операций в секунду
    /// </summary>
    public abstract class OperationsPerSecondCounter: Counter
    {
        /// <summary>
        /// Конструктор OperationsPerSecondCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        protected OperationsPerSecondCounter(string name, string description)
            : base(name, description, CounterTypes.OperationsPerSecond)
        {
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public abstract double CurrentValue { get; }

        /// <summary>
        /// Закончилась 1 операция
        /// </summary>
        public abstract void OperationFinished();

        /// <summary>
        /// Закончилось несколько операций
        /// </summary>
        /// <param name="operationCount">Число операций</param>
        public abstract void OperationFinished(int operationCount);

        /// <summary>
        /// Сброс значения счётчика
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
