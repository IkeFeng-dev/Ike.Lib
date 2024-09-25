using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Ike.Standard
{
	/// <summary>
	/// <see cref="Socket"/> 操作类
	/// </summary>
	public class Sockets
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
			public SocketListen(int portNumber, int listenConnect, Action<string, MessageType> method,Func<string, Task<string>> replyMethod)
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
					_method($"Service is listening: {localEndPoint}", MessageType.Info);
					while (_disposed)
					{
						ReceiveStatus = true;
						try
						{
							using (Socket client = await listener.AcceptAsync())
							{
								if (!_disposed)
								{
									client.Send(Encoding.UTF8.GetBytes("The service terminal is out of service"));
									client.Close();
									listener.Close();
								}
								else
								{
									_method($"Client {client.RemoteEndPoint} connected", MessageType.Info);
									var receive = await ReceiveMessage(client);
									if (_asyncreplyMethod != null && receive != null)
									{
										string handle = await _asyncreplyMethod(receive);
										byte[] data = Encoding.UTF8.GetBytes(handle);
										await client.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
									}
								}
							}
						}
						catch (SocketException ex)
						{
							_method($"Socket error: {ex.Message}", MessageType.Error);
						}
						catch (Exception ex)
						{
							_method($"Exception: {ex.Message}", MessageType.Error);
						}
					}
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
							_method($"Client {client.RemoteEndPoint} disconnected", MessageType.Info);
							client.Close();
							break;
						}
					}
					catch (SocketException ex)
					{
						_method($"Socket error: {ex.Message}", MessageType.Error);
						break;
					}
					catch (Exception ex)
					{
						_method($"Exception: {ex.Message}", MessageType.Error);
						client.Close();
						break;
					}
				}
				return null;
			}
		}

		/// <summary>
		///<see cref="Socket"/>消息发送
		/// </summary>
		public class SocketSend
		{
			/// <summary>
			/// 消息处理方法
			/// </summary>
			private readonly Action<string, MessageType> _method;
			/// <summary>
			/// 发送到的IP地址
			/// </summary>
			private readonly IPAddress _sendIpaddress;
			/// <summary>
			/// 发送的端口号
			/// </summary>
			private readonly int _sendPoetNumber;

			/// <summary>
			/// 初始化消息发送实例化
			/// </summary>
			/// <param name="iPAddress">发送到的地址</param>
			/// <param name="portNumber">端口号</param>
			/// <param name="method">消息处理方法</param>
			public SocketSend(string iPAddress, int portNumber, Action<string, MessageType> method)
			{
				_sendIpaddress = IPAddress.Parse(iPAddress);
				_sendPoetNumber = portNumber;
				_method = method;
			}


			/// <summary>
			/// <see cref="Socket"/>消息发送,发送到指定服务端
			/// </summary>
			/// <param name="iP">IP(192.168.101.55)</param>
			/// <param name="port">端口号</param>
			/// <param name="message">发送的消息</param>
			/// <returns></returns>
			public bool SendToServer(string iP, int port, string message)
			{
				return SendMessage(IPAddress.Parse(iP), port, message);
			}

			/// <summary>
			/// <see cref="Socket"/>消息发送
			/// </summary>
			/// <param name="message">发送的消息</param>
			/// <returns></returns>
			public bool Send(string message)
			{
				return SendMessage(_sendIpaddress, _sendPoetNumber, message);
			}

			/// <summary>
			/// 消息发送,发送到当前指定的位置
			/// </summary>
			/// <param name="iP">IP地址</param>
			/// <param name="port">端口号</param>
			/// <param name="message">发送的消息</param>
			/// <returns></returns>
			private bool SendMessage(IPAddress iP, int port, string message)
			{
				if (_method == null)
				{
					throw new Exception("The message receiving processing method has not been assigned a value. Assignments can be made by calling the 'SetMessage Method' method or when creating an instance");
				}
				byte[] data = Encoding.UTF8.GetBytes(message);
				using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				{
					IPAddress ipAddress = iP;
					IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
					try
					{
						client.Connect(remoteEP);
						_method($"Connect to {remoteEP}", MessageType.Info);
						client.Send(data);
						_method($"Data successfully sent", MessageType.Info);
						client.Shutdown(SocketShutdown.Both);
						client.Close();
						return true;
					}
					catch (Exception exception)
					{
						_method($"Exception: {exception.Message}", MessageType.Error);
						return false;
					}
				}
			}
		}


		/// <summary>
		/// <see cref="Socket"/>消息发送,不接收返回信息
		/// </summary>
		/// <param name="ip">IP地址</param>
		/// <param name="port">端口号</param>
		/// <param name="message">发送的消息</param>
		/// <returns></returns>
		public static async void SendMessage(string ip, int port, string message)
		{
			byte[] data = Encoding.UTF8.GetBytes(message);
			using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				IPAddress ipAddress = IPAddress.Parse(ip);
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
				await client.ConnectAsync(remoteEP);
				await client.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
				client.Shutdown(SocketShutdown.Both);
				client.Close();
			}
		}


		/// <summary>
		/// 异步<see cref="Socket"/>消息发送,发送到指定的服务端，并接收响应
		/// </summary>
		/// <param name="ip">IP地址</param>
		/// <param name="port">端口号</param>
		/// <param name="message">发送的消息</param>
		/// <param name="receiveTimeout">指定接收消息的超时时间(ms)</param>
		/// <returns>接收到的响应消息</returns>
		public static async Task<string> SendAndReceiveAsync(string ip, int port, string message, int receiveTimeout = 5000)
		{
			byte[] data = Encoding.UTF8.GetBytes(message);
			byte[] buffer = new byte[1024];
			string response = string.Empty;
			using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				client.ReceiveTimeout = receiveTimeout;
				IPAddress ipAddress = IPAddress.Parse(ip);
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
				await client.ConnectAsync(remoteEP);
				await client.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
				int bytesReceived = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
				response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
				client.Shutdown(SocketShutdown.Both);
				client.Close();
			}
			return response;
		}



	}
}
