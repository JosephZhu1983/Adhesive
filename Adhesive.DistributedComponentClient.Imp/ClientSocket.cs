
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Adhesive.Common;

namespace Adhesive.DistributedComponentClient
{
    internal class ClientSocket : IDisposable
    {
        private readonly Socket socket;
        private BufferedStream stream;
        private readonly Action<ClientSocket> disposeAction;
        private readonly Action<ClientSocket> lowlevelErrorAction;
        private readonly Action<ClientSocket> highLevelErrorAction;

        internal IPEndPoint Endpoint { get; private set; }
        internal DateTime CreateTime { get; private set; }
        internal DateTime BusyTime { get; private set; }
        internal DateTime IdleTime { get; private set; }
        internal ClientSocketStatus Status { get; private set; }

        private string FormatData(byte[] data)
        {
            return FormatData(data, 0, data.Length);
        }

        private string FormatData(byte[] buffer, int offset, int count)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("数据大小：{0}，数据内容", count);
            for (int i = 0; i < count; i++)
            {
                sb.AppendFormat("{0:x2}", buffer[offset + i]);
                if (i % 2 == 1) sb.Append(" ");
            }
            return sb.ToString();
        }

        private void Reset()
        {
            try
            {
                if (socket == null) return;

                if (stream != null) stream.Flush();

                int available = socket.Available;

                if (available > 0)
                {
                    byte[] data = new byte[available];
                    socket.Receive(data, 0, available, SocketFlags.None);
                    LocalLoggingService.Warning("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientSocket", "Reset",
                        string.Format("Reset socket {0} 时候还有未读取完的数据： {1}！", Endpoint.ToString(), FormatData(data)));
                }
            }
            catch (Exception ex)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientSocket", "Reset", string.Format("从 {0} 接受数据出错", Endpoint.ToString()), ex.ToString());
                lowlevelErrorAction(this);
            }
        }

        internal void Acquire()
        {
            Reset();
            BusyTime = DateTime.Now;
            IdleTime = DateTime.MaxValue;
            Status = ClientSocketStatus.Busy;
        }

        internal void Release()
        {
            BusyTime = DateTime.MaxValue;
            IdleTime = DateTime.Now;
            Status = ClientSocketStatus.Idle;
        }

        internal void Destroy()
        {
            Status = ClientSocketStatus.Destroy;
            if (stream != null)
                stream.Close();
            if (socket != null && socket.Connected)
            {
                try
                {
                    LocalLoggingService.Info("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientSocket", "Destroy",
                        string.Format("Destroy socket {0}", Endpoint.ToString()));
                    socket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientSocket", "Destroy", string.Format("Destroy socket {0} 的时候出现异常", Endpoint.ToString()), ex);
                }
                socket.Close();
            }
        }

        internal ClientSocket(Action<ClientSocket> disposeAction, Action<ClientSocket> lowlevelErrorAction, Action<ClientSocket> highLevelErrorAction, int sendTimeout, int receiveTimeout, string ip, int port)
        {
            this.disposeAction = disposeAction;
            this.lowlevelErrorAction = lowlevelErrorAction;
            this.highLevelErrorAction = highLevelErrorAction;
            Endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            socket.SendTimeout = sendTimeout;
            socket.ReceiveTimeout = receiveTimeout;
            CreateTime = DateTime.Now;
            IdleTime = DateTime.MaxValue;
            BusyTime = DateTime.MaxValue;
            Status = ClientSocketStatus.Idle;
        }

        internal void Connect(int timeout)
        {
            IAsyncResult result = socket.BeginConnect(Endpoint, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(timeout, true);
            if (!success)
                throw new Exception(string.Format("连接到 {0} 超过设定时间 {1} 毫秒", Endpoint.ToString(), timeout));
            else
                stream = new BufferedStream(new NetworkStream(socket));
        }

        internal byte[] Read(int count)
        {
            if (socket == null) return null;


            var buffer = new byte[count];
            int offset = 0;

            int read = 0;
            int shouldRead = count;

            while (read < count)
            {
                try
                {
                    int currentRead = stream.Read(buffer, offset, shouldRead);
                    if (currentRead < 1)
                        continue;

                    read += currentRead;
                    offset += currentRead;
                    shouldRead -= currentRead;
                }
                catch (Exception ex)
                {
                    LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientSocket", "Read", string.Format("从 {0} 接受数据出错", Endpoint.ToString()), ex.ToString());

                    lowlevelErrorAction(this);
                    Status = ClientSocketStatus.Error;
                    return null;
                }
            }

            return buffer;
        }

        internal void Write(byte[] data, int offset, int length)
        {
            if (socket == null)
            {
                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientSocket", "Write",
                    string.Format("Write 发送时没有获取到socket {0}", Endpoint.ToString()));
                return;
            }

            SocketError status;

            try
            {

                socket.Send(data, offset, length, SocketFlags.None, out status);

                if (status != SocketError.Success)
                {
                    Status = ClientSocketStatus.Error;
                    LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientSocket", "Write",
                        string.Format("Write 发送数据到 {0} 出错，错误为：{1}", Endpoint.ToString(), status.ToString()));
                    highLevelErrorAction(this);
                }
            }
            catch (Exception ex)
            {
                Status = ClientSocketStatus.Error;
                LocalLoggingService.Error("{0} {1} {2} {3}", DistributedServerConfiguration.ModuleName, "ClientSocket", "Write", string.Format("发送数据到 {0} 出错", Endpoint.ToString()), ex.ToString());
                highLevelErrorAction(this);
            }
        }

        internal void Write(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            disposeAction(this);
        }
    }
}
