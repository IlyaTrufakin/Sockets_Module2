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
        private static int port = 8005;
        private static string ipAddress = "127.0.0.1";

        private static void HandleClient(object obj)
        {
            Socket handler = (Socket)obj;

            try
            {
                while (true)
                {
                    byte[] data = new byte[256];
                    StringBuilder receivedString = new StringBuilder();
                    int receivedBytes;

                    do
                    {
                        receivedBytes = handler.Receive(data);
                        receivedString.Append(Encoding.Unicode.GetString(data, 0, receivedBytes));
                    } while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + receivedString + $"  (from Client - {handler.RemoteEndPoint.ToString()})");

                    string response = ProcessRequest(receivedString.ToString());
                    handler.Send(Encoding.Unicode.GetBytes(response));

                    if (response == "Closing")
                    {
                        Console.WriteLine($"Client - {handler.RemoteEndPoint.ToString()} Closing connection.");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                Console.WriteLine("Connection closed");
            }
        }

        private static string ProcessRequest(string request)
        {
            string response = string.Empty;

            switch (request.Trim().ToLower())
            {
                case "time":
                    response = DateTime.Now.ToString();
                    break;
                case "info":
                    response = Environment.OSVersion.ToString(); 
                    break;
                case "bye":
                    response = "Closing";
                    break;
                default:
                    response = "Invalid command";
                    break;
            }

            return response;
        }

        private static void Main(string[] args)
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);
                Console.WriteLine("Server start listen...");

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    Console.WriteLine("Client connected: " + handler.RemoteEndPoint.ToString());

                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientThread.Start(handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

