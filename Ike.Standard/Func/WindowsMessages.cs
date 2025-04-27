using System;
using System.ComponentModel;

namespace Ike.Standard
{
    /// <summary>
    /// 常量值
    /// </summary>
    public class WindowsMessages
    {

        #region 控制台相关标志
        /// <summary>
        /// 表示标准输入设备的句柄标识符
        /// </summary>
        public const int STD_INPUT_HANDLE = -10;
        /// <summary>
        /// 控制台扩展功能标志
        /// </summary>
        public const uint ENABLE_EXTENDED_FLAGS = 0x0080;
        /// <summary>
        /// 控制台快速编辑模式标志
        /// </summary>
        public const uint ENABLE_QUICK_EDIT_MODE = 0x0040;

        /// <summary>
        /// 标准输出设备句柄标识符
        /// <para>Windows API 常量，对应标准输出流的句柄值</para>
        /// </summary>
        public const int STD_OUTPUT_HANDLE = -11;

        /// <summary>
        /// 启用虚拟终端处理的控制台模式标志
        /// <para>允许控制台解析ANSI转义序列(如颜色代码)</para>
        /// </summary>
        public const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        #endregion


        #region 包含用于控制窗口显示状态的常量值


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

        #endregion


        #region 表示窗口位置、大小、Z 顺序的操作标志


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
        /// 禁用窗口的客户区部分更新,不改变所有者窗口的 Z 顺序
        /// </summary>
        public const uint SWP_NOOWNERZORDER = 0x0200;

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

        #endregion


        #region 窗口消息

        /// <summary>空消息（用于窗口激活测试）</summary>
        public const uint WM_NULL = 0x0000;

        /// <summary>关闭窗口请求</summary>
        public const uint WM_CLOSE = 0x0010;

        /// <summary>命令消息（按钮点击/菜单选择等）</summary>
        public const uint WM_COMMAND = 0x0111;

        /// <summary>窗口销毁通知</summary>
        public const uint WM_DESTROY = 0x0002;

        /// <summary>窗口尺寸改变</summary>
        public const uint WM_SIZE = 0x0005;

        /// <summary>窗口位置改变</summary>
        public const uint WM_MOVE = 0x0003;

        /// <summary>设置文本内容</summary>
        public const uint WM_SETTEXT = 0x000C;

        /// <summary>获取文本内容</summary>
        public const uint WM_GETTEXT = 0x000D;

        /// <summary>获取文本长度</summary>
        public const uint WM_GETTEXTLENGTH = 0x000E;

        /// <summary>控件启用/禁用状态变更</summary>
        public const uint WM_ENABLE = 0x000A;

        /// <summary>按键按下</summary>
        public const uint WM_KEYDOWN = 0x0100;

        /// <summary>按键释放</summary>
        public const uint WM_KEYUP = 0x0101;

        /// <summary>字符输入</summary>
        public const uint WM_CHAR = 0x0102;

        /// <summary>鼠标移动</summary>
        public const uint WM_MOUSEMOVE = 0x0200;

        /// <summary>左键按下</summary>
        public const uint WM_LBUTTONDOWN = 0x0201;

        /// <summary>左键释放</summary>
        public const uint WM_LBUTTONUP = 0x0202;

        /// <summary>用户自定义消息起始值</summary>
        public const uint WM_USER = 0x0400;

        /// <summary>控件通知消息（如按钮点击）</summary>
        public const uint WM_NOTIFY = 0x004E;

        /// <summary>粘贴操作</summary>
        public const uint WM_PASTE = 0x0302;

        /// <summary>复制操作</summary>
        public const uint WM_COPY = 0x0301;

        /// <summary>剪切操作</summary>
        public const uint WM_CUT = 0x0300;

        #endregion


        #region 系统度量常用指标

        /* ================= 基础显示指标 ================= */
        /// <summary>主屏幕工作区宽度（像素）</summary>
        public const int SM_CXSCREEN = 0;
        /// <summary>主屏幕工作区高度（像素）</summary>
        public const int SM_CYSCREEN = 1;
        /// <summary>垂直滚动条箭头高度（像素）</summary>
        public const int SM_CYVSCROLL = 2;
        /// <summary>水平滚动条箭头宽度（像素）</summary>
        public const int SM_CXHSCROLL = 3;
        /// <summary>窗口标题栏高度（像素）</summary>
        public const int SM_CYCAPTION = 4;
        /// <summary>窗口边框宽度（像素）</summary>
        public const int SM_CXBORDER = 5;
        /// <summary>窗口边框高度（像素）</summary>
        public const int SM_CYBORDER = 6;
        /// <summary>对话框框架宽度（等同于SM_CXFIXEDFRAME）</summary>
        public const int SM_CXDLGFRAME = 7;
        /// <summary>对话框框架高度（等同于SM_CYFIXEDFRAME）</summary>
        public const int SM_CYDLGFRAME = 8;
        /// <summary>垂直滚动条拇指高度（像素）</summary>
        public const int SM_CYVTHUMB = 9;
        /// <summary>水平滚动条拇指宽度（像素）</summary>
        public const int SM_CXHTHUMB = 10;
        /// <summary>图标默认宽度（像素）</summary>
        public const int SM_CXICON = 11;
        /// <summary>图标默认高度（像素）</summary>
        public const int SM_CYICON = 12;
        /// <summary>光标默认宽度（像素）</summary>
        public const int SM_CXCURSOR = 13;
        /// <summary>光标默认高度（像素）</summary>
        public const int SM_CYCURSOR = 14;
        /// <summary>菜单栏高度（像素）</summary>
        public const int SM_CYMENU = 15;
        /// <summary>全屏窗口客户区宽度（像素）</summary>
        public const int SM_CXFULLSCREEN = 16;
        /// <summary>全屏窗口客户区高度（像素）</summary>
        public const int SM_CYFULLSCREEN = 17;
        /// <summary>Kanji窗口高度（日语系统）</summary>
        public const int SM_CYKANJIWINDOW = 18;
        /// <summary>鼠标是否存在（非零表示存在）</summary>
        public const int SM_MOUSEPRESENT = 19;
        /// <summary>垂直滚动条高度（像素）</summary>
        [Obsolete("Use SM_CYVSCROLL (value=2) instead")]
        public const int SM_CYVSCROLL_OBSOLETE = 20;
        /// <summary>水平滚动条宽度（像素）</summary>
        [Obsolete("Use SM_CXHSCROLL (value=3) instead")]
        public const int SM_CXHSCROLL_OBSOLETE = 21;
        /// <summary>调试版Windows标志</summary>
        public const int SM_DEBUG = 22;
        /// <summary>鼠标左右键交换（非零表示已交换）</summary>
        public const int SM_SWAPBUTTON = 23;
        /// <summary>保留值</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public const int SM_RESERVED1 = 24;
        /// <summary>保留值</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public const int SM_RESERVED2 = 25;
        /// <summary>保留值</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public const int SM_RESERVED3 = 26;
        /// <summary>保留值</summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public const int SM_RESERVED4 = 27;
        /// <summary>最小窗口宽度（像素）</summary>
        public const int SM_CXMIN = 28;
        /// <summary>最小窗口高度（像素）</summary>
        public const int SM_CYMIN = 29;
        /// <summary>窗口标题按钮宽度（像素）</summary>
        public const int SM_CXSIZE = 30;
        /// <summary>窗口标题按钮高度（像素）</summary>
        public const int SM_CYSIZE = 31;
        /// <summary>可调整窗口边框宽度（等同于SM_CXFRAME）</summary>
        public const int SM_CXSIZEFRAME = 32;
        /// <summary>可调整窗口边框高度（等同于SM_CYFRAME）</summary>
        public const int SM_CYSIZEFRAME = 33;
        /// <summary>最小窗口跟踪宽度（像素）</summary>
        public const int SM_CXMINTRACK = 34;
        /// <summary>最小窗口跟踪高度（像素）</summary>
        public const int SM_CYMINTRACK = 35;
        /// <summary>双击区域宽度（像素）</summary>
        public const int SM_CXDOUBLECLK = 36;
        /// <summary>双击区域高度（像素）</summary>
        public const int SM_CYDOUBLECLK = 37;
        /// <summary>图标标题换行宽度（像素）</summary>
        public const int SM_CXICONSPACING = 38;
        /// <summary>图标标题换行高度（像素）</summary>
        public const int SM_CYICONSPACING = 39;
        /// <summary>菜单弹出对齐标志（非零表示右对齐）</summary>
        public const int SM_MENUDROPALIGNMENT = 40;
        /// <summary>PenWindows扩展安装标志</summary>
        public const int SM_PENWINDOWS = 41;
        /// <summary>是否启用DBCS（双字节字符集）</summary>
        public const int SM_DBCSENABLED = 42;
        /// <summary>鼠标按钮数量（0=无鼠标）</summary>
        public const int SM_CMOUSEBUTTONS = 43;
        /// <summary>安全启动标志（非零表示安全模式）</summary>
        public const int SM_SECURE = 44;
        /// <summary>3D边框宽度（像素）</summary>
        public const int SM_CXEDGE = 45;
        /// <summary>3D边框高度（像素）</summary>
        public const int SM_CYEDGE = 46;
        /// <summary>最小窗口动画宽度（像素）</summary>
        public const int SM_CXMINSPACING = 47;
        /// <summary>最小窗口动画高度（像素）</summary>
        public const int SM_CYMINSPACING = 48;
        /// <summary>非客户区图标宽度（像素）</summary>
        public const int SM_CXSMICON = 49;
        /// <summary>非客户区图标高度（像素）</summary>
        public const int SM_CYSMICON = 50;
        /// <summary>小标题按钮宽度（像素）</summary>
        public const int SM_CXSMSIZE = 52;
        /// <summary>小标题按钮高度（像素）</summary>
        public const int SM_CYSMSIZE = 53;
        /// <summary>菜单栏按钮宽度（像素）</summary>
        public const int SM_CXMENUSIZE = 54;
        /// <summary>菜单栏按钮高度（像素）</summary>
        public const int SM_CYMENUSIZE = 55;
        /// <summary>窗口排列方向标志（非零表示从右到左）</summary>
        public const int SM_ARRANGE = 56;
        /// <summary>最小窗口动画宽度（像素）</summary>
        public const int SM_CXMINIMIZED = 57;
        /// <summary>最小窗口动画高度（像素）</summary>
        public const int SM_CYMINIMIZED = 58;
        /// <summary>最大窗口跟踪宽度（像素）</summary>
        public const int SM_CXMAXTRACK = 59;
        /// <summary>最大窗口跟踪高度（像素）</summary>
        public const int SM_CYMAXTRACK = 60;
        /// <summary>最大化窗口宽度（像素）</summary>
        public const int SM_CXMAXIMIZED = 61;
        /// <summary>最大化窗口高度（像素）</summary>
        public const int SM_CYMAXIMIZED = 62;
        /// <summary>网络连接标志（非零表示已连接）</summary>
        public const int SM_NETWORK = 63;
        /// <summary>窗口清理标志（非零表示需要清理）</summary>
        public const int SM_CLEANBOOT = 67;
        /// <summary>鼠标滚轮存在标志（非零表示存在）</summary>
        public const int SM_MOUSEWHEELPRESENT = 75;
        /// <summary>垂直滚动条箭头高度（像素）</summary>
        public const int SM_CYVSCROLL_ARROW = 20;
        /// <summary>水平滚动条箭头宽度（像素）</summary>
        public const int SM_CXHSCROLL_ARROW = 21;

        /* ================= 多显示器系统指标 ================= */
        /// <summary>虚拟屏幕宽度（像素）</summary>
        public const int SM_CXVIRTUALSCREEN = 78;
        /// <summary>虚拟屏幕高度（像素）</summary>
        public const int SM_CYVIRTUALSCREEN = 79;
        /// <summary>显示器数量</summary>
        public const int SM_CMONITORS = 80;
        /// <summary>显示器颜色格式一致标志</summary>
        public const int SM_SAMEDISPLAYFORMAT = 81;

        /* ================= 输入法指标 ================= */
        /// <summary>输入法窗口标志（非零表示存在）</summary>
        public const int SM_IMMENABLED = 82;

        /* ================= Windows XP+ 新增指标 ================= */
        /// <summary>像素双击宽度（XP+）</summary>
        public const int SM_CXPADDEDBORDER = 92;
        /// <summary>数字转换器输入设备状态（Win7+）</summary>
        public const int SM_DIGITIZER = 94;
        /// <summary>最大触摸点数（Win7+）</summary>
        public const int SM_MAXIMUMTOUCHES = 95;

        /* ================= 系统配置标志 ================= */
        /// <summary>远程终端会话标志</summary>
        public const int SM_REMOTESESSION = 0x1000;
        /// <summary>关闭按钮存在标志</summary>
        public const int SM_SHUTTINGDOWN = 0x2000;
        /// <summary>远程控制会话标志</summary>
        public const int SM_REMOTECONTROL = 0x2001;

        /* ================= 平板电脑/媒体中心指标 ================= */
        /// <summary>平板电脑模式标志</summary>
        public const int SM_TABLETPC = 86;
        /// <summary>媒体中心版标志</summary>
        public const int SM_MEDIACENTER = 87;
        /// <summary>Starter版标志</summary>
        public const int SM_STARTER = 88;
        /// <summary>Server R2版标志</summary>
        public const int SM_SERVERR2 = 89;

        /* ================= Windows 8+ 新增指标 ================= */
        /// <summary>主显示器水平DPI（Win8.1+）</summary>
        public const int SM_CXSCREENDPI = 88;
        /// <summary>主显示器垂直DPI（Win8.1+）</summary>
        public const int SM_CYSCREENDPI = 89;
        /// <summary>交互接触输入标志（Win8+）</summary>
        public const int SM_CONVERTIBLESLATEMODE = 0x2003;
        /// <summary>系统启动模式标志（Win8+）</summary>
        public const int SM_SYSTEMDOCKED = 0x2004;

        #endregion


        #region 内存管理常量

        /* 内存分配类型 */
        /// <summary>提交物理内存</summary>
        public const uint MEM_COMMIT = 0x00001000;
        /// <summary>保留虚拟地址空间</summary>
        public const uint MEM_RESERVE = 0x00002000;
        /// <summary>重置内存内容（需配合MEM_COMMIT）</summary>
        public const uint MEM_RESET = 0x00080000;
        /// <summary>大页面内存（需SeLockMemory权限）</summary>
        public const uint MEM_LARGE_PAGES = 0x20000000;
        /// <summary>释放已提交/保留的内存</summary>
        public const uint MEM_DECOMMIT = 0x4000;
        /// <summary>完全释放地址空间</summary>
        public const uint MEM_RELEASE = 0x8000;

        /* 内存保护标志 */
        /// <summary>可执行读写</summary>
        public const uint PAGE_EXECUTE_READWRITE = 0x40;
        /// <summary>可读写</summary>
        public const uint PAGE_READWRITE = 0x04;
        /// <summary>仅执行</summary>
        public const uint PAGE_EXECUTE = 0x10;
        /// <summary>不可访问</summary>
        public const uint PAGE_NOACCESS = 0x01;
        /// <summary>写入时拷贝（COW机制）</summary>
        public const uint PAGE_WRITECOPY = 0x08;
        /// <summary>执行时拷贝</summary>
        public const uint PAGE_EXECUTE_WRITECOPY = 0x80;

        /* 进程访问权限 */
        /// <summary>内存操作权限,是内存操作的基础权限</summary>
        public const uint PROCESS_VM_OPERATION = 0x0008;
        /// <summary>内存读取权限,需要搭配<see cref="PROCESS_VM_OPERATION"/>使用</summary>
        public const uint PROCESS_VM_READ = 0x0010;
        /// <summary>内存写入权限,需要搭配<see cref="PROCESS_VM_OPERATION"/>使用</summary>
        public const uint PROCESS_VM_WRITE = 0x0020;
        /// <summary>完全控制权限, 警告：需要管理员权限且可能触发杀毒软件</summary>
        public const uint PROCESS_ALL_ACCESS = 0x001F0FFF;
        /// <summary>
        /// 允许查询有限进程信息 Windows Vista+ 新增的轻量级查询权限 </summary>
        public const uint PROCESS_QUERY_LIMITED_INFORMATION = 0x1000;


        #endregion
    }
}
