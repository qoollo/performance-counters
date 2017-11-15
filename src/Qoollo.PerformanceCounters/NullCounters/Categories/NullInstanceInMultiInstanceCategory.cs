using Qoollo.PerformanceCounters.NullCounters.Counters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters.Categories
{
    /// <summary>
    /// Инстанс в многоинстансовой категории для NullCounters
    /// </summary> 
    public class NullInstanceInMultiInstanceCategory : InstanceInMultiInstanceCategory
    {
        /// <summary>
        /// Конструктор для создания инстанса NullInstanceInMultiInstanceCategory
        /// </summary>
        /// <param name="parent">Родительская многоинстовая категория</param>
        /// <param name="instanceName">Имя инстанса</param>
        public NullInstanceInMultiInstanceCategory(NullMultiInstanceCategory parent, string instanceName)
            : base(parent, instanceName)
        {
        }

        /// <summary>
        /// Есть ли счётчик с указанным именем (перманентно true)
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Есть ли он</returns>
        public override bool HasCounter(string counterName)
        {
            return true;
        }

        /// <summary>
        /// Получение счетчика по имени. Не поддерживается. Бросает исключение.
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        /// <exception cref="NotSupportedException">Всегда</exception>
        public override Counter GetCounter(string counterName)
        {
            throw new NotSupportedException("GetCounter without 'expectedCounterType' is not supported for NullInstanceInMultiInstanceCategory");
        }


        /// <summary>
        /// Получение счетчика определенного типа. Всегда создаёт новый.
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="expectedCounterType">Тип счётчика</param>
        /// <returns>Счётчик</returns>
        public override Counter GetCounter(string counterName, CounterTypes expectedCounterType)
        {
            if (counterName == null)
                throw new ArgumentNullException("counterName");

            switch (expectedCounterType)
            {
                case CounterTypes.NumberOfItems:
                    return new NullNumberOfItemsCounter(counterName, counterName);
                case CounterTypes.DeltaCount:
                    return new NullDeltaCountCounter(counterName, counterName);
                case CounterTypes.AverageTime:
                    return new NullAverageTimeCounter(counterName, counterName);
                case CounterTypes.AverageCount:
                    return new NullAverageCountCounter(counterName, counterName);
                case CounterTypes.OperationsPerSecond:
                    return new NullOperationPerSecondCounter(counterName, counterName);
                case CounterTypes.ElapsedTime:
                    return new NullElapsedTimeCounter(counterName, counterName);
                case CounterTypes.MomentTime:
                    return new NullMomentTimeCounter(counterName, counterName);
                default:
                    throw new InvalidOperationException("Unknown CounterTypes value: " + expectedCounterType.ToString());
            }
        }

        /// <summary>
        /// Удаление инстанса
        /// </summary>
        public override void Remove()
        {
        }

        /// <summary>
        /// Живой ли инстанс (не был ли удалён)
        /// </summary>
        public override bool IsAlive
        {
            get { return true; }
        }
    }
}
