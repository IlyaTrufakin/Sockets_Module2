using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Sockets_Server
{
    internal class Program
    {
        private static int port = 8005;
        private static string IPAdress = "127.0.0.1";
        static void Main(string[] args)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IPAdress), port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(5);
                Console.WriteLine("Server start listen...");
                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    StringBuilder recievedString = new StringBuilder();
                    int recievedBytes = 0;
                    byte[] data = new byte[4096];

                    do
                    {
                        recievedBytes = handler.Receive(data);
                        recievedString.Append(Encoding.Unicode.GetString(data,0,recievedBytes));
                    } while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + recievedString.ToString());

                    string message = "Data recieved";
                    handler.Send(Encoding.Unicode.GetBytes(message));   

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Console.WriteLine("Server stop listen...");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
    }
}
