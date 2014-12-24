using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Стандартный общий контейнер счётчиков.
    /// Для простейших ситуаций.
    /// </summary>
    public class PerfCountersContainer
    {
        /// <summary>
        /// Создание обертки категории счетчиков вместе со всеми счетчиками при помощи NullCounters.
        /// </summary>
        /// <typeparam name="T">Тип обёртки</typeparam>
        /// <returns>Созданная обёртка</returns>
        /// <exception cref="InvalidCounterDefinitionException">При неверном описании счётчика в обёртке</exception>
        protected static T CreateNullCategoryWrapper<T>() where T : CategoryWrapper, new()
        {
            return NullCounters.NullCounterFactory.Instance.CreateCategoryWrapper<T>();
        }




        /// <summary>
        /// Инициализировать счётчики в сборке.
        /// Ищет все типы, унаследованные от PerfCountersContainer или помеченные атрибутом PerformanceCounterContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerformanceCounterInitMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Сборка для сканирования</param>
        protected static void InitializeCountersInAssembly(Category category, Assembly assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            Initialization.Initializer.InitializeCategory(category, assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в сборках.
        /// Ищет все типы, унаследованные от PerfCountersContainer или помеченные атрибутом PerformanceCounterContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerformanceCounterInitMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Перечень сборок</param>
        protected static void InitializeCountersInAssembly(Category category, params Assembly[] assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            Initialization.Initializer.InitializeCategory(category, assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в сборке.
        /// Ищет все типы, унаследованные от PerfCountersContainer или помеченные атрибутом PerformanceCounterContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerformanceCounterInitMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="type">Тип внутри сборки со счётчиками</param>
        protected static void InitializeCountersInAssembly(Category category, Type type)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (type == null)
                throw new ArgumentNullException("type");

            Initialization.Initializer.InitializeCategory(category, type.Assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в сборках.
        /// Ищет все типы, унаследованные от PerfCountersContainer или помеченные атрибутом PerformanceCounterContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerformanceCounterInitMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="types">Перечень типов внутри сборок со счётчиками</param>
        protected static void InitializeCountersInAssembly(Category category, params Type[] types)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (types == null)
                throw new ArgumentNullException("type");

            Initialization.Initializer.InitializeCategory(category, types.Select(o => o.Assembly));
        }









        /// <summary>
        /// Инициализировать счётчики в сборке (для обёрток).
        /// Ищет все типы, унаследованные от PerfCountersContainer или помеченные атрибутом PerformanceCounterContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerformanceCounterInitMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Сборка для сканирования</param>
        protected static void InitializeCountersInAssembly(CategoryWrapper category, Assembly assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            Initialization.Initializer.InitializeCategoryWrapper(category, assembly);
        }


        /// <summary>
        /// Инициализировать счётчики в сборках (для обёрток).
        /// Ищет все типы, унаследованные от PerfCountersContainer или помеченные атрибутом PerformanceCounterContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerformanceCounterInitMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Перечень сборок</param>
        protected static void InitializeCountersInAssembly(CategoryWrapper category, params Assembly[] assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            Initialization.Initializer.InitializeCategoryWrapper(category, assembly);
        }


        /// <summary>
        /// Инициализировать счётчики в сборке (для обёрток).
        /// Ищет все типы, унаследованные от PerfCountersContainer или помеченные атрибутом PerformanceCounterContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerformanceCounterInitMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="type">Тип внутри сборки со счётчиками</param>
        protected static void InitializeCountersInAssembly(CategoryWrapper category, Type type)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (type == null)
                throw new ArgumentNullException("type");

            Initialization.Initializer.InitializeCategoryWrapper(category, type.Assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в сборках (для обёрток).
        /// Ищет все типы, унаследованные от PerfCountersContainer или помеченные атрибутом PerformanceCounterContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerformanceCounterInitMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="types">Перечень типов внутри сборок со счётчиками</param>
        protected static void InitializeCountersInAssembly(CategoryWrapper category, params Type[] types)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (types == null)
                throw new ArgumentNullException("types");

            Initialization.Initializer.InitializeCategoryWrapper(category, types.Select(o => o.Assembly));
        }
    }
}
