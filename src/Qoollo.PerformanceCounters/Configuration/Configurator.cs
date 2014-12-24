using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.Configuration
{
    /// <summary>
    /// Загрузка конфигурации счётчиков
    /// </summary>
    public static class Configurator
    {
        /// <summary>
        /// Функция для загрузки конфигурации счётчиков
        /// </summary>
        public static PerfCountersConfiguration LoadConfiguration(string sectionName)
        {
            return Qoollo.PerformanceCounters.Configuration.PerfCountersAppConfig.AppConfigLoader.GetConfiguration(sectionName);
        }
        /// <summary>
        /// Функция для загрузки конфигурации счётчиков
        /// </summary>
        public static PerfCountersConfiguration LoadConfiguration(string sectionGroupName, string sectionName)
        {
            return Qoollo.PerformanceCounters.Configuration.PerfCountersAppConfig.AppConfigLoader.GetConfiguration(sectionGroupName, sectionName);
        }
    }
}
