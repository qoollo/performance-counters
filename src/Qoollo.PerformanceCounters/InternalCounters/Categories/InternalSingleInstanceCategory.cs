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
    /// Создание категории с одним инстансом для InternalCounters
    /// </summary>
    public class InternalSingleInstanceCategory: SingleInstanceCategory
    {
        private readonly Category _parentCategory;
        private readonly List<Category> _childCategories;
        private readonly ConcurrentDictionary<string, Counter> _counters;

        /// <summary>
        /// Конструктор InternalSingleInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="parentCategory">Родительская категория</param>
        public InternalSingleInstanceCategory(string name, string description, Category parentCategory)
            : base(name, description)
        {
            _parentCategory = parentCategory;
            _childCategories = new List<Category>();
            _counters = new ConcurrentDictionary<string, Counter>();
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
        /// Перечень счётчиков
        /// </summary>
        public IEnumerable<Counter> Counters { get { return _counters.Values; } }


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

            if (_counters.ContainsKey(counterName))
                throw new DuplicateCounterNameException("Counter with the same name is already existed. Name: " + counterName);

            Counter res = null;

            switch (type)
            {
                case CounterTypes.NumberOfItems:
                    res = new InternalNumberOfItemsCounter(counterName, counterDescription);
                    break;
                case CounterTypes.DeltaCount:
                    res = new InternalDeltaCountCounter(counterName, counterDescription);
                    break;
                case CounterTypes.AverageTime:
                    res = new InternalAverageTimeCounter(counterName, counterDescription);
                    break;
                case CounterTypes.AverageCount:
                    res = new InternalAverageCountCounter(counterName, counterDescription);
                    break;
                case CounterTypes.OperationsPerSecond:
                    res = new InternalOperationsPerSecondCounter(counterName, counterDescription);
                    break;
                case CounterTypes.ElapsedTime:
                    res = new InternalElapsedTimeCounter(counterName, counterDescription);
                    break;
                case CounterTypes.MomentTime:
                    res = new InternalMomentTimeCounter(counterName, counterDescription);
                    break;
                default:
                    throw new InvalidOperationException("Unknown CounterTypes value: " + type.ToString());
            }

            if (!_counters.TryAdd(counterName, res))
                throw new DuplicateCounterNameException("Counter with the same name is already existed. Name: " + counterName);

            return res;
        }


        /// <summary>
        /// Есть ли счётчик с указанным именем
        /// </summary>
        /// <param name="counterName">Имя счётчика</param>
        /// <returns>Есть ли он</returns>
        public override bool HasCounter(string counterName)
        {
            return _counters.ContainsKey(counterName);
        }

        /// <summary>
        /// Получение счетчика определенного типа
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <returns>Счётчик</returns>
        public override Counter GetCounter(string counterName)
        {
            Counter res = null;
            if (!_counters.TryGetValue(counterName, out res))
                throw new CounterNotExistException(string.Format("Counter ({0}) is not found in category {1}", counterName, this.Name));

            return res;
        }

        /// <summary>
        /// Получение счетчика определенного типа
        /// </summary>
        /// <param name="counterName">Имя счетчика</param>
        /// <param name="expectedCounterType">Тип счётчика</param>
        /// <returns>Счётчик</returns>
        public override Counter GetCounter(string counterName, CounterTypes expectedCounterType)
        {
            Counter res = null;
            if (!_counters.TryGetValue(counterName, out res))
                throw new CounterNotExistException(string.Format("Counter ({0}) is not found in category {1}", counterName, this.Name));

            if (res.Type != expectedCounterType)
                throw new InvalidCounterTypeException(
                    string.Format("Counter types are not equal. Expected: {0}, Returned: {1}", expectedCounterType, res.Type));

            return res;
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
