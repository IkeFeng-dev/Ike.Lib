using System;
using static Ike.Standard.WinAPI;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Ike.Standard
{
    /// <summary>
    /// Windows API实现方法
    /// </summary>
    public static class WinMethod
    {

#if NETFRAMEWORK
        /// <summary>
        /// 获取文件或扩展名关联的图标
        /// </summary>
        /// <param name="fileNameOrSuffixName">文件路径或后缀名</param>
        /// <param name="iconSizeIsMax">获取的大小选择,<see langword="true"/>表示大图标,反之返回小图标</param>
        /// <returns></returns>
        public static Icon GetFileAssociatedIcon(string fileNameOrSuffixName, bool iconSizeIsMax)
        {
            //272为小图标,273为大图标
            uint siz = 272;
            if (iconSizeIsMax)
            {
                siz = 273;
            }
            SHFILEINFO fi = new SHFILEINFO();
            Icon ic = null;
            int iTotal = (int)SHGetFileInfo(fileNameOrSuffixName, 100, ref fi, 0, siz);
            if (iTotal > 0)
            {
                ic = Icon.FromHandle(fi.hIcon);
            }
            return ic;
        }

#endif


        /// <summary>
        /// 获取当前使用的电源方案
        /// </summary>
        /// <returns></returns>
        public static string GetPowerScheme()
        {
            string powerSchemeName = "";
            var p_ActivePolicyGuid = IntPtr.Zero;
            var aa = PowerGetActiveScheme(IntPtr.Zero, ref p_ActivePolicyGuid);
            if (aa == 0)
            {
                Guid guid = (Guid)Marshal.PtrToStructure(p_ActivePolicyGuid, typeof(Guid));
                uint sizeName = 1024;
                IntPtr pSizeName = Marshal.AllocHGlobal((int)sizeName);
                try
                {
                    PowerReadFriendlyName(IntPtr.Zero, ref guid, IntPtr.Zero, IntPtr.Zero, pSizeName, ref sizeName);
                    powerSchemeName = Marshal.PtrToStringUni(pSizeName);
                }
                finally
                {
                    Marshal.FreeHGlobal(pSizeName);
                }
            }
            return powerSchemeName;
        }




        /// <summary>
        /// 查找指定窗体句柄
        /// </summary>
        /// <param name="name">窗体标题或者窗体类别,取决于<paramref name="nameIsTitle"/>标志</param>
        /// <param name="nameIsTitle">表示查找句柄是通过窗体标题还是窗体类别,<see langword="true"/>表示使用窗体标题查找,反之使用窗体类别查找 </param>
        /// <returns>返回查找的窗体句柄,找不到则返回<see cref="IntPtr.Zero"/> </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IntPtr FindWindow(string name, bool nameIsTitle)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            IntPtr hWnd;
            if (nameIsTitle)
            {
                hWnd = WinAPI.FindWindow(null, name);
            }
            else
            {
                hWnd = WinAPI.FindWindow(name, null);
            }
            return hWnd;
        }


        /// <summary>
        /// 禁用控制台编辑
        /// </summary>
        public static void DisableQuickEdit()
        {
            IntPtr consoleHandle = WinAPI.GetStdHandle(WinMessages.STD_INPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero) return;
            // 获取当前控制台模式
            if (!WinAPI.GetConsoleMode(consoleHandle, out uint currentMode))
                return;
            // 禁用快速编辑模式（保留其他标志）
            uint newMode = currentMode & ~WinMessages.ENABLE_QUICK_EDIT_MODE;
            // 确保保留扩展标志（部分系统需要）
            newMode |= WinMessages.ENABLE_EXTENDED_FLAGS;
            WinAPI.SetConsoleMode(consoleHandle, newMode);
        }


        /// <summary>
        /// 启用控制台快速编辑模式（恢复默认编辑功能）
        /// </summary>
        public static void EnableQuickEdit()
        {
            IntPtr consoleHandle = WinAPI.GetStdHandle(WinMessages.STD_INPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero) return;

            // 获取当前控制台模式
            if (!WinAPI.GetConsoleMode(consoleHandle, out uint currentMode))
                return;

            // 启用快速编辑模式（保留其他标志）
            uint newMode = currentMode | WinMessages.ENABLE_QUICK_EDIT_MODE;

            // 确保保留扩展标志（部分系统需要）
            newMode |= WinMessages.ENABLE_EXTENDED_FLAGS;

            WinAPI.SetConsoleMode(consoleHandle, newMode);
        }


        /// <summary>
        /// 启用控制台ANSI转义序列支持
        /// </summary>
        /// <remarks>
        /// <para>功能说明：</para>
        /// <list type="bullet">
        /// <item>通过设置控制台模式启用虚拟终端处理</item>
        /// <item>允许解析ANSI颜色代码(如\x1b[31m)等转义序列</item>
        /// <item>Windows 10 1607及以上版本原生支持</item>
        /// </list>
        /// </remarks>
        public static bool  EnableAnsiSupport()
        {
            var handle = WinAPI.GetStdHandle(WinMessages.STD_OUTPUT_HANDLE);
            if (handle != IntPtr.Zero)
            {
                WinAPI.GetConsoleMode(handle, out uint mode);
                WinAPI.SetConsoleMode(handle, mode | WinMessages.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 保持系统唤醒状态以及显示器开启（持续生效）
        /// </summary>
        /// <remarks>
        /// [<see langword="2025年4月27日08:41:42" />]
        /// </remarks>
        public static void KeepSystemAwake()
        {
            WinAPI.SetThreadExecutionState(WinAPI.ExecutionState.Continuous | WinAPI.ExecutionState.SystemRequired | WinAPI.ExecutionState.DisplayRequired);
        }


        /// <summary>
        /// 恢复系统默认的电源管理行为,取消<see cref="KeepSystemAwake()"/>的状态
        /// </summary>
        /// <remarks>
        /// [<see langword="2025年4月27日08:41:42" />]
        /// </remarks>
        public static void ReleaseSystemWake()
        {
            WinAPI.SetThreadExecutionState(WinAPI.ExecutionState.Continuous);
        }
    }
}
