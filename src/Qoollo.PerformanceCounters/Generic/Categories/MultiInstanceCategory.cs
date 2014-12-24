
namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Создание категории поддерживающей многочисленные инстансы
    /// </summary>
    public abstract class MultiInstanceCategory : Category
    {
        /// <summary>
        /// Конструктор категории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        protected MultiInstanceCategory(string categoryName, string categoryDescription)
            : base(categoryName, categoryDescription, CategoryTypes.MultiInstance)
        {
        }


        /// <summary>
        /// Получение или создание инстанса
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Инстанс</returns>
        public abstract InstanceInMultiInstanceCategory this[string instanceName] { get; }

        /// <summary>
        /// Существует ли инстанс счётчиков
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существует ли</returns>
        public abstract bool HasInstance(string instanceName);

        /// <summary>
        /// Удалить инстанс
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существал ли</returns>
        public abstract bool RemoveInstance(string instanceName);


        #region Работа со счетчиками

        /// <summary>
        /// Создать счётчик определённого типа
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public abstract void CreateCounter(CounterTypes type, string counterName, string counterDescription);

        /// <summary>
        /// Создать счётчик определённого типа (NumberOfItemsCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public virtual void CreateNumberOfItemsCounter(string counterName, string counterDescription)
        {
            CreateCounter(CounterTypes.NumberOfItems, counterName, counterDescription);
        }

        /// <summary>
        /// Создать счётчик определённого типа (AverageTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public virtual void CreateAverageTimeCounter(string counterName, string counterDescription)
        {
            CreateCounter(CounterTypes.AverageTime, counterName, counterDescription);
        }


        /// <summary>
        /// Создать счётчик определённого типа (AverageCountCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public virtual void CreateAverageCountCounter(string counterName, string counterDescription)
        {
            CreateCounter(CounterTypes.AverageCount, counterName, counterDescription);
        }


        /// <summary>
        /// Создать счётчик определённого типа (OperationsPerSecondCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public virtual void CreateOperationsPerSecondCounter(string counterName, string counterDescription)
        {
            CreateCounter(CounterTypes.OperationsPerSecond, counterName, counterDescription);
        }


        /// <summary>
        /// Создать счётчик определённого типа (ElapsedTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public virtual void CreateElapsedTimeCounter(string counterName, string counterDescription)
        {
            CreateCounter(CounterTypes.ElapsedTime, counterName, counterDescription);
        }


        /// <summary>
        /// Создать счётчик определённого типа (MomentTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public virtual void CreateMomentTimeCounter(string counterName, string counterDescription)
        {
            CreateCounter(CounterTypes.MomentTime, counterName, counterDescription);
        }

        #endregion
    }
}