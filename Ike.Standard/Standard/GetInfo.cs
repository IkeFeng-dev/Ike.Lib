using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace Ike.Standard
{
    /// <summary>
    /// 信息获取类
    /// </summary>
    public static partial class GetInfo
    {
        /// <summary>
        /// 获取本地计算机名称
        /// </summary>
        public static string HostName { get { return Dns.GetHostName(); } }
        /// <summary>
        /// 获取操作系统版本
        /// </summary>
        public static string OperatingSystem { get { return Environment.OSVersion.VersionString; } }
        /// <summary>
        /// 获取本地IPV4 / V6 / 默认网关的地址
        /// </summary>
        public static IPAddress[] HostAddress { get { return Dns.GetHostEntry(HostName).AddressList; } }
        /// <summary>
        /// 获取系统区域
        /// </summary>
        public static string SystemEncoding { get { return System.Threading.Thread.CurrentThread.CurrentCulture.Name; } }
        /// <summary>
        /// 电脑主机名称
        /// </summary>
        public static string MachineName { get { return Environment.MachineName; } }
        /// <summary>
        /// 当前程序位数
        /// </summary>
        public static string Soft_Bit { get { return ((IntPtr.Size == 8) && Environment.Is64BitProcess) ? "x64" : "x86"; } }
        /// <summary>
        /// 当前操作系统位数
        /// </summary>
        public static string OperatingSystemBit { get { return Environment.Is64BitOperatingSystem ? "x64" : "x86"; } }
        /// <summary>
        /// 获取应用程序工作目录(环境目录)
        /// </summary>
        public static string EnvDirectory { get { return Environment.CurrentDirectory; } }
        /// <summary>
        /// 获取应用程序根目录
        /// </summary>
        public static string BaseDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        /// <summary>
        /// 桌面路径
        /// </summary>
        public static string DesktopPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\"; } }
        /// <summary>
        /// 获取当前所在程序位置程序集名称
        /// </summary>
        public static string GetCurrentProgramPath { get { return System.Reflection.Assembly.GetExecutingAssembly().Location; } }

        /// <summary>
        /// 获取网络适配器名称
        /// </summary>
        /// <returns></returns>
        public static string[] GetAdaptersName()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            string[] name = new string[adapters.Length];
            int index = 0;
            foreach (NetworkInterface ni in adapters)
            {
                name[index] = ni.Description;
                index++;
            }
            return name;
        }


        /// <summary>
        /// 获取本地IPv6地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIPv6Address()
        {
            List<string> list = new List<string>();
            foreach (IPAddress item in HostAddress)
            {
                if (!item.ToString().Contains("::") && item.AddressFamily.ToString() == "InterNetworkV6")
                {
                    list.Add(item.ToString());
                }
            }
            return list;
        }

        /// <summary>
        /// 获取本地IPv4地址
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIPv4Address()
        {
            List<string> list = new List<string>();
            foreach (System.Net.IPAddress item in HostAddress)
            {
                if (item.AddressFamily.ToString() == "InterNetwork")
                {
                    list.Add(item.ToString());
                }
            }
            return list;
        }



    }
}
