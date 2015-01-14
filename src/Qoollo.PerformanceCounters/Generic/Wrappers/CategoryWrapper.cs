using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Обёртка для категории
    /// </summary>
    public abstract class CategoryWrapper: IDisposable
    {
        /// <summary>
        /// Инициализировать обёртку в подчинённой сборке
        /// </summary>
        /// <param name="category">Текущая обёртка</param>
        /// <param name="assembly">Сборка, в которой проводится инициализация</param>
        /// <returns>Число проинициализированных контейнеров счётчиков</returns>
        protected static int InitializeCategoryWrapper(CategoryWrapper category, Assembly assembly)
        {
            return Initialization.Initializer.InitializeCategoryWrapper(category, assembly);
        }

        // ===========================

        /// <summary>
        /// Категория
        /// </summary>
        private Category _category;

        /// <summary>
        /// Конструктор CategoryWrapper
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <param name="type">Тип категории</param>
        internal CategoryWrapper(string name, string description, CategoryTypes type)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            Type = type;
            Name = name;
            Description = description;
        }
        /// <summary>
        /// Конструктор CategoryWrapper
        /// </summary>
        /// <param name="type">Тип категории</param>
        internal CategoryWrapper(CategoryTypes type)
        {
            Type = type;
            Name = this.GetType().FullName;
            Description = Name;
        }



        /// <summary>
        /// Имя категории
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Информация о категории
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Тип категории
        /// </summary>
        public CategoryTypes Type { get; private set; }


        /// <summary>
        /// Замена имени и описания
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="description">Описание</param>
        internal void SetNameDescription(string name, string description)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (description == null)
                throw new ArgumentNullException("description");

            Name = name;
            Description = description;
        }

        /// <summary>
        /// Внутренний метод инициализации
        /// </summary>
        /// <param name="category">Обёртываемая категория</param>
        internal void Init(Category category)
        {
            if (category == null)
                throw new ArgumentNullException("category");

            _category = category;

            InitCategory(category);
            InitCounters();
            AfterInit();
        }

        /// <summary>
        /// Установить обёртываемую категорию (внутренний метод)
        /// </summary>
        /// <param name="category">Обёртываемая категория</param>
        internal abstract void InitCategory(Category category);
        /// <summary>
        /// Инициализация счётчиков (внутренний метод)
        /// </summary>
        internal abstract void InitCounters();

        /// <summary>
        /// Функция вызывается после инициализации категории, когда все счётчики заполнены
        /// </summary>
        protected virtual void AfterInit()
        {
        }


        /// <summary>
        /// Создание обертки вложенной категории счетчиков вместе со всеми счетчиками.
        /// Счетчики должны быть одного из типов счетчиков
        /// и должны быть помечены атрибутами CounterAttributes
        /// </summary>
        /// <typeparam name="T">Тип обёртки</typeparam>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание</param>
        /// <returns>Созданная обёртка</returns>
        /// <exception cref="InvalidCounterDefinitionException">При неверном описании счётчика в обёртке</exception>
        public T CreateSubCategory<T>(string name, string description) where T : CategoryWrapper, new()
        {
            var wrapper = new T();
            wrapper.SetNameDescription(name ?? wrapper.Name, description ?? wrapper.Description);

            switch (wrapper.Type)
            {
                case CategoryTypes.Empty:
                    var empty = _category.CreateEmptySubCategory(wrapper.Name, wrapper.Description);
                    wrapper.Init(empty);
                    break;
                case CategoryTypes.SingleInstance:
                    var single = _category.CreateSingleInstanceSubCategory(wrapper.Name, wrapper.Description);
                    wrapper.Init(single);
                    break;
                case CategoryTypes.MultiInstance:
                    var multi = _category.CreateMultiInstanceSubCategory(wrapper.Name, wrapper.Description);
                    wrapper.Init(multi);
                    break;
                default:
                    throw new InvalidOperationException("Unknown CategoryTypes value: " + wrapper.Type.ToString());
            }

            return wrapper;
        }

        /// <summary>
        /// Создание обертки вложенной категории счетчиков вместе со всеми счетчиками.
        /// Счетчики должны быть одного из типов счетчиков
        /// и должны быть помечены атрибутами CounterAttributes
        /// </summary>
        /// <typeparam name="T">Тип обёртки</typeparam>
        /// <returns>Созданная обёртка</returns>
        /// <exception cref="InvalidCounterDefinitionException">При неверном описании счётчика в обёртке</exception>
        public T CreateSubCategory<T>() where T : CategoryWrapper, new()
        {
            return CreateSubCategory<T>(null, null);
        }

        /// <summary>
        /// Создание обёртки для пустой вложенной категории
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание категории</param>
        /// <returns>Созданная обёртка</returns>
        public EmptyCategoryWrapper CreateEmptySubCategory(string name, string description)
        {
            var wrapper = new EmptyCategoryWrapper(name, description);
            var empty = _category.CreateEmptySubCategory(name, description);
            wrapper.Init(empty);
            return wrapper;
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        /// <param name="isUserCall">Вызвано ли явно</param>
        protected virtual void Dispose(bool isUserCall)
        {
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            if (_category != null)
                _category.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}


