namespace Ike.Standard
{
	/// <summary>
	/// 常量类
	/// </summary>
	public class CONST
	{
		/// <summary>
		/// 包含用于控制窗口显示状态的常量值，适用于 <see cref="WinAPI.ShowWindow"/> 函数的 nCmdShow 参数
		/// </summary>
		public static class WindowShowCommand
		{
			/// <summary>
			/// 正常显示窗口。如果窗口被最小化或最大化，窗口会恢复到原始大小和位置
			/// </summary>
			public const int SW_SHOWNORMAL = 1;

			/// <summary>
			/// 隐藏窗口，并激活其他窗口
			/// </summary>
			public const int SW_HIDE = 0;

			/// <summary>
			/// 激活并显示窗口。如果窗口被最小化或最大化，窗口会恢复到原始大小和位置
			/// </summary>
			public const int SW_RESTORE = 9;

			/// <summary>
			/// 激活并最小化窗口
			/// </summary>
			public const int SW_SHOWMINIMIZED = 2;

			/// <summary>
			/// 激活并最大化窗口
			/// </summary>
			public const int SW_MAXIMIZE = 3;

			/// <summary>
			/// 显示窗口，但不改变窗口的激活状态。如果窗口被最小化或最大化，窗口会恢复到原始大小和位置
			/// </summary>
			public const int SW_SHOWNOACTIVATE = 4;

			/// <summary>
			/// 显示窗口，并保持窗口处于最小化状态
			/// </summary>
			public const int SW_MINIMIZE = 6;

			/// <summary>
			/// 显示窗口，并将窗口最小化，但不改变窗口的激活状态
			/// </summary>
			public const int SW_SHOWMINNOACTIVE = 7;

			/// <summary>
			/// 显示窗口的当前大小和位置，并保持窗口处于非激活状态
			/// </summary>
			public const int SW_SHOWNA = 8;

			/// <summary>
			/// 显示窗口，并将其设置为最顶层窗口
			/// </summary>
			public const int SW_SHOWDEFAULT = 10;
		}

		/// <summary>
		/// 包含用于 <see cref="WinAPI.SetWindowPos"/> 函数的常量值，表示窗口位置、大小、Z 顺序的操作标志
		/// </summary>
		public static class SetWindowPosFlags
		{
			/// <summary>
			/// 不调整窗口大小
			/// </summary>
			public const uint SWP_NOSIZE = 0x0001;

			/// <summary>
			/// 不调整窗口位置
			/// </summary>
			public const uint SWP_NOMOVE = 0x0002;

			/// <summary>
			/// 维持窗口的 Z 顺序（忽略 hWndInsertAfter 参数）
			/// </summary>
			public const uint SWP_NOZORDER = 0x0004;

			/// <summary>
			/// 如果窗口被隐藏，保持窗口的隐藏状态
			/// </summary>
			public const uint SWP_NOREDRAW = 0x0008;

			/// <summary>
			/// 不激活窗口。如果未设置此标志，则窗口会被激活
			/// </summary>
			public const uint SWP_NOACTIVATE = 0x0010;

			/// <summary>
			/// 如果窗口已最小化，则保持最小化状态
			/// </summary>
			public const uint SWP_FRAMECHANGED = 0x0020; // 发送 WM_NCCALCSIZE 到窗口，即使窗口大小未更改。必须重绘窗口

			/// <summary>
			/// 隐藏窗口
			/// </summary>
			public const uint SWP_HIDEWINDOW = 0x0080;

			/// <summary>
			/// 显示窗口
			/// </summary>
			public const uint SWP_SHOWWINDOW = 0x0040;

			/// <summary>
			/// 禁用窗口的客户区部分更新
			/// </summary>
			public const uint SWP_NOOWNERZORDER = 0x0200; // 不改变所有者窗口的 Z 顺序

			/// <summary>
			/// 阻止生成 WM_SYNCPAINT 消息
			/// </summary>
			public const uint SWP_NOSENDCHANGING = 0x0400;

			/// <summary>
			/// 画出窗口的边框（如果存在）
			/// </summary>
			public const uint SWP_DRAWFRAME = SWP_FRAMECHANGED;

			/// <summary>
			/// 忽略所有其他标志，并将窗口置于顶层
			/// </summary>
			public const uint HWND_TOP = 0x0000;

			/// <summary>
			/// 将窗口置于非顶层，即使窗口已设置为最顶层窗口
			/// </summary>
			public const uint HWND_NOTOPMOST = 0xFFFF;

			/// <summary>
			/// 将窗口置于顶层窗口之上
			/// </summary>
			public const uint HWND_TOPMOST = 0xFFFF - 1;
		}


	}
}
