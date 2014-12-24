using Qoollo.PerformanceCounters.NetCounters.Categories;
using Qoollo.PerformanceCounters.NetCounters.CountersInfoAggregating;
using Qoollo.PerformanceCounters.NetCounters.CountersValueDistribution;
using Qoollo.PerformanceCounters.NetCounters.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters
{
    /// <summary>
    /// Фабрика сетевых счётчиков
    /// </summary>
    public class NetCounterFactory: CounterFactory
    {
        private readonly NetEmptyCategory _internalCategory;
        private readonly CountersInfoAggregator _valuesAggreagator;
        private readonly PerfCountersStableNetClient _netClient;
        private readonly NetCountersValueDistributor _valuesDistributor;
        private volatile bool _isDisposed = false;

        /// <summary>
        /// Конструктор NetCounterFactory
        /// </summary>
        /// <param name="distributionPeriodMs">Период рассылки данных счётчиков</param>
        /// <param name="serverAddress">ip-адрес сервера</param>
        /// <param name="serverPort">Порт сервера</param>
        public NetCounterFactory(int distributionPeriodMs, string serverAddress, int serverPort)
        {
            if (distributionPeriodMs <= 1)
                throw new ArgumentException("distributionPeriodMs <= 1");
            if (serverAddress == null)
                throw new ArgumentNullException("serverAddress");
            if (serverPort <= 0 || serverPort > 65535)
                throw new ArgumentException("serverPort <= 0 || serverPort > 65535");

            _internalCategory = new NetEmptyCategory("", "root", null);
            _valuesAggreagator = new CountersInfoAggregator(_internalCategory);
            _netClient = PerfCountersStableNetClient.CreateOnTcp(serverAddress, serverPort);
            _valuesDistributor = new NetCountersValueDistributor(_valuesAggreagator, distributionPeriodMs, _netClient);
        }
        /// <summary>
        /// Конструктор NetCounterFactory по конфигурации
        /// </summary>
        /// <param name="config">Объект конфига</param>
        public NetCounterFactory(Configuration.NetCountersConfiguration config)
            : this(config.DistributionPeriodMs, config.ServerAddress, config.ServerPort)
        {
        }

        /// <summary>
        /// Перечень дочерних категорий
        /// </summary>
        public IEnumerable<Category> Categories { get { return _internalCategory.ChildCategories; } }


        /// <summary>
        /// Создание пустой категории
        /// Для добавления элемента в названии категории - Name.SubName.SubSubName
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override EmptyCategory CreateEmptyCategory(string categoryName, string categoryDescription)
        {
            return _internalCategory.CreateEmptySubCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание категории c многими инстансами
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override MultiInstanceCategory CreateMultiInstanceCategory(string categoryName, string categoryDescription)
        {
            return _internalCategory.CreateMultiInstanceSubCategory(categoryName, categoryDescription);
        }

        /// <summary>
        /// Создание категории
        /// </summary>
        /// <param name="categoryName">Имя категории</param>
        /// <param name="categoryDescription">Описание категории</param>
        /// <returns>Категория</returns>
        public override SingleInstanceCategory CreateSingleInstanceCategory(string categoryName, string categoryDescription)
        {
            return _internalCategory.CreateSingleInstanceSubCategory(categoryName, categoryDescription);
        }


        /// <summary>
        /// Инициализация счетчиков во всех дочерних категориях (создание категорий, инстанцирование самих счетчиков)
        /// </summary>
        public override void InitAll()
        {
            base.InitAll();
            _netClient.Start();
            _valuesDistributor.Start();
        }


        /// <summary>
        /// Внутренний код освобождения ресурсов
        /// </summary>
        /// <param name="isUserCall">Вызвано ли пользователем</param>
        protected override void Dispose(bool isUserCall)
        {
            if (isUserCall)
            {
                if (!_isDisposed)
                {
                    _isDisposed = true;
                    _valuesDistributor.Dispose();
                    _netClient.Dispose();
                }
            }
            base.Dispose(isUserCall);
        }
    }
}
