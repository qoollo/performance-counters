using Qoollo.PerformanceCounters.NetCounters.NetAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.Net
{
    /// <summary>
    /// Сервер сетевых счётчиков
    /// </summary>
    public class PerfCountersNetServer : IDisposable
    {
        /// <summary>
        /// Создать сервис на основе конифга в app.config
        /// </summary>
        /// <param name="singleton">Синглтон хэндлера</param>
        /// <returns>Созданный сервер</returns>
        public static PerfCountersNetServer Create(INetworkCountersAPI singleton)
        {
            ServiceHost host = new ServiceHost(singleton);

            return new PerfCountersNetServer(host);
        }

        /// <summary>
        /// Создать сервис на TCP
        /// </summary>
        /// <param name="singleton">Синглтон хэндлера</param>
        /// <param name="port">Порт</param>
        /// <param name="serviceName">Имя сервиса WCF</param>
        /// <returns>Созданный сервер</returns>
        public static PerfCountersNetServer CreateOnTcp(INetworkCountersAPI singleton, int port, string serviceName = "NetCountersService")
        {
            Uri baseAddr = new Uri(string.Format("net.tcp://0.0.0.0:{0}/{1}", port, serviceName));
            ServiceHost host = new ServiceHost(singleton, baseAddr);

            var binding = new NetTcpBinding(SecurityMode.None);
            host.AddServiceEndpoint(typeof(INetworkCountersAPI), binding, baseAddr);

            var behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.InstanceContextMode = InstanceContextMode.Single;

            var debugBehavior = host.Description.Behaviors.Find<System.ServiceModel.Description.ServiceDebugBehavior>();

            if (debugBehavior != null)
                debugBehavior.IncludeExceptionDetailInFaults = true;

            return new PerfCountersNetServer(host);
        }

        /// <summary>
        /// Создать сервис на Pipe
        /// </summary>
        /// <param name="singleton">Синглтон хэндлера</param>
        /// <param name="pipeName">Имя пайпа</param>
        /// <returns>Созданный сервер</returns>
        public static PerfCountersNetServer CreateOnPipe(INetworkCountersAPI singleton, string pipeName = "NetCountersService")
        {
            Uri baseAddr = new Uri(string.Format("net.pipe://localhost/{0}", pipeName));
            ServiceHost host = new ServiceHost(singleton, baseAddr);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            host.AddServiceEndpoint(typeof(INetworkCountersAPI), binding, baseAddr);

            var behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.InstanceContextMode = InstanceContextMode.Single;

            var debugBehavior = host.Description.Behaviors.Find<System.ServiceModel.Description.ServiceDebugBehavior>();

            if (debugBehavior != null)
                debugBehavior.IncludeExceptionDetailInFaults = true;

            return new PerfCountersNetServer(host);
        }


        // =============

        private ServiceHost _host;

        /// <summary>
        /// Конструктор PerfCountersNetServer
        /// </summary>
        /// <param name="host">WCF хост</param>
        protected PerfCountersNetServer(ServiceHost host)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            _host = host;
        }

        /// <summary>
        /// Открыть сервис
        /// </summary>
        public void Open()
        {
            _host.Open();
        }


       
        /// <summary>
        /// Освободить ресурсы
        /// </summary>
        public void Dispose()
        {
            if (_host != null)
            {
                if (_host.State != CommunicationState.Closed)
                {
                    try
                    {
                        _host.Close();
                    }
                    catch
                    {
                        _host.Abort();
                    }
                }
                _host = null;
            }
        }
    }
}
