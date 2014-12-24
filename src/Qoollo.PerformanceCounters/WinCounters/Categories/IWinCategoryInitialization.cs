using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Categories
{
    /// <summary>
    /// Интерфейс для категорий Windows в помощь при инициализации.
    /// </summary>
    internal interface IWinCategoryInitialization
    {
        /// <summary>
        /// Инициализировать данную категорию
        /// </summary>
        void Init();
        /// <summary>
        /// Инициализировать данную и все дочерние категории
        /// </summary>
        void InitCascade();

        /// <summary>
        /// Освободить данную категорию
        /// </summary>
        void Dispose();
        /// <summary>
        /// Освободить данную и все дочерние категории
        /// </summary>
        void DisposeCascade();


        /// <summary>
        /// Удалить данную категорию из Windows
        /// </summary>
        void RemoveCategory();
        /// <summary>
        /// Удалить данную и все дочерние категории из Windows
        /// </summary>
        void RemoveCategoryCascade();


        /// <summary>
        /// Удалить все инстансы в многоинстансовой категории
        /// </summary>
        void RemoveInstances();
        /// <summary>
        /// Удалить все инстансы в многоинстансовой категории каскадом
        /// </summary>
        void RemoveInstancesCascade();
    }
}
