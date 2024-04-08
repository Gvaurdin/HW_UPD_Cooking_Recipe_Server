using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp_UDP_Server_Client;

namespace UDPUserServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await UDPServer.StartServer();
        }
    }
}
