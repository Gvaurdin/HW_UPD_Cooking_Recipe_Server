using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WpfApp_UDP_Server_Client.ServerContent;
using System.Security.Policy;

namespace WpfApp_UDP_Server_Client
{
    public class UDPClient
    {
        private static Socket socket;
        private static EndPoint IPEndPoint;
        public static EventHandler<string> MessageFromServer;
        public static EventHandler<KitchenRecipe> ResponseFromServer;
        private static bool isAuth = false;

        public static async Task StartClient(IPAddress ipAddress, int port, string nickName)
        {

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint = new IPEndPoint(ipAddress, port);
                byte[] buffer = new byte[1024];
                buffer = Encoding.UTF8.GetBytes(nickName+"-n");
                await socket.SendToAsync(new ArraySegment<byte>(buffer), SocketFlags.None, IPEndPoint);
                await WaitingResponse();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
                SocketDispose(socket);
            }

        }

        static async private Task WaitingResponse()
        {

            try
            {
                string message;
                int bytesRead = 0;
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    SocketReceiveFromResult result = await socket.ReceiveFromAsync(new ArraySegment<byte>(buffer), SocketFlags.None, IPEndPoint);
                    bytesRead = result.ReceivedBytes;
                    if (bytesRead > 0)
                    {
                        message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        if (!isAuth || CheckSystemServerError(message))
                        {
                            CheckIsAuth(message);
                            MessageServer(message);
                        }
                        else
                        {
                            ResponseServer(buffer);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
                MessageServer(ex.Message+"-s");
                SocketDispose(socket);
            }
        }

        private static bool CheckSystemServerError(string message)
        {
            string subMessage = message.Substring(message.Length - 2);
            if (subMessage == "-r" || subMessage == "-e")
            {
                isAuth = false;
                return true;
            }
            else return false;
        }

        protected static void ResponseServer(byte[] recipe)
        {
            KitchenRecipe kitchenRecipe = KitchenRecipe.Deserialize(recipe);
            ResponseFromServer?.Invoke(null, kitchenRecipe);

        }

        protected static void MessageServer(string message)
        {
            MessageFromServer?.Invoke(null, message);
        }

        public static async void SendToServer(string recipe)
        {
            byte[] buffer = new byte[1024];
            buffer = Encoding.UTF8.GetBytes(recipe);
            await socket.SendToAsync(new ArraySegment<byte>(buffer), SocketFlags.None, IPEndPoint);
        }

        static void CheckIsAuth(string message)
        {
            if (message.Substring(message.Length - 2) == "-a") isAuth = true;
        }

        static async void SocketDispose(Socket socket)
        {
            await Task.Run(() =>
            {
                socket.Dispose();
            });
        }
    }
}
