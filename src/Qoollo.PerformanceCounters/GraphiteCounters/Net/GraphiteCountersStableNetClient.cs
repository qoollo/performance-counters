using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.GraphiteCounters.Net
{
    internal class GraphiteCountersStableNetClient: IGraphiteCountersAPI, IDisposable
    {
        private readonly string _address;
        private readonly int _port;

        private TcpClient _curClient;

        private Thread _connectToPerfCountersServerThread;
        private CancellationTokenSource _procStopTokenSource = new CancellationTokenSource();
        private object _syncObj = new object();

        private readonly int _connectionTestTimeMsMin;
        private readonly int _connectionTestTimeMsMax;

        private volatile bool _wasReconnected = false;

        public GraphiteCountersStableNetClient(string address, int port, int connectionTestTimeMs)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _address = address;
            _port = port;

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
                return _address + ":" + _port.ToString();
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
                return localClient != null && localClient.Connected;
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
        /// Переслать значения счётчиков
        /// </summary>
        /// <param name="time">Время выборки значений</param>
        /// <param name="counters">Значения счётчиков</param>
        public void SendCounterValues(DateTime time, List<GraphiteCounterData> counters)
        {
            if (counters == null)
                throw new ArgumentNullException("counters");

            var localClient = Volatile.Read(ref _curClient);
            if (localClient == null || !localClient.Connected)
                throw new CommunicationException("Connection is not established. Can't perform SendCounterValues for GraphiteCounters Server: " + RemoteSideName);

            lock (_syncObj)
            {
                localClient = Volatile.Read(ref _curClient);
                if (localClient == null || !localClient.Connected)
                    throw new CommunicationException("Connection is not established. Can't perform SendCounterValues for GraphiteCounters Server: " + RemoteSideName);

                try
                {
                    SendSendCounterValuesInner(localClient.GetStream(), time, counters);
                }
                catch (SocketException sExc)
                {
                    throw new CommunicationException("Network error during sending Graphite counters server", sExc);
                }
                catch (IOException ioExc)
                {
                    throw new CommunicationException("Network error during sending Graphite counters server", ioExc);
                }
            }
        }

        private void SendSendCounterValuesInner(NetworkStream stream, DateTime time, List<GraphiteCounterData> counters)
        {
            var writer = new StreamWriter(stream);
            foreach (var counter in counters)
                writer.Write(ConvertCounterDataToString(time, counter));

            writer.Flush();
        }

        private string ConvertCounterDataToString(DateTime time, GraphiteCounterData counter)
        {
            int unixTimestamp = (Int32)(time.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            return counter.FullName + " " + counter.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) + " " + unixTimestamp.ToString() + "\n";
        }



        /// <summary>
        /// Запустить
        /// </summary>
        public void Start()
        {
            if (_connectToPerfCountersServerThread != null)
                throw new InvalidOperationException("Performance counters Graphite network client is already started");

            _procStopTokenSource = new CancellationTokenSource();

            _connectToPerfCountersServerThread = new Thread(ConnectingToGraphitePerfCountersServerThreadFunc);
            _connectToPerfCountersServerThread.IsBackground = true;
            _connectToPerfCountersServerThread.Name = "GraphiteCounters connection thread: " + RemoteSideName;

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



        private void DestroyClient(TcpClient client)
        {
            if (client == null)
                return;

            try { client.Close(); }
            catch { }
        }


        private void ConnectingToGraphitePerfCountersServerThreadFunc()
        {
            var token = _procStopTokenSource.Token;

            bool isConnected = false;
            int currentConnectionTestTimeMs = _connectionTestTimeMsMin;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_curClient == null || !_curClient.Connected)
                    {
                        lock (_syncObj)
                        {
                            if (isConnected)
                            {
                                currentConnectionTestTimeMs = _connectionTestTimeMsMin;
                                isConnected = false;
                            }


                            var newClient = new TcpClient();
                            try
                            {
                                newClient.Connect(_address, _port);
                                isConnected = true;
                            }
                            catch (SocketException)
                            {
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
            catch (OperationCanceledException)
            {
                if (!token.IsCancellationRequested)
                    throw;
            }
        }




        public void Dispose()
        {
            Stop();
        }
    }
}
