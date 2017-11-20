using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Конвертер типов счётчиков (Reflection Type в CounterTypes)
    /// </summary>
    internal static class CounterTypeConverter
    {
        /// <summary>
        /// Получить перечисление CounterTypes по Reflection Type счётчика
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <returns>CounterTypes</returns>
        public static CounterTypes Convert(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type == typeof(NumberOfItemsCounter))
                return CounterTypes.NumberOfItems;

            if (type == typeof(DeltaCounter))
                return CounterTypes.Delta;

            if (type == typeof(AverageTimeCounter))
                return CounterTypes.AverageTime;

            if (type == typeof(AverageCountCounter))
                return CounterTypes.AverageCount;

            if (type == typeof(OperationsPerSecondCounter))
                return CounterTypes.OperationsPerSecond;

            if (type == typeof(ElapsedTimeCounter))
                return CounterTypes.ElapsedTime;

            if (type == typeof(MomentTimeCounter))
                return CounterTypes.MomentTime;

            throw new UnknownCounterTypeException("Unknown counter type: " + type.ToString());
        }
    }
}
