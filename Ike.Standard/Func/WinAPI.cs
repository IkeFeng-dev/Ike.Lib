using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Ike.Standard
{
	/// <summary>
	/// Windows API方法,非托管动态链接库方法调用
	/// </summary>
	public class WinAPI
	{

        /// <summary>
        /// <see cref="ExecutionState"/> 枚举标志,表示 <see cref="WinAPI.SetThreadExecutionState"/> 方法的参数,用于指定执行状态
        /// </summary>
        [Flags]
        public enum ExecutionState : uint
        {
            /// <summary>
            /// 通过复位系统空闲定时器,强制系统进入工作状态
            /// </summary>
            SystemRequired = 0x01,
            /// <summary>
            /// 通过重置显示空闲计时器来强制打开显示
            /// </summary>
            DisplayRequired = 0x02,
            /// <summary>
            /// 该值不支持,如果<see cref="UserPresent"/> 与其他esFlags值相结合,则调用将失败,没有指定的状态设置
            /// </summary>
            [Obsolete("该值不支持")]
            UserPresent = 0x04,
            /// <summary>
            /// 启用离开模式,此值必须指定<see cref="Continuous"/>
            /// <para />
            /// 离开模式应该只用于媒体记录和媒体分发应用程序,这些应用程序必须在桌面计算机上执行关键的后台处理,而计算机似乎处于睡眠状态
            /// </summary>
            AwaymodeRequired = 0x40,
            /// <summary>
            ///通知系统正在设置的状态应该保持有效,直到下一次调用使用<see cref="Continuous"/>并且其他状态标志之一被清除
            /// </summary>
            Continuous = 0x80000000,
        }


        /// <summary>
        /// 从 INI 文件中检索指定的键的值
        /// </summary>
        /// <param name="section">要检索的键所在的节名称</param>
        /// <param name="key">要检索的项的名称</param>
        /// <param name="def">如果在文件中找不到指定的键，则返回的默认值</param>
        /// <param name="retVal">用于保存返回的字符串值的缓冲区</param>
        /// <param name="size">缓冲区大小,用于保存返回的字符串</param>
        /// <param name="filePath">INI 文件的完整路径</param>
        /// <remarks>
        ///   <list type="bullet">
        ///     <item>如果找不到指定的键,则返回默认值<paramref name="def"/></item>
        ///     <item>如果找到指定的键,但其值为空字符串,则返回空字符串</item>
        ///     <item>如果 INI 文件或指定的节和键不存在,或者发生其他错误,函数将返回空字符串</item>
        ///   </list>
        /// </remarks>
        /// <returns>从 INI 文件中检索到的字符串的字节长度</returns>
        [DllImport("kernel32")]
		public static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);


		/// <summary>
		/// 向 INI 文件中写入指定的键和值,如果文件不存在,会创建文件;如果键已经存在,会更新键的值
		/// </summary>
		/// <param name="section">要写入的键所在的节名称</param>
		/// <param name="key">要写入的项的名称</param>
		/// <param name="val">要写入的项的新字符串</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>如果指定的 INI 文件不存在,此函数会创建文件</item>
		/// <item>如果指定的键已经存在,此函数会更新键的值</item>
		/// <item>如果 INI 文件中指定的节和键不存在,此函数会创建它们</item>
		/// </list>
		/// </remarks>
		/// <returns>如果函数成功,则返回 <seealso langword="true"/>;否则,返回 <seealso langword="false"/></returns>
		[DllImport("kernel32")]
		public static extern bool WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);


		/// <summary>
		/// 根据指定的窗口类名或窗口标题查找顶层窗口的句柄
		/// </summary>
		/// <param name="className">窗口类名（可以为 <see langword="null" />，表示忽略类名）</param>
		/// <param name="captionName">窗口标题名（可以为 <see langword="null" />，表示忽略窗口标题）</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>如果找到了符合条件的窗口，此函数返回窗口的句柄</item>
		/// <item>如果没有找到符合条件的窗口，返回 <see cref="IntPtr.Zero"/></item>
		/// <item>此函数只能查找顶层窗口，不能查找子窗口</item>
		/// </list>
		/// </remarks>
		/// <returns>返回找到的窗口句柄。如果找不到窗口，返回 <see cref="IntPtr.Zero"/></returns>
		[DllImport("User32.dll")]
		public static extern IntPtr FindWindow(string className, string captionName);


		/// <summary>
		/// 改变指定窗口的大小、位置和 Z 顺序
		/// </summary>
		/// <param name="hWnd">窗口句柄，表示要操作的窗口</param>
		/// <param name="hWndInsertAfter">决定窗口 Z 顺序的句柄</param>
		/// <param name="X">窗口新位置的左边缘坐标（相对于屏幕左上角）</param>
		/// <param name="Y">窗口新位置的顶边缘坐标（相对于屏幕左上角）</param>
		/// <param name="cx">窗口的新宽度，以像素为单位</param>
		/// <param name="cy">窗口的新高度，以像素为单位</param>
		/// <param name="uFlags">指定窗口大小和位置的标志位（组合了多种选项）</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>可以使用此函数改变窗口的大小、位置以及它在 Z 顺序中的位置</item>
		/// <item>如果窗口的大小或位置没有改变，该函数会返回 <seealso langword="true"/>，即使指定的参数没有改变</item>
		/// <item>uFlags 可以指定如何操作窗口，常用的标志包括 <see cref="CONST.SetWindowPosFlags.SWP_NOSIZE"/>（保持窗口大小不变）和 <see cref="CONST.SetWindowPosFlags.SWP_NOMOVE"/>（保持窗口位置不变）</item>
		/// </list>
		/// </remarks>
		/// <returns>如果函数成功，则返回 <seealso langword="true"/>；否则，返回 <seealso langword="false"/></returns>
		[DllImport("user32.dll ")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


		/// <summary>
		/// 改变指定窗口的显示状态（例如最小化、最大化、隐藏、显示）
		/// </summary>
		/// <param name="hwnd">窗口句柄，表示要操作的窗口</param>
		/// <param name="nCmdShow">指定窗口显示状态的参数</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>nCmdShow 的常用值包括 <see cref="CONST.WindowShowCommand.SW_SHOWNORMAL"/>、<see cref="CONST.WindowShowCommand.SW_MINIMIZE"/>、<see cref="CONST.WindowShowCommand.SW_MAXIMIZE"/> 等</item>
		/// <item>此函数不能改变子窗口的显示状态，只能操作顶层窗口或弹出窗口</item>
		/// <item>如果窗口当前已处于指定的显示状态，<see cref="ShowWindow"></see> 不会做任何操作</item>
		/// </list>
		/// </remarks>
		/// <returns>如果窗口之前是可见的且现在被隐藏，返回非零值；如果窗口之前是隐藏的且现在被显示，返回<see langword="0"/></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);


		/// <summary>
		/// 设置当前线程的执行状态，从而影响系统的电源管理行为。通过设置不同的执行状态标志，可以防止系统进入睡眠、休眠或关闭显示器
		/// </summary>
		/// <param name="esFlags">指定执行状态的枚举标志，可以是多个 <see cref="ExecutionState"/> 标志的组合</param>
		/// <returns>返回上一个线程的 <see cref="ExecutionState"/> 状态，如果失败则返回 <see langword="0"></see></returns>
		/// <remarks>
		/// <list type="bullet">
		/// <item>此函数可以用来阻止系统进入休眠或关闭显示器，常用于长时间运行的任务或需要保持显示器开启的操作</item>
		/// <item>传递 <see cref="ExecutionState.Continuous"/> 标志时，状态将持续生效，直到下一次调用清除状态</item>
		/// <item>如果不传递 <see cref="ExecutionState.Continuous"/>，设置的状态仅会在该调用结束时生效，之后系统将恢复默认行为</item>
		/// <item>可以通过再次调用 <see cref="SetThreadExecutionState"/> 并只传递 <see cref="ExecutionState.Continuous"/> 来清除之前设置的状态</item>
		/// <item>示例:阻止休眠-> SetThreadExecutionState(<see cref="ExecutionState.Continuous"/> | <see cref="ExecutionState.SystemRequired"/> | <see cref="ExecutionState.DisplayRequired"/>);</item>
		/// <item>示例:恢复休眠-> SetThreadExecutionState(<see cref="ExecutionState.Continuous"/>);</item>
		/// </list>
		/// </remarks>
		[DllImport("kernel32")]
		public static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);


		/// <summary>
		/// 通过窗体句柄获取进程ID
		/// </summary>
		/// <param name="hwnd">要检索线程和进程 ID 的窗口句柄,该句柄可以通过 <see cref="FindWindow(string, string)"/>,<see cref="GetForegroundWindow()"/>等等函数获得</param>
		/// <param name="ID">当此函数返回时，包含拥有该窗口的进程的标识符</param>
		/// <returns>创建窗口的线程的标识符</returns>
		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);


		/// <summary>
		/// 获取当前焦点窗体句柄
		/// </summary>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern IntPtr GetForegroundWindow();


		/// <summary>
		/// 获取窗体大小矩形
		/// </summary>
		/// <param name="hWnd">窗体句柄</param>
		/// <param name="lpRect">返回矩形数据</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hWnd, out Structure.RECT lpRect);



		/// <summary>
		/// 设置本地系统时间
		/// </summary>
		/// <param name="sysTime">包含要设置的本地时间信息的 <see cref="Structure.SystemTime"/> 结构体</param>
		/// <remarks>
		/// 该方法通过调用 Win32 API `SetLocalTime` 来设置系统的本地时间。调用此方法需要管理员权限
		/// </remarks>
		/// <returns>
		/// 如果函数执行成功，则返回 <seealso langword="true"/>；否则，返回 <seealso langword="false"/>
		/// </returns>
		[DllImport("Kernel32.dll")]
		public static extern bool SetLocalTime(ref Structure.SystemTime sysTime);



		/// <summary>
		/// 向调试器输出调试信息
		/// </summary>
		/// <param name="message">要输出的调试信息字符串</param>
		/// <remarks>
		/// 该方法通过调用 Win32 API `OutputDebugString` 将指定的调试信息发送到调试器
		/// 调试信息可以通过调试工具（如 DebugView）捕获和查看
		/// </remarks>
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern void OutputDebugString(string message);


		/// <summary>
		/// 为指定窗口创建一个关机阻止原因，以防止系统关闭或重启
		/// </summary>
		/// <param name="hWnd">要设置关机阻止原因的窗口句柄</param>
		/// <param name="reason">描述阻止关机的原因的字符串</param>
		/// <returns>如果函数成功，返回值为非零；如果函数失败，返回值为零</returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string reason);


		/// <summary>
		/// 销毁与指定窗口关联的关机阻止原因
		/// </summary>
		/// <param name="hWnd">要销毁关机阻止原因的窗口句柄</param>
		/// <returns>如果函数成功，返回值为非零；如果函数失败，返回值为零</returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);


		/// <summary>
		/// 通过窗口句柄获取其类名
		/// </summary>
		/// <param name="hWnd">目标窗口的句柄</param>
		/// <param name="lpClassName">用于接收类名的缓冲区</param>
		/// <param name="nMaxCount">缓冲区的最大字符容量</param>
		/// <returns>
		/// 成功时返回复制到缓冲区的字符数（不包括终止<see langword="null"/> 字符），失败时返回<see langword="0"/> ,可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API GetClassName获取指定窗口的类名
		/// 窗口类名是窗口在注册时指定的标识符，通常用于区分不同类型的窗口
		/// </remarks>
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


		/// <summary>
		/// 获取指定窗口的标题栏文本
		/// </summary>
		/// <param name="hWnd">目标窗口的句柄</param>
		/// <param name="lpString">用于接收标题文本的缓冲区</param>
		/// <param name="nMaxCount">缓冲区的最大字符容量</param>
		/// <returns>
		/// 成功时返回复制到缓冲区的字符数（不包括终止<see langword="null"/> 字符），失败时返回<see langword="0"/> ,可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API GetWindowText获取窗口标题
		/// 如果目标窗口没有标题栏、标题为空或句柄无效，将返回空字符串
		/// </remarks>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);


		/// <summary>
		/// 定义用于<see cref="EnumWindows"/>回调的委托类型
		/// </summary>
		/// <param name="hWnd">当前枚举到的窗口句柄</param>
		/// <param name="lParam">应用程序定义的值，来自EnumWindows调用</param>
		/// <returns>
		/// 返回true继续枚举，返回false停止枚举
		/// </returns>
		/// <remarks>
		/// 该委托用于处理<see cref="EnumWindows"/>函数的回调
		/// 每个被枚举的窗口都会调用一次该委托。
		/// </remarks>
		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


		/// <summary>
		/// 枚举屏幕上的所有顶层窗口
		/// </summary>
		/// <param name="lpEnumFunc">指向回调函数的委托</param>
		/// <param name="lParam">应用程序定义的要传递给回调函数的值</param>
		/// <returns>
		/// 如果函数成功，返回非零值；如果函数失败，返回零,可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API EnumWindows枚举所有顶层窗口
		/// 枚举顺序为Z序（即窗口叠放顺序），从最顶层窗口到最底层窗口
		/// 回调函数返回true将继续枚举，返回false将停止枚举
		/// </remarks>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);


		/// <summary>
		/// 获取创建指定窗口的线程标识符和进程标识符
		/// </summary>
		/// <param name="hWnd">目标窗口的句柄</param>
		/// <param name="lpdwProcessId">输出参数，接收进程标识符</param>
		/// <returns>
		/// 返回创建窗口的线程标识符。如果窗口句柄无效，返回<see langword="0"/>
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API GetWindowThreadProcessId获取窗口的线程和进程信息
		/// 即使目标进程处于调试状态或被调试器附加，仍能正确获取标识符
		/// </remarks>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


		/// <summary>
		/// 创建或打开一个命名的或未命名的文件映射对象
		/// </summary>
		/// <param name="hFile">要创建映射的文件的句柄，可创建基于内存的映射</param>
		/// <param name="lpFileMappingAttributes">指向SECURITY_ATTRIBUTES结构的指针，决定返回的句柄能否被子进程继承</param>
		/// <param name="flProtect">指定文件映射的保护类型</param>
		/// <param name="dwMaximumSizeHigh">文件映射对象的最大大小的高32位</param>
		/// <param name="dwMaximumSizeLow">文件映射对象的最大大小的低32位</param>
		/// <param name="lpName">文件映射对象的名称，为空则创建匿名映射</param>
		/// <returns>
		/// 如果成功，返回文件映射对象的句柄；
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API CreateFileMapping创建文件映射对象
		/// 文件映射对象的大小不能超过磁盘上的实际文件大小
		/// </remarks>
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern SafeFileHandle CreateFileMapping(SafeFileHandle hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

		/// <summary>
		/// 将文件映射对象的视图映射到当前进程的地址空间
		/// </summary>
		/// <param name="hFileMappingObject">文件映射对象的句柄</param>
		/// <param name="dwDesiredAccess">指定对文件视图的访问类型</param>
		/// <param name="dwFileOffsetHigh">视图起始偏移的高32位</param>
		/// <param name="dwFileOffsetLow">视图起始偏移的低32位</param>
		/// <param name="dwNumberOfBytesToMap">要映射的字节数，为0则映射整个文件</param>
		/// <returns>
		/// 如果成功，返回映射视图的起始地址；如果失败，返回<see cref="IntPtr.Zero"/>，
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API MapViewOfFile映射文件视图
		/// 映射的视图必须与系统的内存分配粒度对齐(通常为64KB)
		/// 多个进程可以通过映射同一文件映射对象来共享内存
		/// </remarks>
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr MapViewOfFile(SafeFileHandle hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

		/// <summary>
		/// 取消映射当前进程地址空间中的文件视图
		/// </summary>
		/// <param name="lpBaseAddress">要取消映射的视图的基地址</param>
		/// <returns>
		/// 如果成功，返回非零值；如果失败，返回零
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API UnmapViewOfFile取消文件视图映射
		/// 取消映射后，所有指向该视图的指针都将无效
		/// 系统会延迟写入磁盘直到所有视图都被取消映射
		/// </remarks>
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

		/// <summary>
		/// 获取标准输入、输出或错误设备的句柄
		/// </summary>
		/// <param name="nStdHandle">标准设备类型</param>
		/// <returns>
		/// 如果成功，返回请求的设备句柄；如果失败，返回<see cref="IntPtr.Zero"/>，
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API GetStdHandle获取标准设备句柄
		/// 标准设备包括标准输入(STD_INPUT_HANDLE)、标准输出(STD_OUTPUT_HANDLE)和标准错误(STD_ERROR_HANDLE)
		/// 在GUI应用程序中调用可能返回无效句柄
		/// </remarks>
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetStdHandle(int nStdHandle);

		/// <summary>
		/// 获取指定控制台输入或屏幕缓冲区的当前输入模式
		/// </summary>
		/// <param name="hConsoleHandle">控制台输入或屏幕缓冲区的句柄</param>
		/// <param name="lpMode">输出参数，接收当前控制台模式</param>
		/// <returns>
		/// 如果成功，返回非零值；如果失败，返回零
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API GetConsoleMode获取控制台模式
		/// 控制台模式决定了控制台如何处理输入和输出，如启用虚拟终端处理等
		/// 输入和输出缓冲区需要分别获取其模式
		/// </remarks>
		[DllImport("kernel32.dll")]
		public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

		/// <summary>
		/// 设置指定控制台输入或屏幕缓冲区的输入模式
		/// </summary>
		/// <param name="hConsoleHandle">控制台输入或屏幕缓冲区的句柄</param>
		/// <param name="dwMode">要设置的控制台模式标志</param>
		/// <returns>
		/// 如果成功，返回非零值；如果失败，返回零
		/// </returns>
		/// <remarks>
		/// 该方法通过调用Win32 API SetConsoleMode设置控制台模式
		/// 常见的模式包括ENABLE_VIRTUAL_TERMINAL_PROCESSING(0x0004)等
		/// 修改输入模式会影响ReadConsole等函数的行为
		/// </remarks>
		[DllImport("kernel32.dll")]
		public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

	}
}
