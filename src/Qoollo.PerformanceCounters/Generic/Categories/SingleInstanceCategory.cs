using System;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Создание категории с одним инстансом
    /// </summary>
    public abstract class SingleInstanceCategory : Category
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        protected SingleInstanceCategory(string categoryName, string categoryDescription)
            : base(categoryName, categoryDescription, CategoryTypes.SingleInstance)
        {
        }


        #region Работа со счетчиками

        /// <summary>
        /// Создать счётчик определённого типа
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <param name="counterName">Имя счётчика</param>
        /// <param name="counterDescription">Описание счётчика</param>
        /// <returns>Счётчик</returns>
        public abstract Counter CreateCounter(CounterTypes type, string counterName, string counterDescription);

        /// <summary>
        /// Создать счётчик определённого типа (NumberOfItemsCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual NumberOfItemsCounter CreateNumberOfItemsCounter(string counterName, string counterDescription)
        {
            return (NumberOfItemsCounter)CreateCounter(CounterTypes.NumberOfItems, counterName, counterDescription);
        }


        /// <summary>
        /// Создать счётчик определённого типа (AverageTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual AverageTimeCounter CreateAverageTimeCounter(string counterName, string counterDescription)
        {
            return (AverageTimeCounter)CreateCounter(CounterTypes.AverageTime, counterName, counterDescription);
        }

        /// <summary>
        /// Создать счётчик определённого типа (AverageCountCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual AverageCountCounter CreateAverageCountCounter(string counterName, string counterDescription)
        {
            return (AverageCountCounter)CreateCounter(CounterTypes.AverageCount, counterName, counterDescription);
        }

        /// <summary>
        /// Создать счётчик определённого типа (OperationsPerSecondCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual OperationsPerSecondCounter CreateOperationsPerSecondCounter(string counterName, string counterDescription)
        {
            return (OperationsPerSecondCounter)CreateCounter(CounterTypes.OperationsPerSecond, counterName, counterDescription);
        }

        /// <summary>
        /// Создать счётчик определённого типа (ElapsedTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual ElapsedTimeCounter CreateElapsedTimeCounter(string counterName, string counterDescription)
        {
            return (ElapsedTimeCounter)CreateCounter(CounterTypes.ElapsedTime, counterName, counterDescription);
        }


        /// <summary>
        /// Создать счётчик определённого типа (MomentTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual MomentTimeCounter CreateMomentTimeCounter(string counterName, string counterDescription)
        {
            return (MomentTimeCounter)CreateCounter(CounterTypes.MomentTime, counterName, counterDescription);
        }


        /// <summary>
        /// Есть ли счётчик с указанным именем
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Есть ли он</returns>
        public abstract bool HasCounter(string counterName);

        /// <summary>
        /// Получение счетчика определенного типа
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public abstract Counter GetCounter(string counterName);

        /// <summary>
        /// Получение счетчика определенного типа
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="expectedCounterType">Тип счётчика</param>
        /// <returns>Счётчик</returns>
        public abstract Counter GetCounter(string counterName, CounterTypes expectedCounterType);

        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (NumberOfItemsCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual NumberOfItemsCounter GetNumberOfItemsCounter(string counterName)
        {
            return (NumberOfItemsCounter)GetCounter(counterName, CounterTypes.NumberOfItems);
        }


        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (AverageTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual AverageTimeCounter GetAverageTimeCounter(string counterName)
        {
            return (AverageTimeCounter)GetCounter(counterName, CounterTypes.AverageTime);
        }


        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (AverageCountCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual AverageCountCounter GetAverageCountCounter(string counterName)
        {
            return (AverageCountCounter)GetCounter(counterName, CounterTypes.AverageCount);
        }

        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (OperationsPerSecondCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual OperationsPerSecondCounter GetOperationsPerSecondCounter(string counterName)
        {
            return (OperationsPerSecondCounter)GetCounter(counterName, CounterTypes.OperationsPerSecond);
        }


        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (ElapsedTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual ElapsedTimeCounter GetElapsedTimeCounter(string counterName)
        {
            return (ElapsedTimeCounter)GetCounter(counterName, CounterTypes.ElapsedTime);
        }

        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (MomentTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public virtual MomentTimeCounter GetMomentTimeCounter(string counterName)
        {
            return (MomentTimeCounter)GetCounter(counterName, CounterTypes.MomentTime);
        }


        #endregion

    }
}