using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.GraphiteCounters.Net
{
    /// <summary>
    /// API для Graphite
    /// </summary>
    internal interface IGraphiteCountersAPI
    {
        /// <summary>
        /// Переслать значения счётчиков
        /// </summary>
        /// <param name="time">Время выборки значений</param>
        /// <param name="counters">Значения счётчиков</param>
        void SendCounterValues(DateTime time, List<GraphiteCounterData> counters);
    }
}
