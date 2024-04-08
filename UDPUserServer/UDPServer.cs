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

        public static async Task StartServer()
        {
            try
            {
                int bytesRead = 0;
                IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress networkIPAddress = iPHostEntry.AddressList.FirstOrDefault(
                    ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                    && !IPAddress.IsLoopback(ip));
                int port = 49165;
                requestClients = new Dictionary<string, string>();
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint endPoint = new IPEndPoint(networkIPAddress, port);
                socket.Bind(endPoint);
                Console.WriteLine("Server is run!...........");

                while (true)
                {
                    byte[] buffer = new byte[1024];
                    EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    SocketReceiveFromResult result = await socket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, clientEndPoint);
                    bytesRead = result.ReceivedBytes;
                    if (bytesRead > 0)
                    {
                        clientEndPoint = result.RemoteEndPoint;
                        await HandleClient(clientEndPoint, buffer, bytesRead);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error : {ex.Message}"); }
        }

        private static async Task HandleClient(EndPoint clientEndPoint, byte[] buffer, int bytesRead)
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
                            }, null, TimeSpan.FromMinutes(10), TimeSpan.Zero);

                            responseBytes = Encoding.UTF8.GetBytes($"Welcome to the server {nickName}-a");
                            await socket.SendToAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None, clientEndPoint);
                        }
                        else
                        {
                            responseBytes = Encoding.UTF8.GetBytes($"Enter a different name, the current name is already occupied-r");
                            await socket.SendToAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None, clientEndPoint);
                        }
                    }

                }
                else
                {
                    if (CheckRecipe(message))
                    {
                        KitchenRecipe kitchenRecipe = GetRecipe(message);
                        await socket.SendToAsync(new ArraySegment<byte>(KitchenRecipe.Serialize(kitchenRecipe)), SocketFlags.None, clientEndPoint);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error : {ex.Message}"); }
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

        static bool CheckRecipe(string recipe)
        {
            bool trueRecipe = false;
            if (recipe != "")
            {
                foreach (KitchenRecipe kitchenRecipe in kitchenRecipes)
                {
                    if (kitchenRecipe.name == recipe) { trueRecipe = true; break; }
                }
            }

            return trueRecipe;
        }

        static KitchenRecipe GetRecipe(string recipe)
        {
            KitchenRecipe tmp = new KitchenRecipe();
            foreach (KitchenRecipe kitchenRecipe in kitchenRecipes)
            {
                if(kitchenRecipe.name == recipe) {  tmp = kitchenRecipe; break; }
            }
            return tmp;
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
                    responseBytes = Encoding.UTF8.GetBytes("You have been removed from the system-e");
                    await socket.SendToAsync(new ArraySegment<byte>(responseBytes), SocketFlags.None, endPoint);
                }
            }
            catch (Exception ex) { Console.WriteLine($"Error : {ex.Message}"); }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
