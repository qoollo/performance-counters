using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.Categories
{
    internal interface INetCategoriesExtendedInfo
    {
        /// <summary>
        /// Изменился ли список дочерних категории
        /// </summary>
        bool IsChildCategoriesChanged { get; }
        /// <summary>
        /// Изменился ли список счётчиков
        /// </summary>
        bool IsCountersChanged { get; }
        /// <summary>
        /// Изменился ли список инстансов
        /// </summary>
        bool IsChildInstancesChanged { get; }

        /// <summary>
        /// Пометить, что изменение дочерней категории было обработано
        /// </summary>
        void MarkChildCategoriesChangedAsViewed();

        /// <summary>
        /// Родительская категория
        /// </summary>
        Category ParentCategory { get; }
        /// <summary>
        /// Перечень дочерних категорий
        /// </summary>
        List<Category> ChildCategories { get; }
    }
}
