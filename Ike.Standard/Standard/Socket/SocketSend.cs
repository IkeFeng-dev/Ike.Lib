using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Ike.Standard
{
	/// <summary>
	/// <see cref="Socket"/> 操作类
	/// </summary>
	public partial class Sockets
	{
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
			/// <param name="iP">IP</param>
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
				byte[] data = Encoding.UTF8.GetBytes(message);
				using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
				{
					IPAddress ipAddress = iP;
					IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
					try
					{
						client.Connect(remoteEP);
						_method?.Invoke($"Connect to {remoteEP}", MessageType.Info);
						client.Send(data);
						_method?.Invoke($"Send Successfully", MessageType.Info);
						client.Shutdown(SocketShutdown.Both);
						client.Close();
						return true;
					}
					catch (Exception exception)
					{
						_method?.Invoke($"[Exception] {exception.Message}", MessageType.Error);
						return false;
					}
				}
			}

		}
	}
}
