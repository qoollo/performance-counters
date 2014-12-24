using Qoollo.PerformanceCounters.CounterAutoInitialization;
using System;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Обёртка для простой категории (с одним инстансом)
    /// </summary>
    public class SingleInstanceCategoryWrapper : CategoryWrapper
    {
        private SingleInstanceCategory _category;


        /// <summary>
        /// Конструктор SingleInstanceCategoryWrapper
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        protected SingleInstanceCategoryWrapper(string name, string description)
            : base(name, description, CategoryTypes.SingleInstance)
        {
        }


        /// <summary>
        /// Установить обёртываемую категорию (внутренний метод)
        /// </summary>
        /// <param name="category">Обёртываемая категория</param>
        internal sealed override void InitCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (!(category is SingleInstanceCategory))
                throw new ArgumentException("Тип обертки над категорией и самой категории различаются. Тип обёртки: SingleInstanceCategoryWrapper. Тип катеогрии: " + category.Type.ToString());

            _category = (SingleInstanceCategory)category;
        }

        /// <summary>
        /// Инициализация счётчиков (внутренний метод)
        /// </summary>
        internal sealed override void InitCounters()
        {
            var type = this.GetType();

            var counterProps = CounterPropertyInfoExtractor.GetContainerCounterProperties(type);

            foreach (var prop in counterProps)
            {
                Counter counter = _category.CreateCounter(prop.CounterType, prop.GetCounterName(), prop.GetCounterDescription());
                prop.SetCounterValue(this, counter);              
            }

            _category.Init();
        }


        /// <summary>
        /// Сбросить значения всех счётчиков
        /// </summary>
        protected void ResetAllCounters()
        {
            var counterProps = CounterPropertyInfoExtractor.GetContainerCounterProperties(this.GetType());

            foreach (var prop in counterProps)
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
            var counterProps = CounterPropertyInfoExtractor.GetContainerCounterProperties(this.GetType());

            foreach (var prop in counterProps)
            {
                if (prop.CounterType == CounterTypes.AverageTime)
                    ((AverageTimeCounter)prop.GetCounterValue(this)).DisableMeasurement();
                else if (prop.CounterType == CounterTypes.MomentTime)
                    ((MomentTimeCounter)prop.GetCounterValue(this)).DisableMeasurement();
            }
        }
    }
}