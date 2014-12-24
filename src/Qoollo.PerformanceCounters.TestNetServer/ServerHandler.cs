using Qoollo.PerformanceCounters.NetCounters.NetAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.TestNetServer
{
    public class ServerHandler: INetworkCountersAPI
    {
        public void UpdateDescription(ClientData clientData, CategoryDescriptionData[] data)
        {
            Console.WriteLine("UpdateDescription:");
            Console.WriteLine("ClientData: " + clientData.ToString());

            foreach (var categoryDesc in data)
            {
                Console.WriteLine(categoryDesc.ToString());
                foreach (var counter in categoryDesc.Counters)
                    Console.WriteLine("    " + counter.ToString());
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        public void UpdateValues(DateTimeOffset time, CategoryValueData[] values)
        {
            Console.WriteLine("UpdateValues at " + time.ToString() + ":");

            foreach (var cat in values)
            {
                Console.WriteLine(cat.ToString());
                foreach (var inst in cat.Instances)
                {
                    Console.WriteLine("    " + inst.ToString());
                    foreach (var counter in inst.Counters)
                        Console.WriteLine("        " + counter.ToString());
                }
            }

            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
