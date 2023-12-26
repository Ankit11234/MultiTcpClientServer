using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiClientTcpServer
{
    internal class Program
    {
        private static TcpListener serverSocket;
        private static List<TcpClient> clients = new List<TcpClient>();

        static async Task Main(string[] args)
        {
            IPAddress ipAd = IPAddress.Parse("127.0.0.1");
            serverSocket = new TcpListener(ipAd, 8888);
            serverSocket.Start();
            Console.WriteLine("***********Server Started *********");

            while (true)
            {
                TcpClient clientSocket = await serverSocket.AcceptTcpClientAsync();
                clients.Add(clientSocket);

                Console.WriteLine($"Accepted connection from client {clients.Count}");

                Task.Run(() => HandleClient(clientSocket));
            }
        }

        private static async Task HandleClient(TcpClient clientSocket)
        {
            try
            {
                using (NetworkStream networkStream = clientSocket.GetStream())
                {
                    byte[] bytesFrom = new byte[10025];

                    while (true)
                    {
                        int bytesRead = await networkStream.ReadAsync(bytesFrom, 0, bytesFrom.Length);
                        if (bytesRead == 0)
                        {
                            // If bytesRead is 0, the client has disconnected
                            Console.WriteLine($"Client {clientSocket.Client.RemoteEndPoint} disconnected.");
                            break;
                        }

                        string dataFromClient = Encoding.ASCII.GetString(bytesFrom, 0, bytesRead);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf('\0'));

                        Console.WriteLine($"Data from client {clientSocket.Client.RemoteEndPoint}: {dataFromClient}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client {clientSocket.Client.RemoteEndPoint}: {ex.Message}");
            }
            finally
            {
                clients.Remove(clientSocket);
                clientSocket.Close();
            }
        }
    }
}
