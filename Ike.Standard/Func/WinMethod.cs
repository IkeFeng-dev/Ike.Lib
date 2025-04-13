using System;

namespace Ike.Standard
{
    /// <summary>
    /// Windows API实现方法
    /// </summary>
    public static class WinMethod
    {
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
            IntPtr consoleHandle = WinAPI.GetStdHandle(ConstValue.STD_INPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero) return;
            // 获取当前控制台模式
            if (!WinAPI.GetConsoleMode(consoleHandle, out uint currentMode))
                return;
            // 禁用快速编辑模式（保留其他标志）
            uint newMode = currentMode & ~ConstValue.ENABLE_QUICK_EDIT_MODE;
            // 确保保留扩展标志（部分系统需要）
            newMode |= ConstValue.ENABLE_EXTENDED_FLAGS;
            WinAPI.SetConsoleMode(consoleHandle, newMode);
        }


        /// <summary>
        /// 启用控制台快速编辑模式（恢复默认编辑功能）
        /// </summary>
        public static void EnableQuickEdit()
        {
            IntPtr consoleHandle = WinAPI.GetStdHandle(ConstValue.STD_INPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero) return;

            // 获取当前控制台模式
            if (!WinAPI.GetConsoleMode(consoleHandle, out uint currentMode))
                return;

            // 启用快速编辑模式（保留其他标志）
            uint newMode = currentMode | ConstValue.ENABLE_QUICK_EDIT_MODE;

            // 确保保留扩展标志（部分系统需要）
            newMode |= ConstValue.ENABLE_EXTENDED_FLAGS;

            WinAPI.SetConsoleMode(consoleHandle, newMode);
        }



    }
}
