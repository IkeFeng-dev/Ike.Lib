using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ike.Standard
{
	/// <summary>
	/// <see cref="Socket"/> 操作类
	/// </summary>
	public partial class Sockets
	{
		/// <summary>
		/// 侦听消息类型
		/// </summary>
		public enum MessageType
		{
			/// <summary>
			/// 接收
			/// </summary>
			Receive,
			/// <summary>
			/// 发送
			/// </summary>
			Send,
			/// <summary>
			/// 异常
			/// </summary>
			Error,
			/// <summary>
			/// 信息
			/// </summary>
			Info
		}

		/// <summary>
		/// 端口消息侦听
		/// </summary>
		public class SocketListen
		{
			/// <summary>
			/// 获取Socket侦听是否开启
			/// </summary>
			public bool ReceiveStatus { get; private set; } = false;
			/// <summary>
			/// 控制监听状态
			/// </summary>
			private bool _disposed = true;
			/// <summary>
			/// 侦听端口号
			/// </summary>
			private readonly int _port;
			/// <summary>
			/// 侦听连接数
			/// </summary>
			private readonly int _listenConnect = 100;
			/// <summary>
			/// 侦听实例创建状态
			/// </summary>
			private readonly bool _createAnInstance = false;
			/// <summary>
			/// 消息处理的回调方法
			/// </summary>
			private readonly Action<string, MessageType> _method;
			/// <summary>
			/// 消息处理回复的回调方法
			/// </summary>
			private readonly Func<string, Task<string>> _asyncreplyMethod;


			/// <summary>
			/// 初始化侦听实例,创建一个接收<see cref="Socket"/>消息的服务端,不会回复客户端消息
			/// </summary>
			/// <param name="portNumber">侦听端口号</param>
			/// <param name="listenConnect">侦听最大连接数</param>
			/// <param name="method">接收消息的以及日志信息的回调方法</param>
			public SocketListen(int portNumber, int listenConnect, Action<string, MessageType> method)
			{
				if (method is null)
				{
					throw new ArgumentNullException(nameof(method));
				}
				if (portNumber <= 0 || portNumber > 65535)
				{
					throw new ArgumentException(nameof(portNumber));
				}
				if (listenConnect <= 0)
				{
					throw new ArgumentException(nameof(listenConnect));
				}
				_port = portNumber;
				_listenConnect = listenConnect;
				_method = method;
				_createAnInstance = true;
			}



			/// <summary>
			/// 初始化侦听实例,创建一个接收并且响应回复的<see cref="Socket"/>服务端
			/// </summary>
			/// <param name="portNumber">侦听端口号</param>
			/// <param name="listenConnect">侦听最大连接数</param>
			/// <param name="method">接收消息的以及日志信息的回调方法,在此处用于获取日志信息</param>
			/// <param name="replyMethod">处理并且回复客户端的回调方法</param>
			public SocketListen(int portNumber, int listenConnect, Action<string, MessageType> method, Func<string, Task<string>> replyMethod)
			{
				if (method is null)
				{
					throw new ArgumentNullException(nameof(method));
				}
				if (replyMethod is null)
				{
					throw new ArgumentNullException(nameof(replyMethod));
				}
				if (portNumber <= 0 || portNumber > 65535)
				{
					throw new ArgumentException(nameof(portNumber));
				}
				if (listenConnect <= 0)
				{
					throw new ArgumentException(nameof(listenConnect));
				}
				_method = method;
				_asyncreplyMethod = replyMethod;
				_port = portNumber;
				_listenConnect = listenConnect;
				_createAnInstance = true;
			}


			/// <summary>
			/// 停止消息侦听
			/// </summary>
			/// <returns></returns>
			public void Stop()
			{
				_disposed = false;
				ReceiveStatus = false;
				_method("Stopped listening", MessageType.Info);
			}

			/// <summary>
			/// 启动消息侦听
			/// </summary>
			/// <returns></returns>
			public async Task Start()
			{
				if (ReceiveStatus)
				{
					string message = "There is already a listening thread executing, unable to continue creating listening";
					_method(message, MessageType.Error);
					return;
				}
				if (!_createAnInstance)
				{
					string message = "Instance object not created";
					_method(message, MessageType.Error);
					throw new Exception(message);
				}
				_disposed = true;
				using (Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				{
					IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _port);
					listener.Bind(localEndPoint);
					listener.Listen(_listenConnect);
					_method?.Invoke($"Service is listening: {localEndPoint}", MessageType.Info);
					while (_disposed)
					{
						ReceiveStatus = true;
						try
						{
							Socket client = await listener.AcceptAsync();
							_ = Task.Run(() => HandleClientAsync(client));
						}
						catch (Exception ex)
						{
							_method?.Invoke($"[Error] {ex.Message}", MessageType.Error);
						}
					}
				}
			}

			/// <summary>
			/// 独立处理客户端的方法
			/// </summary>
			/// <param name="client">连接对象</param>
			/// <returns></returns>
			private async Task HandleClientAsync(Socket client)
			{
				try
				{
					_method?.Invoke($"Client {client.RemoteEndPoint} connected", MessageType.Info);
					var receive = await ReceiveMessage(client);
					if (_asyncreplyMethod != null && receive != null)
					{
						string handle = await _asyncreplyMethod(receive);
						byte[] data = Encoding.UTF8.GetBytes(handle);
						await client.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
						_method?.Invoke(handle, MessageType.Send);
					}
				}
				catch (Exception ex)
				{
					_method?.Invoke($"[Error] {ex.Message}", MessageType.Error);
				}
				finally
				{
					client.Dispose();
				}
			}

			/// <summary>
			/// 接收消息转换为字符串
			/// </summary>
			/// <param name="client">socket对象</param>
			/// <returns></returns>
			private async Task<string> ReceiveMessage(Socket client)
			{
				byte[] buffer = new byte[1024];
				while (client.Connected)
				{
					try
					{
						int bytesRead = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
						if (bytesRead > 0)
						{
							string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
							_method(message, MessageType.Receive);
							return message;
						}
						else
						{
							_method?.Invoke($"Client {client.RemoteEndPoint} disconnected", MessageType.Info);
							client.Close();
							break;
						}
					}
					catch (SocketException ex)
					{
						_method?.Invoke($"Socket error: {ex.Message}", MessageType.Error);
						break;
					}
					catch (Exception ex)
					{
						_method?.Invoke($"Exception: {ex.Message}", MessageType.Error);
						client.Close();
						break;
					}
				}
				return null;
			}
		}
	}
}
