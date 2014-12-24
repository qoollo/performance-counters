using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.Configuration.PerfCountersAppConfig
{
    internal static class ConfigurationFormatConverter
    {
        internal static PerfCountersConfiguration Convert(IPerfCountersConfigurationSection section)
        {
            return Convert(section.RootCounters);
        }


        // ==================

        private static Qoollo.PerformanceCounters.WinCounters.WinCountersInstantiationMode Convert(WinCountersInstantiationModeCfg src)
        {
            switch (src)
            {
                case WinCountersInstantiationModeCfg.AlwaysCreateNew:
                    return WinCounters.WinCountersInstantiationMode.AlwaysCreateNew;
                case WinCountersInstantiationModeCfg.UseExistedIfPossible:
                    return WinCounters.WinCountersInstantiationMode.UseExistedIfPossible;
                case WinCountersInstantiationModeCfg.UseOnlyExisted:
                    return WinCounters.WinCountersInstantiationMode.UseOnlyExisted;
                default:
                    throw new ArgumentException("src");
            }
        }

        private static Qoollo.PerformanceCounters.WinCounters.WinCountersPreferedBitness Convert(WinCountersPreferedBitnessCfg src)
        {
            switch (src)
            {
                case WinCountersPreferedBitnessCfg.Prefer32BitCounters:
                    return WinCounters.WinCountersPreferedBitness.Prefer32BitCounters;
                case WinCountersPreferedBitnessCfg.Prefer64BitCounters:
                    return WinCounters.WinCountersPreferedBitness.Prefer64BitCounters;
                case WinCountersPreferedBitnessCfg.SameAsOperatingSystemBitness:
                    return WinCounters.WinCountersPreferedBitness.SameAsOperatingSystemBitness;
                default:
                    throw new ArgumentException("src");
            }
        }

        private static Qoollo.PerformanceCounters.WinCounters.WinCountersExistedInstancesTreatment Convert(WinCountersExistedInstancesTreatmentCfg src)
        {
            switch (src)
            {
                case WinCountersExistedInstancesTreatmentCfg.IgnoreExisted:
                    return WinCounters.WinCountersExistedInstancesTreatment.IgnoreExisted;
                case WinCountersExistedInstancesTreatmentCfg.LoadExisted:
                    return WinCounters.WinCountersExistedInstancesTreatment.LoadExisted;
                case WinCountersExistedInstancesTreatmentCfg.RemoveExisted:
                    return WinCounters.WinCountersExistedInstancesTreatment.RemoveExisted;
                default:
                    throw new ArgumentException("src");
            }
        }


        private static NullCountersConfiguration Convert(INullCounters src)
        {
            return new NullCountersConfiguration();
        }

        private static InternalCountersConfiguration Convert(IInternalCounters src)
        {
            return new InternalCountersConfiguration();
        }

        private static WinCountersConfiguration Convert(IWinCounters src)
        {
            return new WinCountersConfiguration(
                Convert(src.InstantiationMode), src.CategoryNamePrefix, src.MachineName, src.IsReadOnlyCounters, 
                Convert(src.PreferedBitness), Convert(src.ExistedInstancesTreatment));
        }

        private static CompositeCountersConfiguration Convert(ICompositeCounters src)
        {
            return new CompositeCountersConfiguration(src.WrappedCounters.Select(o => Convert(o)));
        }

        private static NetCountersConfiguration Convert(INetCounters src)
        {
            return new NetCountersConfiguration(src.DistributionPeriodMs, src.ServerAddress, src.ServerPort);
        }

        private static GraphiteCountersConfiguration Convert(IGraphiteCounters src)
        {
            return new GraphiteCountersConfiguration(src.DistributionPeriodMs, src.NamePrefixFormatString, src.ServerAddress, src.ServerPort);
        }

        private static PerfCountersConfiguration Convert(IPerfCountersConfiguration src)
        {
            if (src is INullCounters)
                return Convert((INullCounters)src);
            if (src is IInternalCounters)
                return Convert((IInternalCounters)src);
            if (src is IWinCounters)
                return Convert((IWinCounters)src);
            if (src is ICompositeCounters)
                return Convert((ICompositeCounters)src);
            if (src is INetCounters)
                return Convert((INetCounters)src);
            if (src is IGraphiteCounters)
                return Convert((IGraphiteCounters)src);

            throw new ArgumentException("Unknown counters configuration type");
        }
    }
}
