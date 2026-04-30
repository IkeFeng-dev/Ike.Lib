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
	public partial class Sockets
	{

		/// <summary>
		/// <see cref="Socket"/>消息发送,不接收返回信息
		/// </summary>
		/// <param name="ip">IP地址</param>
		/// <param name="port">端口号</param>
		/// <param name="message">发送的消息</param>
		public static void SendMessage(string ip, int port, string message)
		{
			byte[] data = Encoding.UTF8.GetBytes(message);
			using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				IPAddress ipAddress = IPAddress.Parse(ip);
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
				client.Connect(remoteEP);
				client.Send(data, SocketFlags.None);
				client.Shutdown(SocketShutdown.Both);
			}

		}


		/// <summary>
		/// 异步<see cref="Socket"/>消息发送,不接收返回信息
		/// </summary>
		/// <param name="ip">IP地址</param>
		/// <param name="port">端口号</param>
		/// <param name="message">发送的消息</param>
		/// <returns>返回任务对象</returns>
		public static async Task SendMessageAsync(string ip, int port, string message)
		{
			byte[] data = Encoding.UTF8.GetBytes(message);
			using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				IPAddress ipAddress = IPAddress.Parse(ip);
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
				await client.ConnectAsync(remoteEP);
				await client.SendAsync(new ArraySegment<byte>(data), SocketFlags.None);
				client.Shutdown(SocketShutdown.Both);
			}
		}


		/// <summary>
		/// <see cref="Socket"/>消息发送,发送到指定的服务端，并接收响应
		/// </summary>
		/// <param name="ip">IP地址</param>
		/// <param name="port">端口号</param>
		/// <param name="message">发送的消息</param>
		/// <param name="receiveTimeout">指定接收消息的超时时间(ms)</param>
		/// <returns>接收到的响应消息</returns>
		public static string SendAndReceive(string ip, int port, string message, int receiveTimeout = 5000)
		{
			byte[] data = Encoding.UTF8.GetBytes(message);
			byte[] buffer = new byte[1024];
			string response = string.Empty;
			using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
			{
				client.ReceiveTimeout = receiveTimeout;
				IPAddress ipAddress = IPAddress.Parse(ip);
				IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
				client.Connect(remoteEP);
				client.Send(data, SocketFlags.None);
				int bytesReceived = client.Receive(buffer, SocketFlags.None);
				response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
				client.Shutdown(SocketShutdown.Both);
			}
			return response;
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
