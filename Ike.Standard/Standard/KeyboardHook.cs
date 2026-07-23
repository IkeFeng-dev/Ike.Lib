#if NETFRAMEWORK
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Ike.Standard
{
    /// <summary>
    /// 键盘相关操作
    /// </summary>
    public partial class Keyboard
    {
        /// <summary>
        /// 键盘钩子
        /// </summary>
        public partial class Hook
        {
            /// <summary>
            /// 声明钩子ID变量
            /// </summary>
            private int hookID = 0;
            /// <summary>
            /// 钩子回调函数[LowLevel键盘截获，如果是WH_KEYBOARD＝2，并不能对系统键盘截取，Acrobat Reader会在你截取之前获得键盘]
            /// </summary>
            private WinAPI.HookProc KeyBoardHookProcedure;

            #region 事件委托
            /// <summary>
            /// 键盘钩子:按键事件委托方法
            /// </summary>
            /// <param name="keys">抬起键值</param>
            /// <param name="hookStruct">按键数据结构</param>
            public delegate void DelegateKeyEvents(Keys keys, KeyBoardHookStruct hookStruct);
            /// <summary>
            /// 键盘钩子监听按键事件
            /// </summary>
            public event DelegateKeyEvents KeyEvents;

            /// <summary>
            /// 键盘钩子是否启用
            /// </summary>
            public bool Enabled { get; private set; }
            #endregion

            /// <summary>
            /// 安装键盘钩子
            /// </summary>
            public void InstallHook()
            {
                if (hookID == 0)
                {
                    KeyBoardHookProcedure = new WinAPI.HookProc(KeyBoardHookProc);
                    hookID = WinAPI.SetWindowsHookEx(WinMessages.HookCap.WH_KEYBOARD_LL, KeyBoardHookProcedure, WinAPI.GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
                    if (hookID == 0)
                    {
                        UninstallHook();
                        throw new Exception("安装键盘钩子失败");
                    }
                    else
                    {
                        Enabled = true;
                    }
                }
                else
                {
                    throw new Exception($"ID {hookID} 已安装");
                }
            }

            /// <summary>
            /// 卸载钩子
            /// </summary>
            public bool UninstallHook()
            {
                if (hookID != 0)
                {
                    if (WinAPI.UnhookWindowsHookEx(hookID))
                    {
                        hookID = 0;
                        Enabled = false;
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 钩子事件
            /// </summary>
            /// <param name="nCode"></param>
            /// <param name="wParam"></param>
            /// <param name="lParam"></param>
            /// <returns></returns>
            private int KeyBoardHookProc(int nCode, int wParam, IntPtr lParam)
            {
                if (nCode >= 0)
                {
                    KeyBoardHookStruct kbh = (KeyBoardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyBoardHookStruct));
                    Keys keys = (Keys)Enum.Parse(typeof(Keys), kbh.KeyCode.ToString());
                    KeyEvents?.Invoke(keys, kbh);
                }
                return WinAPI.CallNextHookEx(hookID, nCode, wParam, lParam);
            }
        }


        #region 键盘相关操作

        /// <summary>
        /// 按下
        /// </summary>
        private static readonly int down = 0;
        /// <summary>
        /// 抬起
        /// </summary>
        private static readonly int up = 2;


        /// <summary>
        /// 当前组合键是否按下(Hook可监视系统组合键,窗体按键监控无法获取)
        /// </summary>
        /// <param name="vkCode">按键码[当前被触发的按键]</param>
        /// <param name="combinationKey">组合键</param>
        /// <returns></returns>
        public static bool CombinationKeyIsPress(int vkCode, CombinationKey combinationKey)
        {
            switch (combinationKey)
            {
                case CombinationKey.Alt_F4:
                    if (vkCode == (int)Keys.F4 && (int)Control.ModifierKeys == (int)Keys.Alt)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.Alt_Tab:
                    if (vkCode == (int)Keys.Tab && (int)Control.ModifierKeys == (int)Keys.Alt)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.Alt_Space:
                    if (vkCode == (int)Keys.Space && (int)Control.ModifierKeys == (int)Keys.Alt)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.Ctrl_Esc:
                    if (vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Control)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.Ctrl_A:
                    if (vkCode == (int)Keys.A && (int)Control.ModifierKeys == (int)Keys.Control)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.Ctrl_C:
                    if (vkCode == (int)Keys.C && (int)Control.ModifierKeys == (int)Keys.Control)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.Ctrl_Z:
                    if (vkCode == (int)Keys.Z && (int)Control.ModifierKeys == (int)Keys.Control)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.Ctrl_X:
                    if (vkCode == (int)Keys.X && (int)Control.ModifierKeys == (int)Keys.Control)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.Ctrl_Shift_Esc:
                    if (vkCode == (int)Keys.Escape && (int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Shift)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.Ctrl_Alt_Delete:
                    if (vkCode == (int)Keys.Delete && (int)Control.ModifierKeys == (int)Keys.Control + (int)Keys.Alt)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.LeftWindows:
                    if (vkCode == 91)
                    {
                        return true;
                    }
                    break;
                case CombinationKey.RightWindows:
                    if (vkCode == 92)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// 模拟Control+C
        /// </summary>
        public static void SendCopyKey()
        {
            WinAPI.keybd_event(WinMessages.KeyValue.Ctrl, 0, down, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.C, 0, down, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.Ctrl, 0, up, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.C, 0, up, 0);
        }
        /// <summary>
        /// 模拟Control+V
        /// </summary>
        public static void SendPasteKey()
        {
            WinAPI.keybd_event(WinMessages.KeyValue.Ctrl, 0, down, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.V, 0, down, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.Ctrl, 0, up, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.V, 0, up, 0);
        }

        /// <summary>
        /// 模拟Control+A
        /// </summary>
        public static void SendSelectAllKey()
        {
            WinAPI.keybd_event(WinMessages.KeyValue.Ctrl, 0, down, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.A, 0, down, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.Ctrl, 0, up, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.A, 0, up, 0);
        }

        /// <summary>
        /// 模拟Control+Z
        /// </summary>
        public static void SendRevocationKey()
        {
            WinAPI.keybd_event(WinMessages.KeyValue.Ctrl, 0, down, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.Z, 0, down, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.Ctrl, 0, up, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.Z, 0, up, 0);
        }

        /// <summary>
        /// 模拟回车
        /// </summary>
        public static void SendEnterKey()
        {
            WinAPI.keybd_event(WinMessages.KeyValue.Enter, 0, down, 0);
            WinAPI.keybd_event(WinMessages.KeyValue.Enter, 0, up, 0);
        }


        /// <summary>
        /// 按下指定键(按下并抬起)
        /// </summary>
        /// <param name="key">按键码,可通过<see cref="WinMessages.KeyValue"/>中查询以及传递</param>
        /// <param name="sleep">按下后延时抬起时间(ms)</param>
        public static void SendKey(byte key,int sleep = 0)
        {
            WinAPI.keybd_event(key, 0, down, 0);
            if (sleep > 0)
            {
                Thread.Sleep(sleep);
            }
            WinAPI.keybd_event(key, 0, up, 0);
        }

        /// <summary>
        ///  模拟键盘按键单步(按下或抬起)
        /// </summary>
        /// <param name="key">按键码,可通过<see cref="WinMessages.KeyValue"/>中查询以及传递</param>
        /// <param name="isDown">选项标志：<see langword="true"/> 表示按下，<see langword="false"/>表示松开</param>
        public static void SendKey(byte key, bool isDown)
        {
            int action = isDown ? 0 : 2;
            if (action != 0 && action != 2)
            {
                action = 2;
            }
            WinAPI.keybd_event(key, 0, action, 0);
        }


        /// <inheritdoc cref="WinAPI.keybd_event(byte, byte, int, int)"/>
        public static void SendKeyValue(byte key, byte bScan, int dwFlags, int dwExtraInfo)
        {
            if (dwFlags != 0 && dwFlags != 2)
            {
                dwFlags = 2;
            }
            WinAPI.keybd_event(key, bScan, dwFlags, dwExtraInfo);
        }




        #endregion

    }
}


#endif