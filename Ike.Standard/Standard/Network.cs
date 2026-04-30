using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Ike.Standard
{
	/// <summary>
	/// 网络操作类
	/// </summary>
	public class Network
	{
		/// <summary>
		/// 检测是否连通指定IP
		/// </summary>
		/// <param name="ip">IP地址如<see langword="127.0.0.1"/> 格式</param>
		/// <returns>是否成功</returns>
		public static bool PingIp(string ip)
		{
			using (var ping = new System.Net.NetworkInformation.Ping())
			{
				System.Net.NetworkInformation.PingReply pr = ping.Send(ip);
				return pr.Status == System.Net.NetworkInformation.IPStatus.Success;
			}
		}


		/// <summary>
		/// 检测指定端口号是否打开
		/// </summary>
		/// <param name="ip">IP</param>
		/// <param name="port">端口号</param>
		/// <returns></returns>
		public static bool IsPortOpen(string ip, int port)
		{
			try
			{
				using (TcpClient client = new TcpClient())
				{
					client.Connect(ip, port);
					return true;
				}
			}
			catch
			{
				return false;
			}
		}


		/// <summary>
		/// 判断IPV4格式IP是否为私有地址
		/// </summary>
		/// <param name="ipAddress">IP</param>
		/// <returns></returns>
		public static bool IsPrivateIPAddress(string ipAddress)
		{
			if (IPAddress.TryParse(ipAddress, out IPAddress parsedIpAddress) && parsedIpAddress != null)
			{
				byte[] bytes = parsedIpAddress.GetAddressBytes();
				if (bytes.Length == 4 &&
					(bytes[0] == 10 ||
					 (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
					 (bytes[0] == 192 && bytes[1] == 168)))
				{
					return true;
				}
			}
			return false;
		}



	}
}
