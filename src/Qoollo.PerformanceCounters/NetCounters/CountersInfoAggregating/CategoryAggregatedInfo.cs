using Qoollo.PerformanceCounters.NetCounters.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.CountersInfoAggregating
{
    /// <summary>
    /// Агрегированная информация о категории
    /// </summary>
    internal class CategoryAggregatedInfo
    {
        public CategoryAggregatedInfo(Category srcCategory, CategoryAggregatedInfo parent)
        {
            if (srcCategory == null)
                throw new ArgumentNullException("srcCategory");

            SourceCategory = srcCategory;
            Parent = parent;
            Instances = new List<InstanceAggregatedInfo>();
            CounterDescriptors = new List<CounterDescriptorAggregatedInfo>();

            if (parent != null)
            {
                NamePath = new string[parent.NamePath.Length + 1];
                Array.Copy(parent.NamePath, NamePath, parent.NamePath.Length);
                NamePath[NamePath.Length - 1] = srcCategory.Name;
            }
            else
            {
                NamePath = new string[1] { srcCategory.Name };
            }

            JoinedNamePath = String.Join(".", NamePath);
        }

        public Category SourceCategory { get; private set; }

        public INetCategoriesExtendedInfo SourceCategoryExtended { get { return SourceCategory as INetCategoriesExtendedInfo; } }
        public NetEmptyCategory SourceEmptyCategory { get { return SourceCategory as NetEmptyCategory; } }
        public NetSingleInstanceCategory SourceSingleInstanceCategory { get { return SourceCategory as NetSingleInstanceCategory; } }
        public NetMultiInstanceCategory SourceMultiInstanceCategory { get { return SourceCategory as NetMultiInstanceCategory; } }

        public CategoryAggregatedInfo Parent { get; private set; }

        public string Name { get { return SourceCategory.Name; } }
        public string Description { get { return SourceCategory.Description; } }
        public CategoryTypes Type { get { return SourceCategory.Type; } }
        public string[] NamePath { get; private set; }
        public string JoinedNamePath { get; set; }

        /// <summary>
        /// Инстансы категории (для SingleInstance тут должна быть 1 запись)
        /// </summary>
        public List<InstanceAggregatedInfo> Instances { get; private set; }
        /// <summary>
        /// Счётчики в категории
        /// </summary>
        public List<CounterDescriptorAggregatedInfo> CounterDescriptors { get; private set; }
    }
}
