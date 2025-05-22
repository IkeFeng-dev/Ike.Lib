using System;
using System.Threading;
using System.Threading.Tasks;
using static Ike.Standard.Sockets;

namespace Ike.Standard.DataInteraction
{
    /// <summary>
    /// 局域网网络通讯
    /// </summary>
    public class NetworkSocket : IDisposable
    {
        /// <summary>
        /// 添加释放标志
        /// </summary>
        private bool _disposed = false;
        /// <summary>
        /// 目标IP地址
        /// </summary>
        private readonly string _ipAddress;
        /// <summary>
        /// 目标IP端口号
        /// </summary>
        private readonly int _port;
        /// <summary>
        /// 当接收数据时触发的事件
        /// </summary>
        public event Action<string> ReceiveEvent;
        /// <summary>
        /// 日志事件,收发时系统消息
        /// </summary>
        public event Action<string> LogEvent;
        /// <summary>
        /// 监听当前实例消息接收
        /// </summary>
        private readonly SocketListen socketListen;
        /// <summary>
        /// 发送消息实例
        /// </summary>
        private readonly SocketSend socketSend;
        /// <summary>
        /// 等待接收-接收消息
        /// </summary>
        private volatile string _receivedMessage;
        /// <summary>
        /// 等待接收-是否接收成功
        /// </summary>
        private volatile bool _isMessageReceived;
        /// <summary>
        /// 等待接收-是否触发信号
        /// </summary>
        private volatile bool _isWaitReceived;
        /// <summary>
        /// 等待接收-信号
        /// </summary>
        private readonly AutoResetEvent _messageEvent = new AutoResetEvent(false);

        /// <summary>
        /// 局域网TPC通讯构造函数
        /// </summary>
        /// <param name="targetIpaddress">目标IP地址</param>
        /// <param name="targetPort">目标IP端口号</param>
        /// <param name="thisPort">本机IP端口号</param>
        /// <param name="maxConnect">本机支持最大连接数</param>
        /// <param name="receviceCallBreak">处理接收数据并且回复消息的方法,如果不通过此方法返回数据也可使用 [<see cref="ReceiveEvent"/>] 事件接收后主动调用 [<see cref="Send(string)"/>] 发送数据到目标地址 </param>
        public NetworkSocket(string targetIpaddress, int targetPort, int thisPort, int maxConnect, Func<string, Task<string>> receviceCallBreak)
        {
            _ipAddress = targetIpaddress;
            _port = targetPort;
            if (receviceCallBreak == null)
            {
                socketListen = new SocketListen(thisPort, maxConnect, ListenCallBreak);
            }
            else
            {
                socketListen = new SocketListen(thisPort, maxConnect, ListenCallBreak, receviceCallBreak);
            }
            socketSend = new SocketSend(targetIpaddress, targetPort, SendCallBreak);
            Task.Run(async () =>
            {
                try
                {
                    await socketListen.Start();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }


        /// <summary>
        /// 监听回调事件
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="type">消息类型</param>
        private void ListenCallBreak(string message, MessageType type)
        {
            if (type == MessageType.Receive)
            {
                if (_isWaitReceived)
                {
                    _receivedMessage = message;
                    _isMessageReceived = true;
                    _isWaitReceived = false;
                    _messageEvent.Set();
                }
                ReceiveEvent?.Invoke(message);
            }
            else
            {
                LogEvent?.Invoke(message);
            }
        }


        /// <summary>
        /// 发送回调事件
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="type">消息类型</param>
        private void SendCallBreak(string message, MessageType type)
        {
            LogEvent?.Invoke(message);
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public bool Send(string message)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            return socketSend.Send(message);
        }


        /// <summary>
        /// 发送并且等待接收
        /// </summary>
        /// <param name="message">发送的消息</param>
        /// <param name="timeout">超时时间(ms)</param>
        /// <param name="result">如果接收成功则为接收信息,反之则为异常消息</param>
        public bool SendWaitReceive(string message, int timeout, out string result)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            try
            {
                result = SendAndReceive(_ipAddress, _port, message, timeout);
                return true;
            }
            catch (Exception ex)
            {
                LogEvent?.Invoke($"[Exception] {ex.Message}");
                result = ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 等待接收一条消息
        /// </summary>
        /// <param name="result">等待结果,如果成功则为接收的消息,如果失败,则为失败说明</param>
        /// <param name="timeout">等待超时时间(ms)</param>
        /// <returns>是否接收成功</returns>
        public bool WaitReceiveMessage(out string result, int timeout)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            try
            {
                _isWaitReceived = true;
                _isMessageReceived = false;
                if (_messageEvent.WaitOne(timeout) && _isMessageReceived)
                {
                    result = _receivedMessage;
                    _isMessageReceived = false;
                    return true;
                }
                result = "Wait timeout";
                return false;
            }
            catch (Exception ex)
            {
                LogEvent?.Invoke($"[Exception] {ex.Message}");
                result = ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// 实际的资源释放逻辑
        /// </summary>
        /// <param name="disposing">是否由Dispose方法调用</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _messageEvent?.Dispose();
                ReceiveEvent = null;
                LogEvent = null;
                socketListen.Stop();
            }
            _disposed = true;
        }


        /// <summary>
        /// 可选：终结器（安全网）
        /// </summary>
        ~NetworkSocket()
        {
            Dispose(false);
        }
    }


}
