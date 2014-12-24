using Qoollo.PerformanceCounters.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.GraphiteCounters.CountersValueDistribution
{
    /// <summary>
    /// Преобразователь форматной строки префикса
    /// </summary>
    static internal class GraphiteCountersPrefixConverter
    {
        private enum SubstitutionalStringKind
        {
            MachineName,
            MachineAddress,
            ProcessName,
            ProcessId
        }

        private static readonly Dictionary<string, SubstitutionalStringKind> _substitutionTypes = new Dictionary<string, SubstitutionalStringKind>()
        {
            { "MachineName", SubstitutionalStringKind.MachineName },
            { "machineName", SubstitutionalStringKind.MachineName },
            { "machine_name", SubstitutionalStringKind.MachineName },

            { "MachineAddress", SubstitutionalStringKind.MachineAddress },
            { "machineAddress", SubstitutionalStringKind.MachineAddress },
            { "machine_address", SubstitutionalStringKind.MachineAddress },

            { "ProcessName", SubstitutionalStringKind.ProcessName },
            { "processName", SubstitutionalStringKind.ProcessName },
            { "process_name", SubstitutionalStringKind.ProcessName },

            { "ProcessId", SubstitutionalStringKind.ProcessId },
            { "ProcessID", SubstitutionalStringKind.ProcessId },
            { "processId", SubstitutionalStringKind.ProcessId },
            { "processID", SubstitutionalStringKind.ProcessId },
            { "process_id", SubstitutionalStringKind.ProcessId },
        };

        /// <summary>
        /// Преобразовать форматную строку в префикс
        /// </summary>
        /// <param name="prefixFormatString">Форматная строка префикса</param>
        /// <returns>Префикс</returns>
        public static string TransformPrefix(string prefixFormatString)
        {
            if (prefixFormatString == null)
                throw new ArgumentNullException("prefixFormatString");

            string currentResult = prefixFormatString;

            foreach (var elem in _substitutionTypes)
                currentResult = currentResult.Replace("{" + elem.Key + "}", GetSubstitutionReplacement(elem.Value));

            currentResult = currentResult.Replace(' ', '_');

            return currentResult;
        }


        private static string GetSubstitutionReplacement(SubstitutionalStringKind kind)
        {
            switch (kind)
            {
                case SubstitutionalStringKind.MachineName:
                    return LocalMachineInfo.MachineName.Replace(' ', '_').Replace('.', '_');
                case SubstitutionalStringKind.MachineAddress:
                    return LocalMachineInfo.MachineAddress.Replace(' ', '_').Replace('.', '_');;
                case SubstitutionalStringKind.ProcessName:
                    return LocalMachineInfo.ProcessName.Replace(' ', '_').Replace('.', '_');;
                case SubstitutionalStringKind.ProcessId:
                    return LocalMachineInfo.ProcessId.ToString();
                default:
                    throw new InvalidProgramException("Unknown substitution kind: " + kind.ToString());
            }
        }
    }
}
