using Qoollo.PerformanceCounters.CompositeCounters.Counters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CompositeCounters.Categories
{
    /// <summary>
    /// Создание категории поддерживающей многочисленные инстансы для CompositeCounters
    /// </summary>
    public class CompositeMultiInstanceCategory: MultiInstanceCategory
    {
        private readonly List<Category> _childCategories;
        private readonly List<MultiInstanceCategory> _wrappedCategories;
        private readonly ConcurrentDictionary<string, CompositeInstanceInMultiInstanceCategory> _instances;
        private readonly ConcurrentDictionary<string, CompositeCounterDescriptor> _counters;

        /// <summary>
        /// Конструктор категории CompositeMultiInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="wrappedCategories">Оборачиваемые категории</param>
        public CompositeMultiInstanceCategory(string name, string description, IEnumerable<MultiInstanceCategory> wrappedCategories)
            : base(name, description)
        {
            if (wrappedCategories == null)
                throw new ArgumentNullException("wrappedCategories");

            _childCategories = new List<Category>();
            _counters = new ConcurrentDictionary<string, CompositeCounterDescriptor>();
            _instances = new ConcurrentDictionary<string, CompositeInstanceInMultiInstanceCategory>();
            _wrappedCategories = new List<MultiInstanceCategory>(wrappedCategories);
        }

        /// <summary>
        /// Перечень дочерних категорий
        /// </summary>
        public IEnumerable<Category> ChildCategories { get { return _childCategories; } }
        /// <summary>
        /// Обёрнутые категории
        /// </summary>
        public IEnumerable<MultiInstanceCategory> WrappedCategories { get { return _wrappedCategories; } }

        /// <summary>
        /// Перечень инстансов
        /// </summary>
        public IEnumerable<InstanceInMultiInstanceCategory> Instances { get { return _instances.Values; } }

        /// <summary>
        /// Словарь дескрипторов счётчиков (для внутреннего использования)
        /// </summary>
        internal ConcurrentDictionary<string, CompositeCounterDescriptor> Counters { get { return _counters; } }

        /// <summary>
        /// Получение или создание инстанса
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Инстанс</returns>
        public override InstanceInMultiInstanceCategory this[string instanceName]
        {
            get
            {
                return _instances.GetOrAdd(instanceName, (name) => new CompositeInstanceInMultiInstanceCategory(this, name, _wrappedCategories.Select(wc => wc[name])));
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
            CompositeInstanceInMultiInstanceCategory inst = null;
            if (_instances.TryRemove(instanceName, out inst))
            {
                inst.OnRemoveFromMultiInstanceCategory();

                foreach (var wrappedCat in _wrappedCategories)
                    wrappedCat.RemoveInstance(instanceName);

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


            CompositeCounterDescriptor res = null;

            switch (type)
            {
                case CounterTypes.NumberOfItems:
                    res = CompositeNumberOfItemsCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.AverageTime:
                    res = CompositeAverageTimeCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.AverageCount:
                    res = CompositeAverageCountCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.OperationsPerSecond:
                    res = CompositeOperationsPerSecondCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.ElapsedTime:
                    res = CompositeElapsedTimeCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                case CounterTypes.MomentTime:
                    res = CompositeMomentTimeCounter.CreateDescriptor(counterName, counterDescription);
                    break;
                default:
                    throw new InvalidOperationException("Unknown CounterTypes value: " + type.ToString());
            }

            if (!_counters.TryAdd(counterName, res))
                throw new DuplicateCounterNameException("Counter with the same name is already existed. Name: " + counterName);

            foreach (var wrappedCat in _wrappedCategories)
                wrappedCat.CreateCounter(type, counterName, counterDescription);
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

                var res = new CompositeEmptyCategory(categoryName, categoryDescription, _wrappedCategories.Select(wc => wc.CreateEmptySubCategory(categoryName, categoryDescription)));
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

                var res = new CompositeMultiInstanceCategory(categoryName, categoryDescription, _wrappedCategories.Select(wc => wc.CreateMultiInstanceSubCategory(categoryName, categoryDescription)));
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

                var res = new CompositeSingleInstanceCategory(categoryName, categoryDescription, _wrappedCategories.Select(wc => wc.CreateSingleInstanceSubCategory(categoryName, categoryDescription)));
                _childCategories.Add(res);
                return res;
            }
        }



        /// <summary>
        /// Инициализация счетчиков (создание категорий, инстанцирование самих счетчиков)
        /// </summary>
        protected internal override void Init()
        {
            foreach (var wrappedCat in WrappedCategories)
                wrappedCat.Init();
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        /// <param name="isUserCall">Было ли инициировано пользователем</param>
        protected override void Dispose(bool isUserCall)
        {
            if (isUserCall)
            {
                foreach (var wrappedCat in WrappedCategories)
                    wrappedCat.Dispose();
            }

            base.Dispose(isUserCall);
        }
    }
}
