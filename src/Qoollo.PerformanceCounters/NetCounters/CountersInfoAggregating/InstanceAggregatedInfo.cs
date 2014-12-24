using Qoollo.PerformanceCounters.NetCounters.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.CountersInfoAggregating
{
    /// <summary>
    /// Описатель инстанса
    /// </summary>
    internal class InstanceAggregatedInfo
    {
        public InstanceAggregatedInfo(NetInstanceInMultiInstanceCategory srcInstance)
        {
            SourceInstance = srcInstance;
            SourceCategory = null;
            Counters = new List<Counter>();
        }
        public InstanceAggregatedInfo(NetSingleInstanceCategory srcCategory)
        {
            SourceInstance = null;
            SourceCategory = srcCategory;
            Counters = new List<Counter>();
        }

        /// <summary>
        /// Исходный инстанс (null для singleInstanceCategory)
        /// </summary>
        public NetInstanceInMultiInstanceCategory SourceInstance { get; private set; }
        /// <summary>
        /// Исходная категория (не null для singleInstanceCategory)
        /// </summary>
        public NetSingleInstanceCategory SourceCategory { get; private set; }

        public string InstanceName { get { return SourceInstance != null ? SourceInstance.InstanceName : null; } }
        public bool IsInstanceAlive { get { return SourceInstance != null ? SourceInstance.IsAlive : true; } }

        /// <summary>
        /// Счётчики
        /// </summary>
        public List<Counter> Counters { get; private set; }
    }
}
