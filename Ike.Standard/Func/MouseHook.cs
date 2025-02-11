//using System;
//using System.Diagnostics;
//using System.Runtime.InteropServices;

//namespace Ike.Standard
//{
//	/// <summary>
//	/// 鼠标钩子
//	/// </summary>
//	public class MouseHook
//	{

//		#region 成员变量、回调函数、事件
//		/// <summary>
//		/// 钩子回调函数
//		/// </summary>
//		/// <param name="nCode">如果代码小于零，则挂钩过程必须将消息传递给CallNextHookEx函数，而无需进一步处理，并且应返回CallNextHookEx返回的值。此参数可以是下列值之一。(来自官网手册)</param>
//		/// <param name="wParam">记录了按下的按钮</param>
//		/// <param name="lParam"></param>
//		/// <returns></returns>
//		private delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
//		/// <summary>
//		/// 全局的鼠标事件
//		/// </summary>
//		/// <param name="mouseKey"> 代表发生的鼠标的事件 </param>
//		/// <param name="mouseMsg">钩子的结构体，存储着鼠标的位置及其他信息</param>
//		public delegate void MouseEventHandler(MouseKey mouseKey, MouseHookStruct mouseMsg);
//		/// <summary>
//		/// 鼠标委托事件
//		/// </summary>
//		public static event MouseEventHandler MouseEvents;
//		/// <summary>
//		/// 声明鼠标钩子事件类型
//		/// </summary>
//		private static HookProc MouseHookProcedure;
//		/// <summary>
//		/// 鼠标钩子句柄
//		/// </summary>
//		private static int mouseHook = 0;
//		/// <summary>
//		/// 鼠标钩子是否启用
//		/// </summary>
//		public static bool Enabled { get; private set; }
//		/// <summary>
//		/// 是否拦截
//		/// </summary>
//		public static bool IsIntercept { get; set; } = false;
//		#endregion

//		#region Win32的API
//		/// <summary>
//		/// 钩子结构体
//		/// </summary>
//		[StructLayout(LayoutKind.Sequential)]
//		public class MouseHookStruct
//		{
//			/// <summary>
//			/// 鼠标位置
//			/// </summary>
//			public POINT Point;
//			/// <summary>
//			/// .
//			/// </summary>
//			public int Wnd;
//			/// <summary>
//			/// .
//			/// </summary>
//			public int HitTestCode;
//			/// <summary>
//			/// .
//			/// </summary>
//			public int DwExtraInfo;
//		}


//		/// <summary>
//		/// Point坐标数据
//		/// </summary>
//		[StructLayout(LayoutKind.Sequential)]
//		public class POINT
//		{
//			/// <summary>
//			/// X坐标
//			/// </summary>
//			public int X;
//			/// <summary>
//			/// Y坐标
//			/// </summary>
//			public int Y;
//		}

//		/// <summary>
//		/// 鼠标动作键
//		/// </summary>
//		public enum MouseKey
//		{
//			/// <summary>
//			/// 移动
//			/// </summary>
//			Move,
//			/// <summary>
//			/// 左抬起
//			/// </summary>
//			LeftUp,
//			/// <summary>
//			/// 左按下
//			/// </summary>
//			LeftDown,
//			/// <summary>
//			/// 右抬起
//			/// </summary>
//			RightUp,
//			/// <summary>
//			/// 右按下
//			/// </summary>
//			RightDown,
//			/// <summary>
//			/// 中抬起
//			/// </summary>
//			MiddleUp,
//			/// <summary>
//			/// 中按下
//			/// </summary>
//			MiddleDown,
//			/// <summary>
//			/// 滚轮滚动
//			/// </summary>
//			Roll
//		}


//		/// <summary>
//		/// 鼠标钩子常量值
//		/// </summary>
//		public class MouseHookValue
//		{
//			/// <summary>
//			/// 鼠标移动(512)
//			/// </summary>
//			public const int WM_MOUSEMOVE = 0x200;
//			/// <summary>
//			/// 鼠标左键按下(513)
//			/// </summary>
//			public const int WM_LBUTTONDOWN = 0x201;
//			/// <summary>
//			/// 鼠标右键按下(516)
//			/// </summary>
//			public const int WM_RBUTTONDOWN = 0x204;
//			/// <summary>
//			/// 鼠标中键按下(519)
//			/// </summary>
//			public const int WM_MBUTTONDOWN = 0x207;
//			/// <summary>
//			/// 鼠标左键抬起(514)
//			/// </summary>
//			public const int WM_LBUTTONUP = 0x202;
//			/// <summary>
//			/// 鼠标右键抬起(517)
//			/// </summary>
//			public const int WM_RBUTTONUP = 0x205;
//			/// <summary>
//			/// 鼠标中键抬起(520)
//			/// </summary>
//			public const int WM_MBUTTONUP = 0x208;
//			/// <summary>
//			/// 鼠标左键双击(515)
//			/// </summary>
//			public const int WM_LBUTTONDBLCLK = 0x203;
//			/// <summary>
//			/// 鼠标右键双击(518)
//			/// </summary>
//			public const int WM_RBUTTONDBLCLK = 0x206;
//			/// <summary>
//			/// 鼠标中键双击(521)
//			/// </summary>
//			public const int WM_MBUTTONDBLCLK = 0x209;
//			/// <summary>
//			/// 鼠标滚轮滚动
//			/// </summary>
//			public const int WM_MOUSEROLL = 0x20A;
//			/// <summary>
//			/// 可以截获整个系统所有模块的鼠标事件
//			/// </summary>
//			public const int WH_MOUSE_LL = 14;
//		}

//		/// <summary>
//		/// 装置钩子的函数
//		/// </summary>
//		/// <param name="idHook"></param>
//		/// <param name="lpfn"></param>
//		/// <param name="hInstance"></param>
//		/// <param name="threadId"></param>
//		/// <returns></returns>
//		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
//		private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

//		/// <summary>
//		/// 卸下钩子的函数
//		/// </summary>
//		/// <param name="idHook"></param>
//		/// <returns></returns>
//		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
//		private static extern bool UnhookWindowsHookEx(int idHook);

//		/// <summary>
//		/// 下一个钩挂的函数
//		/// </summary>
//		/// <param name="idHook"></param>
//		/// <param name="nCode"></param>
//		/// <param name="wParam"></param>
//		/// <param name="lParam"></param>
//		/// <returns></returns>
//		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
//		private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
//		/// <summary>
//		/// 获取模块句柄
//		/// </summary>
//		/// <param name="name">进程名称(pro.exe)</param>
//		/// <returns></returns>
//		[DllImport("kernel32.dll")]
//		private static extern IntPtr GetModuleHandle(string name);
//		// 此方法貌似也可以获取-> Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
//		#endregion

//		#region 调用方法
//		/// <summary>
//		/// 安装钩子
//		/// </summary>
//		public static void InstallHook()
//		{
//			// 安装鼠标钩子
//			if (mouseHook == 0)
//			{
//				// 生成一个HookProc的实例.
//				MouseHookProcedure = new HookProc(MouseHookProc);
//				mouseHook = SetWindowsHookEx(14, MouseHookProcedure, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
//				//钩子是否安装成功
//				if (mouseHook == 0)
//				{
//					UninstallHook();
//					throw new Exception("鼠标钩子安装失败");
//				}
//				else
//				{
//					Enabled = true;
//				}
//			}
//		}

//		/// <summary>
//		/// 卸载钩子
//		/// </summary>
//		public static void UninstallHook()
//		{
//			UnhookWindowsHookEx(mouseHook);
//			mouseHook = 0;
//			Enabled = false;
//		}

//		/// <summary>
//		/// 获取鼠标操作值
//		/// </summary>
//		/// <param name="param">键值</param>
//		/// <returns></returns>
//		private static MouseKey GetMouseKey(int param)
//		{
//			MouseKey mouseKey = MouseKey.Move;
//			switch (param)
//			{
//				case MouseHookValue.WM_LBUTTONDOWN:
//					mouseKey = MouseKey.LeftDown;
//					break;
//				case MouseHookValue.WM_LBUTTONUP:
//					mouseKey = MouseKey.LeftUp;
//					break;
//				case MouseHookValue.WM_MBUTTONDOWN:
//					mouseKey = MouseKey.MiddleDown;
//					break;
//				case MouseHookValue.WM_MBUTTONUP:
//					mouseKey = MouseKey.MiddleUp;
//					break;
//				case MouseHookValue.WM_RBUTTONDOWN:
//					mouseKey = MouseKey.RightDown;
//					break;
//				case MouseHookValue.WM_RBUTTONUP:
//					mouseKey = MouseKey.RightUp;
//					break;
//				case MouseHookValue.WM_MOUSEROLL:
//					mouseKey = MouseKey.Roll;
//					break;
//			}
//			return mouseKey;
//		}
//		/// <summary>
//		/// 鼠标钩子回调函数
//		/// </summary>
//		private static int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
//		{
//			// 假设正常执行而且用户要监听鼠标的消息
//			if ((nCode >= 0) && (MouseEvents != null))
//			{
//				MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
//				MouseEvents(GetMouseKey(wParam), MyMouseHookStruct);
//				//判断是否拦截
//				if (IsIntercept)
//				{
//					return 1;
//				}
//			}
//			// 启动下一次钩子
//			return CallNextHookEx(mouseHook, nCode, wParam, lParam);
//		}
//		#endregion
//	}
//}
