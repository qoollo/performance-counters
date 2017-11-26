using Qoollo.PerformanceCounters.GraphiteCounters.CountersInfoAggregating;
using Qoollo.PerformanceCounters.GraphiteCounters.Net;
using Qoollo.PerformanceCounters.Helpers;
using Qoollo.PerformanceCounters.NetCounters.CountersInfoAggregating;
using Qoollo.PerformanceCounters.NetCounters.CountersValueDistribution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.GraphiteCounters.CountersValueDistribution
{
    /// <summary>
    /// Рассыльщик значений счётчиков в Graphite
    /// </summary>
    internal class GraphiteCountersValueDistributor : CounterValueDistributorBase
    {
        private readonly GraphiteCountersStableNetClient _netClient;
        private readonly string _prefix;

        public GraphiteCountersValueDistributor(GraphiteCountersInfoAggregator aggregator, int distributionPeriodMs, string prefixFormatString, GraphiteCountersStableNetClient netClient)
            : base(aggregator, distributionPeriodMs, "GraphiteCounters")
        {
            if (netClient == null)
                throw new ArgumentNullException("netClient");
            if (prefixFormatString == null)
                throw new ArgumentNullException("prefixFormatString");

            _netClient = netClient;
            _prefix = GraphiteCountersPrefixConverter.TransformPrefix(prefixFormatString);
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

            List<GraphiteCounterData> graphiteData = new List<GraphiteCounterData>(50);

            try
            {
                foreach (var categoryInfo in data)
                {
                    if (token.IsCancellationRequested)
                        return;

                    graphiteData.Clear();

                    // Добавляем счётчики
                    DateTime startTime = DateTime.Now;
                    AddCountersFromCategory(_prefix, categoryInfo, graphiteData);
                    DateTime endTime = DateTime.Now;
                    DateTime averageTime = startTime + TimeSpan.FromMilliseconds((endTime - startTime).TotalMilliseconds / 2);

                    // Рассылаем значения
                    if (graphiteData.Count > 0)
                        _netClient.SendCounterValues(averageTime, graphiteData);
                }
            }
            catch (CommunicationException)
            {
            }
            catch (TimeoutException)
            {
            }
        }



        private List<GraphiteCounterData> ConvertCategories(string prefix, List<CategoryAggregatedInfo> data)
        {
            List<GraphiteCounterData> result = new List<GraphiteCounterData>(data.Count * 5);

            foreach (var categoryInfo in data)
                AddCountersFromCategory(prefix, categoryInfo, result);

            return result;
        }
        /// <summary>
        /// Добавить все счётчики из категории
        /// </summary>
        private void AddCountersFromCategory(string prefix, CategoryAggregatedInfo categoryInfo, List<GraphiteCounterData> result)
        {
            if (categoryInfo.Type == CategoryTypes.Empty && categoryInfo.Instances.Count > 0)
                throw new ArgumentException("Bad empty categoryInfo");
            if (categoryInfo.Type == CategoryTypes.SingleInstance && categoryInfo.Instances.Count != 1)
                throw new ArgumentException("Bad singleInstance categoryInfo");
            
            if (categoryInfo.Type == CategoryTypes.Empty)
                return;

            string newPrefix = categoryInfo.JoinedNamePath;
            if (!string.IsNullOrEmpty(prefix))
                newPrefix = prefix + "." + categoryInfo.JoinedNamePath;

            foreach (var instance in categoryInfo.Instances)
                AddCountersFromInstance(newPrefix, instance, result);
        }
        /// <summary>
        /// Добавить все счётчики из инстанса
        /// </summary>
        private void AddCountersFromInstance(string prefix, InstanceAggregatedInfo instance, List<GraphiteCounterData> result)
        {
            string newPrefix = prefix;
            if (!string.IsNullOrEmpty(instance.InstanceName))
                newPrefix = prefix + "." + EscapeName(instance.InstanceName);

            foreach (var counter in instance.Counters)
                result.Add(ConvertCounterData(newPrefix, counter));
        }
        /// <summary>
        /// Сконвертировать представление счётчика из внутреннего в Graphite
        /// </summary>
        private GraphiteCounterData ConvertCounterData(string prefix, Counter counter)
        {
            string name = prefix + "." + EscapeName(counter.Name);

            switch (counter.Type)
            {
                case CounterTypes.NumberOfItems:
                    return new GraphiteCounterData(name, ((NumberOfItemsCounter)counter).CurrentValue);
                case CounterTypes.Delta:
                    return new GraphiteCounterData(name, ((InternalCounters.Counters.InternalDeltaCounter)counter).MeasureInternal());
                case CounterTypes.OperationsPerSecond:
                    return new GraphiteCounterData(name, ((OperationsPerSecondCounter)counter).CurrentValue);
                case CounterTypes.AverageCount:
                    return new GraphiteCounterData(name, ((AverageCountCounter)counter).CurrentValue);
                case CounterTypes.AverageTime:
                    return new GraphiteCounterData(name, ((AverageTimeCounter)counter).CurrentValue.TotalMilliseconds);
                case CounterTypes.MomentTime:
                    return new GraphiteCounterData(name, ((MomentTimeCounter)counter).CurrentValue.TotalMilliseconds);
                case CounterTypes.ElapsedTime:
                    return new GraphiteCounterData(name, ((ElapsedTimeCounter)counter).CurrentValue.TotalMilliseconds);
                default:
                    throw new InvalidProgramException("Unknown counter type: " + counter.Type.ToString());
            }
        }

        /// <summary>
        /// Заменяет запрещённые сиволы в именах инстансов и счётчиков
        /// </summary>
        /// <param name="name">Искходное имя</param>
        /// <returns>Скорректированное имя</returns>
        private static string EscapeName(string name)
        {
            return name.Replace(' ', '_').Replace('.', '_');
        }
    }
}
