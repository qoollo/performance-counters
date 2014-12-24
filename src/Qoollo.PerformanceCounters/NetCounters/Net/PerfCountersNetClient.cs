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
    /// Клиент сетевых счётчиков
    /// </summary>
    public class PerfCountersNetClient : ClientBase<INetworkCountersAPI>
    {
        /// <summary>
        /// Конструктор PerfCountersNetClient
        /// </summary>
        /// <param name="endpointConfigurationName">Имя конфигурации</param>
        public PerfCountersNetClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }
        /// <summary>
        /// Конструктор PerfCountersNetClient
        /// </summary>
        /// <param name="endpointConfigurationName">Имя конфигурации</param>
        /// <param name="remoteAddress">Удалённый адрес</param>
        public PerfCountersNetClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }
        /// <summary>
        /// Конструктор PerfCountersNetClient
        /// </summary>
        /// <param name="endpointConfigurationName">Имя конфигурации</param>
        /// <param name="remoteAddress">Удалённый адрес</param>
        public PerfCountersNetClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }
        /// <summary>
        /// Конструктор PerfCountersNetClient
        /// </summary>
        /// <param name="binding">binding</param>
        /// <param name="remoteAddress">Удалённый адрес</param>
        public PerfCountersNetClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        /// <summary>
        /// Сигнал обрыва соединения
        /// </summary>
        //public event EventHandler Faulted
        //{
        //    add { (this as ICommunicationObject).Faulted += value; }
        //    remove { (this as ICommunicationObject).Faulted -= value; }
        //}

        
        /// <summary>
        /// API удалённой стороны
        /// </summary>
        public INetworkCountersAPI RemoteSide
        {
            get
            {
                return this.Channel;
            }
        }
    }
}
