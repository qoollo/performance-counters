using Qoollo.PerformanceCounters.WinCounters.Categories;
using Qoollo.PerformanceCounters.WinCounters.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters
{
    /// <summary>
    /// Режим создания категорий и счётчиков в WinCounters
    /// </summary>
    public enum WinCountersInstantiationMode
    {
        /// <summary>
        /// Всегда удалять старые и создавать новые
        /// </summary>
        AlwaysCreateNew,
        /// <summary>
        /// Если все необходимые счётчики есть, то не пересоздавать
        /// </summary>
        UseExistedIfPossible,
        /// <summary>
        /// Всегда использовать только существующие счётчики (прозрачная работа)
        /// </summary>
        UseOnlyExisted
    }

    /// <summary>
    /// Битность счётчиков, используемых внутри
    /// </summary>
    public enum WinCountersPreferedBitness
    {
        /// <summary>
        /// Битность ОС
        /// </summary>
        SameAsOperatingSystemBitness,
        /// <summary>
        /// Использовать 32-ух битные
        /// </summary>
        Prefer32BitCounters,
        /// <summary>
        /// Использовать 64-ёх битные
        /// </summary>
        Prefer64BitCounters
    }

    /// <summary>
    /// Способ обработки существующих в Windows инстансов в MultiInstance категориях
    /// </summary>
    public enum WinCountersExistedInstancesTreatment
    {
        /// <summary>
        /// Игнорировать существующие
        /// </summary>
        IgnoreExisted,
        /// <summary>
        /// Загружать существующие
        /// </summary>
        LoadExisted,
        /// <summary>
        /// Удалять существующие
        /// </summary>
        RemoveExisted
    }

    /// <summary>
    /// Фабрика для счётчиков Windows
    /// </summary>
    public class WinCounterFactory : CounterFactory
    {
        private readonly WinCountersWorkingInfo _info;
        private readonly List<Category> _childCategories;
        private readonly string _namePrefix;
        private volatile WinCategoryState _state;

        /// <summary>
        /// Конструктор WinCounterFactory
        /// </summary>
        /// <param name="instMode">Режим работы</param>
        /// <param name="namePrefix">Префикс имени категории</param>
        /// <param name="machineName">Имя машины (null или '.' для локальной)</param>
        /// <param name="readOnlyCounters">Счётчики только для чтения</param>
        /// <param name="preferedBitness">Предпочитать ли 64 битные счётчики</param>
        /// <param name="existedInstancesTreatment">Как обрабатывать существующие в Windows инстансы</param>
        public WinCounterFactory(WinCountersInstantiationMode instMode, string namePrefix, string machineName, bool readOnlyCounters, WinCountersPreferedBitness preferedBitness, WinCountersExistedInstancesTreatment existedInstancesTreatment)
        {
            bool prefer64BitCounters = Environment.Is64BitOperatingSystem;
            if (preferedBitness == WinCountersPreferedBitness.Prefer32BitCounters)
                prefer64BitCounters = false;
            else if (preferedBitness == WinCountersPreferedBitness.Prefer64BitCounters)
                prefer64BitCounters = true;

            _info = new WinCountersWorkingInfo(instMode, machineName, readOnlyCounters, prefer64BitCounters, existedInstancesTreatment);
            _namePrefix = namePrefix != null ? namePrefix.TrimEnd('.') : "";
            _state = WinCategoryState.Created;
            _childCategories = new List<Category>();
        }

        /// <summary>
        /// Конструктор WinCounterFactory
        /// </summary>
        /// <param name="instMode">Режим работы</param>
        /// <param name="namePrefix">Префикс имени категории</param>
        /// <param name="machineName">Имя машины (null или '.' для локальной)</param>
        /// <param name="readOnlyCounters">Счётчики только для чтения</param>
        public WinCounterFactory(WinCountersInstantiationMode instMode, string namePrefix, string machineName, bool readOnlyCounters)
            : this(instMode, namePrefix, machineName, readOnlyCounters, WinCountersPreferedBitness.SameAsOperatingSystemBitness, WinCountersExistedInstancesTreatment.LoadExisted)
        {
        }

        /// <summary>
        /// Конструктор WinCounterFactory
        /// </summary>
        /// <param name="instMode">Режим работы</param>
        /// <param name="namePrefix">Префикс имени категории</param>
        public WinCounterFactory(WinCountersInstantiationMode instMode, string namePrefix)
            : this(instMode, namePrefix, ".", false, WinCountersPreferedBitness.SameAsOperatingSystemBitness, WinCountersExistedInstancesTreatment.LoadExisted)
        {
        }

        /// <summary>
        /// Конструктор WinCounterFactory
        /// </summary>
        /// <param name="instMode">Режим работы</param>
        public WinCounterFactory(WinCountersInstantiationMode instMode)
            : this(instMode, null, ".", false, WinCountersPreferedBitness.SameAsOperatingSystemBitness, WinCountersExistedInstancesTreatment.LoadExisted)
        {
        }

        /// <summary>
        /// Конструктор WinCounterFactory
        /// </summary>
        public WinCounterFactory()
            : this(WinCountersInstantiationMode.UseExistedIfPossible, null, ".", false, WinCountersPreferedBitness.SameAsOperatingSystemBitness, WinCountersExistedInstancesTreatment.LoadExisted)
        {
        }

        /// <summary>
        /// Конструктор WinCounterFactory по конфигу
        /// </summary>
        /// <param name="config">Конфигурация</param>
        public WinCounterFactory(Qoollo.PerformanceCounters.Configuration.WinCountersConfiguration config)
            : this(config.InstantiationMode, config.CategoryNamePrefix, config.MachineName, config.IsReadOnlyCounters, config.PreferedBitness, config.ExistedInstancesTreatment)
        {
        }

        /// <summary>
        /// Режим работы счётчиков
        /// </summary>
        public WinCountersInstantiationMode InstantiationMode { get { return _info.InstantiationMode; } }
        /// <summary>
        /// Фиксированный префикс в имени категорий
        /// </summary>
        public string NamePrefix { get { return _namePrefix; } }
        /// <summary>
        /// Имя машины
        /// </summary>
        public string MachineName { get { return _info.MachineName; } }
        /// <summary>
        /// Используются ли счётчики только для чтения
        /// </summary>
        public bool ReadOnlyCounters { get { return _info.ReadOnlyCounters; } }
        /// <summary>
        /// Предпочитать 64-ёх разрядные счётчики (если есть выбор)
        /// </summary>
        public bool Prefer64BitCounters { get { return _info.Prefer64BitCounters; } }
        /// <summary>
        /// Как обрабатывать существующие в Windows инстансы
        /// </summary>
        public WinCountersExistedInstancesTreatment ExistedInstancesTreatment { get { return _info.ExistedInstancesTreatment; } }

        /// <summary>
        /// Состояние фабрики
        /// </summary>
        public WinCategoryState State { get { return _state; } }
        /// <summary>
        /// Перечень дочерних категорий
        /// </summary>
        public IEnumerable<Category> Categories { get { return _childCategories; } }



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
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = CategoryHelper.CreateEmptyCategory(categoryName, categoryDescription, NamePrefix, _info);
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
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = CategoryHelper.CreateMultiInstanceCategory(categoryName, categoryDescription, NamePrefix, _info);
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
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            lock (_childCategories)
            {
                if (_childCategories.Any(o => o.Name == categoryName))
                    throw new DuplicateCategoryNameException("Category with the same name is already registered. Name: " + categoryName);

                var res = CategoryHelper.CreateSingleInstanceCategory(categoryName, categoryDescription, NamePrefix, _info);
                _childCategories.Add(res);
                return res;
            }
        }


        /// <summary>
        /// Выполнить инициализацию всех дочерних категорий.
        /// Без инициализации они не будут работать.
        /// </summary>
        public override void InitAll()
        {
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            _state = WinCategoryState.Initialized;

            foreach (var category in _childCategories)
                ((IWinCategoryInitialization)category).InitCascade();
        }

        /// <summary>
        /// Удалить все инстансы в многоинстансовых категориях Windows
        /// </summary>
        public void RemoveAllInstances()
        {
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);


            foreach (var category in _childCategories)
                ((IWinCategoryInitialization)category).RemoveInstancesCascade();
        }

        /// <summary>
        /// Удалить все дочерние категории из Windows
        /// </summary>
        public void RemoveAllCategories()
        {
            if (_state == WinCategoryState.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            foreach (var category in _childCategories)
                ((IWinCategoryInitialization)category).RemoveCategoryCascade();

            this.Dispose();
        }

        /// <summary>
        /// Внутренний метод освобождения ресурсов
        /// </summary>
        /// <param name="isUserCall">Вызван ли явно</param>
        protected override void Dispose(bool isUserCall)
        {
            if (_state != WinCategoryState.Disposed)
            {
                _state = WinCategoryState.Disposed;

                foreach (var category in _childCategories)
                    ((IWinCategoryInitialization)category).DisposeCascade();
            }
        }
    }
}
