using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.Initialization
{
    /// <summary>
    /// Каскадный инициализатор
    /// </summary>
    public static class Initializer
    {
        /// <summary>
        /// Находит в сборке все классы, помеченные атрибутом PerfCountersContainerAttribute или наследники PerfCountersContainer или наследники категории
        /// </summary>
        /// <param name="asm">Сборка для поиска</param>
        /// <returns>Список найденных классов</returns>
        private static List<Type> FindAllPerfCountersInAsmForBase(Assembly asm)
        {
            try
            {
                return asm.GetTypes().Where(o => 
                    o.IsSubclassOf(typeof(PerfCountersContainer)) ||
                    o.IsSubclassOf(typeof(Category)) ||
                    o.IsDefined(typeof(PerfCountersContainerAttribute), false)).ToList();
            }
            catch (ReflectionTypeLoadException tpLdEx)
            {
                throw new TypeLoadException("Can't load assembly [" + asm.ToString() + "]", tpLdEx);
            }
        }

        /// <summary>
        /// Находит в сборке все классы, помеченные атрибутом PerfCountersContainerAttribute или наследники PerfCountersContainer или наследники категории
        /// </summary>
        /// <param name="asm">Сборка для поиска</param>
        /// <returns>Список найденных классов</returns>
        private static List<Type> FindAllPerfCountersInAsmForWrapper(Assembly asm)
        {
            try
            {
                return asm.GetTypes().Where(o => 
                    o.IsSubclassOf(typeof(PerfCountersContainer)) || 
                    o.IsSubclassOf(typeof(CategoryWrapper)) ||
                    o.IsDefined(typeof(PerfCountersContainerAttribute), false)).ToList();
            }
            catch (ReflectionTypeLoadException tpLdEx)
            {
                throw new TypeLoadException("Can't load assembly [" + asm.ToString() + "]", tpLdEx);
            }
        }

        /// <summary>
        /// Помечен ли указанный метод атрибутом, указывающим на возможность его использования при инициализации
        /// </summary>
        /// <param name="method">Метод</param>
        /// <returns>Помечен ли атрибутом</returns>
        private static bool IsMethodMarkedWithInitAttribute(MethodInfo method)
        {
            return method.IsDefined(typeof(PerfCountersInitializationMethodAttribute), false);
        }

        /// <summary>
        /// Подходит ли указанный метод для инициализации счётчиков в базовом варианте (через типы Category)
        /// </summary>
        /// <param name="method">Метод</param>
        /// <returns>Подходит ли</returns>
        private static bool IsAppropriateInitMethodForBase(MethodInfo method)
        {
            if (!method.IsDefined(typeof(PerfCountersInitializationMethodAttribute), false))
                return false;

            var mParam = method.GetParameters();
            if (mParam.Length != 1)
                return false;

            return mParam[0].ParameterType == typeof(Category);
        }
        /// <summary>
        /// Подходит ли указанный метод для инициализации счётчиков в виде обёрток (через типы CategoryWrapper)
        /// </summary>
        /// <param name="method">Метод</param>
        /// <returns>Подходит ли</returns>
        private static bool IsAppropriateInitMethodForWrapper(MethodInfo method)
        {
            if (!method.IsDefined(typeof(PerfCountersInitializationMethodAttribute), false))
                return false;

            var mParam = method.GetParameters();
            if (mParam.Length != 1)
                return false;

            return mParam[0].ParameterType == typeof(CategoryWrapper);
        }

        /// <summary>
        /// Извлечение из типа метода инициализации счётчиков (метод должен быть помечен атрибутом PerformanceCounterInitMethodAttribute)
        /// </summary>
        /// <param name="tp">Тип</param>
        /// <returns>Метод, если найден. Иначе null.</returns>
        private static MethodInfo ExtractInitializationMethodForBase(Type tp)
        {
            var allMeth = tp.GetMethods(BindingFlags.Static | BindingFlags.Public);
            if (allMeth == null || allMeth.Length == 0)
                return null;

            var filteredMethods = allMeth.Where(o => IsMethodMarkedWithInitAttribute(o)).ToList();
            if (filteredMethods.Count == 0)
                return null;

            MethodInfo selectedMethod = filteredMethods[0];
            if (!IsAppropriateInitMethodForBase(selectedMethod))
                throw new PerformanceCountersException("Method marked with 'PerformanceCounterInitMethodAttribute' has some invalid arguments: " + selectedMethod.ToString());

            return selectedMethod;
        }
        /// <summary>
        /// Извлечение из типа метода инициализации счётчиков (метод должен быть помечен атрибутом PerformanceCounterInitMethodAttribute)
        /// </summary>
        /// <param name="tp">Тип</param>
        /// <returns>Метод, если найден. Иначе null.</returns>
        private static MethodInfo ExtractInitializationMethodForWrapper(Type tp)
        {
            var allMeth = tp.GetMethods(BindingFlags.Static | BindingFlags.Public);
            if (allMeth == null || allMeth.Length == 0)
                return null;

            var filteredMethods = allMeth.Where(o => IsMethodMarkedWithInitAttribute(o)).ToList();
            if (filteredMethods.Count == 0)
                return null;

            MethodInfo selectedMethod = filteredMethods[0];
            if (!IsAppropriateInitMethodForWrapper(selectedMethod))
                throw new PerformanceCountersException("Method marked with 'PerformanceCounterInitMethodAttribute' has some invalid arguments: " + selectedMethod.ToString());

            return selectedMethod;
        }


        /// <summary>
        /// Выполнить инициализацию счётчиков путём вызова метода и передачи category
        /// </summary>
        /// <param name="method">Метод инициализации</param>
        /// <param name="category">Категория</param>
        private static void InitPerfCountersBase(MethodInfo method, Category category)
        {
            var allParams = method.GetParameters();

            object[] parameters = new object[allParams.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (allParams[i].ParameterType == typeof(Category))
                    parameters[i] = category;
                else
                    throw new ArgumentException("Bad method description");
            }

            method.Invoke(null, parameters);
        }

        /// <summary>
        /// Выполнить инициализацию счётчиков путём вызова метода и передачи category
        /// </summary>
        /// <param name="method">Метод инициализации</param>
        /// <param name="category">Категория</param>
        private static void InitPerfCountersWrapper(MethodInfo method, CategoryWrapper category)
        {
            var allParams = method.GetParameters();

            object[] parameters = new object[allParams.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                if (allParams[i].ParameterType == typeof(CategoryWrapper))
                    parameters[i] = category;
                else
                    throw new ArgumentException("Bad method description");
            }

            method.Invoke(null, parameters);
        }


        /// <summary>
        /// Инициализировать счётчики в сборке.
        /// Ищет все типы, унаследованные от PerfCountersContainer или от Category или помеченные атрибутом PerfCountersContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerfCountersInitializationMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Сборка для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int InitializeCategory(Category category, Assembly assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var allInitMethods = FindAllPerfCountersInAsmForBase(assembly).Select(o => ExtractInitializationMethodForBase(o)).Where(o => o != null).ToList();

            foreach (var elem in allInitMethods)
                InitPerfCountersBase(elem, category);

            return allInitMethods.Count;
        }

        /// <summary>
        /// Инициализировать счётчики в сборках.
        /// Ищет все типы, унаследованные от PerfCountersContainer или от Category или помеченные атрибутом PerfCountersContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerfCountersInitializationMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Перечень сборок</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int InitializeCategory(Category category, IEnumerable<Assembly> assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            int res = 0;

            foreach (var asm in assembly)
                res += InitializeCategory(category, asm);

            return res;
        }


        /// <summary>
        /// Инициализировать счётчики в сборке (для обёрток).
        /// Ищет все типы, унаследованные от PerfCountersContainer или от CategoryWrapper или помеченные атрибутом PerfCountersContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerfCountersInitializationMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Сборка для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int InitializeCategoryWrapper(CategoryWrapper category, Assembly assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var allInitMethods = FindAllPerfCountersInAsmForWrapper(assembly).Select(o => ExtractInitializationMethodForWrapper(o)).Where(o => o != null).ToList();

            foreach (var elem in allInitMethods)
                InitPerfCountersWrapper(elem, category);

            return allInitMethods.Count;
        }


        /// <summary>
        /// Инициализировать счётчики в сборках (для обёрток).
        /// Ищет все типы, унаследованные от PerfCountersContainer или от CategoryWrapper или помеченные атрибутом PerfCountersContainerAttribute и 
        /// содержащие метод инициализации, помеченный атрибутом PerfCountersInitializationMethodAttribute
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Перечень сборок</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int InitializeCategoryWrapper(CategoryWrapper category, IEnumerable<Assembly> assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            int res = 0;

            foreach (var asm in assembly)
                res += InitializeCategoryWrapper(category, asm);

            return res;
        }







        /// <summary>
        /// Инициализировать счётчики в других сборках как дочерние
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Сборка для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int PassForInitializationToAssembly(this Category category, Assembly assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return InitializeCategory(category, assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в других сборках как дочерние
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Сборки для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int PassForInitializationToAssembly(this Category category, params Assembly[] assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return InitializeCategory(category, assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в других сборках как дочерние
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="type">Тип из сборки для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int PassForInitializationToAssembly(this Category category, Type type)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (type == null)
                throw new ArgumentNullException("type");

            return InitializeCategory(category, type.Assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в других сборках как дочерние
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="types">Типы из сборок для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int PassForInitializationToAssembly(this Category category, params Type[] types)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (types == null)
                throw new ArgumentNullException("types");

            return InitializeCategory(category, types.Select(o => o.Assembly));
        }




        /// <summary>
        /// Инициализировать счётчики в других сборках как дочерние
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Сборка для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int PassForInitializationToAssembly(this CategoryWrapper category, Assembly assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return InitializeCategoryWrapper(category, assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в других сборках как дочерние
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="assembly">Сборки для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int PassForInitializationToAssembly(this CategoryWrapper category, params Assembly[] assembly)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return InitializeCategoryWrapper(category, assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в других сборках как дочерние
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="type">Тип из сборки для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int PassForInitializationToAssembly(this CategoryWrapper category, Type type)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (type == null)
                throw new ArgumentNullException("type");

            return InitializeCategoryWrapper(category, type.Assembly);
        }

        /// <summary>
        /// Инициализировать счётчики в других сборках как дочерние
        /// </summary>
        /// <param name="category">Родительская категория</param>
        /// <param name="types">Типы из сборок для сканирования</param>
        /// <returns>Число классов, которые были проинициализированы</returns>
        public static int PassForInitializationToAssembly(this CategoryWrapper category, params Type[] types)
        {
            if (category == null)
                throw new ArgumentNullException("category");
            if (types == null)
                throw new ArgumentNullException("types");

            return InitializeCategoryWrapper(category, types.Select(o => o.Assembly));
        }
    }
}
