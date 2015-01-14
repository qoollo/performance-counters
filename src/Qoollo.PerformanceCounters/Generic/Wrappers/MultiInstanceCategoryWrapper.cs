using Qoollo.PerformanceCounters.CounterAutoInitialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Базовый враппер для MultiInstanceCategory
    /// </summary>
    public abstract class MultiInstanceCategoryWrapper : CategoryWrapper
    {
        /// <summary>
        /// Конструктор MultiInstanceCategoryWrapper
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        internal MultiInstanceCategoryWrapper(string name, string description)
            : base(name, description, CategoryTypes.MultiInstance)
        {
        }
        /// <summary>
        /// Конструктор MultiInstanceCategoryWrapper
        /// </summary>
        internal MultiInstanceCategoryWrapper()
            : base(CategoryTypes.MultiInstance)
        {
        }

        /// <summary>
        /// Описатели свойств счётчиков для инстансов
        /// </summary>
        internal virtual IEnumerable<CounterPropertyInfo> CounterProperties { get { throw new NotSupportedException("CounterProperties was not implemented"); } }
        /// <summary>
        /// Проверить существование инстанса
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существует ли</returns>
        public abstract bool HasInstance(string instanceName);
        /// <summary>
        /// Удалить инстанс по имени
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существовал ли</returns>
        public abstract bool RemoveInstance(string instanceName);
    }


    /// <summary>
    /// Обертка для многоинставой категории. Не содержит счетчики.
    /// Наследник должен уточнить тип кагории, которая будет содержать счетчики в инстансах.
    /// </summary>
    /// <typeparam name="T">Тип обёртки конкретного инстанса</typeparam>
    public class MultiInstanceCategoryWrapper<T> : MultiInstanceCategoryWrapper where T : InstanceInMultiInstanceCategoryWrapper, new()
    {
        private MultiInstanceCategory _category;

        private readonly ConcurrentDictionary<string, T> _instances;
        private readonly List<CounterPropertyInfo> _counters;


        /// <summary>
        /// Конструктор MultiInstanceCategoryWrapper
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        protected internal MultiInstanceCategoryWrapper(string name, string description)
            : base(name, description)
        {
            _instances = new ConcurrentDictionary<string, T>();
            _counters = CounterPropertyInfoExtractor.GetContainerCounterProperties(typeof(T));
        }
        /// <summary>
        /// Конструктор MultiInstanceCategoryWrapper
        /// </summary>
        protected internal MultiInstanceCategoryWrapper()
        {
            _instances = new ConcurrentDictionary<string, T>();
            _counters = CounterPropertyInfoExtractor.GetContainerCounterProperties(typeof(T));
        }

        /// <summary>
        /// Описатели свойств счётчиков для инстансов
        /// </summary>
        internal override IEnumerable<CounterPropertyInfo> CounterProperties { get { return _counters; } }

        /// <summary>
        /// Установить обёртываемую категорию (внутренний метод)
        /// </summary>
        /// <param name="category">Обёртываемая категория</param>
        internal sealed override void InitCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (!(category is MultiInstanceCategory))
                throw new ArgumentException("Тип обертки над категорией и самой категории различаются. Тип обёртки: MultiInstanceCategoryWrapper. Тип катеогрии: " + category.Type.ToString());

            _category = (MultiInstanceCategory)category;
        }

        /// <summary>
        /// Инициализация счётчиков (внутренний метод)
        /// </summary>
        internal sealed override void InitCounters()
        {
            foreach (var prop in _counters)
            {
                _category.CreateCounter(prop.CounterType, prop.GetCounterName(), prop.GetCounterDescription());
            }

            _category.Init();
        }


        /// <summary>
        /// Получение или создание инстанса
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Инстанс</returns>
        public T this[string instanceName]
        {
            get
            {
                var instance = _instances.GetOrAdd(instanceName, (name) =>
                {
                    var inst = new T();

                    var category = _category[instanceName];
                    inst.InitInstance(this, category, _counters);

                    return inst;
                });

                return instance;
            }
        }


        /// <summary>
        /// Проверить существование инстанса
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существует ли</returns>
        public override bool HasInstance(string instanceName)
        {
            return _instances.ContainsKey(instanceName);
        }


        /// <summary>
        /// Удалить инстанс по имени
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существовал ли</returns>
        public override bool RemoveInstance(string instanceName)
        {
            T instVal = default(T);
            if (_instances.TryRemove(instanceName, out instVal))
            {
                _category.RemoveInstance(instanceName);
                instVal.OnRemoveThisInstanceInternal();

                return true;
            }

            return false;
        }
    }
}