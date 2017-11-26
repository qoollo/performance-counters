using Qoollo.PerformanceCounters.InternalCounters.Counters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.InternalCounters.Categories
{
    /// <summary>
    /// Создание категории поддерживающей многочисленные инстансы для InternalCounters
    /// </summary>
    public class InternalMultiInstanceCategory: MultiInstanceCategory
    {
        private readonly Category _parentCategory;
        private readonly List<Category> _childCategories;
        private readonly ConcurrentDictionary<string, InternalInstanceInMultiInstanceCategory> _instances;
        private readonly ConcurrentDictionary<string, InternalCounterDescriptor> _counters;

        /// <summary>
        /// Конструктор категории InternalMultiInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="parentCategory">Родительская категория</param>
        public InternalMultiInstanceCategory(string name, string description, Category parentCategory)
            : base(name, description)
        {
            _parentCategory = parentCategory;
            _childCategories = new List<Category>();
            _counters = new ConcurrentDictionary<string, InternalCounterDescriptor>();
            _instances = new ConcurrentDictionary<string, InternalInstanceInMultiInstanceCategory>();
        }

        /// <summary>
        /// Родительская категория
        /// </summary>
        public Category ParentCategory { get { return _parentCategory; } }
        /// <summary>
        /// Перечень дочерних категорий
        /// </summary>
        public IEnumerable<Category> ChildCategories { get { return _childCategories; } }
        /// <summary>
        /// Перечень инстансов
        /// </summary>
        public IEnumerable<InstanceInMultiInstanceCategory> Instances { get { return _instances.Values; } }

        /// <summary>
        /// Словарь дескрипторов счётчиков (для внутреннего использования)
        /// </summary>
        internal ConcurrentDictionary<string, InternalCounterDescriptor> Counters { get { return _counters; } }

        /// <summary>
        /// Получение или создание инстанса
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Инстанс</returns>
        public override InstanceInMultiInstanceCategory this[string instanceName]
        {
            get
            {
                InternalInstanceInMultiInstanceCategory result = null;
                if (!_instances.TryGetValue(instanceName, out result))
                    result = _instances.GetOrAdd(instanceName, new InternalInstanceInMultiInstanceCategory(this, instanceName));

                return result;
            }
        }

        /// <summary>
        /// Существует ли инстанс счётчиков
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существует ли</returns>
        public override bool HasInstance(string instanceName)
        {
            return _instances.ContainsKey(instanceName);
        }

        /// <summary>
        /// Удалить инстанс
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Существал ли</returns>
        public override bool RemoveInstance(string instanceName)
        {
            InternalInstanceInMultiInstanceCategory inst = null;
            if (_instances.TryRemove(instanceName, out inst))
            {
                inst.OnRemoveFromMultiInstanceCategory();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Создать счётчик определённого типа
        /// </summary>
        /// <param name="type">Тип счётчика</param>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="counterDescription">Описание счетчика</param>
        public override void CreateCounter(CounterTypes type, string counterName, string counterDescription)
        {
            if (counterName == null)
                throw new ArgumentNullException("counterName");
            if (counterDescription == null)
                throw new ArgumentNullException("counterDescription");

            if (_instances.Count != 0)
            {
                throw new PerformanceCounterCreationException(
                    string.Format("Can't create counter in multiInstanceCategory ({0}) when it has created instances", this.Name));
            }

            if (_counters.ContainsKey(counterName))
                throw new DuplicateCounterNameException("Counter with the same name is already existed. Name: " + counterName);


            InternalCounterDescriptor res = null;

            switch (type)
            {
                case CounterTypes.NumberOfItems:
                    res = InternalNumberOfItemsCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.Delta:
                    res = InternalDeltaCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.AverageTime:
                    res = InternalAverageTimeCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.AverageCount:
                    res = InternalAverageCountCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.OperationsPerSecond:
                    res = InternalOperationsPerSecondCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.ElapsedTime:
                    res = InternalElapsedTimeCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.MomentTime:
                    res = InternalMomentTimeCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                default:
                    throw new InvalidOperationException("Unknown CounterTypes value: " + type.ToString());
            }

            if (!_counters.TryAdd(counterName, res))
                throw new DuplicateCounterNameException("Counter with the same name is already existed. Name: " + counterName);
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
            if (categoryName == null)
                throw new ArgumentNullException("categoryName");
            if (categoryDescription == null)
                throw new ArgumentNullException("categoryDescription");

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = new InternalEmptyCategory(categoryName, categoryDescription, this);
                _childCategories.Add(res);
                return res;
            }
        }

        /// <summary>
        /// Создание подкатегории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override MultiInstanceCategory CreateMultiInstanceSubCategory(string categoryName, string categoryDescription)
        {
            if (categoryName == null)
                throw new ArgumentNullException("categoryName");
            if (categoryDescription == null)
                throw new ArgumentNullException("categoryDescription");

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = new InternalMultiInstanceCategory(categoryName, categoryDescription, this);
                _childCategories.Add(res);
                return res;
            }
        }

        /// <summary>
        /// Создание простой подкатегории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override SingleInstanceCategory CreateSingleInstanceSubCategory(string categoryName, string categoryDescription)
        {
            if (categoryName == null)
                throw new ArgumentNullException("categoryName");
            if (categoryDescription == null)
                throw new ArgumentNullException("categoryDescription");

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = new InternalSingleInstanceCategory(categoryName, categoryDescription, this);
                _childCategories.Add(res);
                return res;
            }
        }
    }
}
