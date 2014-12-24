using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters
{
    /// <summary>
    /// Состояние счётчика в WinCounters
    /// </summary>
    public enum WinCounterState
    {
        /// <summary>
        /// Создан
        /// </summary>
        Created,
        /// <summary>
        /// Проинициализирован
        /// </summary>
        Initialized,
        /// <summary>
        /// Освобождён
        /// </summary>
        Disposed
    }
}
