using Qoollo.PerformanceCounters.NetCounters.NetAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.Net
{
    /// <summary>
    /// Клиент сетевых счётчиков с автоматическим фоновым восстановлением соединения
    /// </summary>
    public class PerfCountersStableNetClient : INetworkCountersAPI, IDisposable
    {
        /// <summary>
        /// Создать клиент сетевых счётчиков на TCP
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <param name="port">Порт</param>
        /// <param name="serviceName">Имя сервиса WCF</param>
        /// <returns>Созданный клиент</returns>
        public static PerfCountersStableNetClient CreateOnTcp(string address, int port, string serviceName = "NetCountersService")
        {
            EndpointAddress addr = new EndpointAddress(string.Format("net.tcp://{0}:{1}/{2}", address, port, serviceName));
            var binding = new NetTcpBinding(SecurityMode.None);

            return new PerfCountersStableNetClient(binding, addr, 16000);
        }

        /// <summary>
        /// Создать клиент сетевых счётчиков на Pipe
        /// </summary>
        /// <param name="address">Адрес</param>
        /// <param name="pipeName">Имя пайпа</param>
        /// <returns>Созданный клиент</returns>
        public static PerfCountersStableNetClient CreateOnPipe(string address, string pipeName = "NetCountersService")
        {
            EndpointAddress addr = new EndpointAddress(string.Format("net.pipe://{0}/{1}", address, pipeName));
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

            return new PerfCountersStableNetClient(binding, addr, 16000);
        }


        // =======================

        private string _crEnpointConfigName;
        private System.ServiceModel.EndpointAddress _crRemoteAddr;
        private System.ServiceModel.Channels.Binding _crBinding;

        private PerfCountersNetClient _curClient;

        private Thread _connectToPerfCountersServerThread;
        private CancellationTokenSource _procStopTokenSource = new CancellationTokenSource();
        private object _syncObj = new object();

        private readonly int _connectionTestTimeMsMin;
        private readonly int _connectionTestTimeMsMax;

        private volatile bool _wasReconnected = false;

        /// <summary>
        /// Конструктор PerfCountersStableNetClient
        /// </summary>
        /// <param name="endpointConfigurationName">Имя конфигурации</param>
        /// <param name="connectionTestTimeMs">Время переподключения</param>
        public PerfCountersStableNetClient(string endpointConfigurationName, int connectionTestTimeMs)
        {
            _crEnpointConfigName = endpointConfigurationName;

            _connectionTestTimeMsMin = Math.Max(500, connectionTestTimeMs / 32);
            _connectionTestTimeMsMax = connectionTestTimeMs;
        }
        /// <summary>
        /// Конструктор PerfCountersStableNetClient
        /// </summary>
        /// <param name="endpointConfigurationName">Имя конфигурации</param>
        /// <param name="remoteAddress">Удалённый адрес</param>
        /// <param name="connectionTestTimeMs">Время переподключения</param>
        public PerfCountersStableNetClient(string endpointConfigurationName, string remoteAddress, int connectionTestTimeMs) 
        {
            _crEnpointConfigName = endpointConfigurationName;
            _crRemoteAddr = new System.ServiceModel.EndpointAddress(remoteAddress);

            _connectionTestTimeMsMin = Math.Max(500, connectionTestTimeMs / 32);
            _connectionTestTimeMsMax = connectionTestTimeMs;
        }
        /// <summary>
        /// Конструктор PerfCountersStableNetClient
        /// </summary>
        /// <param name="endpointConfigurationName">Имя конфигурации</param>
        /// <param name="remoteAddress">Удалённый адрес</param>
        /// <param name="connectionTestTimeMs">Время переподключения</param>
        public PerfCountersStableNetClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress, int connectionTestTimeMs)
        {
            _crEnpointConfigName = endpointConfigurationName;
            _crRemoteAddr = remoteAddress;

            _connectionTestTimeMsMin = Math.Max(500, connectionTestTimeMs / 32);
            _connectionTestTimeMsMax = connectionTestTimeMs;
        }
        /// <summary>
        /// Конструктор PerfCountersStableNetClient
        /// </summary>
        /// <param name="binding">binding</param>
        /// <param name="remoteAddress">Удалённый адрес</param>
        /// <param name="connectionTestTimeMs">Время переподключения</param>
        public PerfCountersStableNetClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress, int connectionTestTimeMs)
        {
            _crBinding = binding;
            _crRemoteAddr = remoteAddress;

            _connectionTestTimeMsMin = Math.Max(500, connectionTestTimeMs / 32);
            _connectionTestTimeMsMax = connectionTestTimeMs;
        }

        /// <summary>
        /// Имя удалённой стороны
        /// </summary>
        public string RemoteSideName
        {
            get
            {
                if (_crRemoteAddr != null)
                    return _crRemoteAddr.Uri.ToString();

                return _crEnpointConfigName;
            }
        }

        /// <summary>
        /// Есть ли в данный момент соединение
        /// </summary>
        public bool HasConnection
        {
            get
            {
                var localClient = Volatile.Read(ref _curClient);
                return 
                    localClient != null && 
                    localClient.State == System.ServiceModel.CommunicationState.Opened;
            }
        }

        /// <summary>
        /// Запущен ли клиент
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return _connectToPerfCountersServerThread != null;
            }
        }

        /// <summary>
        /// Было ли пересоздано подключение
        /// </summary>
        public bool WasReconnected { get { return _wasReconnected; } }
        /// <summary>
        /// Пометить, что переподключение было обработано внешним кодом
        /// </summary>
        public void MarkReconnectedWasProcessed()
        {
            _wasReconnected = false;
        }

        /// <summary>
        /// Залогировать ошибку
        /// </summary>
        /// <param name="ex">Исключение (если есть)</param>
        /// <param name="message">Сообщение</param>
        protected virtual void LogError(Exception ex, string message)
        {

        }
        /// <summary>
        /// Залогировать предупреждение
        /// </summary>
        /// <param name="ex">Исключение (если есть)</param>
        /// <param name="message">Сообщение</param>
        protected virtual void LogWarn(Exception ex, string message)
        {

        }


        /// <summary>
        /// Обновить описание счётчиков
        /// </summary>
        /// <param name="clientData">Данные о процессе клиента</param>
        /// <param name="data">Данные</param>
        public void UpdateDescription(ClientData clientData, CategoryDescriptionData[] data)
        {
            if (clientData == null)
                throw new ArgumentNullException("clientData");
            if (data == null)
                throw new ArgumentNullException("data");

            var localClient = Volatile.Read(ref _curClient);
            if (localClient == null || localClient.State != System.ServiceModel.CommunicationState.Opened)
                throw new CommunicationException("Connection is not established. Can't perform UpdateDescription for PerfCounters Server: " + RemoteSideName);
            
            lock (_syncObj)
            {
                localClient = Volatile.Read(ref _curClient);
                if (localClient == null || localClient.State != System.ServiceModel.CommunicationState.Opened)
                    throw new CommunicationException("Connection is not established. Can't perform UpdateDescription for PerfCounters Server: " + RemoteSideName);

                localClient.RemoteSide.UpdateDescription(clientData, data);
            }
        }

        /// <summary>
        /// Обновить значения счётчиков
        /// </summary>
        /// <param name="time">Время получения данных счётчиков</param>
        /// <param name="values">Значения</param>
        public void UpdateValues(DateTimeOffset time, CategoryValueData[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            var localClient = Volatile.Read(ref _curClient);
            if (localClient == null || localClient.State != System.ServiceModel.CommunicationState.Opened)
                throw new CommunicationException("Connection is not established. Can't perform UpdateValues for PerfCounters Server: " + RemoteSideName);

            lock (_syncObj)
            {
                localClient = Volatile.Read(ref _curClient);
                if (localClient == null || localClient.State != System.ServiceModel.CommunicationState.Opened)
                    throw new CommunicationException("Connection is not established. Can't perform UpdateValues for PerfCounters Server: " + RemoteSideName);

                localClient.RemoteSide.UpdateValues(time, values);
            }
        }


        /// <summary>
        /// Запустить
        /// </summary>
        public void Start()
        {
            if (_connectToPerfCountersServerThread != null)
                throw new InvalidOperationException("Performance counters network client is already started");

            _procStopTokenSource = new CancellationTokenSource();

            _connectToPerfCountersServerThread = new Thread(ConnectingToPerfCountersServerThreadFunc);
            _connectToPerfCountersServerThread.IsBackground = true;
            _connectToPerfCountersServerThread.Name = "NetCounters connection thread: " + RemoteSideName;

            _connectToPerfCountersServerThread.Start();
        }

        /// <summary>
        /// Остановить
        /// </summary>
        public void Stop()
        {
            if (_connectToPerfCountersServerThread == null)
                return;

            _procStopTokenSource.Cancel();
            _connectToPerfCountersServerThread.Join();

            lock (_syncObj)
            {
                var oldClient = Interlocked.Exchange(ref _curClient, null);
                if (oldClient != null)
                    DestroyClient(oldClient);
            }
        }


        private PerfCountersNetClient CreateNewClient()
        {
            if (_crBinding != null)
                return new PerfCountersNetClient(_crBinding, _crRemoteAddr);

            if (_crRemoteAddr != null)
                return new PerfCountersNetClient(_crEnpointConfigName, _crRemoteAddr);

            return new PerfCountersNetClient(_crEnpointConfigName);
        }

        private void DestroyClient(PerfCountersNetClient client)
        {
            if (client == null || client.State == CommunicationState.Closed)
                return;

            try { client.Close(); }
            catch { client.Abort(); }
        }



        private void ConnectingToPerfCountersServerThreadFunc()
        {
            var token = _procStopTokenSource.Token;

            bool wasErrorPrinted = false;
            bool isConnected = false;
            int currentConnectionTestTimeMs = _connectionTestTimeMsMin;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_curClient == null || _curClient.State != System.ServiceModel.CommunicationState.Opened)
                    {
                        lock (_syncObj)
                        {
                            if (isConnected)
                            {
                                currentConnectionTestTimeMs = _connectionTestTimeMsMin;
                                isConnected = false;
                            }


                            var newClient = CreateNewClient();
                            try
                            {
                                newClient.Open();
                                wasErrorPrinted = false;
                                isConnected = true;
                            }
                            catch (TimeoutException tmExc)
                            {
                                if (!wasErrorPrinted)
                                {
                                    LogError(tmExc, "Can't connect to PerfCounters server: " + RemoteSideName);
                                    wasErrorPrinted = true;
                                }
                            }
                            catch (CommunicationException cmExc)
                            {
                                if (!wasErrorPrinted)
                                {
                                    LogError(cmExc, "Can't connect to PerfCounters server: " + RemoteSideName);
                                    wasErrorPrinted = true;
                                }
                            }

                            if (isConnected)
                            {
                                var oldClient = Interlocked.Exchange(ref _curClient, newClient);
                                if (oldClient != null)
                                    DestroyClient(oldClient);
                                _wasReconnected = true;
                            }
                            else
                            {
                                DestroyClient(newClient);
                            }


                            if (isConnected)
                            {
                                currentConnectionTestTimeMs = (_connectionTestTimeMsMax + _connectionTestTimeMsMin) / 2;
                            }
                            else
                            {
                                currentConnectionTestTimeMs *= 2;
                                if (currentConnectionTestTimeMs > _connectionTestTimeMsMax)
                                    currentConnectionTestTimeMs = _connectionTestTimeMsMax;
                            }
                        }
                    }

                    token.WaitHandle.WaitOne(currentConnectionTestTimeMs);
                }
            }
            catch (OperationCanceledException cex)
            {
                if (!token.IsCancellationRequested)
                {
                    LogError(cex, "Unknown error inside connecting to PerfCounters server: " + RemoteSideName);
                    throw;
                }
            }
            catch (Exception ex)
            {
                LogError(ex, "Unknown error inside connecting to PerfCounters server: " + RemoteSideName);
                throw;
            }
        }

        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}
