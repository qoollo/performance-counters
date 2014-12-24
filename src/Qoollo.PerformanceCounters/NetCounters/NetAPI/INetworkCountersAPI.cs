using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.NetAPI
{
    /// <summary>
    /// Интерфейс API сетевых счётчиков
    /// </summary>
    [ServiceContract]
    public interface INetworkCountersAPI
    {
        /// <summary>
        /// Обновить описание счётчиков
        /// </summary>
        /// <param name="clientData">Данные о клиенте</param>
        /// <param name="data">Данные</param>
        [OperationContract]
        void UpdateDescription(ClientData clientData, CategoryDescriptionData[] data);

        /// <summary>
        /// Обновить значения счётчиков
        /// </summary>
        /// <param name="time">Время получения данных счётчиков</param>
        /// <param name="values">Значения</param>
        [OperationContract(IsOneWay = true)]
        void UpdateValues(DateTimeOffset time, CategoryValueData[] values);
    }
}
