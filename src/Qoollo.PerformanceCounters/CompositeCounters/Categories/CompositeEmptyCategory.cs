using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.CompositeCounters.Categories
{
    /// <summary>
    /// Пустая категория для CompositeCounters
    /// </summary>
    public class CompositeEmptyCategory: EmptyCategory
    {
        private readonly List<Category> _childCategories;
        private readonly List<EmptyCategory> _wrappedCategories;

        /// <summary>
        /// Конструктор для создания пустой подкатегории CompositeEmptyCategory.
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="wrappedCategories">Оборачиваемые категории</param>
        public CompositeEmptyCategory(string name, string description, IEnumerable<EmptyCategory> wrappedCategories)
            : base(name, description)
        {
            if (wrappedCategories == null)
                throw new ArgumentNullException("wrappedCategories");

            _childCategories = new List<Category>();
            _wrappedCategories = new List<EmptyCategory>(wrappedCategories);
        }

        /// <summary>
        /// Перечень дочерних категорий
        /// </summary>
        public IEnumerable<Category> ChildCategories { get { return _childCategories; } }
        /// <summary>
        /// Обёрнутые категории
        /// </summary>
        public IEnumerable<EmptyCategory> WrappedCategories { get { return _wrappedCategories; } }


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
