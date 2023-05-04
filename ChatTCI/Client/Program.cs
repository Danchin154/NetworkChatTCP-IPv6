using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите ваше имя: ");
            string name = Console.ReadLine();
            TcpClient client = new TcpClient(AddressFamily.InterNetworkV6);
            client.Connect(IPAddress.IPv6Loopback, 8888);

            byte[] nameBuffer = Encoding.UTF8.GetBytes(name);
            client.GetStream().Write(nameBuffer, 0, nameBuffer.Length);

            Thread receiveThread = new Thread(() => ReceiveMessages(client));
            receiveThread.Start();

            while (true)
            {
                string message = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                client.GetStream().Write(buffer, 0, buffer.Length);
            }
        }

        static void ReceiveMessages(TcpClient client)
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = client.GetStream().Read(buffer, 0, buffer.Length);
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
