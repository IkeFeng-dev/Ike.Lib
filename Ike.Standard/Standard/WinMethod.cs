using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#if NETFRAMEWORK
using System.Windows.Forms;
#endif

namespace Ike.Standard
{
    /// <summary>
    /// Windows API实现方法
    /// </summary>
    public static partial class WinMethod
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
            WinAPI.SHFILEINFO fi = new WinAPI.SHFILEINFO();
            Icon ic = null;
            int iTotal = (int)WinAPI.SHGetFileInfo(fileNameOrSuffixName, 100, ref fi, 0, siz);
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
            var aa = WinAPI.PowerGetActiveScheme(IntPtr.Zero, ref p_ActivePolicyGuid);
            if (aa == 0)
            {
                Guid guid = (Guid)Marshal.PtrToStructure(p_ActivePolicyGuid, typeof(Guid));
                uint sizeName = 1024;
                IntPtr pSizeName = Marshal.AllocHGlobal((int)sizeName);
                try
                {
                    WinAPI.PowerReadFriendlyName(IntPtr.Zero, ref guid, IntPtr.Zero, IntPtr.Zero, pSizeName, ref sizeName);
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
            IntPtr consoleHandle = WinAPI.GetStdHandle(WinMessages.ConsoleCap.STD_INPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero) return;
            // 获取当前控制台模式
            if (!WinAPI.GetConsoleMode(consoleHandle, out uint currentMode))
                return;
            // 禁用快速编辑模式（保留其他标志）
            uint newMode = currentMode & ~WinMessages.ConsoleCap.ENABLE_QUICK_EDIT_MODE;
            // 确保保留扩展标志（部分系统需要）
            newMode |= WinMessages.ConsoleCap.ENABLE_EXTENDED_FLAGS;
            WinAPI.SetConsoleMode(consoleHandle, newMode);
        }


        /// <summary>
        /// 启用控制台快速编辑模式（恢复默认编辑功能）
        /// </summary>
        public static void EnableQuickEdit()
        {
            IntPtr consoleHandle = WinAPI.GetStdHandle(WinMessages.ConsoleCap.STD_INPUT_HANDLE);
            if (consoleHandle == IntPtr.Zero) return;

            // 获取当前控制台模式
            if (!WinAPI.GetConsoleMode(consoleHandle, out uint currentMode))
                return;

            // 启用快速编辑模式（保留其他标志）
            uint newMode = currentMode | WinMessages.ConsoleCap.ENABLE_QUICK_EDIT_MODE;

            // 确保保留扩展标志（部分系统需要）
            newMode |= WinMessages.ConsoleCap.ENABLE_EXTENDED_FLAGS;

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
        public static bool EnableAnsiSupport()
        {
            var handle = WinAPI.GetStdHandle(WinMessages.ConsoleCap.STD_OUTPUT_HANDLE);
            if (handle != IntPtr.Zero)
            {
                WinAPI.GetConsoleMode(handle, out uint mode);
                WinAPI.SetConsoleMode(handle, mode | WinMessages.ConsoleCap.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
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



        /// <summary>
        /// 查找窗体句柄
        /// </summary>
        /// <param name="windowNameorClassName">根据窗体标题或类名查找</param>
        /// <returns></returns>
        public static IntPtr FindWindowIntPtr(string windowNameorClassName)
        {
            IntPtr handle = WinAPI.FindWindow(null, windowNameorClassName);
            if (handle.Equals(IntPtr.Zero))
            {
                handle = WinAPI.FindWindow(windowNameorClassName, null);
                if (handle.Equals(IntPtr.Zero))
                {
                    return IntPtr.Zero;
                }
                else
                {
                    return handle;
                }
            }
            else
            {
                return handle;
            }
        }


        /// <summary>
        /// 根据窗体标题或类名设置窗体焦点
        /// </summary>
        /// <param name="windowNameorClassName">窗体标题或类名</param>
        /// <returns></returns>
        public static bool SetWindowFocus(string windowNameorClassName)
        {
            IntPtr intPtr = FindWindowIntPtr(windowNameorClassName);
            if (intPtr.Equals(IntPtr.Zero))
            {
                return false;
            }
            else
            {
                return WinAPI.SetWindowPos(intPtr, IntPtr.Zero, 0, 0, 0, 0, 2 | 1 | WinMessages.WindowsCap.WM_SHOWWINDOW);
            }
        }


        /// <summary>
        /// 根据窗体标题或类名设置窗体焦点
        /// </summary>
        /// <param name="intPtr">窗体句柄</param>
        /// <returns></returns>
        public static bool SetWindowFocus(IntPtr intPtr)
        {
            if (intPtr.Equals(IntPtr.Zero))
            {
                return false;
            }
            else
            {
                return WinAPI.SetWindowPos(intPtr, IntPtr.Zero, 0, 0, 0, 0, 2 | 1 | WinMessages.WindowsCap.WM_SHOWWINDOW);
            }
        }



        /// <summary>
        /// 设置窗体状态
        /// </summary>
        /// <param name="windowIntPtr">窗体句柄</param>
        /// <param name="windowsState">操作值,默认显示正常窗口</param>
        /// <returns></returns>
        public static bool SetWindowState(IntPtr windowIntPtr, WindowsState windowsState)
        {
            int number = 1;
            if (windowsState == WindowsState.Close)
            {
                number = 0;
            }
            else if (windowsState == WindowsState.Mini)
            {
                number = 2;
            }
            else if (windowsState == WindowsState.Max)
            {
                number = 3;
            }
            if (!windowIntPtr.Equals(IntPtr.Zero))
            {
                SetWindowFocus(windowIntPtr);
                WinAPI.ShowWindow(windowIntPtr, number);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 设置窗体状态
        /// </summary>
        /// <param name="windowNameorClassName">窗体名称或类名</param>
        /// <param name="windowsState">操作值,默认显示正常窗口</param>
        /// <returns></returns>
        public static bool SetWindowState(string windowNameorClassName, WindowsState windowsState)
        {
            IntPtr intPtr = FindWindowIntPtr(windowNameorClassName);
            if (!intPtr.Equals(IntPtr.Zero))
            {
                return SetWindowState(intPtr, windowsState);
            }
            return false;
        }


        /// <summary>
        /// 获取当前焦点窗体句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetCurrentForegroundWindow()
        {
            return WinAPI.GetForegroundWindow();
        }




        /// <summary>
        /// 获取窗体起始坐标
        /// </summary>
        /// <param name="intPtr">窗体句柄</param>
        /// <returns></returns>
        public static Structure.Point GetWindowStartPoint(IntPtr intPtr)
        {
            WinAPI.GetWindowRect(intPtr, out Structure.RECT rect);
            var point = new Structure.Point
            {
                X = rect.Left,
                Y = rect.Top
            };
            return point;
        }



        /// <summary>
        /// 获取窗体起始坐标
        /// </summary>
        /// <param name="windowNameorClassName">窗体类名或标题名称</param>
        /// <returns></returns>
        public static Structure.Point GetWindowStartPoint(string windowNameorClassName)
        {
            IntPtr intPtr = FindWindowIntPtr(windowNameorClassName);
            if (!intPtr.Equals(IntPtr.Zero))
            {
                WinAPI.GetWindowRect(intPtr, out Structure.RECT rect);
                var point = new Structure.Point
                {
                    X = rect.Left,
                    Y = rect.Top
                };
                return point;
            }
            return new Structure.Point();
        }





        /// <summary>
        /// 获取窗体上下左右四个坐标
        /// </summary>
        /// <param name="windowNameorClassName">窗体类名或标题名称</param>
        /// <returns></returns>
        public static Structure.RECT GetWindowRectInfo(string windowNameorClassName)
        {
            Structure.RECT rect = new Structure.RECT();
            IntPtr intPtr = FindWindowIntPtr(windowNameorClassName);
            if (!intPtr.Equals(IntPtr.Zero))
            {
                WinAPI.GetWindowRect(intPtr, out rect);
                return rect;
            }
            return rect;
        }



        /// <summary>
        /// 获取窗体上下左右四个坐标
        /// </summary>
        /// <param name="intPtr">窗体句柄</param>
        /// <returns></returns>
        public static Structure.RECT GetWindowRectInfo(IntPtr intPtr)
        {
            WinAPI.GetWindowRect(intPtr, out Structure.RECT rect);
            return rect;
        }



        /// <summary>
        /// 获取窗口宽
        /// </summary>
        /// <param name="intPtr">窗体句柄</param>
        /// <returns></returns>
        public static int GetWindowWidth(IntPtr intPtr)
        {
            if (!intPtr.Equals(IntPtr.Zero))
            {
                WinAPI.GetWindowRect(intPtr, out Structure.RECT rect);
                return rect.Right - rect.Left;
            }
            return 0;
        }


        /// <summary>
        /// 获取窗口宽
        /// </summary>
        /// <param name="windowNameorClassName">窗体类名或标题名称</param>
        /// <returns></returns>
        public static int GetWindowWidth(string windowNameorClassName)
        {
            IntPtr intPtr = FindWindowIntPtr(windowNameorClassName);
            if (!intPtr.Equals(IntPtr.Zero))
            {
                WinAPI.GetWindowRect(intPtr, out Structure.RECT rect);
                return rect.Right - rect.Left;
            }
            return 0;
        }


        /// <summary>
        /// 获取窗口高
        /// </summary>
        /// <param name="intPtr">窗体句柄</param>
        /// <returns></returns>
        public static int GetWindowHeight(IntPtr intPtr)
        {
            if (!intPtr.Equals(IntPtr.Zero))
            {
                WinAPI.GetWindowRect(intPtr, out Structure.RECT rect);
                return rect.Bottom - rect.Top;
            }
            return 0;
        }



        /// <summary>
        /// 获取窗口高
        /// </summary>
        /// <param name="windowNameorClassName">窗体类名或标题名称</param>
        /// <returns></returns>
        public static int GetWindowHeight(string windowNameorClassName)
        {
            IntPtr intPtr = FindWindowIntPtr(windowNameorClassName);
            if (!intPtr.Equals(IntPtr.Zero))
            {
                WinAPI.GetWindowRect(intPtr, out Structure.RECT rect);
                return rect.Bottom - rect.Top;
            }
            return 0;
        }


        /// <summary>
        /// 根据句柄设置TextBox内容
        /// </summary>
        /// <param name="windowHandle">窗体句柄</param>
        /// <param name="textBoxClassName">TextBox类名</param>
        /// <param name="text">设置的内容</param>
        /// <returns></returns>
        public static bool SetTextBoxTxt(IntPtr windowHandle, string textBoxClassName, string text)
        {
            //判断这个窗体是否有效
            if (!windowHandle.Equals(IntPtr.Zero))
            {
                IntPtr EdithWnd = WinAPI.FindWindowEx(windowHandle, (IntPtr)0, textBoxClassName, "");
                if (!EdithWnd.Equals(IntPtr.Zero))
                {
                    WinAPI.SendMessage(EdithWnd, WinMessages.WindowsCap.WM_SETTEXT, IntPtr.Zero, text);
                    return true;
                }
                else
                {
                    throw new Exception("未找到TextBox控件");
                }
            }
            else
            {
                throw new Exception("未找到父窗体");
            }
        }



        /// <summary>
        /// 根据句柄设置TextBox内容
        /// </summary>
        /// <param name="windowNameorClassName">窗体标题或类名</param>
        /// <param name="textBoxClassName">TextBox类名</param>
        /// <param name="text">设置的内容</param>
        /// <returns></returns>
        public static bool SetTextBoxTxt(string windowNameorClassName, string textBoxClassName, string text)
        {
            if (!SetTextBoxTxt(WinAPI.FindWindow(null, windowNameorClassName), textBoxClassName, text))
            {
                return SetTextBoxTxt(WinAPI.FindWindow(windowNameorClassName, null), textBoxClassName, text);
            }
            return false;
        }




        /// <summary>
        /// 根据句柄获取TextBox内容
        /// </summary>
        /// <param name="windowNameorClassName">窗体类名或标题</param>
        /// <param name="textBoxClassName">文本框标题或类名</param>
        /// <returns></returns>
        public static string GetTextBoxTxt(string windowNameorClassName, string textBoxClassName)
        {
            string text = GetTextBoxTxt(WinAPI.FindWindow(null, windowNameorClassName), textBoxClassName);
            if (text == string.Empty)
            {
                return GetTextBoxTxt(WinAPI.FindWindow(windowNameorClassName, null), textBoxClassName);
            }
            else
            {
                return text;
            }
        }


        /// <summary>
        /// 根据句柄获取TextBox内容
        /// </summary>
        /// <param name="windowHandle">窗体句柄</param>
        /// <param name="textBoxClassName">TextBox类名</param>
        /// <param name="getTextLength">要获取的字符串的长度</param>
        /// <returns></returns>
        public static string GetTextBoxTxt(IntPtr windowHandle, string textBoxClassName, int getTextLength = 100)
        {
            //判断这个窗体是否有效
            if (!windowHandle.Equals(IntPtr.Zero))
            {
                IntPtr EdithWnd = WinAPI.FindWindowEx(windowHandle, IntPtr.Zero, textBoxClassName, "");
                if (!EdithWnd.Equals(IntPtr.Zero))
                {
                    StringBuilder strbu = new StringBuilder(getTextLength);
                    WinAPI.SendMessage(EdithWnd, (int)WinMessages.WindowsCap.WM_GETTEXT, getTextLength, strbu);
                    return strbu.ToString();
                }
                else
                {
                    throw new Exception("未找到TextBox控件");
                }
            }
            else
            {
                throw new Exception("未找到父窗体");
            }
        }



        /// <summary>
        /// 根据句柄点击Button按钮
        /// </summary>
        /// <param name="windowNameorClassName">窗口标题或窗体类名</param>
        /// <param name="buttonClassName">按钮类名</param>
        /// <param name="buttonName">按钮Text</param>
        /// <returns></returns>
        public static bool ClickButton(string windowNameorClassName, string buttonClassName, string buttonName)
        {
            if (!ClickButton(WinAPI.FindWindow(null, windowNameorClassName), buttonClassName, buttonName))
            {
                return ClickButton(WinAPI.FindWindow(windowNameorClassName, null), buttonClassName, buttonName);
            }
            return true;
        }


        /// <summary>
        /// 根据句柄点击Button按钮
        /// </summary>
        /// <param name="windowHandle">父窗体句柄</param>
        /// <param name="buttonClassName">按钮类名</param>
        /// <param name="buttonName">按钮Text</param>
        /// <returns></returns>
        public static bool ClickButton(IntPtr windowHandle, string buttonClassName, string buttonName)
        {
            //判断这个窗体是否有效
            if (!windowHandle.Equals(IntPtr.Zero))
            {
                //触发按钮点击事件[窗体类名 子窗体类名 控件类名 控件名称]
                IntPtr handle = WinAPI.FindWindowEx(windowHandle, IntPtr.Zero, buttonClassName, buttonName);
                if (!handle.Equals(IntPtr.Zero))
                {
                    WinAPI.SendMessage(handle, WinMessages.WindowsCap.BM_CLICK, IntPtr.Zero, "0");
                    return true;
                }
                else
                {
                    throw new Exception("未找到Button控件");
                }
            }
            else
            {
                throw new Exception("未找到父窗体");
            }
        }

        /// <summary>
        /// 显示任务栏
        /// </summary>
        public static void ShowTaskbar()
        {
           WinAPI.ShowWindow((int)FindWindowIntPtr("Shell_TrayWnd"), WinMessages.WindowsCap.SW_RESTORE);
        }

        /// <summary>
        /// 隐藏任务栏
        /// </summary>
        public static void HideTaskbar()
        {
           WinAPI.ShowWindow((int)FindWindowIntPtr("Shell_TrayWnd"), WinMessages.WindowsCap.SW_HIDE);
        }


        /// <summary>
        /// 设置系统日期时间
        /// </summary>
        /// <param name="dateTime">日期时间数据</param>
        /// <returns>返回是否设置成功</returns>
        public static bool SetSystemDateTime(DateTime dateTime)
        {
            WinAPI.SYSTEMTIME t = new WinAPI.SYSTEMTIME
            {
                Year = (short)dateTime.Year,
                Month = (short)dateTime.Month,
                Day = (short)dateTime.Day,
                //这个函数使用的是0时区的时间,例如，要设12点，则为12-8   
                Hour = (short)(dateTime.Hour - 8),
                Minute = (short)dateTime.Minute,
                Second = (short)dateTime.Second
            };
            return WinAPI.SetSystemTime(ref t);
        }


        /// <summary>
        /// 设置桌面背景图
        /// </summary>
        /// <param name="picturePath">图片路径</param>
        /// <returns></returns>
        public static int SetDesktopPicture(string picturePath)
        {
            if (!File.Exists(picturePath)) throw new FileNotFoundException();
            return WinAPI.SystemParametersInfo(WinMessages.DeviceCap.NUMMARKERS, 0, picturePath, 1);

        }

#if NETFRAMEWORK

        /// <summary>
        /// 熄屏
        /// </summary>
        public static void TurnOffMonitor()
        {
            WinAPI.SendMessage(WinMessages.HWND_BROADCAST, WinMessages.WindowsCap.WM_SYSCOMMAND, (IntPtr)WinMessages.DeviceCap.SC_MONITORPOWER, 1);
        }


        /// <summary>
        /// 亮屏
        /// </summary>
        public static void TurnOnMonitor()
        {
            WinAPI.SendMessage(WinMessages.HWND_BROADCAST, WinMessages.WindowsCap.WM_SYSCOMMAND, (IntPtr)WinMessages.DeviceCap.SC_MONITORPOWER, -1);
        }


        /// <summary>
        /// 设置窗体鼠标穿透效果
        /// </summary>
        /// <param name="form">窗体</param>
        /// <returns>之前的属性,失败返回0</returns>
        public static uint SetFormPenetrate(Form form)
        {
            return WinAPI.SetWindowLong(form.Handle, -20, WinMessages.WindowsCap.WM_SETCURSOR | 0x80000);
        }


#endif


    }
}
