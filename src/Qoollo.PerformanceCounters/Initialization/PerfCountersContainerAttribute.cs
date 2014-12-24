using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Атрибут для пометки класса-контейнера счётчиков, 
    /// который должен содержать статический метод инициаилзации, помеченный атрибутом PerformanceCounterInitMethodAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PerfCountersContainerAttribute: Attribute
    {
    }
}
