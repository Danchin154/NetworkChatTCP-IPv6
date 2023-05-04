using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Server
{

    class Program
    {
        static Dictionary<TcpClient, string> clients = new Dictionary<TcpClient, string>();

        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.IPv6Any, 8888);
            listener.Start();
            Console.WriteLine("Сервер запущен и ожидает подключение.");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                byte[] nameBuffer = new byte[1024];
                int bytesRead;
                bytesRead = client.GetStream().Read(nameBuffer, 0, nameBuffer.Length);
                string name = Encoding.UTF8.GetString(nameBuffer, 0, bytesRead);

                clients.Add(client, name);

                Console.WriteLine("{0} подключился к чату", name);

                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
        }

        static void HandleClient(TcpClient client)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (string.IsNullOrEmpty(message))
                    {
                        clients.Remove(client);
                        Console.WriteLine("{0} покинул чат", clients[client]);
                        break;
                    }

                    Console.WriteLine("{0}: {1}", clients[client], message);

                    foreach (KeyValuePair<TcpClient, string> pair in clients)
                    {
                        if (pair.Key != client)
                        {
                            string output = string.Format("{0}: {1}", clients[client], message);
                            byte[] outputBuffer = Encoding.UTF8.GetBytes(output);
                            pair.Key.GetStream().Write(outputBuffer, 0, outputBuffer.Length);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

}

