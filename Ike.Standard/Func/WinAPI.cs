using System;
using System.Runtime.InteropServices;
using static Ike.Standard.Enums;

namespace Ike.Standard
{
	/// <summary>
	/// Windows API方法,非托管动态链接库方法调用
	/// </summary>
	public class WinAPI
	{
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
		/// <param name="className">窗口类名（可以为 null，表示忽略类名）</param>
		/// <param name="captionName">窗口标题名（可以为 null，表示忽略窗口标题）</param>
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


	}
}
