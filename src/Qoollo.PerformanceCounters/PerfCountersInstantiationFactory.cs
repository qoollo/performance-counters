using Qoollo.PerformanceCounters.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Фабрика для инстанцирования фабрик счётчиков
    /// </summary>
    public static class PerfCountersInstantiationFactory
    {
        /// <summary>
        /// Загрузить из конфига и создать фабрику счётчиков
        /// </summary>
        /// <param name="sectionName">Имя секции</param>
        /// <returns>Созданная фабрика счётчиков</returns>
        public static CounterFactory CreateCounterFactoryFromAppConfig(string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            var config = Configurator.LoadConfiguration(sectionName);

            if (config == null)
                throw new PerformanceCountersConfigurationException("Counters configuration error");

            return CreateCounterFactory(config);
        }
        /// <summary>
        /// Загрузить из конфига и создать фабрику счётчиков
        /// </summary>
        /// <param name="sectionGroupName">Имя группы секций</param>
        /// <param name="sectionName">Имя секции</param>
        /// <returns>Созданная фабрика счётчиков</returns>
        public static CounterFactory CreateCounterFactoryFromAppConfig(string sectionGroupName, string sectionName)
        {
            if (sectionGroupName == null)
                throw new ArgumentNullException("sectionGroupName");
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            var config = Configurator.LoadConfiguration(sectionGroupName, sectionName);

            if (config == null)
                throw new PerformanceCountersConfigurationException("Counters configuration error");

            return CreateCounterFactory(config);
        }




        /// <summary>
        /// Создать фабрику счётчиков на основе конфигурации
        /// </summary>
        /// <param name="config">Конфигурация</param>
        /// <returns>Созданная фабрика счётчиков</returns>
        public static CounterFactory CreateCounterFactory(PerfCountersConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            switch (config.CounterType)
            {
                case Configuration.PerfCountersTypeEnum.NullCounters:
                    return NullCounters.NullCounterFactory.Instance;

                case Configuration.PerfCountersTypeEnum.InternalCounters:
                    return new InternalCounters.InternalCounterFactory((InternalCountersConfiguration)config);

                case Configuration.PerfCountersTypeEnum.WinCounters:
                    return new WinCounters.WinCounterFactory((WinCountersConfiguration)config);

                case Configuration.PerfCountersTypeEnum.CompositeCounters:
                    return new CompositeCounters.CompositeCounterFactory((CompositeCountersConfiguration)config);

                case PerfCountersTypeEnum.NetCounters:
                    return new NetCounters.NetCounterFactory((NetCountersConfiguration)config);

                case PerfCountersTypeEnum.GraphiteCounters:
                    return new GraphiteCounters.GraphiteCounterFactory((GraphiteCountersConfiguration)config);

                default:
                    throw new NotImplementedException("Unknown performance counters type: " + config.CounterType.ToString());
            }
        }
    }
}
