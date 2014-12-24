using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters.Counters
{
    /// <summary>
    /// Счётчик числа операций в секунду для NullCounters
    /// </summary>
    public sealed class NullOperationPerSecondCounter: OperationsPerSecondCounter
    {
        /// <summary>
        /// Конструктор NullOperationPerSecondCounter
        /// </summary>
        /// <param name="name">Имя счётчика</param>
        /// <param name="description">Описание счётчика</param>
        public NullOperationPerSecondCounter(string name, string description)
            : base(name, description)
        {
        }

        /// <summary>
        /// Закончилась 1 операция
        /// </summary>
        public override void OperationFinished()
        {
        }

        /// <summary>
        /// Закончилось несколько операций
        /// </summary>
        /// <param name="operationCount">Число операций</param>
        public override void OperationFinished(int operationCount)
        {
        }

        /// <summary>
        /// Текущее значение
        /// </summary>
        public override double CurrentValue
        {
            get { return 0; }
        }
    }
}
