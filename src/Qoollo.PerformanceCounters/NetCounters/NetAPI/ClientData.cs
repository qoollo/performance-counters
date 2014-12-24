using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.NetCounters.NetAPI
{
    /// <summary>
    /// Данные о процессе клиента
    /// </summary>
    [DataContract]
    public class ClientData
    {
        /// <summary>
        /// Конструктор ClientData
        /// </summary>
        /// <param name="machineName">Имя машины</param>
        /// <param name="machineAddress">Адрес машины</param>
        /// <param name="processName">Имя процесса</param>
        /// <param name="processId">ID процесса</param>
        public ClientData(string machineName, string machineAddress, string processName, int processId)
        {
            if (machineName == null)
                throw new ArgumentNullException("machineName");
            if (machineAddress == null)
                throw new ArgumentNullException("machineAddress");
            if (processName == null)
                throw new ArgumentNullException("processName");

            MachineName = machineName;
            MachineAddress = machineAddress;
            ProcessName = processName;
            ProcessId = processId;
        }

        /// <summary>
        /// Имя машины
        /// </summary>
        [DataMember(Order = 1)]
        public string MachineName { get; private set; }
        /// <summary>
        /// Адрес машины
        /// </summary>
        [DataMember(Order = 2)]
        public string MachineAddress { get; private set; }
        /// <summary>
        /// Имя процесса
        /// </summary>
        [DataMember(Order = 3)]
        public string ProcessName { get; private set; }
        /// <summary>
        /// ID процесса
        /// </summary>
        [DataMember(Order = 4)]
        public int ProcessId { get; private set; }

        /// <summary>
        /// Преобразовать к строке
        /// </summary>
        /// <returns>Строка</returns>
        public override string ToString()
        {
            return string.Format("[{0} ({1}); {2} ({3})]", MachineName, MachineAddress, ProcessName, ProcessId);
        }
    }
}
