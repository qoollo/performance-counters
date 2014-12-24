using Qoollo.PerformanceCounters.NullCounters.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NullCounters
{
    /// <summary>
    /// Фабрика для счётчиков, которые не считают.
    /// Можно использовать для создания счётчиков, когда они не нужны, но нужно избежать NullReferenceException
    /// </summary>
    public class NullCounterFactory: CounterFactory
    {
        private static NullCounterFactory _instance = null;
        private static readonly object _syncObj = new object();

        /// <summary>
        /// Инстанс фабрики
        /// </summary>
        public static NullCounterFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncObj)
                    {
                        if (_instance == null)
                            _instance = new NullCounterFactory();
                    }
                }
                return _instance;
            }
        }

        // =============

        /// <summary>
        /// Конструктор NullCounterFactory
        /// </summary>
        private NullCounterFactory() { }


        /// <summary>
        /// Создание пустой категории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override EmptyCategory CreateEmptyCategory(string categoryName, string categoryDescription)
        {
            return new NullEmptyCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание категории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override MultiInstanceCategory CreateMultiInstanceCategory(string categoryName, string categoryDescription)
        {
            return new NullMultiInstanceCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override SingleInstanceCategory CreateSingleInstanceCategory(string categoryName, string categoryDescription)
        {
            return new NullSingleInstanceCategory(categoryName, categoryDescription);
        }
    }
}
