using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Adhesive.ComponentPerformance.Core
{
    internal class MemcachedSocket : IDisposable
    {
        private readonly string CommandString = "stats" + Environment.NewLine;
        private readonly int ErrorResponseLength = 13;
        private readonly string GenericErrorResponse = "ERROR";
        private readonly string ClientErrorResponse = "CLIENT_ERROR ";
        private readonly string ServerErrorResponse = "SERVER_ERROR ";
        private Socket socket;
        private IPEndPoint endpoint;
        private BufferedStream bufferedStream;
        private NetworkStream networkStream;

        public MemcachedSocket(IPEndPoint ip)
        {
            endpoint = ip;

            socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, (int)TimeSpan.FromSeconds(10).TotalMilliseconds);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, (int)TimeSpan.FromSeconds(10).TotalMilliseconds);
            socket.NoDelay = true;
        }

        public Dictionary<string, string> GetStats()
        {
            socket.Connect(endpoint);
            networkStream = new NetworkStream(socket);
            bufferedStream = new BufferedStream(networkStream);
            byte[] buffer = Encoding.ASCII.GetBytes(CommandString);

            SocketError socketError;
            socket.Send(buffer, 0, buffer.Length, SocketFlags.None, out socketError);
            if (socketError != SocketError.Success)
            {
                throw new Exception(string.Format("发送数据没有得到正确的返回：{0}", socketError));
            }
            else
            {
                string result = ReadLine();
                Dictionary<string, string> serverData = new Dictionary<string, string>(StringComparer.Ordinal);
                while (!string.IsNullOrEmpty(result))
                {
                    if (String.Compare(result, "END", StringComparison.Ordinal) == 0)
                        break;

                    if (result.Length < 6 || String.Compare(result, 0, "STAT ", 0, 5, StringComparison.Ordinal) != 0)
                    {
                        continue;
                    }

                    string[] parts = result.Remove(0, 5).Split(' ');
                    if (parts.Length != 2)
                    {
                        continue;
                    }
                    serverData[parts[0]] = parts[1];
                    result = ReadLine();
                }
                return serverData;
            }
        }

        private string ReadLine()
        {
            MemoryStream ms = new MemoryStream(50);

            bool gotR = false;
            byte[] buffer = new byte[1];
            int data;

            try
            {
                while (true)
                {
                    data = bufferedStream.ReadByte();

                    if (data == 13)
                    {
                        gotR = true;
                        continue;
                    }

                    if (gotR)
                    {
                        if (data == 10)
                            break;

                        ms.WriteByte(13);

                        gotR = false;
                    }

                    ms.WriteByte((byte)data);
                }
            }
            catch (IOException)
            {
                throw;
            }

            string retureValue = Encoding.ASCII.GetString(ms.GetBuffer(), 0, (int)ms.Length);


            if (String.IsNullOrEmpty(retureValue))
                throw new Exception("接收到空响应");

            if (String.Compare(retureValue, GenericErrorResponse, StringComparison.Ordinal) == 0)
                throw new NotSupportedException("无效的指令");

            if (retureValue.Length >= ErrorResponseLength)
            {
                if (String.Compare(retureValue, 0, ClientErrorResponse, 0, ErrorResponseLength, StringComparison.Ordinal) == 0)
                {
                    throw new Exception(retureValue.Remove(0, ErrorResponseLength));
                }
                else if (String.Compare(retureValue, 0, ServerErrorResponse, 0, ErrorResponseLength, StringComparison.Ordinal) == 0)
                {
                    throw new Exception(retureValue.Remove(0, ErrorResponseLength));
                }
            }

            return retureValue;
        }

        public void Dispose()
        {
            if (socket != null)
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            socket = null;
            networkStream.Dispose();
            networkStream = null;
            bufferedStream.Dispose();
            bufferedStream = null;
        }
    }
}
