#if NETFRAMEWORK
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ike.Standard
{
    /// <summary>
    /// 获取设备相关信息
    /// </summary>
    public static partial class GetInfo
    {
        /// <summary>
        /// 屏幕真实宽/边框宽
        /// </summary>
        public static double ScreenWidth { get { return Screen.PrimaryScreen.Bounds.Width; } }
        /// <summary>
        /// 屏幕真实高/宽框宽
        /// </summary>
        public static double ScreenHeight { get { return Screen.PrimaryScreen.Bounds.Height; } }



        /// <summary>
        /// 获取CPU使用占比(占比值,自行添加%)
        /// </summary>
        /// <returns></returns>
        public static float GetCPUUsage()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            System.Threading.Thread.Sleep(1000);
            return cpuCounter.NextValue();
        }


        /// <summary>
        /// 获取本机网卡名称("MS TCP Loopback interface"为集成网卡)
        /// </summary>
        /// <returns></returns>
        public static string[] GetNetworkCardName()
        {
            PerformanceCounterCategory category = new PerformanceCounterCategory("Network Interface");
            return category.GetInstanceNames();
        }



        /// <summary>
        /// 获取屏幕缩放比例
        /// </summary>
        /// <returns></returns>
        public static double GetScreenScalingFactor()
        {
            var g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            var physicalScreenHeight =WinAPI.GetDeviceCaps(desktop, 117);
            var screenScalingFactor = (double)physicalScreenHeight / Screen.PrimaryScreen.Bounds.Height;
            return screenScalingFactor;
        }
    }
}
#endif