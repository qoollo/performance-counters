using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.Configuration
{
    /// <summary>
    /// Базовый класс для конфигураций счётчиков
    /// </summary>
    public abstract class PerfCountersConfiguration
    {
        /// <summary>
        /// Конструктор PerfCountersConfiguration
        /// </summary>
        /// <param name="type">Тип счётчиков</param>
        public PerfCountersConfiguration(PerfCountersTypeEnum type)
        {
            CounterType = type;
        }

        /// <summary>
        /// Тип счётчиков
        /// </summary>
        public PerfCountersTypeEnum CounterType { get; private set; }
    }


    /// <summary>
    /// Конфигурация для композитных счётчиков
    /// </summary>
    public class CompositeCountersConfiguration : PerfCountersConfiguration
    {
        /// <summary>
        /// Конструктор CompositeCountersConfiguration
        /// </summary>
        /// <param name="wrappedCounters">Конфигурации для агрегируемых счётчиков</param>
        public CompositeCountersConfiguration(IEnumerable<PerfCountersConfiguration> wrappedCounters)
            : base(PerfCountersTypeEnum.CompositeCounters)
        {
            if (wrappedCounters == null)
                WrappedCounters = new List<PerfCountersConfiguration>();
            else
                WrappedCounters = new List<PerfCountersConfiguration>(wrappedCounters);
        }
        /// <summary>
        /// Конструктор CompositeCountersConfiguration
        /// </summary>
        public CompositeCountersConfiguration()
            : this(null)
        {
        }

        /// <summary>
        /// Конфигурации агрегируемых счётчиков
        /// </summary>
        public List<PerfCountersConfiguration> WrappedCounters { get; private set; }
    }

    /// <summary>
    /// Конфигурация для NullCounters
    /// </summary>
    public class NullCountersConfiguration : PerfCountersConfiguration
    {
        /// <summary>
        /// Конструктор NullCountersConfiguration
        /// </summary>
        public NullCountersConfiguration() : base(PerfCountersTypeEnum.NullCounters) { }
    }

    /// <summary>
    /// Конфигурация для InternalCounters
    /// </summary>
    public class InternalCountersConfiguration : PerfCountersConfiguration
    {
        /// <summary>
        /// Конструктор InternalCountersConfiguration
        /// </summary>
        public InternalCountersConfiguration() : base(PerfCountersTypeEnum.InternalCounters) { }
    }

    /// <summary>
    /// Конфигурация для счётчиков Windows
    /// </summary>
    public class WinCountersConfiguration : PerfCountersConfiguration
    {
        /// <summary>
        /// Конструктор WinCountersConfiguration
        /// </summary>
        /// <param name="instMode">Режим инстанцирования счётчиков</param>
        /// <param name="categoryNamePrefix">Фиксированный префикс категорий</param>
        /// <param name="machineName">Имя машины (текущая - '.')</param>
        /// <param name="readOnlyCounters">Создавать счётчики только для чтения</param>
        /// <param name="preferedBitness">Предпочитаемая разрядность счётчиков</param>
        /// <param name="existedInstancesTreatment">Как обрабатывать существующие в Windows инстансы</param>
        public WinCountersConfiguration(
            Qoollo.PerformanceCounters.WinCounters.WinCountersInstantiationMode instMode, string categoryNamePrefix, string machineName, bool readOnlyCounters,
            Qoollo.PerformanceCounters.WinCounters.WinCountersPreferedBitness preferedBitness,
            Qoollo.PerformanceCounters.WinCounters.WinCountersExistedInstancesTreatment existedInstancesTreatment)
            : base(PerfCountersTypeEnum.WinCounters)
        {
            InstantiationMode = instMode;
            CategoryNamePrefix = categoryNamePrefix;
            MachineName = machineName;
            IsReadOnlyCounters = readOnlyCounters;
            PreferedBitness = preferedBitness;
            ExistedInstancesTreatment = existedInstancesTreatment;
        }
        /// <summary>
        /// Конструктор WinCountersConfiguration
        /// </summary>
        /// <param name="instMode">Режим инстанцирования счётчиков</param>
        /// <param name="categoryNamePrefix">Фиксированный префикс категорий</param>
        /// <param name="machineName">Имя машины (текущая - '.')</param>
        /// <param name="readOnlyCounters">Создавать счётчики только для чтения</param>
        public WinCountersConfiguration(Qoollo.PerformanceCounters.WinCounters.WinCountersInstantiationMode instMode, string categoryNamePrefix, string machineName, bool readOnlyCounters)
            : this(instMode, categoryNamePrefix, machineName, readOnlyCounters, Qoollo.PerformanceCounters.WinCounters.WinCountersPreferedBitness.SameAsOperatingSystemBitness, WinCounters.WinCountersExistedInstancesTreatment.LoadExisted)
        {
        }
        /// <summary>
        /// Конструктор WinCountersConfiguration
        /// </summary>
        /// <param name="instMode">Режим инстанцирования счётчиков</param>
        /// <param name="categoryNamePrefix">Фиксированный префикс категорий</param>
        public WinCountersConfiguration(Qoollo.PerformanceCounters.WinCounters.WinCountersInstantiationMode instMode, string categoryNamePrefix)
            : this(instMode, categoryNamePrefix, ".", false, Qoollo.PerformanceCounters.WinCounters.WinCountersPreferedBitness.SameAsOperatingSystemBitness, WinCounters.WinCountersExistedInstancesTreatment.LoadExisted)
        {
        }
        /// <summary>
        /// Конструктор WinCountersConfiguration
        /// </summary>
        /// <param name="instMode">Режим инстанцирования счётчиков</param>
        public WinCountersConfiguration(Qoollo.PerformanceCounters.WinCounters.WinCountersInstantiationMode instMode)
            : this(instMode, null, ".", false, Qoollo.PerformanceCounters.WinCounters.WinCountersPreferedBitness.SameAsOperatingSystemBitness, WinCounters.WinCountersExistedInstancesTreatment.LoadExisted)
        {
        }
        /// <summary>
        /// Конструктор WinCountersConfiguration
        /// </summary>
        public WinCountersConfiguration()
            : this(Qoollo.PerformanceCounters.WinCounters.WinCountersInstantiationMode.UseExistedIfPossible, null, ".", false,
                   Qoollo.PerformanceCounters.WinCounters.WinCountersPreferedBitness.SameAsOperatingSystemBitness, WinCounters.WinCountersExistedInstancesTreatment.LoadExisted)
        {
        }

        /// <summary>
        /// Режим инстанцирования счётчиков
        /// </summary>
        public Qoollo.PerformanceCounters.WinCounters.WinCountersInstantiationMode InstantiationMode { get; private set; }
        /// <summary>
        /// Фиксированный префикс категорий
        /// </summary>
        public string CategoryNamePrefix { get; private set; }
        /// <summary>
        /// Имя машины (текущая - '.')
        /// </summary>
        public string MachineName { get; private set; }
        /// <summary>
        /// Создавать счётчики только для чтения
        /// </summary>
        public bool IsReadOnlyCounters { get; private set; }
        /// <summary>
        /// Предпочитаемая разрядность счётчиков
        /// </summary>
        public Qoollo.PerformanceCounters.WinCounters.WinCountersPreferedBitness PreferedBitness { get; private set; }
        /// <summary>
        /// Как обрабатывать существующие в Windows инстансы
        /// </summary>
        public Qoollo.PerformanceCounters.WinCounters.WinCountersExistedInstancesTreatment ExistedInstancesTreatment { get; private set; }
    }

    /// <summary>
    /// Конфигурация для сетевых счётчиков
    /// </summary>
    public class NetCountersConfiguration: PerfCountersConfiguration
    {
        /// <summary>
        /// Стандартный порт сервера счётчиков
        /// </summary>
        public const int DefaultPort = 26115;

        /// <summary>
        /// Конструктор NetCountersConfiguration
        /// </summary>
        /// <param name="distributionPeriodMs">Период между рассылкой значений счётчиков</param>
        /// <param name="serverAddress">Адрес сервера</param>
        /// <param name="serverPort">Порт сервера</param>
        public NetCountersConfiguration(int distributionPeriodMs, string serverAddress, int serverPort)
            : base(PerfCountersTypeEnum.NetCounters)
        {
            if (distributionPeriodMs <= 0)
                throw new ArgumentException("distributionPeriodMs <= 0");
            if (string.IsNullOrEmpty(serverAddress))
                throw new ArgumentException("serverAddress");
            if (serverPort <= 0 || serverPort > 65535)
                throw new ArgumentException("serverPort <= 0 || serverPort > 65535");

            DistributionPeriodMs = distributionPeriodMs;
            ServerAddress = serverAddress;
            ServerPort = serverPort;
        }
        /// <summary>
        /// Конструктор NetCountersConfiguration
        /// </summary>
        /// <param name="distributionPeriodMs">Период между рассылкой значений счётчиков</param>
        /// <param name="serverAddress">Адрес сервера</param>
        public NetCountersConfiguration(int distributionPeriodMs, string serverAddress)
            : this(distributionPeriodMs, serverAddress, DefaultPort)
        {
        }
        /// <summary>
        /// Конструктор NetCountersConfiguration
        /// </summary>
        /// <param name="serverAddress">Адрес сервера</param>
        public NetCountersConfiguration(string serverAddress)
            : this(1000, serverAddress, DefaultPort)
        {
        }


        /// <summary>
        /// Период между рассылкой значений счётчиков
        /// </summary>
        public int DistributionPeriodMs { get; private set; }
        /// <summary>
        /// Адрес сервера
        /// </summary>
        public string ServerAddress { get; private set; }
        /// <summary>
        /// Порт сервера
        /// </summary>
        public int ServerPort { get; private set; }
    }


    /// <summary>
    /// Конфигурация для сетевых счётчиков Graphite
    /// </summary>
    public class GraphiteCountersConfiguration : PerfCountersConfiguration
    {
        /// <summary>
        /// Стандартный порт сервера счётчиков
        /// </summary>
        public const int DefaultPort = 2003;
        /// <summary>
        /// Стандартный префикс имени счётчиков
        /// </summary>
        public const string DefaultNamePrefix = "{MachineName}.{ProcessName}";

        /// <summary>
        /// Конструктор GraphiteCountersConfiguration
        /// </summary>
        /// <param name="distributionPeriodMs">Период между рассылкой значений счётчиков</param>
        /// <param name="namePrefixFormatString">Строка с префиксом имени всех счётчиков</param>
        /// <param name="serverAddress">Адрес сервера</param>
        /// <param name="serverPort">Порт сервера</param>
        public GraphiteCountersConfiguration(int distributionPeriodMs, string namePrefixFormatString, string serverAddress, int serverPort)
            : base(PerfCountersTypeEnum.GraphiteCounters)
        {
            if (distributionPeriodMs <= 0)
                throw new ArgumentException("distributionPeriodMs <= 0");
            if (namePrefixFormatString == null)
                throw new ArgumentNullException("namePrefixFormatString");
            if (string.IsNullOrEmpty(serverAddress))
                throw new ArgumentException("serverAddress");
            if (serverPort <= 0 || serverPort > 65535)
                throw new ArgumentException("serverPort <= 0 || serverPort > 65535");

            DistributionPeriodMs = distributionPeriodMs;
            NamePrefixFormatString = namePrefixFormatString;
            ServerAddress = serverAddress;
            ServerPort = serverPort;
        }
        /// <summary>
        /// Конструктор GraphiteCountersConfiguration
        /// </summary>
        /// <param name="distributionPeriodMs">Период между рассылкой значений счётчиков</param>
        /// <param name="namePrefixFormatString">Строка с префиксом имени всех счётчиков</param>
        /// <param name="serverAddress">Адрес сервера</param>
        public GraphiteCountersConfiguration(int distributionPeriodMs, string namePrefixFormatString, string serverAddress)
            : this(distributionPeriodMs, namePrefixFormatString, serverAddress, DefaultPort)
        {
        }
        /// <summary>
        /// Конструктор GraphiteCountersConfiguration
        /// </summary>
        /// <param name="distributionPeriodMs">Период между рассылкой значений счётчиков</param>
        /// <param name="serverAddress">Адрес сервера</param>
        public GraphiteCountersConfiguration(int distributionPeriodMs, string serverAddress)
            : this(distributionPeriodMs, DefaultNamePrefix, serverAddress, DefaultPort)
        {
        }
        /// <summary>
        /// Конструктор GraphiteCountersConfiguration
        /// </summary>
        /// <param name="serverAddress">Адрес сервера</param>
        public GraphiteCountersConfiguration(string serverAddress)
            : this(1000, DefaultNamePrefix, serverAddress, DefaultPort)
        {
        }


        /// <summary>
        /// Период между рассылкой значений счётчиков
        /// </summary>
        public int DistributionPeriodMs { get; private set; }
        /// <summary>
        /// Строка с префиксом имени всех счётчиков
        /// </summary>
        public string NamePrefixFormatString { get; private set; }
        /// <summary>
        /// Адрес сервера
        /// </summary>
        public string ServerAddress { get; private set; }
        /// <summary>
        /// Порт сервера
        /// </summary>
        public int ServerPort { get; private set; }
    }
}
