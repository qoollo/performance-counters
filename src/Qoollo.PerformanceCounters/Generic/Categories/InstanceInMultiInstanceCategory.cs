using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Инстанс в многоинстансовой категории
    /// </summary>
    public abstract class InstanceInMultiInstanceCategory : IDisposable
    {
        /// <summary>
        /// Конструктор для создания инстанса
        /// </summary>
        /// <param name="parent">Родительская многоинстовая категория</param>
        /// <param name="instanceName">Имя инстанса</param>
        protected InstanceInMultiInstanceCategory(MultiInstanceCategory parent, string instanceName)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (instanceName == null)
                throw new ArgumentNullException("instanceName");

            Parent = parent;
            InstanceName = instanceName;
        }

        /// <summary>
        /// Родительская категория
        /// </summary>
        public MultiInstanceCategory Parent { get; private set; }

        /// <summary>
        /// Имя инстанса
        /// </summary>
        public string InstanceName { get; private set; }




        #region Работа со счетчиками


        /// <summary>
        /// Есть ли счётчик с указанным именем
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Есть ли он</returns>
        public abstract bool HasCounter(string counterName);

        /// <summary>
        /// Получение счетчика по имени
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
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Счётчик</returns>
        public virtual NumberOfItemsCounter GetNumberOfItemsCounter(string counterName)
        {
            return (NumberOfItemsCounter)GetCounter(counterName, CounterTypes.NumberOfItems);
        }

        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (NumberOfItemsCounter)
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Счётчик</returns>
        public virtual DeltaCountCounter GetCounterDeltaCounter(string counterName)
        {
            return (DeltaCountCounter)GetCounter(counterName, CounterTypes.DeltaCount);
        }

        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (AverageTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Счётчик</returns>
        public virtual AverageTimeCounter GetAverageTimeCounter(string counterName)
        {
            return (AverageTimeCounter)GetCounter(counterName, CounterTypes.AverageTime);
        }


        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (AverageCountCounter)
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Счётчик</returns>
        public virtual AverageCountCounter GetAverageCountCounter(string counterName)
        {
            return (AverageCountCounter)GetCounter(counterName, CounterTypes.AverageCount);
        }


        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (OperationsPerSecondCounter)
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Счётчик</returns>
        public virtual OperationsPerSecondCounter GetOperationsPerSecondCounter(string counterName)
        {
            return (OperationsPerSecondCounter)GetCounter(counterName, CounterTypes.OperationsPerSecond);
        }

        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (ElapsedTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Счётчик</returns>
        public virtual ElapsedTimeCounter GetElapsedTimeCounter(string counterName)
        {
            return (ElapsedTimeCounter)GetCounter(counterName, CounterTypes.ElapsedTime);
        }


        /// <summary>
        /// Возвращает ранее созданный счетчик производительности (MomentTimeCounter)
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Счётчик</returns>
        public virtual MomentTimeCounter GetMomentTimeCounter(string counterName)
        {
            return (MomentTimeCounter)GetCounter(counterName, CounterTypes.MomentTime);
        }

        #endregion

        /// <summary>
        /// Удаление инстанса
        /// </summary>
        public abstract void Remove();

        /// <summary>
        /// Живой ли инстанс (не был ли удалён)
        /// </summary>
        public abstract bool IsAlive { get; }


        /// <summary>
        /// Преобразование в строковое представление
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[Category: {0}, Instance: {1}]", Parent.Name, InstanceName);
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        void IDisposable.Dispose()
        {
            Remove();
        }
    }
}