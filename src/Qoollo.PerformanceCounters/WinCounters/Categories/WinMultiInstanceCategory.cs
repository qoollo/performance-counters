using Qoollo.PerformanceCounters.WinCounters.Counters;
using Qoollo.PerformanceCounters.WinCounters.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Categories
{
    /// <summary>
    /// Категория, поддерживающая многочисленные инстансы для WinCounters (Базовый класс)
    /// </summary>
    public abstract class WinMultiInstanceCategory : MultiInstanceCategory, IWinCategoryInitialization
    {
        private readonly WinCountersWorkingInfo _info;
        private readonly List<Category> _childCategories;
        private volatile WinCategoryState _state;

        /// <summary>
        /// Конструктор WinMultiInstanceCategory
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="rootName">Корневое имя</param>
        /// <param name="info">Информация о функционировании</param>
        internal WinMultiInstanceCategory(string name, string description, string rootName, WinCountersWorkingInfo info)
            : base(name, description)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            FullName = string.IsNullOrEmpty(rootName) ? name : rootName + "." + name;
            _info = info;

            _state = WinCategoryState.Created;
            _childCategories = new List<Category>();
        }

        /// <summary>
        /// Информация о функционировании
        /// </summary>
        internal WinCountersWorkingInfo Info { get { return _info; } }
        /// <summary>
        /// Состояние
        /// </summary>
        public WinCategoryState State { get { return _state; } }
        /// <summary>
        /// Полное имя (включая корневое), которое будет видно в Windows
        /// </summary>
        public string FullName { get; private set; }
        /// <summary>
        /// Перечень дочерних категорий
        /// </summary>
        public IEnumerable<Category> ChildCategories { get { return _childCategories; } }
        /// <summary>
        /// Перечень инстансов
        /// </summary>
        public abstract IEnumerable<InstanceInMultiInstanceCategory> Instances { get; }
        /// <summary>
        /// Перечень описателей счётчиков для внутреннего использования
        /// </summary>
        internal abstract IEnumerable<WinCounterDescriptor> Counters { get; }


        /// <summary>
        /// Внутренний метод создания дочерней пустой категории
        /// </summary>
        /// <param name="categoryName">Имя подкатегории</param>
        /// <param name="categoryDescription">Описание подкатегории</param>
        /// <returns>Созданная подкатегория</returns>
        protected virtual EmptyCategory CreateEmptySubCategoryInner(string categoryName, string categoryDescription)
        {
            return CategoryHelper.CreateEmptyCategory(categoryName, categoryDescription, FullName, _info);
        }
        /// <summary>
        /// Внутренний метод создания дочерней категории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя подкатегории</param>
        /// <param name="categoryDescription">Описание подкатегории</param>
        /// <returns>Созданная подкатегория</returns>
        protected virtual MultiInstanceCategory CreateMultiInstanceSubCategoryInner(string categoryName, string categoryDescription)
        {
            return CategoryHelper.CreateMultiInstanceCategory(categoryName, categoryDescription, FullName, _info);
        }
        /// <summary>
        /// Внутренний метод создания дочерней категории
        /// </summary>
        /// <param name="categoryName">Имя подкатегории</param>
        /// <param name="categoryDescription">Описание подкатегории</param>
        /// <returns>Созданная подкатегория</returns>
        protected virtual SingleInstanceCategory CreateSingleInstanceSubCategoryInner(string categoryName, string categoryDescription)
        {
            return CategoryHelper.CreateSingleInstanceCategory(categoryName, categoryDescription, FullName, _info);
        }


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
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = CreateEmptySubCategoryInner(categoryName, categoryDescription);
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
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = CreateMultiInstanceSubCategoryInner(categoryName, categoryDescription);
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
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = CreateSingleInstanceSubCategoryInner(categoryName, categoryDescription);
                _childCategories.Add(res);
                return res;
            }
        }

        /// <summary>
        /// Инициализация категории
        /// </summary>
        protected abstract void InitCategory();


        /// <summary>
        /// Инициализация счетчиков (создание категорий, инстанцирование самих счетчиков)
        /// </summary>
        protected internal override void Init()
        {
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            if (_state == WinCategoryState.Initialized)
                return;

            _state = WinCategoryState.Initialized;

            base.Init();

            InitCategory();
        }

        /// <summary>
        /// Удаление категории из Windows
        /// </summary>
        public abstract void RemoveCategory();

        /// <summary>
        /// Удалить все инстансы в многоинстансовой категории (в том числе и из Windows)
        /// </summary>
        public abstract void RemoveInstances();

        /// <summary>
        /// Есть ли в Windows инстанс с указанным именем
        /// </summary>
        /// <param name="instanceName">Имя инстанса</param>
        /// <returns>Есть ли</returns>
        public bool HasWindowsInstance(string instanceName)
        {
            return PerformanceCounterCategory.InstanceExists(instanceName, this.FullName, Info.MachineName);
        }


        /// <summary>
        /// Инициализировать данную категорию
        /// </summary>
        void IWinCategoryInitialization.Init()
        {
            Init();
        }
        /// <summary>
        /// Удалить все инстансы в многоинстансовой категории
        /// </summary>
        void IWinCategoryInitialization.RemoveInstances()
        {
            RemoveInstances();
        }
        /// <summary>
        /// Удалить данную категорию из Windows
        /// </summary>
        void IWinCategoryInitialization.RemoveCategory()
        {
            RemoveCategory();
        }



        /// <summary>
        /// Инициализировать данную и все дочерние категории
        /// </summary>
        void IWinCategoryInitialization.InitCascade()
        {
            this.Init();
            foreach (var child in _childCategories)
                ((IWinCategoryInitialization)child).InitCascade();
        }
        /// <summary>
        /// Удалить все инстансы в многоинстансовой категории каскадом
        /// </summary>
        void IWinCategoryInitialization.RemoveInstancesCascade()
        {
            foreach (var child in _childCategories)
                ((IWinCategoryInitialization)child).RemoveInstancesCascade();

            (this as IWinCategoryInitialization).RemoveInstances();
        }
        /// <summary>
        /// Удалить данную и все дочерние категории из Windows
        /// </summary>
        void IWinCategoryInitialization.RemoveCategoryCascade()
        {
            foreach (var child in _childCategories)
                ((IWinCategoryInitialization)child).RemoveCategoryCascade();

            (this as IWinCategoryInitialization).RemoveCategory();
        }
        /// <summary>
        /// Освободить данную и все дочерние категории
        /// </summary>
        void IWinCategoryInitialization.DisposeCascade()
        {
            foreach (var child in _childCategories)
                ((IWinCategoryInitialization)child).DisposeCascade();

            this.Dispose();
        }


        /// <summary>
        /// Освобождение ресурсов
        /// </summary>
        /// <param name="isUserCall">Было ли инициировано пользователем</param>
        protected override void Dispose(bool isUserCall)
        {
            if (isUserCall)
            {
                _state = WinCategoryState.Disposed;
            }
            base.Dispose(isUserCall);
        }
    }
}
