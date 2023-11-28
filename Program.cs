using Sockets_Server_Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sockets_Server
{
    // работает с несколькими клиентами
    internal class Program
    {
        private static Dictionary<string, Socket> connectedClients = new Dictionary<string, Socket>();
        private static object lockObject = new object();

        private static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 8005);
            server.Start();
        }
    }
}

