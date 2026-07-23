using System;
using System.Runtime.InteropServices;

namespace Ike.Standard
{
    /// <summary>
    /// 鼠标钩子[注:首先安装钩子,然后调用事件时需要为MouseEvents事件赋值,委托赋值后才能执行对应事件]
    /// </summary>
    public partial class MouseHook
    {
        /// <summary>
        /// 全局的鼠标事件
        /// </summary>
        /// <param name="mouseKey"> 代表发生的鼠标的事件 </param>
        /// <param name="mouseMsg">钩子的结构体，存储着鼠标的位置及其他信息</param>
        public delegate void MouseEventHandler(MouseKey mouseKey, MSLLHOOKSTRUCT mouseMsg);
        /// <summary>
        /// 鼠标监听事件
        /// </summary>
        public event MouseEventHandler MouseEvents;
        /// <summary>
        /// 声明鼠标钩子事件类型
        /// </summary>
        private WinAPI.HookProc MouseHookProcedure;
        /// <summary>
        /// 鼠标钩子句柄
        /// </summary>
        private int mouseHook = 0;
        /// <summary>
        /// 鼠标钩子是否启用
        /// </summary>
        public bool Enabled { get; private set; }
        /// <summary>
        /// 是否拦截
        /// </summary>
        public bool IsIntercept { get; set; } = false;


        /// <summary>
        /// 安装钩子
        /// </summary>
        public void InstallHook()
        {
            // 安装鼠标钩子
            if (mouseHook == 0)
            {
                // 生成一个HookProc的实例.
                MouseHookProcedure = new WinAPI.HookProc(MouseHookProc);
                mouseHook = WinAPI.SetWindowsHookEx(WinMessages.HookCap.WH_MOUSE_LL, MouseHookProcedure, WinAPI.GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName), 0);
                //钩子是否安装成功
                if (mouseHook == 0)
                {
                    UninstallHook();
                    throw new Exception("鼠标钩子安装失败");
                }
                else
                {
                    Enabled = true;
                }
            }
            else
            {
                throw new Exception($"ID {mouseHook} 已安装");
            }
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public bool UninstallHook()
        {
            if (WinAPI.UnhookWindowsHookEx(mouseHook))
            {
                mouseHook = 0;
                Enabled = false;
                return true;
            }
            return false;
        }


        /// <summary>
        /// 获取鼠标操作值
        /// </summary>
        /// <param name="param">键值</param>
        /// <returns></returns>
        private MouseKey GetMouseKey(int param)
        {
            MouseKey mouseKey = MouseKey.Move;
            uint par = (uint)param;
            switch (par)
            {
                case WinMessages.HookCap.WM_LBUTTONDOWN:
                    mouseKey = MouseKey.LeftDown;
                    break;
                case WinMessages.HookCap.WM_LBUTTONUP:
                    mouseKey = MouseKey.LeftUp;
                    break;
                case WinMessages.HookCap.WM_MBUTTONDOWN:
                    mouseKey = MouseKey.MiddleDown;
                    break;
                case WinMessages.HookCap.WM_MBUTTONUP:
                    mouseKey = MouseKey.MiddleUp;
                    break;
                case WinMessages.HookCap.WM_RBUTTONDOWN:
                    mouseKey = MouseKey.RightDown;
                    break;
                case WinMessages.HookCap.WM_RBUTTONUP:
                    mouseKey = MouseKey.RightUp;
                    break;
                case WinMessages.HookCap.WM_MOUSEROLL:
                    mouseKey = MouseKey.Roll;
                    break;
            }
            return mouseKey;
        }


        /// <summary>
        /// 鼠标钩子回调函数
        /// </summary>
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 假设正常执行而且用户要监听鼠标的消息
            if ((nCode >= 0) && (MouseEvents != null))
            {
                MSLLHOOKSTRUCT MyMouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseEvents(GetMouseKey(wParam), MyMouseHookStruct);
                //判断是否拦截
                if (IsIntercept)
                {
                    return 1;
                }
            }
            // 启动下一次钩子
            return WinAPI.CallNextHookEx(mouseHook, nCode, wParam, lParam);

        }
    }
}
