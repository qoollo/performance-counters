using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Пустая категория
    /// Для добавления элемента в названии категории - Name.SubName.SubSubName
    /// </summary>
    public abstract class EmptyCategory: Category
    {
        /// <summary>
        /// Конструктор для создания пустой подкатегории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <param name="description">Описание категории</param>
        protected EmptyCategory(string name, string description)
            : base(name, description, CategoryTypes.Empty)
        {
        }
    }
}
