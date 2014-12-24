using Qoollo.PerformanceCounters.NetCounters.CountersInfoAggregating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.CountersValueDistribution
{
    /// <summary>
    /// Распространитель знаачений счётчиков.
    /// Рассылает знаения через фиксированный период времени
    /// </summary>
    internal abstract class CounterValueDistributorBase: IDisposable
    {
        private CancellationTokenSource _isStopRequested;
        private Thread _distributionThread;
        private int _distributionPeriodMs;
        private CountersInfoAggregator _aggregator;

        /// <summary>
        /// Конструктор CounterValueDistributorBase
        /// </summary>
        /// <param name="aggregator">Агрегатор данных счётчиков</param>
        /// <param name="distributionPeriodMs">Период рассылки</param>
        /// <param name="name">Имя</param>
        public CounterValueDistributorBase(CountersInfoAggregator aggregator, int distributionPeriodMs, string name = "generic")
        {
            if (aggregator == null)
                throw new ArgumentNullException("aggregator");
            if (distributionPeriodMs <= 1)
                throw new ArgumentException("distributionPeriodMs <= 1");

            _distributionPeriodMs = distributionPeriodMs;
            _aggregator = aggregator;

            _isStopRequested = new CancellationTokenSource();
            _distributionThread = null;
            Name = name;
        }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Запущен ли
        /// </summary>
        public bool IsStarted { get { return _distributionThread != null; } }


        /// <summary>
        /// Залогировать ошибку
        /// </summary>
        /// <param name="ex">Исключение (если есть)</param>
        /// <param name="message">Сообщение</param>
        protected virtual void LogError(Exception ex, string message)
        {

        }

        /// <summary>
        /// Выполнить рассылку данных
        /// </summary>
        /// <param name="data">Агрегированные данные счётчиков</param>
        /// <param name="changes">Произошедшие изменения</param>
        /// <param name="token">Токен отмены</param>
        protected abstract void PerformDistribution(List<CategoryAggregatedInfo> data, CountersInfoAggregatorChangeKind changes, CancellationToken token);


        /// <summary>
        /// Запустить
        /// </summary>
        public virtual void Start()
        {
            if (_distributionThread != null)
                throw new InvalidOperationException("Performance counters data distributor is already started");

            _isStopRequested = new CancellationTokenSource();

            _distributionThread = new Thread(DistributionThreadFunc);
            _distributionThread.IsBackground = true;
            _distributionThread.Name = "PerfCounters data distribution thread: " + Name;

            _distributionThread.Start();
        }


        private void DistributionThreadFunc()
        {
            var token = _isStopRequested.Token;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    var changes = _aggregator.Refresh();
                    PerformDistribution(_aggregator.AggregatedCategoriesInfo, changes, token);

                    token.WaitHandle.WaitOne(_distributionPeriodMs);
                }
            }
            catch (OperationCanceledException cex)
            {
                if (!token.IsCancellationRequested)
                {
                    LogError(cex, "Unknown error during performance counters distribution: " + Name);
                    throw;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Unknown error during performance counters distribution: " + Name);
                throw;
            }
        }


        /// <summary>
        /// Остановить
        /// </summary>
        private void Stop()
        {
            if (_distributionThread == null)
                return;

            _isStopRequested.Cancel();
            _distributionThread.Join();
        }


        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        /// <param name="isUserCall">Вызвано ли пользователем</param>
        protected virtual void Dispose(bool isUserCall)
        {
            if (isUserCall)
            {
                Stop();
            }
            else
            {
                if (_isStopRequested != null)
                    _isStopRequested.Cancel();
            }
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
