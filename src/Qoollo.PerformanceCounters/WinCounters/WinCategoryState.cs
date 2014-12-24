using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters
{
    /// <summary>
    /// Состояние категории в WinCounters
    /// </summary>
    public enum WinCategoryState
    {
        /// <summary>
        /// Категория создана
        /// </summary>
        Created,
        /// <summary>
        /// Категория проинициализирована и готова к использованию
        /// </summary>
        Initialized,
        /// <summary>
        /// Категория освобождена
        /// </summary>
        Disposed
    }
}
