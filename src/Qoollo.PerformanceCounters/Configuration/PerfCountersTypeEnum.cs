using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.Configuration
{
    /// <summary>
    /// Тип счётчиков
    /// </summary>
    public enum PerfCountersTypeEnum
    {
        /// <summary>
        /// Пустые счётчики (не считают)
        /// </summary>
        NullCounters,
        /// <summary>
        /// Внутренние счётчики (доступны в рамках приложения)
        /// </summary>
        InternalCounters,
        /// <summary>
        /// Счётчики Windows
        /// </summary>
        WinCounters,
        /// <summary>
        /// Композитные счётчики (агрегируют несколько разных типов)
        /// </summary>
        CompositeCounters,
        /// <summary>
        /// Сетевые счётчики
        /// </summary>
        NetCounters,
        /// <summary>
        /// Счётчики Graphite
        /// </summary>
        GraphiteCounters
    }
}
