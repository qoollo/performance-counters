using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Атрибут для указания метода инициализации счётчиков.
    /// Метод должен быть статическим и иметь 1 параметр для приёма родительской категории
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class PerfCountersInitializationMethodAttribute : Attribute
    {
    }
}
