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
        private static string ipAddress = "127.0.0.1";

        private static string ProcessRequest(string request)
        {
            string response = string.Empty;

            switch (request.Trim().ToLower())
            {
                case "time":
                    response = "Current time: " + DateTime.Now.ToString();
                    break;
                case "info":
                    response = "Some information";
                    break;
                case "close":
                    response = "Closing connection";
                    break;
                default:
                    response = "Invalid command";
                    break;
            }

            return response;
        }


        static void Main(string[] args)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(5);
                Console.WriteLine("Server start listen...");
                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    Console.WriteLine("Client connected...");

                    while (true)
                    {
                        StringBuilder receivedString = new StringBuilder();
                        int recievedBytes = 0;
                        byte[] data = new byte[256];

                        do
                        {
                            recievedBytes = handler.Receive(data);
                            receivedString.Append(Encoding.Unicode.GetString(data, 0, recievedBytes));
                        } while (handler.Available > 0);

                        Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + receivedString.ToString());

                        string response = ProcessRequest(receivedString.ToString());
                        handler.Send(Encoding.Unicode.GetBytes(response));

                        if (response == "Closing connection")
                        {
                            Console.WriteLine("Closing connection");
                            break;
                        }
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    Console.WriteLine("Connection closed");

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
    }
}
