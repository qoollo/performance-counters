using Qoollo.PerformanceCounters.NullCounters.Counters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters.Categories
{
    /// <summary>
    /// Создание категории с одним инстансом для NullCounters
    /// </summary>
    public class NullSingleInstanceCategory : SingleInstanceCategory
    {
        /// <summary>
        /// Конструктор NullSingleInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        public NullSingleInstanceCategory(string name, string description)
            : base(name, description)
        {
        }

        /// <summary>
        /// Создать счётчик определённого типа
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <param name="counterName">Имя счётчика</param>
        /// <param name="counterDescription">Описание счётчика</param>
        /// <returns>Счётчик</returns>
        public override Counter CreateCounter(CounterTypes type, string counterName, string counterDescription)
        {
            if (counterName == null)
                throw new ArgumentNullException("counterName");
            if (counterDescription == null)
                throw new ArgumentNullException("counterDescription");

            switch (type)
            {
                case CounterTypes.NumberOfItems:
                    return new NullNumberOfItemsCounter(counterName, counterDescription);
                case CounterTypes.Delta:
                    return new NullDeltaCounter(counterName, counterDescription);
                case CounterTypes.AverageTime:
                    return new NullAverageTimeCounter(counterName, counterDescription);
                case CounterTypes.AverageCount:
                    return new NullAverageCountCounter(counterName, counterDescription);
                case CounterTypes.OperationsPerSecond:
                    return new NullOperationPerSecondCounter(counterName, counterDescription);
                case CounterTypes.ElapsedTime:
                    return new NullElapsedTimeCounter(counterName, counterDescription);
                case CounterTypes.MomentTime:
                    return new NullMomentTimeCounter(counterName, counterDescription);
                default:
                    throw new InvalidOperationException("Unknown CounterTypes value: " + type.ToString());
            }
        }

        /// <summary>
        /// Есть ли счётчик с указанным именем. Перманентно true.
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
            throw new NotSupportedException("GetCounter without 'expectedCounterType' is not supported for NullSingleInstanceCategory");
        }

        /// <summary>
        /// Получение счетчика определенного типа. Всегда создаёт новый.
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="expectedCounterType">Тип счётчика</param>
        /// <returns>Счётчик</returns>
        public override Counter GetCounter(string counterName, CounterTypes expectedCounterType)
        {
            return CreateCounter(expectedCounterType, counterName, counterName);
        }


        /// <summary>
        /// Создание пустой подкатегории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="categoryName">Имя подкатегории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override EmptyCategory CreateEmptySubCategory(string categoryName, string categoryDescription)
        {
            return new NullEmptyCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание подкатегории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override MultiInstanceCategory CreateMultiInstanceSubCategory(string categoryName, string categoryDescription)
        {
            return new NullMultiInstanceCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание простой подкатегории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override SingleInstanceCategory CreateSingleInstanceSubCategory(string categoryName, string categoryDescription)
        {
            return new NullSingleInstanceCategory(categoryName, categoryDescription);
        }
    }
}
