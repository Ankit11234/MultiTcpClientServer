using System;
using System.Net.Sockets;
using System.Text;

namespace MultiClientTcpClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your name:");
            string clientName = Console.ReadLine();

            using (TcpClient clientSocket = new TcpClient())
            {
                clientSocket.Connect("127.0.0.1", 8888);
                Console.WriteLine($"Connected to the server.");

                try
                {
                    using (NetworkStream serverStream = clientSocket.GetStream())
                    {
                        while (true)
                        {
                            Console.Write($"{clientName}: ");
                            string message = Console.ReadLine();
                            if (string.IsNullOrEmpty(message))
                                continue;

                            byte[] outStream = Encoding.ASCII.GetBytes($"{clientName}: {message}\0");
                            serverStream.Write(outStream, 0, outStream.Length);
                            serverStream.Flush();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
