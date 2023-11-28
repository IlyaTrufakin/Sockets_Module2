﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sockets_Server_Lib
{
    public class Server
    {
        IPEndPoint ipPoint;
        private Socket listenSocket;
        //private Thread consoleThread;
        private Dictionary<Socket, string> connectedClients = new Dictionary<Socket, string>();
        private int clientIdCounter = 0;
        private object lockObject = new object(); // для блокировки доступа нескольких потоков к изменяемому объекту

            //consoleThread = new Thread(ConsoleInput);


        public Server(string ipAddress = "127.0.0.1", int port = 8005)
        {
            try
            {
                ipPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Ошибка IP-адреса: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Ошибка в аргументах: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }



        public void Start()
        {
            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);
                Console.WriteLine("Server start listen...");
                //consoleThread.Start();

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    AddClient(handler);
                    Console.WriteLine($"Client connected:  {connectedClients[handler]} IP({handler.RemoteEndPoint.ToString()})");
                    foreach (var clients in connectedClients)
                    {
                        Console.WriteLine($"Clients already connected:  {clients.Value} IP({clients.Key.RemoteEndPoint.ToString()})");
                    }


                    Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    clientThread.Start(handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Добавление клиента и его идентификатора в словарь connectedClients
        private void AddClient(Socket clientSocket)
        {
              lock (lockObject)
            {
                clientIdCounter++;
                connectedClients.Add(clientSocket, "Client ID#" + clientIdCounter.ToString());
            }
        }

 



        private void HandleClient(object obj)
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

                   if (receivedString.ToString() != "timeQuiet") // когда клиент запрашивает время в автоматическом режиме, не выводим об этом инфо в консоль
                    {
                     Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + receivedString + $"  (from Client - {handler.RemoteEndPoint.ToString()})");
                    }
 

                    string response = ProcessRequest(receivedString.ToString()); // обработка строки запроса от клиента
                    handler.Send(Encoding.Unicode.GetBytes(response)); // отправка ответа клиенту

                    if (response == "Closing") // отработка запроса клиента на закрытие соединения
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

        private void ConsoleInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    // Пользователь ввел "exit" для завершения сервера
                    Environment.Exit(0);
                }
                else
                {
                    // Обработка пользовательского ввода и отправка ответа клиентам
                    string response = ProcessRequest(input);
                    Console.WriteLine("Server response: " + response);
                    // Здесь вы можете рассылать ответ всем клиентам или реализовать логику по своему усмотрению
                }
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
                case "timequiet":
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
    }

}
