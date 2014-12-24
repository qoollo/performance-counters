using Qoollo.PerformanceCounters.NetCounters.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.CountersInfoAggregating
{
    /// <summary>
    /// Флаги, помечающие, что изменилось
    /// </summary>
    [Flags]
    internal enum CountersInfoAggregatorChangeKind
    {
        None = 0,
        Categoties = 1,
        Instances = 2,
        Counters = 4,
        All = Categoties | Instances | Counters
    }

    /// <summary>
    /// Сборщик данных о категориях и их счётчиках
    /// </summary>
    internal class CountersInfoAggregator
    {
        private readonly NetEmptyCategory _rootCategory;
        private List<CategoryAggregatedInfo> _aggreagetedCategoriesInfo;
        private CountersInfoAggregatorChangeKind _lastCheckChanges;

        public CountersInfoAggregator(NetEmptyCategory rootCategory)
        {
            if (rootCategory == null)
                throw new ArgumentNullException("rootCategory");

            _rootCategory = rootCategory;
            _aggreagetedCategoriesInfo = new List<CategoryAggregatedInfo>();
            _lastCheckChanges = CountersInfoAggregatorChangeKind.None;
        }

        public NetEmptyCategory RootCategory { get { return _rootCategory; } }
        public bool ConfigurationChangedOnLastCheck { get { return _lastCheckChanges != CountersInfoAggregatorChangeKind.None; } }
        public CountersInfoAggregatorChangeKind LastCheckChanges { get { return _lastCheckChanges; } }
        public List<CategoryAggregatedInfo> AggregatedCategoriesInfo { get { return _aggreagetedCategoriesInfo; } }


        protected virtual CategoryAggregatedInfo CreateCategoryAggregatedInfo(Category srcCategory, CategoryAggregatedInfo parent)
        {
            return new CategoryAggregatedInfo(srcCategory, parent);
        }
        protected virtual InstanceAggregatedInfo CreateInstanceAggregatedInfo(NetInstanceInMultiInstanceCategory instance, CategoryAggregatedInfo parent)
        {
            return new InstanceAggregatedInfo(instance);
        }
        protected virtual InstanceAggregatedInfo CreateInstanceAggregatedInfo(NetSingleInstanceCategory category, CategoryAggregatedInfo thisCategoryInfo)
        {
            return new InstanceAggregatedInfo(category);
        }

        /// <summary>
        /// Собрать данные по категориям (пересобирает только изменившиеся категории)
        /// </summary>
        /// <param name="force">Пересканировать всё полностью</param>
        /// <returns>Что изменилось</returns>
        public CountersInfoAggregatorChangeKind Refresh(bool force = false)
        {
            CountersInfoAggregatorChangeKind checkChanges = CountersInfoAggregatorChangeKind.None;
            bool needFullRescan = force || _rootCategory.IsChildCategoriesChanged || _aggreagetedCategoriesInfo.Count == 0;

            if (!needFullRescan)
            {
                foreach (var categoryInfo in _aggreagetedCategoriesInfo)
                {
                    if (categoryInfo.SourceCategoryExtended.IsChildCategoriesChanged)
                    {
                        needFullRescan = true;
                        break;
                    }

                    if (categoryInfo.SourceCategoryExtended.IsChildInstancesChanged || categoryInfo.SourceCategoryExtended.IsCountersChanged)
                    {
                        if (categoryInfo.SourceCategoryExtended.IsChildInstancesChanged)
                            checkChanges = checkChanges | CountersInfoAggregatorChangeKind.Instances;
                        if (categoryInfo.SourceCategoryExtended.IsCountersChanged)
                            checkChanges = checkChanges | CountersInfoAggregatorChangeKind.Counters;
                        categoryInfo.Instances.Clear();
                        FillInstanceInfo(categoryInfo, categoryInfo.Instances);
                        FillCounterDescriptors(categoryInfo, categoryInfo.CounterDescriptors);
                    }
                }
            }

            if (needFullRescan)
            {
                _rootCategory.MarkChildCategoriesChangedAsViewed();
                _aggreagetedCategoriesInfo = RescanCategories(_rootCategory);
                checkChanges = CountersInfoAggregatorChangeKind.All;
            }

            _lastCheckChanges = checkChanges;
            return checkChanges;
        }


        /// <summary>
        /// Пересканирование категорий
        /// </summary>
        /// <param name="root">Корневая категория</param>
        /// <returns>Список описателей категорий</returns>
        private List<CategoryAggregatedInfo> RescanCategories(NetEmptyCategory root)
        {
            List<CategoryAggregatedInfo> result = new List<CategoryAggregatedInfo>();

            var childCategories = (root as INetCategoriesExtendedInfo).ChildCategories;
            for (int i = 0; i < childCategories.Count; i++)
                FillCategotyInfo(childCategories[i], null, result);

            return result;
        }

        /// <summary>
        /// Заполнить информацию по категории (работает рекурсивно)
        /// </summary>
        /// <param name="category">Исходная категория</param>
        /// <param name="parent">Описатель родительской категории</param>
        /// <param name="result">Список для заполнения</param>
        private void FillCategotyInfo(Category category, CategoryAggregatedInfo parent, List<CategoryAggregatedInfo> result)
        {
            CategoryAggregatedInfo current = CreateCategoryAggregatedInfo(category, parent);
            current.SourceCategoryExtended.MarkChildCategoriesChangedAsViewed();
            FillInstanceInfo(current, current.Instances);
            FillCounterDescriptors(current, current.CounterDescriptors);
            result.Add(current);

            var childCategories = current.SourceCategoryExtended.ChildCategories;
            for (int i = 0; i < childCategories.Count; i++)
                FillCategotyInfo(childCategories[i], current, result);
        }

        /// <summary>
        /// Заполнить информацию по инстансам категории
        /// </summary>
        private void FillInstanceInfo(CategoryAggregatedInfo categoryInfo, List<InstanceAggregatedInfo> result)
        {
            if (categoryInfo.Type == CategoryTypes.SingleInstance)
                FillInstanceInfo(categoryInfo.SourceSingleInstanceCategory, categoryInfo, result);
            else if (categoryInfo.Type == CategoryTypes.MultiInstance)
                FillInstanceInfo(categoryInfo.SourceMultiInstanceCategory, categoryInfo, result);
        }
        private void FillInstanceInfo(NetMultiInstanceCategory category, CategoryAggregatedInfo categoryInfo, List<InstanceAggregatedInfo> result)
        {
            category.MarkChildInstancesChangedAsViewed();

            foreach (var instance in category.Instances.Cast<NetInstanceInMultiInstanceCategory>())
            {
                var info = CreateInstanceAggregatedInfo(instance, categoryInfo);
                FillCountersInfo(instance, info.Counters);
                result.Add(info);
            }
        }
        private void FillInstanceInfo(NetSingleInstanceCategory category, CategoryAggregatedInfo categoryInfo, List<InstanceAggregatedInfo> result)
        {
            var info = CreateInstanceAggregatedInfo(category, categoryInfo);
            FillCountersInfo(category, info.Counters);
            result.Add(info);
        }

        /// <summary>
        /// Заполнить список описателей счётчиков в категории
        /// </summary>
        private void FillCounterDescriptors(CategoryAggregatedInfo categoryInfo, List<CounterDescriptorAggregatedInfo> result)
        {
            if (categoryInfo.Type == CategoryTypes.SingleInstance)
                FillCounterDescriptors(categoryInfo.SourceSingleInstanceCategory, result);
            else if (categoryInfo.Type == CategoryTypes.MultiInstance)
                FillCounterDescriptors(categoryInfo.SourceMultiInstanceCategory, result);
        }
        private void FillCounterDescriptors(NetSingleInstanceCategory category, List<CounterDescriptorAggregatedInfo> result)
        {
            category.MarkCountersChangedAsViewed();

            foreach (var counter in category.Counters)
                result.Add(new CounterDescriptorAggregatedInfo(counter.Name, counter.Description, counter.Type));
        }
        private void FillCounterDescriptors(NetMultiInstanceCategory category, List<CounterDescriptorAggregatedInfo> result)
        {
            category.MarkCountersChangedAsViewed();

            foreach (var counter in category.Counters)
                result.Add(new CounterDescriptorAggregatedInfo(counter.Value.Name, counter.Value.Description, counter.Value.Type));
        }


        /// <summary>
        /// Заполнить список счётчиков в инстансе
        /// </summary>
        private void FillCountersInfo(NetInstanceInMultiInstanceCategory instance, List<Counter> result)
        {
            result.AddRange(instance.Counters);
        }
        /// <summary>
        /// Заполнить список счётчиков в singleInstanceCategory
        /// </summary>
        private void FillCountersInfo(NetSingleInstanceCategory category, List<Counter> result)
        {
            category.MarkCountersChangedAsViewed();

            result.AddRange(category.Counters);
        }
    }
}
