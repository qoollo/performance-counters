using Qoollo.PerformanceCounters.CompositeCounters.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CompositeCounters
{
    /// <summary>
    /// Фабрика для композиции счётчиков различных видов
    /// </summary>
    public class CompositeCounterFactory: CounterFactory
    {
        private readonly List<CounterFactory> _wrappedFactories;
        private readonly List<Category> _childCategories;

        /// <summary>
        /// Конструктор CompositeCounterFactory
        /// </summary>
        public CompositeCounterFactory()
        {
            _wrappedFactories = new List<CounterFactory>();
            _childCategories = new List<Category>();
        }
        /// <summary>
        /// Конструктор CompositeCounterFactory
        /// </summary>
        /// <param name="factories">Перечень оборачиваемых фабрик</param>
        public CompositeCounterFactory(IEnumerable<CounterFactory> factories)
        {
            if (factories == null)
                throw new ArgumentNullException("factories");

            _childCategories = new List<Category>();
            _wrappedFactories = new List<CounterFactory>(factories);
        }
        /// <summary>
        /// Конструктор CompositeCounterFactory из конфига
        /// </summary>
        /// <param name="config">Конфигурация</param>
        public CompositeCounterFactory(Qoollo.PerformanceCounters.Configuration.CompositeCountersConfiguration config)
            : this(config.WrappedCounters.Select(o => PerfCountersInstantiationFactory.CreateCounterFactory(o)))
        {
        }

        /// <summary>
        /// Перечень дочерних категорий
        /// </summary>
        public IEnumerable<Category> Categories { get { return _childCategories; } }
        /// <summary>
        /// Список оборачиваемых фабрик
        /// </summary>
        public List<CounterFactory> WrappedFactories { get { return _wrappedFactories; } }



        /// <summary>
        /// Создание пустой категории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override EmptyCategory CreateEmptyCategory(string categoryName, string categoryDescription)
        {
            if (categoryName == null)
                throw new ArgumentNullException("categoryName");
            if (categoryDescription == null)
                throw new ArgumentNullException("categoryDescription");

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = new CompositeEmptyCategory(categoryName, categoryDescription, _wrappedFactories.Select(wc => wc.CreateEmptyCategory(categoryName, categoryDescription)));
                _childCategories.Add(res);
                return res;
            }
        }

        /// <summary>
        /// Создание категории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override MultiInstanceCategory CreateMultiInstanceCategory(string categoryName, string categoryDescription)
        {
            if (categoryName == null)
                throw new ArgumentNullException("categoryName");
            if (categoryDescription == null)
                throw new ArgumentNullException("categoryDescription");

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = new CompositeMultiInstanceCategory(categoryName, categoryDescription, _wrappedFactories.Select(wc => wc.CreateMultiInstanceCategory(categoryName, categoryDescription)));
                _childCategories.Add(res);
                return res;
            }
        }


        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override SingleInstanceCategory CreateSingleInstanceCategory(string categoryName, string categoryDescription)
        {
            if (categoryName == null)
                throw new ArgumentNullException("categoryName");
            if (categoryDescription == null)
                throw new ArgumentNullException("categoryDescription");

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = new CompositeSingleInstanceCategory(categoryName, categoryDescription, _wrappedFactories.Select(wc => wc.CreateSingleInstanceCategory(categoryName, categoryDescription)));
                _childCategories.Add(res);
                return res;
            }
        }


        /// <summary>
        /// Инициализация счетчиков во всех дочерних категориях (создание категорий, инстанцирование самих счетчиков)
        /// </summary>
        public override void InitAll()
        {
            foreach (var factory in _wrappedFactories)
                factory.InitAll();
        }

        /// <summary>
        /// Обойти все фабрики указанного типа
        /// </summary>
        /// <typeparam name="TFactory">Тип фабрик для обхода</typeparam>
        /// <param name="action">Действие, выполняемое при входе в фабрику указанного типа</param>
        public override void WalkFactories<TFactory>(Action<TFactory> action)
        {
            base.WalkFactories<TFactory>(action);

            foreach (var factory in _wrappedFactories)
                factory.WalkFactories<TFactory>(action);
        }


        /// <summary>
        /// Внутренний код освобождения ресурсов
        /// </summary>
        /// <param name="isUserCall">Вызвано ли пользователем</param>
        protected override void Dispose(bool isUserCall)
        {
            if (isUserCall)
            {
                foreach (var factory in _wrappedFactories)
                    factory.Dispose();
            }

            base.Dispose(isUserCall);
        }
    }
}
