using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Обёртка для пустой категории
    /// </summary>
    public class EmptyCategoryWrapper : CategoryWrapper
    {
        private EmptyCategory _category;

        /// <summary>
        /// Конструктор EmptyCategoryWrapper
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        public EmptyCategoryWrapper(string name, string description)
            : base(name, description, CategoryTypes.Empty)
        {
        }
        /// <summary>
        /// Конструктор EmptyCategoryWrapper
        /// </summary>
        public EmptyCategoryWrapper()
            : base(CategoryTypes.Empty)
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
            if (!(category is EmptyCategory))
                throw new ArgumentException("Тип обертки над категорией и самой категории различаются. Тип обёртки: EmptyCategoryWrapper. Тип катеогрии: " + category.Type.ToString());

            _category = (EmptyCategory)category;
        }

        /// <summary>
        /// Инициализация счётчиков (внутренний метод)
        /// </summary>
        internal sealed override void InitCounters()
        {
        }
    }
}
