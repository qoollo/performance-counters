using Qoollo.PerformanceCounters.Helpers;
using Qoollo.PerformanceCounters.NetCounters.Categories;
using Qoollo.PerformanceCounters.NetCounters.CountersInfoAggregating;
using Qoollo.PerformanceCounters.NetCounters.Net;
using Qoollo.PerformanceCounters.NetCounters.NetAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.CountersValueDistribution
{
    /// <summary>
    /// Рассыльщик значений сетевых счётчиков
    /// </summary>
    internal class NetCountersValueDistributor : CounterValueDistributorBase
    {
        private readonly PerfCountersStableNetClient _netClient;
        private readonly ClientData _clientData;

        public NetCountersValueDistributor(CountersInfoAggregator aggregator, int distributionPeriodMs, PerfCountersStableNetClient netClient)
            : base(aggregator, distributionPeriodMs, "NetCounters (" + netClient.RemoteSideName + ")")
        {
            if (netClient == null)
                throw new ArgumentNullException("netClient");

            _netClient = netClient;
            _clientData = new ClientData(LocalMachineInfo.MachineName, LocalMachineInfo.MachineAddress, LocalMachineInfo.ProcessName, LocalMachineInfo.ProcessId);
        }


        /// <summary>
        /// Выполнить рассылку данных
        /// </summary>
        /// <param name="data">Агрегированные данные счётчиков</param>
        /// <param name="changes">Произошедшие изменения</param>
        /// <param name="token">Токен отмены</param>
        protected override void PerformDistribution(List<CategoryAggregatedInfo> data, CountersInfoAggregatorChangeKind changes, CancellationToken token)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (!_netClient.HasConnection)
                return;

            try
            {
                // Проверяем, надо ли разослать обновлённое описание структуры
                if (changes.HasFlag(CountersInfoAggregatorChangeKind.Categoties) ||
                    changes.HasFlag(CountersInfoAggregatorChangeKind.Counters) ||
                    _netClient.WasReconnected)
                {
                    var description = BuildCategoriesDescription(data);
                    _netClient.UpdateDescription(_clientData, description);
                    _netClient.MarkReconnectedWasProcessed();
                }

                // Рассылаем значения
                DateTimeOffset startTime = DateTimeOffset.Now;
                var values = BuildCategoriesValueData(data);
                DateTimeOffset endTime = DateTimeOffset.Now;
                DateTimeOffset averageTime = startTime + TimeSpan.FromMilliseconds((endTime - startTime).TotalMilliseconds / 2);
                _netClient.UpdateValues(averageTime, values);
            }
            catch (CommunicationException cex)
            {
                LogError(cex, "Network error during data distribution to " + _netClient.RemoteSideName);
            }
            catch (TimeoutException tex)
            {
                LogError(tex, "Network error during data distribution to " + _netClient.RemoteSideName);
            }
        }

        /// <summary>
        /// Сформировать массив описателей категории по агрегированным данным
        /// </summary>
        private CategoryDescriptionData[] BuildCategoriesDescription(List<CategoryAggregatedInfo> data)
        {
            CategoryDescriptionData[] result = new CategoryDescriptionData[data.Count];

            for (int i = 0; i < data.Count; i++)
                result[i] = BuildCategoryDescription(data[i]);

            return result;
        }
        /// <summary>
        /// Сформировать описатель категории
        /// </summary>
        private CategoryDescriptionData BuildCategoryDescription(CategoryAggregatedInfo categoryInfo)
        {
            if (categoryInfo.Type == CategoryTypes.Empty && categoryInfo.CounterDescriptors.Count > 0)
                throw new ArgumentException("categoryInfo");

            var counterDescriptors = categoryInfo.CounterDescriptors;
            var resultCounters = new CounterDescriptionData[counterDescriptors.Count];
            for (int i = 0; i < counterDescriptors.Count; i++)
                resultCounters[i] = new CounterDescriptionData(counterDescriptors[i].Name, counterDescriptors[i].Description, counterDescriptors[i].Type);

            return new CategoryDescriptionData(categoryInfo.NamePath, categoryInfo.Description, categoryInfo.Type, resultCounters);
        }



        /// <summary>
        /// Получить список значений счётчиков по агрегированной информации
        /// </summary>
        private CategoryValueData[] BuildCategoriesValueData(List<CategoryAggregatedInfo> data)
        {
            List<CategoryValueData> result = new List<CategoryValueData>(data.Count);

            foreach (var categoryInfo in data)
            {
                if (categoryInfo.Type == CategoryTypes.Empty)
                    continue;

                result.Add(BuildCategoryValueData(categoryInfo));
            }

            return result.ToArray();
        }
        /// <summary>
        /// Получить значения по категории
        /// </summary>
        private CategoryValueData BuildCategoryValueData(CategoryAggregatedInfo categoryInfo)
        {
            if (categoryInfo.Type == CategoryTypes.Empty && categoryInfo.Instances.Count > 0)
                throw new ArgumentException("Bad empty categoryInfo");
            if (categoryInfo.Type == CategoryTypes.SingleInstance && categoryInfo.Instances.Count != 1)
                throw new ArgumentException("Bad singleInstance categoryInfo");

            InstanceValueData[] instances = new InstanceValueData[categoryInfo.Instances.Count];
            for (int i = 0; i < instances.Length; i++)
                instances[i] = BuildInstanceValueData(categoryInfo.Instances[i]);

            return new CategoryValueData(categoryInfo.NamePath, categoryInfo.Type, instances);
        }
        /// <summary>
        /// Получить значения по инстансу
        /// </summary>
        private InstanceValueData BuildInstanceValueData(InstanceAggregatedInfo instanceInfo)
        {
            CounterValueData[] counters = new CounterValueData[instanceInfo.Counters.Count];
            for (int i = 0; i < counters.Length; i++)
                counters[i] = BuildCounterValueData(instanceInfo.Counters[i]);

            if (instanceInfo.SourceInstance != null)
                return new InstanceValueData(instanceInfo.InstanceName, counters);
            else
                return new InstanceValueData(counters);
        }
        /// <summary>
        /// Вычитать значение счётчика
        /// </summary>
        private CounterValueData BuildCounterValueData(Counter counter)
        {
            switch (counter.Type)
            {
                case CounterTypes.NumberOfItems:
                    return new ItemCounterValueData(counter.Name, counter.Type, ((NumberOfItemsCounter)counter).CurrentValue);
                case CounterTypes.OperationsPerSecond:
                    return new FractionCounterValueData(counter.Name, counter.Type, ((OperationsPerSecondCounter)counter).CurrentValue);
                case CounterTypes.AverageCount:
                    return new FractionCounterValueData(counter.Name, counter.Type, ((AverageCountCounter)counter).CurrentValue);
                case CounterTypes.AverageTime:
                    return new TimeCounterValueData(counter.Name, counter.Type, ((AverageTimeCounter)counter).CurrentValue);
                case CounterTypes.MomentTime:
                    return new TimeCounterValueData(counter.Name, counter.Type, ((MomentTimeCounter)counter).CurrentValue);
                case CounterTypes.ElapsedTime:
                    return new TimeCounterValueData(counter.Name, counter.Type, ((ElapsedTimeCounter)counter).CurrentValue);
                default:
                    throw new InvalidProgramException("Unknown counter type: " + counter.Type.ToString());
            }
        }
    }
}
