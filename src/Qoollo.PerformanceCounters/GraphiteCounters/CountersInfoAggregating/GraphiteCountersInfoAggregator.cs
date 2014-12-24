using Qoollo.PerformanceCounters.NetCounters.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.GraphiteCounters.CountersInfoAggregating
{
    internal class GraphiteCountersInfoAggregator : Qoollo.PerformanceCounters.NetCounters.CountersInfoAggregating.CountersInfoAggregator
    {
        public GraphiteCountersInfoAggregator(NetEmptyCategory rootCategory)
            : base(rootCategory)
        {
        }

        protected override NetCounters.CountersInfoAggregating.CategoryAggregatedInfo CreateCategoryAggregatedInfo(Category srcCategory, NetCounters.CountersInfoAggregating.CategoryAggregatedInfo parent)
        {
            var result = base.CreateCategoryAggregatedInfo(srcCategory, parent);

            result.JoinedNamePath = String.Join(".", 
                result.NamePath.SkipWhile(o => o == "").
                Select(o => string.IsNullOrWhiteSpace(o) ? "no_name" : o).
                Select(o => o.Replace(' ', '_')));

            return result;
        }
    }
}
