using System.Collections.Generic;
using System;
using Qoollo.PerformanceCounters.CounterAutoInitialization;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Обёртка для инстанса в многоинстансовой категории
    /// </summary>
    public class InstanceInMultiInstanceCategoryWrapper: IDisposable
    {
        private MultiInstanceCategoryWrapper _parent;
        private InstanceInMultiInstanceCategory _category;
        private bool _isAlive;
        
        /// <summary>
        /// Конструктор InstanceInMultiInstanceCategoryWrapper
        /// </summary>
        protected InstanceInMultiInstanceCategoryWrapper()
        {
            _isAlive = true;
        }

        /// <summary>
        /// Инициализация инстанса
        /// </summary>
        /// <param name="parent">Родительская многоинстансовая категория</param>
        /// <param name="category">Обёртываемый инстанс</param>
        /// <param name="counters">Список счётчиков</param>
        internal void InitInstance(MultiInstanceCategoryWrapper parent, InstanceInMultiInstanceCategory category, List<CounterPropertyInfo> counters)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (category == null)
                throw new ArgumentNullException("category");
            if (counters == null)
                throw new ArgumentNullException("counters");

            _category = category;
            _parent = parent;

            foreach (var prop in counters)
            {
                var counter = _category.GetCounter(prop.GetCounterName(), prop.CounterType);

                if (counter.Type != prop.CounterType)
                    throw new InvalidCounterTypeException(
                        string.Format("Counter types are not equal. Declared: {0}, Returned: {1}", prop.CounterType, counter.Type));

                prop.SetCounterValue(this, counter);   
            }

            AfterInit();
        }

        /// <summary>
        /// Сбросить значения всех счётчиков
        /// </summary>
        protected void ResetAllCounters()
        {
            foreach (var prop in _parent.CounterProperties)
            {
                Counter counter = (Counter)prop.GetCounterValue(this);
                counter.Reset();
            }
        }
        /// <summary>
        /// Отключить все счётчики времени
        /// </summary>
        protected void DisableAllTimeCounters()
        {
            foreach (var prop in _parent.CounterProperties)
            {
                if (prop.CounterType == CounterTypes.AverageTime)
                    ((AverageTimeCounter)prop.GetCounterValue(this)).DisableMeasurement();
                else if (prop.CounterType == CounterTypes.MomentTime)
                    ((MomentTimeCounter)prop.GetCounterValue(this)).DisableMeasurement();
            }
        }

        /// <summary>
        /// Функция вызывается после инициализации категории, когда все счётчики заполнены
        /// </summary>
        protected virtual void AfterInit()
        {
        }

        /// <summary>
        /// Служебный метод, вызываемый из родительской MultiInstanceCategoryWrapper при удалении инстанса
        /// </summary>
        internal void OnRemoveThisInstanceInternal()
        {
            _isAlive = false;
            Dispose(true);
        }

        /// <summary>
        /// Имя инстанса
        /// </summary>
        public string InstanceName
        {
            get
            {
                if (_category == null)
                    return "";
                return _category.InstanceName;
            }
        }

        /// <summary>
        /// Жив ли инстанс (не был ли удалён)
        /// </summary>
        public bool IsAlive
        {
            get
            {
                return _category != null && _parent != null && _category.IsAlive && _isAlive;
            }
        }

        /// <summary>
        /// Удалить данный инстанс
        /// </summary>
        public void Remove()
        {
            if (_category != null && _parent != null)
                _parent.RemoveInstance(this.InstanceName);
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        /// <param name="isUserCall">Вызвано ли явно пользователем</param>
        protected virtual void Dispose(bool isUserCall)
        {
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