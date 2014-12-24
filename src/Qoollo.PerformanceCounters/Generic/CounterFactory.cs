using System;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Базовый класс для фабрики счётчиков
    /// </summary>
    public abstract class CounterFactory: IDisposable
    {
        /// <summary>
        /// Создание пустой категории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public abstract EmptyCategory CreateEmptyCategory(string categoryName, string categoryDescription);

        /// <summary>
        /// Создание категории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public abstract MultiInstanceCategory CreateMultiInstanceCategory(string categoryName, string categoryDescription);

        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public abstract SingleInstanceCategory CreateSingleInstanceCategory(string categoryName, string categoryDescription);


        /// <summary>
        /// Создание обертки категории счетчиков вместе со всеми счетчиками.
        /// Счетчики должны быть одного из типов счетчиков
        /// и также должны быть помечены атрибутами CounterAttributes
        /// </summary>
        /// <typeparam name="T">Тип обёртки</typeparam>
        /// <returns>Созданная обёртка</returns>
        /// <exception cref="InvalidCounterDefinitionException">При неверном описании счётчика в обёртке</exception>
        public T CreateCategoryWrapper<T>() where T : CategoryWrapper, new()
        {
            var wrapper = new T();

            switch (wrapper.Type)
            {
                case CategoryTypes.Empty:
                    var empty = this.CreateEmptyCategory(wrapper.Name, wrapper.Description);
                    wrapper.Init(empty);
                    break;
                case CategoryTypes.SingleInstance:
                    var single = this.CreateSingleInstanceCategory(wrapper.Name, wrapper.Description);
                    wrapper.Init(single);
                    break;
                case CategoryTypes.MultiInstance:
                    var multi = this.CreateMultiInstanceCategory(wrapper.Name, wrapper.Description);
                    wrapper.Init(multi);
                    break;
                default:
                    throw new InvalidOperationException("Unknown CategoryTypes value: " + wrapper.Type.ToString());
            }

            return wrapper;
        }



        /// <summary>
        /// Создаёт корневой пустой враппер для инициаилизации
        /// </summary>
        /// <param name="name">Имя категории</param>
        /// <param name="description">Описание</param>
        /// <returns>Корневой враппер</returns>
        public EmptyCategoryWrapper CreateRootWrapper(string name, string description)
        {
            var result = new EmptyCategoryWrapper(name, description);
            var emptyCat = this.CreateEmptyCategory(name, description);
            result.Init(emptyCat);
            return result;
        }

        /// <summary>
        /// Создаёт корневой пустой враппер для инициаилизации
        /// </summary>
        /// <returns>Корневой враппер</returns>
        public EmptyCategoryWrapper CreateRootWrapper()
        {
            return CreateRootWrapper("", "root");
        }


        /// <summary>
        /// Инициализация счетчиков во всех дочерних категориях (создание категорий, инстанцирование самих счетчиков)
        /// </summary>
        public virtual void InitAll()
        {
        }

        /// <summary>
        /// Обойти все фабрики указанного типа
        /// </summary>
        /// <typeparam name="TFactory">Тип фабрик для обхода</typeparam>
        /// <param name="action">Действие, выполняемое при входе в фабрику указанного типа</param>
        public virtual void WalkFactories<TFactory>(Action<TFactory> action) where TFactory: CounterFactory
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var type = this.GetType();
            if (type == typeof(TFactory) || type.IsSubclassOf(typeof(TFactory)))
                action((TFactory)this);
        }


        /// <summary>
        /// Внутренний код освобождения ресурсов
        /// </summary>
        /// <param name="isUserCall">Вызвано ли пользователем</param>
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