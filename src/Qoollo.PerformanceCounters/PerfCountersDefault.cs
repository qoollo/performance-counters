using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters
{
    /// <summary>
    /// Контейнер стандартных счётчиков
    /// </summary>
    public static class PerfCountersDefault
    {
        private static CounterFactory _default;
        private static readonly object _syncObj = new object();


        /// <summary>
        /// Фабрика для NullCounters
        /// </summary>
        public static NullCounters.NullCounterFactory NullCounterFactory
        {
            get { return NullCounters.NullCounterFactory.Instance; }
        }


        /// <summary>
        /// Инстанс фабрики счётчиков.
        /// По умолчанию тут InternalCounterFactory
        /// </summary>
        public static CounterFactory DefaultFactory
        {
            get
            {
                if (_default == null)
                {
                    lock (_syncObj)
                    {
                        if (_default == null)
                            _default = new InternalCounters.InternalCounterFactory();
                    }
                }
                return _default;
            }
        }


        /// <summary>
        /// Задать новое значение инстанса фабрики счётчиков
        /// </summary>
        /// <param name="factory">Фабрика счётчиков</param>
        public static void SetDefaultFactory(CounterFactory factory)
        {
            if (factory == null)
                factory = new InternalCounters.InternalCounterFactory();

            CounterFactory oldVal = null;
            lock (_syncObj)
            {
                oldVal = System.Threading.Interlocked.Exchange(ref _default, factory);
            }

            if (oldVal != null)
                oldVal.Dispose();
        }

        /// <summary>
        /// Сбросить значение фабрики счётчиков.
        /// Возвращает InternalCounterFactory.
        /// </summary>
        public static void ResetDefaultFactory()
        {
            SetDefaultFactory(new InternalCounters.InternalCounterFactory());
        }


        /// <summary>
        /// Загрузить фабрику счётчиков из файла конфигурации
        /// </summary>
        /// <param name="sectionName">Имя секции в файле конфигурации</param>
        public static void LoadDefaultFactoryFromAppConfig(string sectionName = "PerfCountersConfigurationSection")
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName");

            var counters = PerfCountersInstantiationFactory.CreateCounterFactoryFromAppConfig(sectionName);
            SetDefaultFactory(counters);
        }
    }
}
