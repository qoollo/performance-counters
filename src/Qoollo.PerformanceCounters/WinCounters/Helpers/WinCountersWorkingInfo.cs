using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.WinCounters.Helpers
{
    /// <summary>
    /// Информация по режиму работы WinCounters
    /// </summary>
    internal class WinCountersWorkingInfo
    {
        /// <summary>
        /// Конструктор WinCountersWorkingInfo
        /// </summary>
        /// <param name="instMode">Режим работы</param>
        /// <param name="machineName">Имя машины</param>
        /// <param name="readOnlyCounters">Счётчики только для чтения</param>
        /// <param name="prefer64bitCounters">Предпочтение 64-ёх битным счётчикам</param>
        /// <param name="existedInstancesTreatment">Как обрабатывать существующие в Windows инстансы</param>
        public WinCountersWorkingInfo(WinCountersInstantiationMode instMode, string machineName, bool readOnlyCounters, bool prefer64bitCounters, WinCountersExistedInstancesTreatment existedInstancesTreatment)
        {
            InstantiationMode = instMode;
            MachineName = string.IsNullOrEmpty(machineName) ? "." : machineName;
            IsLocalMachine = MachineName == ".";
            ReadOnlyCounters = readOnlyCounters;
            Prefer64BitCounters = prefer64bitCounters;
            ExistedInstancesTreatment = existedInstancesTreatment;

            if (!IsLocalMachine)
            {
                if (instMode != WinCountersInstantiationMode.UseOnlyExisted)
                    throw new ArgumentException("Counters on remote machine can be used only in UseOnlyExisted mode");
                if (!readOnlyCounters)
                    throw new ArgumentException("Counters on remote machine should be in ReadOnly mode");
            }
        }

        /// <summary>
        /// Режим работы
        /// </summary>
        public WinCountersInstantiationMode InstantiationMode { get; private set; }
        /// <summary>
        /// Имя машины
        /// </summary>
        public string MachineName { get; private set; }
        /// <summary>
        /// Счётчики только для чтения
        /// </summary>
        public bool ReadOnlyCounters { get; private set; }
        /// <summary>
        /// Предпочтение 64-ёх битным счётчикам
        /// </summary>
        public bool Prefer64BitCounters { get; private set; }
        /// <summary>
        /// Как обрабатывать существующие в Windows инстансы
        /// </summary>
        public WinCountersExistedInstancesTreatment ExistedInstancesTreatment { get; private set; }

        /// <summary>
        /// Локальная ли машина
        /// </summary>
        public bool IsLocalMachine { get; private set; }
    }
}
