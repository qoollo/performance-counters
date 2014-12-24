using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Qoollo.PerformanceCounters.TestNetServer
{
    class Program
    {
        static void RunNetCountersServer()
        {
            var server = NetCounters.Net.PerfCountersNetServer.CreateOnTcp(new ServerHandler(), 26115);

            server.Open();

            Console.ReadLine();

            server.Dispose();
        }

        static void RunGraphiteCountersServer()
        {
            TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 2003);
            listener.Start();

            var client = listener.AcceptTcpClient();
            listener.Stop();

            //byte[] buffer = new byte[1024 * 1024];
            //int readed = client.GetStream().Read(buffer, 0, buffer.Length);
            //var str213 = Encoding.UTF8.GetString(buffer, 0, readed);

            var reader = new StreamReader(client.GetStream());

            while (true)
            {
                var str = reader.ReadLine();
                Console.WriteLine(str);
            }

            Console.ReadLine();

            client.Close();
        }


        static void Main(string[] args)
        {
            //RunNetCountersServer();
            RunGraphiteCountersServer();
        }
    }
}
