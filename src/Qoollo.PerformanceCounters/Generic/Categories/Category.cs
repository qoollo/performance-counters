using System;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Категория счётчиков
    /// </summary>
    public abstract class Category: IDisposable
    {
        /// <summary>
        /// Конструктор Category
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <param name="type">Тип категории</param>
        internal Category(string categoryName, string categoryDescription, CategoryTypes type)
        {
            if (categoryName == null)
                throw new ArgumentNullException("categoryName");
            if (categoryDescription == null)
                throw new ArgumentNullException("categoryDescription");

            Name = categoryName;
            Description = categoryDescription;
            Type = type;
        }


        /// <summary>
        /// Имя категории
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Описание категории
        /// </summary>
        public string Description { get; private set; }


        /// <summary>
        /// Тип категории
        /// </summary>
        public CategoryTypes Type { get; private set; }


        /// <summary>
        /// Создание пустой подкатегории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="categoryName">Имя подкатегории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public abstract EmptyCategory CreateEmptySubCategory(string categoryName, string categoryDescription);

        /// <summary>
        /// Создание подкатегории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public abstract MultiInstanceCategory CreateMultiInstanceSubCategory(string categoryName, string categoryDescription);

        /// <summary>
        /// Создание простой подкатегории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Созданная подкатегория</returns>
        public abstract SingleInstanceCategory CreateSingleInstanceSubCategory(string categoryName, string categoryDescription);



        /// <summary>
        /// Инициализация счетчиков (создание категорий, инстанцирование самих счетчиков)
        /// </summary>
        protected internal virtual void Init()
        {
        }


        /// <summary>
        /// Преобразовать в строковое представление
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[CategoryType: {0}, Name: {1}]", Type, Name);
        }


        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        /// <param name="isUserCall">Было ли инициировано пользователем</param>
        protected virtual void Dispose(bool isUserCall)
        {
        }

        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}