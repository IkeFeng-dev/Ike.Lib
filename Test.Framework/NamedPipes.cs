//using System;
//using System.Collections.Generic;
//using System.IO.Pipes;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading;

//namespace Ike.Standard.ProcessChannel
//{
//    /// <summary>
//    /// 命名管道通讯
//    /// </summary>
//    public class NamedPipes : IDisposable
//    {
//        private NamedPipeServerStream _pipeServer;
//        private NamedPipeClientStream _pipeClient;
//        private readonly string _pipeName;
//        private const int BufferSize = 4096;
//        private bool _isDisposed;

//        /// <summary>
//        /// 构造函数
//        /// </summary>
//        /// <param name="pipeName">管道名称</param>
//        public NamedPipes(string pipeName)
//        {
//            _pipeName = pipeName ?? throw new ArgumentNullException(nameof(pipeName));
//        }

//        /// <summary>
//        /// 启动服务器端
//        /// </summary> 
//        /// <param name="onMessageReceived">接收到消息时的回调函数</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        public async Task StartServerAsync(Action<string> onMessageReceived, CancellationToken cancellationToken = default)
//        {
//            if (_pipeServer != null)
//                throw new InvalidOperationException("Server is already running.");

//            _pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

//            while (!cancellationToken.IsCancellationRequested)
//            {
//                System.Console.WriteLine("Waiting for client connection...");
//                await _pipeServer.WaitForConnectionAsync(cancellationToken);
//                System.Console.WriteLine("Client connected.");

//                try
//                {
//                    while (_pipeServer.IsConnected && !cancellationToken.IsCancellationRequested)
//                    {
//                        var message = await ReceiveMessageAsync(_pipeServer, cancellationToken);
//                        if (message != null)
//                        {
//                            onMessageReceived?.Invoke(message);
//                        }
//                    }
//                }
//                catch (IOException)
//                {
//                    System.Console.WriteLine("Client disconnected.");
//                }
//                finally
//                {
//                    if (_pipeServer.IsConnected)
//                    {
//                        _pipeServer.Disconnect();
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// 启动客户端
//        /// </summary>
//        /// <param name="cancellationToken">取消令牌</param>
//        public async Task ConnectClientAsync(CancellationToken cancellationToken = default)
//        {
//            if (_pipeClient != null)
//                throw new InvalidOperationException("Client is already connected.");

//            _pipeClient = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
//            System.Console.WriteLine("Connecting to server...");
//            await _pipeClient.ConnectAsync(cancellationToken);
//            System.Console.WriteLine("Connected to server.");
//        }

//        private PipeStream GetActivePipe()
//        {
//            if (_pipeServer != null && _pipeServer.IsConnected)
//                return _pipeServer;
//            if (_pipeClient != null && _pipeClient.IsConnected)
//                return _pipeClient;
//            throw new InvalidOperationException("No active pipe connection.");
//        }

//        /// <summary>
//        /// 发送消息
//        /// </summary>
//        /// <param name="message">消息内容</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
//        {
//            if (string.IsNullOrEmpty(message))
//                throw new ArgumentNullException(nameof(message));

//            var pipe = GetActivePipe();
//            var buffer = Encoding.UTF8.GetBytes(message);
//            await pipe.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
//            await pipe.FlushAsync(cancellationToken);
//        }

//        /// <summary>
//        /// 接收消息
//        /// </summary>
//        /// <param name="pipe">管道流</param>
//        /// <param name="cancellationToken">取消令牌</param>
//        /// <returns>接收到的消息</returns>
//        private async Task<string> ReceiveMessageAsync(PipeStream pipe, CancellationToken cancellationToken = default)
//        {
//            var buffer = new byte[BufferSize];
//            var messageBuilder = new StringBuilder();

//            do
//            {
//                int bytesRead = await pipe.ReadAsync(buffer, 0, BufferSize, cancellationToken);
//                if (bytesRead == 0)
//                    break;

//                messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
//            } while (!pipe.IsMessageComplete);

//            return messageBuilder.ToString();
//        }

//        /// <summary>
//        /// 释放资源
//        /// </summary>
//        public void Dispose()
//        {
//            if (_isDisposed)
//                return;

//            _pipeServer?.Dispose();
//            _pipeClient?.Dispose();
//            _isDisposed = true;
//        }

//    }
//}
