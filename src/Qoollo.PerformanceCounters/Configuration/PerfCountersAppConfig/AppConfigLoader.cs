using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.Configuration.PerfCountersAppConfig
{
    internal static class AppConfigLoader
    {
        /// <summary>
        /// Возвращает конфигурацию счётчиков из конфигурационного файла
        /// </summary>
        /// <param name="sectionName">Имя конфигурационной секции в AppConfig </param>
        /// <returns>Счётчики</returns>
        internal static IPerfCountersConfigurationSection LoadSection(string sectionName)
        {
            var cfgSec = (Qoollo.PerformanceCounters.Configuration.PerfCountersConfigurationSectionConfigClass)ConfigurationManager.GetSection(sectionName);

            return cfgSec.ExtractConfigData();
        }

        /// <summary>
        /// Возвращает конфигурацию счётчиков из конфигурационного файла
        /// </summary>
        /// <param name="sectionGroup">Имя группы секций в AppConfig </param>
        /// <param name="sectionName">Имя конфигурационной секции в AppConfig </param>
        /// <returns>Счётчики</returns>
        internal static IPerfCountersConfigurationSection LoadSection(string sectionGroup, string sectionName)
        {
            var cfgSec = (Qoollo.PerformanceCounters.Configuration.PerfCountersConfigurationSectionConfigClass)ConfigurationManager.GetSection(string.Format("{0}/{1}", sectionGroup, sectionName));

            return cfgSec.ExtractConfigData();
        }

        /// <summary>
        /// Возвращает конфигурацию счётчиков из конфигурационного файла
        /// </summary>
        /// <param name="sectionName">Имя конфигурационной секции в AppConfig </param>
        /// <returns>Счётчики</returns>
        public static PerfCountersConfiguration GetConfiguration(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            var section = LoadSection(sectionName);

            if (section == null)
                throw new PerformanceCountersConfigurationException("Performance counters config section was not loaded");

            var configuration = ConfigurationFormatConverter.Convert(section);

            if (configuration == null)
                throw new PerformanceCountersConfigurationException("Performance counters config can't be converted");

            return configuration;
        }


        /// <summary>
        /// Возвращает конфигурацию счётчиков из конфигурационного файла
        /// </summary>
        /// <param name="sectionGroupName">Имя группы секций в AppConfig </param>
        /// <param name="sectionName">Имя конфигурационной секции в AppConfig </param>
        /// <returns>Счётчики</returns>
        public static PerfCountersConfiguration GetConfiguration(string sectionGroupName, string sectionName)
        {
            return GetConfiguration(string.Format("{0}/{1}", sectionGroupName, sectionName));
        }
    }
}
