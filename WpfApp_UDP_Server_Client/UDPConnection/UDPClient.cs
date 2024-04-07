using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_UDP_Server_Client
{
    public class UDPClient
    {
        private static Socket socket;
        private static EndPoint IPEndPoint;

        public static async Task StartClient(string ipAddress, int port, string nickName)
        {

                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    IPEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
                    byte[] buffer = new byte[1024];
                    buffer = Encoding.UTF8.GetBytes(nickName);
                    await socket.SendToAsync(new ArraySegment<byte>(buffer), SocketFlags.None, IPEndPoint);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error : {ex.Message}");
                    CloseSocket(socket);
                }

        }

        static async private Task WaitingResponse(Socket socket, EndPoint endPoint)
        {

            try
            {
                string message;
                int bytesRead = 0;
                byte[] buffer = new byte[1024];
                await Task.Run(() =>
                {
                    while (true)
                    {
                        bytesRead = socket.ReceiveFrom(buffer,ref endPoint);

                    }
                });

            }
            catch (SocketException ex)
            {
                Console.WriteLine($"SocketException: {ex.ErrorCode} - {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error : {ex.Message}");
                CloseSocket(socket);
            }
        }



        static async void CloseSocket(Socket socket)
        {
            await Task.Run(() =>
            {
                socket.Close();
                socket.Dispose();
            });
        }
    }
}
