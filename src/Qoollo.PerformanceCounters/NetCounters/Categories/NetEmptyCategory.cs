using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.Categories
{
    /// <summary>
    /// Пустая категория для NetCounters
    /// </summary>
    public class NetEmptyCategory : EmptyCategory, INetCategoriesExtendedInfo
    {
        private readonly Category _parentCategory;
        private readonly List<Category> _childCategories;
        private volatile bool _isChildCategoriesChanged;

        /// <summary>
        /// Конструктор для создания пустой подкатегории NetEmptyCategory.
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="parentCategory">Родительская категория</param>
        public NetEmptyCategory(string name, string description, Category parentCategory)
            : base(name, description)
        {
            _parentCategory = parentCategory;
            _childCategories = new List<Category>();
            _isChildCategoriesChanged = false;
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
        /// Изменился ли список дочерних категории
        /// </summary>
        internal bool IsChildCategoriesChanged { get { return _isChildCategoriesChanged; } }
        /// <summary>
        /// Пометить, что изменение дочерней категории было обработано
        /// </summary>
        internal void MarkChildCategoriesChangedAsViewed()
        {
            _isChildCategoriesChanged = false;
        }

        bool INetCategoriesExtendedInfo.IsChildCategoriesChanged { get { return _isChildCategoriesChanged; } }
        bool INetCategoriesExtendedInfo.IsCountersChanged { get { return false; } }
        bool INetCategoriesExtendedInfo.IsChildInstancesChanged { get { return false; } }
        void INetCategoriesExtendedInfo.MarkChildCategoriesChangedAsViewed() { this.MarkChildCategoriesChangedAsViewed(); }
        List<Category> INetCategoriesExtendedInfo.ChildCategories { get { return _childCategories; } }

        /// <summary>
        /// Создание пустой подкатегории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="categoryName">Имя подкатегории</param>
        /// <param name="categoryDescription">Описание подкатегории</param>
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

                var res = new NetEmptyCategory(categoryName, categoryDescription, this);
                _childCategories.Add(res);
                _isChildCategoriesChanged = true;
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

                var res = new NetMultiInstanceCategory(categoryName, categoryDescription, this);
                _childCategories.Add(res);
                _isChildCategoriesChanged = true;
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

                var res = new NetSingleInstanceCategory(categoryName, categoryDescription, this);
                _childCategories.Add(res);
                _isChildCategoriesChanged = true;
                return res;
            }
        }
    }
}
