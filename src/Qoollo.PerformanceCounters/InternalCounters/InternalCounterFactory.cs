using Qoollo.PerformanceCounters.InternalCounters.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.InternalCounters
{
    /// <summary>
    /// Фабрика для внутренних счётчиков.
    /// Счётчики полностью функциональны, но доступны лишь в рамках приложения.
    /// </summary>
    public class InternalCounterFactory: CounterFactory
    {
        private readonly InternalEmptyCategory _internalCategory;

        /// <summary>
        /// Конструктор InternalCounterFactory
        /// </summary>
        public InternalCounterFactory()
        {
            _internalCategory = new InternalEmptyCategory("root", "root category", null);
        }
        /// <summary>
        /// Конструктор InternalCounterFactory из конфигурации
        /// </summary>
        /// <param name="config">Конфигурация</param>
        public InternalCounterFactory(Qoollo.PerformanceCounters.Configuration.InternalCountersConfiguration config)
            : this()
        {
        }

        /// <summary>
        /// Перечень дочерних категорий
        /// </summary>
        public IEnumerable<Category> Categories { get { return _internalCategory.ChildCategories; } }

        /// <summary>
        /// Создание пустой категории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override EmptyCategory CreateEmptyCategory(string categoryName, string categoryDescription)
        {
            return _internalCategory.CreateEmptySubCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание категории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override MultiInstanceCategory CreateMultiInstanceCategory(string categoryName, string categoryDescription)
        {
            return _internalCategory.CreateMultiInstanceSubCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override SingleInstanceCategory CreateSingleInstanceCategory(string categoryName, string categoryDescription)
        {
            return _internalCategory.CreateSingleInstanceSubCategory(categoryName, categoryDescription);
        }
    }
}
