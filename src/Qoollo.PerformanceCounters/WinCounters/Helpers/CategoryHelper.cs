using Qoollo.PerformanceCounters.WinCounters.Categories;
using Qoollo.PerformanceCounters.WinCounters.Categories.RecreateModeCategories;
using Qoollo.PerformanceCounters.WinCounters.Categories.UseOnlyExistedModeCategrories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Helpers
{
    /// <summary>
    /// Помошник для работы с категориями
    /// </summary>
    internal static class CategoryHelper
    {
        /// <summary>
        /// Проверка, все ли необходимые счётчики есть в категории Windows
        /// </summary>
        /// <param name="existed">Массив существующих счётчиков, полученный из категории Windows</param>
        /// <param name="requested">Перечень необходимых счётчиков</param>
        /// <returns>Все ли необходимые счётчики присутствуют</returns>
        public static bool IsAllRequestedCountersExist(PerformanceCounter[] existed, CounterCreationDataCollection requested)
        {
            Dictionary<string, PerformanceCounterType> check = existed.ToDictionary(o => o.CounterName, o => o.CounterType);
            for (int i = 0; i < requested.Count; i++)
            {
                PerformanceCounterType expectedType;
                if (!check.TryGetValue(requested[i].CounterName, out expectedType))
                    return false;

                if (requested[i].CounterType != expectedType)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Создать пустую категорию
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <param name="rootName">Корневое имя</param>
        /// <param name="wrkInfo">Информация по функционированию</param>
        /// <returns>Созданная категория</returns>
        public static WinEmptyCategory CreateEmptyCategory(string categoryName, string categoryDescription, string rootName, WinCountersWorkingInfo wrkInfo)
        {
            return new WinEmptyCategory(categoryName, categoryDescription, rootName, wrkInfo);
        }

        /// <summary>
        /// Создать простую категорию (с 1 инстансом).
        /// Конкретный тип определяется на основе wrkInfo.
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <param name="rootName">Корневое имя</param>
        /// <param name="wrkInfo">Информация по функционированию</param>
        /// <returns>Созданная категория</returns>
        public static WinSingleInstanceCategory CreateSingleInstanceCategory(string categoryName, string categoryDescription, string rootName, WinCountersWorkingInfo wrkInfo)
        {
            if (wrkInfo.InstantiationMode == WinCountersInstantiationMode.UseOnlyExisted)
                return new UseOnlyExistedWinSingleInstanceCategory(categoryName, categoryDescription, rootName, wrkInfo);

            return new RecreateWinSingleInstanceCategory(categoryName, categoryDescription, rootName, wrkInfo);
        }

        /// <summary>
        /// Создать категорию с множеством инстансов.
        /// Конкретный тип определяется на основе wrkInfo.
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <param name="rootName">Корневое имя</param>
        /// <param name="wrkInfo">Информация по функционированию</param>
        /// <returns>Созданная категория</returns>
        public static WinMultiInstanceCategory CreateMultiInstanceCategory(string categoryName, string categoryDescription, string rootName, WinCountersWorkingInfo wrkInfo)
        {
            if (wrkInfo.InstantiationMode == WinCountersInstantiationMode.UseOnlyExisted)
                return new UseOnlyExistedWinMultiInstanceCategory(categoryName, categoryDescription, rootName, wrkInfo);

            return new RecreateWinMultiInstanceCategory(categoryName, categoryDescription, rootName, wrkInfo);
        }
    }
}
