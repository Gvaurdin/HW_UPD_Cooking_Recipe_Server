using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfApp_UDP_Server_Client.ServerContent;

namespace WpfApp_UDP_Server_Client
{
    public class UDPServer
    {
        private static Socket socket;
        private static Dictionary<string, string> requestClients;
        private static Timer timerAuth;
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private static List<KitchenRecipe> kitchenRecipes = KitchenRecipe.CreateListKitchenRecipes();

        public static void StartServer(string ipAddress, int port)
        {
            try
            {

                requestClients = new Dictionary<string, string>();
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                socket.Bind(endPoint);
                MessageBox.Show("Server is run!", "Server Application", MessageBoxButton.OK, MessageBoxImage.Information);

                while (true)
                {
                    byte[] buffer = new byte[1024];
                    EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    int bytesRead = socket.ReceiveFrom(buffer, ref clientEndPoint);
                    if (bytesRead > 0)
                    {
                        Thread clientThread = new Thread(() => HandleClient(clientEndPoint, buffer, bytesRead));
                        clientThread.Start();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Server error : {ex.Message}", "Server Application", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private static void HandleClient(EndPoint clientEndPoint, byte[] buffer, int bytesRead)
        {
            try
            {
                byte[] responseBytes = new byte[1024];
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                if (!checkClient(clientEndPoint))
                {
                    if (CheckNickName(message))
                    {
                        string nickName = message.Remove(message.Length - 2);
                        if (!CheckNameForDublicate(nickName))
                        {
                            requestClients.Add(nickName, ((IPEndPoint)clientEndPoint).ToString());
                            timerAuth = new Timer(async (_) =>
                            {
                                await DeleteClientFromBase(nickName, clientEndPoint);
                            }, null, TimeSpan.FromMinutes(5), TimeSpan.Zero);

                            responseBytes = Encoding.UTF8.GetBytes($"Welcome to the server {nickName}");
                            socket.SendTo(responseBytes, clientEndPoint);
                        }
                        else
                        {
                            responseBytes = Encoding.UTF8.GetBytes($"Enter a different name, the current name is already occupied-r");
                            socket.SendTo(responseBytes, clientEndPoint);
                        }
                    }
                    else
                    {
                        responseBytes = Encoding.UTF8.GetBytes($"You are not in the server database. You need to enter a nickname-r");
                        socket.SendTo(responseBytes, clientEndPoint);
                    }
                }
                else
                {

                    responseBytes = Encoding.UTF8.GetBytes("Server received your message");
                    socket.SendTo(responseBytes, clientEndPoint);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Server error : {ex.Message}", "Server Application", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        static bool CheckNickName(string nickName)
        {
            if (nickName.Length > 5 && nickName.Substring(nickName.Length - 2) == "-n") return true;
            else return false;
        }

        static bool checkClient(EndPoint pEnd)
        {

            bool isBase = false;
            if (requestClients.ContainsValue(((IPEndPoint)pEnd).ToString())) { isBase = true; }
            if (isBase) { return true; }
            else return false;
        }

        static bool CheckNameForDublicate(string nickName)
        {
            if (requestClients.ContainsKey(nickName)) return true;
            else return false;
        }

        static async Task DeleteClientFromBase(string nickName, EndPoint endPoint)
        {
            byte[] responseBytes = new byte[1024];
            await semaphore.WaitAsync();
            try
            {
                if (requestClients.ContainsKey(nickName))
                {
                    requestClients.Remove(nickName);
                    responseBytes = Encoding.UTF8.GetBytes("You have been removed from the system");
                    socket.SendTo(responseBytes, endPoint);
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
