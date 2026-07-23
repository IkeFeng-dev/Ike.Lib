using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using System.Text;


namespace Ike.Standard
{
	/// <summary>
	/// Windows API方法,非托管动态链接库方法调用
	/// </summary>
	public partial class WinAPI
	{
        /// <summary>
        /// 钩子回调函数
        /// </summary>
        /// <param name="nCode">如果代码小于零，则挂钩过程必须将消息传递给CallNextHookEx函数，而无需进一步处理，并且应返回CallNextHookEx返回的值。此参数可以是下列值之一。(来自官网手册)</param>
        /// <param name="wParam">记录了按下的按钮</param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate int HookProc(int nCode, int wParam, IntPtr lParam);


        /// <summary>
        /// 定义用于<see cref="EnumWindows"/>回调的委托类型
        /// </summary>
        /// <param name="hWnd">当前枚举到的窗口句柄</param>
        /// <param name="lParam">应用程序定义的值，来自EnumWindows调用</param>
        /// <returns>
        /// 返回<see langword="true"/> 继续枚举，返回<see langword="false"/> 停止枚举
        /// </returns>
        /// <remarks>
        /// 该委托用于处理<see cref="EnumWindows"/>函数的回调
        /// 每个被枚举的窗口都会调用一次该委托
        /// </remarks>
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

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


		#region 窗口操作相关

		/// <summary>
		///  <see langword="[√]"/> 根据指定的窗口类名或窗口标题查找顶层窗口的句柄
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
        /// <item>uFlags 可以指定如何操作窗口，常用的标志包括 <see cref="WinMessages.WindowsCap.SWP_NOSIZE"/>（保持窗口大小不变）和 <see cref="WinMessages.WindowsCap.SWP_NOMOVE"/>（保持窗口位置不变）</item>
        /// </list>
        /// </remarks>
        /// <returns>如果函数成功，则返回 <seealso langword="true"/>；否则，返回 <seealso langword="false"/></returns>
        [DllImport("user32.dll ")]
		public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


        /// <summary>
        ///  <see langword="[√]"/> 改变指定窗口的显示状态（例如最小化、最大化、隐藏、显示）
        /// </summary>
        /// <param name="hwnd">窗口句柄，表示要操作的窗口</param>
        /// <param name="nCmdShow">指定窗口显示状态的参数,常用值包括 <see cref="WinMessages.WindowsCap.SW_SHOWNORMAL"/>、<see cref="WinMessages.WindowsCap.SW_MINIMIZE"/>、<see cref="WinMessages.WindowsCap.SW_MAXIMIZE"/> 等</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>此函数不能改变子窗口的显示状态，只能操作顶层窗口或弹出窗口</item>
        /// <item>如果窗口当前已处于指定的显示状态，<see cref="ShowWindow(IntPtr, int)"></see> 不会做任何操作</item>
        /// </list>
        /// </remarks>
        /// <returns>如果窗口之前是可见的且现在被隐藏，返回非零值；如果窗口之前是隐藏的且现在被显示，返回<see langword="0"/></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);


		/// <summary>
		///  <see langword="[√]"/> 改变指定窗口的位置和大小
		/// </summary>
		/// <param name="hWnd">窗口句柄，表示要操作的窗口</param>
		/// <param name="X">窗口新的左上角X坐标（屏幕坐标系）</param>
		/// <param name="Y">窗口新的左上角Y坐标（屏幕坐标系）</param>
		/// <param name="nWidth">窗口的新宽度（像素）</param>
		/// <param name="nHeight">窗口的新高度（像素）</param>
		/// <param name="bRepaint">是否立即重绘窗口（通常设为true）</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>此函数可以用于顶层窗口和子窗口</item>
		/// <item>坐标和尺寸参数使用屏幕坐标系（对于顶层窗口）或父窗口客户区坐标系（对于子窗口）</item>
		/// <item>如果窗口有菜单，新宽度和新高度应包括菜单的高度</item>
		/// <item>对于子窗口，X和Y参数是相对于父窗口客户区的坐标</item>
		/// </list>
		/// </remarks>
		/// <returns>如果函数成功，返回非零值；如果失败，返回<see langword="false"/></returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		#endregion



		/// <summary>
		///  <see langword="[√]"/> 设置当前线程的执行状态，从而影响系统的电源管理行为。通过设置不同的执行状态标志，可以防止系统进入睡眠、休眠或关闭显示器
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
		/// <see langword="[√]"/> 获取当前焦点窗体句柄
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
		///  <see langword="[√]"/> 设置本地系统时间
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
		///  <see langword="[√]"/> 向调试器输出调试信息
		/// </summary>
		/// <param name="message">要输出的调试信息字符串</param>
		/// <remarks>
		/// 该方法通过调用 Win32 API `OutputDebugString` 将指定的调试信息发送到调试器
		/// 调试信息可以通过调试工具（如 DebugView）捕获和查看
		/// </remarks>
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		public static extern void OutputDebugString(string message);


		/// <summary>
		/// <see langword="[√]"/> 为指定窗口创建一个关机阻止原因，以防止系统关闭或重启
		/// </summary>
		/// <param name="hWnd">要设置关机阻止原因的窗口句柄</param>
		/// <param name="reason">描述阻止关机的原因的字符串</param>
		/// <returns>如果函数成功，返回值为非零；如果函数失败，返回值为零</returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShutdownBlockReasonCreate(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] string reason);


		/// <summary>
		/// <see langword="[√]"/> 销毁与指定窗口关联的关机阻止原因
		/// </summary>
		/// <param name="hWnd">要销毁关机阻止原因的窗口句柄</param>
		/// <returns>如果函数成功，返回值为非零；如果函数失败，返回值为零</returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);


		/// <summary>
		/// <see langword="[√]"/> 通过窗口句柄获取其类名
		/// </summary>
		/// <param name="hWnd">目标窗口的句柄</param>
		/// <param name="lpClassName">用于接收类名的缓冲区</param>
		/// <param name="nMaxCount">缓冲区的最大字符容量</param>
		/// <returns>
		/// 成功时返回复制到缓冲区的字符数（不包括终止<see langword="null"/> 字符），失败时返回<see langword="0"/> ,可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
		/// </returns>
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


		/// <summary>
		/// <see langword="[√]"/> 获取指定窗口的标题栏文本
		/// </summary>
		/// <param name="hWnd">目标窗口的句柄</param>
		/// <param name="lpString">用于接收标题文本的缓冲区</param>
		/// <param name="nMaxCount">缓冲区的最大字符容量</param>
		/// <returns>
		/// 成功时返回复制到缓冲区的字符数（不包括终止<see langword="null"/> 字符），失败时返回<see langword="0"/> ,可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码,如果目标窗口没有标题栏、标题为空或句柄无效，将返回空字符串
		/// </returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		/// <summary>
		/// <see langword="[√]"/> 枚举屏幕上的所有顶层窗口,枚举顺序为Z序（即窗口叠放顺序），从最顶层窗口到最底层窗口,回调函数返回<see langword="true"/> 将继续枚举，返回<see langword="false"/> 将停止枚举
		/// </summary>
		/// <param name="lpEnumFunc">指向回调函数的委托</param>
		/// <param name="lParam">应用程序定义的要传递给回调函数的值</param>
		/// <returns>
		/// 如果函数成功，返回非零值；如果函数失败，返回零,可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
		/// </returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);


		/// <summary>
		/// <see langword="[√]"/> 获取创建指定窗口的线程标识符和进程标识符,即使目标进程处于调试状态或被调试器附加，仍能正确获取标识符
		/// </summary>
		/// <param name="hWnd">目标窗口的句柄</param>
		/// <param name="lpdwProcessId">输出参数，接收进程标识符</param>
		/// <returns>
		/// 返回创建窗口的线程标识符。如果窗口句柄无效，返回<see langword="0"/>
		/// </returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


		/// <summary>
		/// <see langword="[√]"/> 创建或打开一个命名的或未命名的文件映射对象,文件映射对象的大小不能超过磁盘上的实际文件大小
		/// </summary>
		/// <param name="hFile">要创建映射的文件的句柄，可创建基于内存的映射</param>
		/// <param name="lpFileMappingAttributes">指向<see cref="SECURITY_ATTRIBUTES"/>结构的指针，决定返回的句柄能否被子进程继承</param>
		/// <param name="flProtect">指定文件映射的保护类型</param>
		/// <param name="dwMaximumSizeHigh">文件映射对象的最大大小的高32位</param>
		/// <param name="dwMaximumSizeLow">文件映射对象的最大大小的低32位</param>
		/// <param name="lpName">文件映射对象的名称，为空则创建匿名映射</param>
		/// <returns>
		/// 如果成功，返回文件映射对象的句柄
		/// </returns>
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern SafeFileHandle CreateFileMapping(SafeFileHandle hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

		/// <summary>
		/// <see langword="[√]"/> 将文件映射对象的视图映射到当前进程的地址空间,映射的视图必须与系统的内存分配粒度对齐(通常为64KB),多个进程可以通过映射同一文件映射对象来共享内存
		/// </summary>
		/// <param name="hFileMappingObject">文件映射对象的句柄</param>
		/// <param name="dwDesiredAccess">指定对文件视图的访问类型</param>
		/// <param name="dwFileOffsetHigh">视图起始偏移的高32位</param>
		/// <param name="dwFileOffsetLow">视图起始偏移的低32位</param>
		/// <param name="dwNumberOfBytesToMap">要映射的字节数，为0则映射整个文件</param>
		/// <returns>
		/// 如果成功，返回映射视图的起始地址；如果失败，返回<see cref="IntPtr.Zero"/>，
		/// </returns>
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr MapViewOfFile(SafeFileHandle hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

		/// <summary>
		/// <see langword="[√]"/> 取消映射当前进程地址空间中的文件视图
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
		/// <see langword="[√]"/> 获取标准输入、输出或错误设备的句柄
		/// </summary>
		/// <param name="nStdHandle">标准设备类型</param>
		/// <returns>
		/// 如果成功，返回请求的设备句柄；如果失败，返回<see cref="IntPtr.Zero"/>
		/// </returns>
		/// <remarks>
		/// 标准设备包括标准输入(<see cref="WinMessages.ConsoleCap.STD_INPUT_HANDLE"/>)、标准输出(<see cref="WinMessages.ConsoleCap.STD_OUTPUT_HANDLE"/>)和标准错误(<see cref="WinMessages.ConsoleCap.STD_ERROR_HANDLE"/>)
		/// 在GUI应用程序中调用可能返回无效句柄
		/// </remarks>
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr GetStdHandle(int nStdHandle);

		/// <summary>
		/// <see langword="[√]"/> 获取指定控制台输入或屏幕缓冲区的当前输入模式 <br/><br/>控制台模式决定了控制台如何处理输入和输出，如启用虚拟终端处理等,输入和输出缓冲区需要分别获取其模式
		/// </summary>
		/// <param name="hConsoleHandle">控制台输入或屏幕缓冲区的句柄</param>
		/// <param name="lpMode">输出参数，接收当前控制台模式</param>
		/// <returns>
		/// 如果成功，返回非零值；如果失败，返回零
		/// </returns>
		[DllImport("kernel32.dll")]
		public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

		/// <summary>
		/// <see langword="[√]"/> 设置指定控制台输入或屏幕缓冲区的输入模式
		/// </summary>
		/// <param name="hConsoleHandle">控制台输入或屏幕缓冲区的句柄</param>
		/// <param name="dwMode">要设置的控制台模式标志</param>
		/// <returns>
		/// 如果成功，返回非零值；如果失败，返回零
		/// </returns>
		/// <remarks>
		/// 常见的模式包括<see cref="WinMessages.ConsoleCap.ENABLE_VIRTUAL_TERMINAL_PROCESSING"/>等
		/// 修改输入模式会影响读取行为执行
		/// </remarks>
		[DllImport("kernel32.dll")]
		public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

		/// <summary>
		/// <see langword="[√]"/> 向指定窗口发送消息并等待消息处理完成
		/// </summary>
		/// <param name="hWnd">目标窗口的句柄。如果此参数为 <see cref="IntPtr.Zero"/>，则消息会被发送到系统中的所有顶层窗口</param>
		/// <param name="Msg">要发送的消息标识符,常用消息常量定义在 <see cref="WinMessages"/> 类中</param>
		/// <param name="wParam">附加的消息特定信息</param>
		/// <param name="lParam">附加的消息特定信息</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>此函数是同步调用，会等待目标窗口处理完消息后才返回</item>
		/// <item>如果目标窗口属于另一个线程，<see cref="SendMessage(IntPtr, uint, IntPtr, IntPtr)"/> 会切换到该线程的上下文</item>
		/// <item>对于跨进程发送消息，参数中的数据必须能被目标进程访问</item>
		/// </list>
		/// </remarks>
		/// <returns>返回值取决于发送的具体消息，通常表示消息处理的结果</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// <see langword="[√]"/> 向指定窗口发送消息并等待消息处理完成
		/// </summary>
		/// <param name="hWnd">目标窗口的句柄。如果此参数为 <see cref="IntPtr.Zero"/>，则消息会被发送到系统中的所有顶层窗口</param>
		/// <param name="Msg">要发送的消息标识符,常用消息常量定义在 <see cref="WinMessages"/> 类中</param>
		/// <param name="wParam">附加的消息特定信息</param>
		/// <param name="lParam">附加的消息特定信息</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>此函数是同步调用，会等待目标窗口处理完消息后才返回</item>
		/// <item>如果目标窗口属于另一个线程，<see cref="SendMessage(IntPtr, uint, IntPtr, string)"/> 会切换到该线程的上下文</item>
		/// <item>对于跨进程发送消息，参数中的数据必须能被目标进程访问</item>
		/// </list>
		/// </remarks>
		/// <returns>返回值取决于发送的具体消息，通常表示消息处理的结果</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

		/// <summary>
		/// <see langword="[√]"/> 向指定窗口发送消息并等待消息处理完成
		/// </summary>
		/// <param name="hWnd">目标窗口的句柄。如果此参数为 <see cref="IntPtr.Zero"/>，则消息会被发送到系统中的所有顶层窗口</param>
		/// <param name="Msg">要发送的消息标识符,常用消息常量定义在 <see cref="WinMessages"/> 类中</param>
		/// <param name="wParam">附加的消息特定信息</param>
		/// <param name="lParam">附加的消息特定信息</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>此函数是同步调用，会等待目标窗口处理完消息后才返回</item>
		/// <item>如果目标窗口属于另一个线程，<see cref="SendMessage(IntPtr, uint, IntPtr, string)"/> 会切换到该线程的上下文</item>
		/// <item>对于跨进程发送消息，参数中的数据必须能被目标进程访问</item>
		/// </list>
		/// </remarks>
		/// <returns>返回值取决于发送的具体消息，通常表示消息处理的结果</returns>
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, int lParam);

        /// <summary>
        /// <see langword="[√]"/> 向指定窗口发送消息并等待消息处理完成
        /// </summary>
        /// <param name="hWnd">目标窗口的句柄。如果此参数为 <see cref="IntPtr.Zero"/>，则消息会被发送到系统中的所有顶层窗口</param>
        /// <param name="Msg">要发送的消息标识符,常用消息常量定义在 <see cref="WinMessages"/> 类中</param>
        /// <param name="wParam">附加的消息特定信息</param>
        /// <param name="lParam">附加的消息特定信息</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>此函数是同步调用，会等待目标窗口处理完消息后才返回</item>
        /// <item>如果目标窗口属于另一个线程，<see cref="SendMessage(IntPtr, int, int, StringBuilder)"/> 会切换到该线程的上下文</item>
        /// <item>对于跨进程发送消息，参数中的数据必须能被目标进程访问</item>
        /// </list>
        /// </remarks>
        /// <returns>返回值取决于发送的具体消息，通常表示消息处理的结果</returns>
        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern void SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);

        /// <summary>
        /// <see langword="[√]"/> 将消息放入与创建指定窗口的线程关联的消息队列后立即返回（异步发送）
        /// </summary>
        /// <param name="hWnd">目标窗口的句柄。特殊值 <see cref="WinMessages.WindowsCap.HWND_NOTOPMOST"/>表示发送到所有顶层窗口</param>
        /// <param name="Msg">要发送的消息标识符,常用消息常量定义在 <see cref="WinMessages"/> 类中</param>
        /// <param name="wParam">附加的消息特定信息</param>
        /// <param name="lParam">附加的消息特定信息</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>此函数是异步调用，将消息放入队列后立即返回，不等待消息处理</item>
        /// <item>如果目标窗口属于当前线程，消息会被直接发送到窗口过程</item>
        /// <item>对于跨线程/跨进程通信，参数中的指针数据必须有效且可访问</item>
        /// <item>如果目标窗口句柄无效，函数会失败但不会报错（返回<see langword="false"/> ）</item>
        /// </list>
        /// </remarks>
        /// <returns>如果函数成功，返回 <see langword="true"/>；否则返回 <see langword="false"/></returns>
        [DllImport("user32.dll", SetLastError = true)]
		public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


		/// <summary>
		/// <see langword="[√]"/> 获取当前进程运行所在系统的信息
		/// </summary>
		/// <param name="lpSystemInfo">接收系统信息的结构体</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>在WOW64环境下，返回的是32位系统的信息</item>
		/// <item>dwNumberOfProcessors返回的是逻辑处理器数量</item>
		/// </list>
		/// </remarks>
		[DllImport("kernel32.dll")]
		public static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

		/// <summary>
		/// <see langword="[√]"/> 获取本地系统的真实信息（不受WOW64影响）
		/// </summary>
		/// <param name="lpSystemInfo">接收系统信息的结构体</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>在64位系统上运行32位进程时，仍返回64位系统的真实信息</item>
		/// <item>与<see cref="GetSystemInfo"/>的区别仅在于WOW64环境下的行为</item>
		/// <item>建议在需要确定实际系统架构时使用此函数</item>
		/// </list>
		/// </remarks>
		[DllImport("kernel32.dll")]
		public static extern void GetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);

		/// <summary>
		/// <see langword="[√]"/> 获取系统度量信息（屏幕、界面元素尺寸等）
		/// </summary>
		/// <param name="nIndex">系统度量指标ID,在<see cref="WinMessages"/>类中,以SM_*的常量参数ID,如<see cref="WinMessages.WindowsCap.SM_CXSCREEN"/></param>
		/// <returns>请求的度量值（像素或布尔值）</returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int GetSystemMetrics(int nIndex);

		/// <summary>
		/// <see langword="[√]"/> 获取当前线程的唯一标识符
		/// </summary>
		/// <returns>当前线程的ID（非零值）</returns>
		/// <remarks>
		/// 线程ID在系统范围内唯一，直到线程终止后可能被回收重用
		/// </remarks>
		[DllImport("kernel32.dll", SetLastError = false)]
		public static extern uint GetCurrentThreadId();

		/// <summary>
		/// <see langword="[√]"/> 获取当前进程的唯一标识符
		/// </summary>
		/// <returns>当前进程的ID（非零值）</returns>
		/// <remarks>
		/// 进程ID在系统范围内唯一，直到进程终止后可能被回收重用
		/// </remarks>
		[DllImport("kernel32.dll", SetLastError = false)]
		public static extern uint GetCurrentProcessId();

		/// <summary>
		/// 获取当前进程的伪句柄
		/// </summary>
		/// <returns>总返回值为 -1 的伪句柄（无需关闭）</returns>
		/// <remarks>
		/// <list type="bullet">
		/// <item>伪句柄仅在当前进程上下文中有效</item>
		/// <item>不可传递给其他进程使用</item>
		/// <item>无需调用释放句柄</item>
		/// </list>
		/// </remarks>
		[DllImport("kernel32.dll", SetLastError = false)]
		public static extern IntPtr GetCurrentProcess();

		/// <summary>
		/// 在调用进程的虚拟地址空间中分配内存
		/// </summary>
		/// <param name="lpAddress">
		/// 期望的起始地址（传<see cref="IntPtr.Zero"/>表示系统自动分配）
		/// </param>
		/// <param name="dwSize">要分配的内存大小（字节）</param>
		/// <param name="flAllocationType">分配类型,在<see cref="WinMessages"/>类中,以<see langword="MEM_*"/>的常量参数,如<see cref="WinMessages.MemoryCap.MEM_COMMIT"/></param>
		/// <param name="flProtect">内存保护属性,在<see cref="WinMessages"/>类中,以<see langword="PAGE_*"/>的常量参数,如<see cref="WinMessages.MemoryCap.PAGE_READWRITE"/></param>
		/// <returns>
		/// 成功返回分配的内存地址，失败返回<see cref="IntPtr.Zero"/>
		/// 可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
		/// </returns>
		/// <remarks>
		/// 分配大小会自动对齐到系统页面边界（通常4KB）
		/// </remarks>
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        /// <summary>
        /// 释放虚拟内存
        /// </summary>
        /// <param name="lpAddress">要释放的内存起始地址</param>
        /// <param name="dwSize">
        /// 要释放的大小（<see cref="WinMessages.MemoryCap.MEM_DECOMMIT"/>时有效，<see cref="WinMessages.MemoryCap.MEM_RELEASE"/> 必须为0）
        /// </param>
        /// <param name="dwFreeType">释放类型,在<see cref="WinMessages"/>类中,以<see langword="MEM_*"/>的常量参数,如<see cref="WinMessages.MemoryCap.MEM_DECOMMIT"/></param>
        /// <returns>成功返回true，失败返回false</returns>
        /// <remarks>
        /// <see cref="WinMessages.MemoryCap.MEM_RELEASE"/>会完全释放由<see cref="VirtualAlloc"/>保留的整个区域
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool VirtualFree(IntPtr lpAddress, uint dwSize, uint dwFreeType);

        /// <summary>
        /// 在指定进程的虚拟地址空间中分配内存
        /// </summary>
        /// <param name="hProcess">目标进程句柄（需<see cref="WinMessages.MemoryCap.PROCESS_VM_OPERATION"/>权限）</param>
        /// <param name="lpAddress">
        /// 期望的起始地址（传<see cref="IntPtr.Zero"/>表示系统自动分配）
        /// </param>
        /// <param name="dwSize">要分配的内存大小（字节）</param>
        /// <param name="flAllocationType">分配类型,在<see cref="WinMessages"/>类中,以<see langword="MEM_*"/>的常量参数,如<see cref="WinMessages.MemoryCap.MEM_COMMIT"/></param>
        /// <param name="flProtect">内存保护属性,在<see cref="WinMessages"/>类中,以<see langword="PAGE_*"/>的常量参数,如<see cref="WinMessages.MemoryCap.PAGE_READWRITE"/></param>
        /// <returns>
        /// 成功返回分配的内存地址，失败返回<see cref="IntPtr.Zero"/>
        /// 可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
        /// </returns>
        /// <remarks>
        /// 分配的内存初始内容为0，大小会自动对齐到系统页面边界
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        /// <summary>
        /// 释放指定进程中的虚拟内存
        /// </summary>
        /// <param name="hProcess">目标进程句柄（需<see cref="WinMessages.MemoryCap.PROCESS_VM_OPERATION"/>权限）</param>
        /// <param name="lpAddress">要释放的内存起始地址</param>
        /// <param name="dwSize">
        /// 要释放的大小（<see cref="WinMessages.MemoryCap.MEM_DECOMMIT"/>时有效，<see cref="WinMessages.MemoryCap.MEM_RELEASE"/>必须为0）
        /// </param>
        /// <param name="dwFreeType">释放类型（<see cref="WinMessages.MemoryCap.MEM_RELEASE"/>）</param>
        /// <returns>成功返回true，失败返回false</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint dwFreeType);

        /// <summary>
        /// 向目标进程写入内存数据
        /// </summary>
        /// <param name="hProcess">目标进程句柄（需<see cref="WinMessages.MemoryCap.PROCESS_VM_WRITE"/>权限）</param>
        /// <param name="lpBaseAddress">目标内存起始地址</param>
        /// <param name="lpBuffer">要写入的数据缓冲区</param>
        /// <param name="nSize">要写入的字节数</param>
        /// <param name="lpNumberOfBytesWritten">实际写入的字节数</param>
        /// <returns>成功返回true，失败可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误码</returns>
        /// <remarks>
        /// 写入前应确保内存区域有<see cref="WinMessages.MemoryCap.PAGE_READWRITE"/>或<see cref="WinMessages.MemoryCap.PAGE_EXECUTE_READWRITE"/>权限
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        /// <summary>
        /// 修改目标进程的内存保护属性
        /// </summary>
        /// <param name="hProcess">目标进程句柄（需<see cref="WinMessages.MemoryCap.PROCESS_VM_WRITE"/>权限）</param>
        /// <param name="lpAddress">内存起始地址</param>
        /// <param name="dwSize">内存区域大小</param>
        /// <param name="flNewProtect">新的保护属性（<see langword="PAGE_*"/> 常量）</param>
        /// <param name="lpflOldProtect">返回原来的保护属性</param>
        /// <returns>成功返回true，失败可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误码</returns>
        /// <remarks>
        /// 典型用法：临时修改为可写(<see cref="WinMessages.MemoryCap.PAGE_READWRITE"/>)，写入后恢复原属性
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

        /// <summary>
        /// 读取目标进程的内存数据
        /// </summary>
        /// <param name="hProcess">目标进程句柄（需<see cref="WinMessages.MemoryCap.PROCESS_VM_WRITE"/>权限）</param>
        /// <param name="lpBaseAddress">要读取的内存起始地址</param>
        /// <param name="lpBuffer">接收数据的缓冲区</param>
        /// <param name="nSize">要读取的字节数</param>
        /// <param name="lpNumberOfBytesRead">实际读取的字节数</param>
        /// <returns>成功返回true，失败可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误码</returns>
        /// <remarks>
        /// 读取前应确保内存区域有可读权限
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, uint nSize, out int lpNumberOfBytesRead);

        /// <summary>
        /// 检测目标进程是否为WOW64（32位进程运行在64位系统）
        /// </summary>
        /// <param name="hProcess">目标进程句柄（需<see cref="WinMessages.MemoryCap.PROCESS_QUERY_LIMITED_INFORMATION"/>权限）</param>
        /// <param name="wow64Process">返回是否为WOW64进程</param>
        /// <returns>成功返回true，失败可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误码</returns>
        /// <remarks>
        /// 在64位系统上检测32位进程的关键API
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool IsWow64Process(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

        /// <summary>
        /// 设置指定进程的优先级类别
        /// </summary>
        /// <param name="hProcess">
        /// 目标进程的句柄（需具备<see cref="WinMessages.MemoryCap.PROCESS_SET_INFORMATION"/>权限）
        /// 可使用<see cref="GetCurrentProcess"/>获取当前进程的伪句柄
        /// </param>
        /// <param name="dwPriorityClass">
        /// 优先级类别，在<see cref="WinMessages"/>类中，以<see langword="PRIORITY_*"/>的常量参数，
        /// 如<see cref="WinMessages.MemoryCap.PRIORITY_HIGH"/>或<see cref="WinMessages.MemoryCap.PRIORITY_NORMAL"/>
        /// </param>
        /// <returns>
        /// 成功返回非零值，失败返回零
        /// 可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
        /// </returns>
        /// <remarks>
        /// 修改优先级可能影响系统稳定性，尤其是<see cref="WinMessages.MemoryCap.PRIORITY_REALTIME"/>级别
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetPriorityClass(IntPtr hProcess, uint dwPriorityClass);

        /// <summary>
        /// 获取指定进程的退出代码
        /// </summary>
        /// <param name="hProcess">
        /// 目标进程的句柄（需具备<see cref="WinMessages.MemoryCap.PROCESS_QUERY_INFORMATION"/>或<see cref="WinMessages.MemoryCap.PROCESS_QUERY_LIMITED_INFORMATION"/>权限）
        /// </param>
        /// <param name="lpExitCode">
        /// 接收退出代码的变量指针。如果进程仍在运行，返回<see cref="WinMessages.MemoryCap.STILL_ACTIVE"/>（259）
        /// </param>
        /// <returns>
        /// 成功返回非零值，失败返回零
        /// 可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
        /// </returns>
        /// <remarks>
        /// 此函数可用于监控进程是否已终止
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);

        /// <summary>
        /// 获取指定进程的时间统计信息（CPU占用、创建时间等）
        /// </summary>
        /// <param name="hProcess">
        /// 目标进程的句柄（需具备<see cref="WinMessages.MemoryCap.PROCESS_QUERY_INFORMATION"/>或<see cref="WinMessages.MemoryCap.PROCESS_QUERY_LIMITED_INFORMATION"/>权限）
        /// </param>
        /// <param name="lpCreationTime">
        /// 接收进程创建时间（FILETIME结构，单位100纳秒，从1601-01-01 UTC计算）
        /// </param>
        /// <param name="lpExitTime">
        /// 接收进程退出时间（未退出则为零）
        /// </param>
        /// <param name="lpKernelTime">
        /// 接收进程在内核模式下的CPU时间（单位100纳秒）
        /// </param>
        /// <param name="lpUserTime">
        /// 接收进程在用户模式下的CPU时间（单位100纳秒）
        /// </param>
        /// <returns>
        /// 成功返回非零值，失败返回零
        /// 可通过<see cref="Marshal.GetLastWin32Error()"/>获取错误代码
        /// </returns>
        /// <remarks>
        /// 时间值可通过<see cref="DateTime.FromFileTime(long)"/>转换为可读格式
        /// </remarks>
        [DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetProcessTimes(IntPtr hProcess, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);
		
		/// <summary>
		/// 获取当前正在使用的电源方案<see cref="Guid"/>
		/// </summary>
		/// <param name="RootPowerKey">电源设置的根键，通常传入<see cref="IntPtr.Zero"/></param>
		/// <param name="p_ActivePolicyGuid">用于接收当前激活的电源方案<see cref="Guid"/>的指针</param>
		/// <returns>成功返回0，失败返回错误代码</returns>
		/// <remarks>调用后需要释放<paramref name="p_ActivePolicyGuid"/>指向的内存</remarks>
		[DllImport("PowrProf.dll")]
		public static extern uint PowerGetActiveScheme(IntPtr RootPowerKey, ref IntPtr p_ActivePolicyGuid);

		/// <summary>
		/// 读取电源方案的名称
		/// </summary>
		/// <param name="RootPowerKey">电源设置的根键，通常传入<see cref="IntPtr.Zero"/></param>
		/// <param name="SchemeGuid">要查询的电源方案<see cref="Guid"/></param>
		/// <param name="SubGroupOfPowerSettingGuid">电源设置子组的<see cref="Guid"/>，获取方案名称时传入<see cref="IntPtr.Zero"/></param>
		/// <param name="PowerSettingGuid">电源设置的<see cref="Guid"/>，获取方案名称时传入<see cref="IntPtr.Zero"/></param>
		/// <param name="Buffer">接收名称的缓冲区指针</param>
		/// <param name="BufferSize">缓冲区大小（传入时是最大大小，传出时是实际大小）</param>
		/// <returns>成功返回0，缓冲区不足返回122，失败返回其他错误代码</returns>
		/// <remarks>通常需要调用两次：第一次获取所需缓冲区大小，第二次获取实际名称</remarks>
		[DllImport("PowrProf.dll")]
		public static extern uint PowerReadFriendlyName(IntPtr RootPowerKey, ref Guid SchemeGuid, IntPtr SubGroupOfPowerSettingGuid, IntPtr PowerSettingGuid, IntPtr Buffer, ref UInt32 BufferSize);

        /// <summary>
        /// 获取文件或文件夹的图标和类型信息
        /// </summary>
        /// <param name="pszPath">文件路径、文件夹路径或文件扩展名（如".txt"）</param>
        /// <param name="dwFileAttributes">文件属性标志，如FILE_ATTRIBUTE_NORMAL（0x80）表示普通文件</param>
        /// <param name="psfi">接收图标和文件信息的SHFILEINFO结构体</param>
        /// <param name="cbFileInfo">SHFILEINFO结构体的大小（字节）</param>
        /// <param name="uFlags">指定要获取的信息类型的标志组合</param>
        /// <returns>成功返回非零值，失败返回0</returns>
        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        public static extern int SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        /// <summary>
        /// 查找窗体中控件
        /// </summary>
        /// <param name="parent">窗体句柄</param>
        /// <param name="child">子控件句柄</param>
        /// <param name="className">子控件类名</param>
        /// <param name="captionName">子控件名称</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr child, string className, string captionName);


        /// <summary>
        /// 将指定的钩子过程安装到钩子链中。用于监视系统事件或特定线程的事件
        /// </summary>
        /// <param name="idHook">钩子类型。指定要安装的钩子过程类型</param>
        /// <param name="lpfn">指向钩子过程的指针。该参数是一个回调函数（<see cref="HookProc"/> 委托），当钩子监测到事件时系统会调用它</param>
        /// <param name="hInstance">包含钩子过程的 DLL 或应用程序实例的句柄。对于全局钩子（threadId == 0），此参数必须设置为模块句柄；对于线程特定钩子，通常可设为 IntPtr.Zero（但建议仍提供模块句柄以确保 DLL 句柄有效）</param>
        /// <param name="threadId">与钩子过程关联的线程标识符。如果为 0，则钩子过程与所有现有线程关联（即全局钩子）</param>
        /// <returns>如果函数成功，返回钩子过程的句柄；如果失败，返回 0（<see cref="IntPtr.Zero"/>）。可通过 <see cref="Marshal.GetLastWin32Error"/> 获取错误码</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        /// <summary>
        /// 移除由 SetWindowsHookEx 安装的钩子  成功返回 true，失败返回 false
        /// </summary>
        /// <param name="idHook">要移除的钩子句柄（由 SetWindowsHookEx 返回）</param>
        /// <returns>是否成功移除</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        /// <summary>
        /// 将钩子事件传递给钩子链中的下一个钩子过程  必须在钩子回调中调用，否则系统会忽略该事件
        /// </summary>
        /// <param name="idHook">当前钩子句柄（由 SetWindowsHookEx 返回）</param>
        /// <param name="nCode">钩子代码，用于判断是否处理事件</param>
        /// <param name="wParam">事件类型标识（如 WM_KEYDOWN）</param>
        /// <param name="lParam">事件数据指针（结构体指针）</param>
        /// <returns>下一个钩子过程的返回结果</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        /// <summary>
        /// 获取指定模块的句柄（如 .exe 或 .dll）  常用于获取当前应用程序实例句柄，作为 hInstance 参数传入 <br/>
		/// 此方法貌似也可以获取-> Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
        /// </summary>
        /// <param name="name">模块名称（如 "MyApp.exe"），为 null 时返回当前进程句柄</param>
        /// <returns>模块句柄，失败返回 IntPtr.Zero</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);

        /// <summary>
        /// 导入模拟键盘的方法
        /// </summary>
        /// <param name="key" >按键的虚拟键值,可通过<see cref="WinMessages.KeyValue"/>中查询以及传递</param>
        /// <param name= "bScan" >扫描码，一般不用设置，用<see langword="0"/>代替就行</param>
        /// <param name= "dwFlags" >选项标志：<see langword="0"/> 表示按下，<see langword="2"/>表示松开,其它数值默认<see langword="2"/>处理</param>
        /// <param name= "dwExtraInfo">一般设置为<see langword="0"/></param>
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte key, byte bScan, int dwFlags, int dwExtraInfo);


        /// <summary>显示或隐藏窗口</summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="nCmdShow">窗口显示状态（如<see cref="WinMessages.WindowsCap.SWP_HIDEWINDOW"/>,<see cref="WinMessages.WindowsCap.SWP_SHOWWINDOW"/>）</param>
        /// <returns>之前窗口的可见性（非0可见，0隐藏）</returns>
        [DllImport("user32.dll")] 
		public static extern int ShowWindow(int hwnd, int nCmdShow);


        /// <summary>查询或设置系统全局参数（如屏保、电源、桌面等）</summary>
        /// <param name="uAction">要执行的操作</param>
        /// <param name="uParam">操作相关参数</param>
        /// <param name="lpvparam">操作相关数据</param>
        /// <param name="fuwinIni">更新标志</param>
        /// <returns>成功返回非0，失败返回0</returns>
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvparam, int fuwinIni);


        /// <summary>设置系统时间</summary>
        /// <param name="time">要设置的新时间</param>
        /// <returns>成功返回<see langword="true"/> ，失败返回<see langword="false"/></returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        public static extern bool SetSystemTime(ref SYSTEMTIME time);


        /// <summary>修改窗口的属性（如样式、扩展样式、窗口过程等）</summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="nIndex">要修改的属性索</param>
        /// <param name="dwNewLong">新的属性值</param>
        /// <returns>之前属性值，失败返回0</returns>
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);



        /// <summary>
        /// 获取设备上下文的能力/属性信息
        /// </summary>
        /// <param name="hdc">设备上下文的句柄</param>
        /// <param name="nIndex">要获取的信息类型</param>
        /// <returns>指定类型的设备信息值,失败返回 0</returns>
        [DllImport("gdi32.dll", EntryPoint = "GetDeviceCaps", SetLastError = true)]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);


        /// <summary>
        /// 获取指定进程内模块的完整路径
        /// </summary>
        /// <param name="hProcess">目标进程的句柄，需具有 PROCESS_QUERY_INFORMATION 和 PROCESS_VM_READ 权限</param>
        /// <param name="hModule">目标模块的句柄。如为 <see cref="IntPtr.Zero"/>，则获取进程主模块（可执行文件）的路径</param>
        /// <param name="lpFilename">用于接收路径的 <see cref="StringBuilder"/> 缓冲区</param>
        /// <param name="nSize">缓冲区大小（字符数）</param>
        /// <returns>返回写入缓冲区的字符数（不含终止符）,失败返回 0</returns>
        [DllImport("psapi.dll")]
        public static extern int GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, int nSize);


        /// <summary>
        /// 打开一个现有进程对象，获取其句柄<br/> 
		/// 句柄用完后必须调用 <see cref="CloseHandle(IntPtr)"/> 关闭，否则造成句柄泄漏
        /// </summary>
        /// <param name="dwDesiredAccess">进程访问权限，常用值：<br/> 
        ///  <see langword="0x0400"/> — PROCESS_QUERY_INFORMATION（查询信息）<br/> 
        ///  <see langword="0x0010"/> — PROCESS_VM_READ（读取内存）<br/> 
        ///  <see langword="0x0410"/> — 两者组合<br/> 
        ///  <see langword="0x1F0FFF"/> — PROCESS_ALL_ACCESS（全部权限）<br/> 
        /// </param>
        /// <param name="bInheritHandle">是否允许子进程继承该句柄，通常传 <see langword="false"/> </param>
        /// <param name="dwProcessId">目标进程的 PID</param>
        /// <returns>进程句柄,失败返回 <see cref="IntPtr.Zero"/>，需调用 <see cref="Marshal.GetLastWin32Error()"/> 获取错误码</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);


        /// <summary>
        /// 关闭一个内核对象句柄
        /// </summary>
        /// <param name="hObject">要关闭的句柄</param>
        /// <returns>成功返回 <see langword="true"/> ，失败返回 <see langword="false"/> </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

    }
}

