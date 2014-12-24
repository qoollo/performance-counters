using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters.Categories
{
    /// <summary>
    /// Пустая категория для NullCounters
    /// </summary>
    public class NullEmptyCategory: EmptyCategory
    {
        /// <summary>
        /// Конструктор для создания пустой подкатегории NullEmptyCategory.
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <param name="description">Описание категории</param>
        public NullEmptyCategory(string name, string description)
            : base(name, description)
        {
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
            return new NullEmptyCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание подкатегории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override MultiInstanceCategory CreateMultiInstanceSubCategory(string categoryName, string categoryDescription)
        {
            return new NullMultiInstanceCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание простой подкатегории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public override SingleInstanceCategory CreateSingleInstanceSubCategory(string categoryName, string categoryDescription)
        {
            return new NullSingleInstanceCategory(categoryName, categoryDescription);
        }
    }
}
