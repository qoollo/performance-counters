using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.Configuration.PerfCountersAppConfig
{
    enum WinCountersInstantiationModeCfg
    {
        AlwaysCreateNew,
        UseExistedIfPossible,
        UseOnlyExisted
    }

    enum WinCountersPreferedBitnessCfg
    {
        SameAsOperatingSystemBitness,
        Prefer32BitCounters,
        Prefer64BitCounters
    }

    enum WinCountersExistedInstancesTreatmentCfg
    {
        IgnoreExisted,
        LoadExisted,
        RemoveExisted
    }



    interface IPerfCountersConfigurationSection
    {
        IPerfCountersConfiguration RootCounters { get; }
    }



    interface IPerfCountersConfiguration
    {
    }

    interface ICompositeCounters : IPerfCountersConfiguration
    {
        List<IPerfCountersConfiguration> WrappedCounters { get; }
    }

    interface INullCounters: IPerfCountersConfiguration
    {
    }

    interface IInternalCounters: IPerfCountersConfiguration
    {
    }

    interface IWinCounters: IPerfCountersConfiguration
    {
        WinCountersInstantiationModeCfg InstantiationMode { get; }
        string CategoryNamePrefix { get; }
        string MachineName { get; }
        bool IsReadOnlyCounters { get; }
        WinCountersPreferedBitnessCfg PreferedBitness { get; }
        WinCountersExistedInstancesTreatmentCfg ExistedInstancesTreatment { get; }
    }

    interface INetCounters : IPerfCountersConfiguration
    {
        int DistributionPeriodMs { get; }
        string ServerAddress { get; }
        int ServerPort { get; }
    }

    interface IGraphiteCounters : IPerfCountersConfiguration
    {
        int DistributionPeriodMs { get; }
        string NamePrefixFormatString { get; }
        string ServerAddress { get; }
        int ServerPort { get; }
    }
}
