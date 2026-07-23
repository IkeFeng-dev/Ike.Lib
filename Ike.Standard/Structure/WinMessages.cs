using System;
using System.ComponentModel;

namespace Ike.Standard
{
    /// <summary>
    /// 常量值
    /// </summary>
    public class WinMessages
    {
        /// <summary>
        /// 是特殊句柄常量,用于向所有顶级窗口广播消息​
        /// </summary>
        public readonly static IntPtr HWND_BROADCAST = (IntPtr)0xFFFF;
        /// <summary>
        /// 鼠标操作值
        /// </summary>
        public class MouseCap
        {
            /// <summary>
            /// 鼠标移动
            /// </summary>
            public const int MOVE = 0x0001;
            /// <summary>
            /// 左键按下
            /// </summary>
            public const int LEFTDOWN = 0x0002;
            /// <summary>
            /// 左键抬起
            /// </summary>
            public const int LEFTUP = 0x0004;
            /// <summary>
            /// 右键按下
            /// </summary>
            public const int RIGHTDOWN = 0x0008;
            /// <summary>
            /// 右键抬起
            /// </summary>
            public const int RIGHTUP = 0x0010;
            /// <summary>
            /// 中键按下
            /// </summary>
            public const int MIDDLEDOWN = 0x0020;
            /// <summary>
            /// 中键抬起
            /// </summary>
            public const int MIDDLEUP = 0x0040;
            /// <summary>
            /// XDown
            /// </summary>
            public const int XDOWN = 0x0080;
            /// <summary>
            /// XUp
            /// </summary>
            public const int XUP = 0x0100;
            /// <summary>
            /// 鼠标滚动
            /// </summary>
            public const int WHEEL = 0x0800;
            /// <summary>
            /// VirtualDesk
            /// </summary>
            public const int VIRTUALDESK = 0x4000;
            /// <summary>
            /// Absolute
            /// </summary>
            public const int ABSOLUTE = 0x8000;
            /// <summary>
            /// 鼠标光标(标准)
            /// </summary>
            public const int OCR_NORAAC = 32512;
        }

        /// <summary>
        /// 鼠标钩子常量
        /// </summary>
        public class HookCap
        {
            /// <summary>
            /// 鼠标移动(512)
            /// </summary>
            public const int WM_MOUSEMOVE = 0x200;
            /// <summary>
            /// 鼠标左键按下(513)
            /// </summary>
            public const int WM_LBUTTONDOWN = 0x201;
            /// <summary>
            /// 鼠标右键按下(516)
            /// </summary>
            public const int WM_RBUTTONDOWN = 0x204;
            /// <summary>
            /// 鼠标中键按下(519)
            /// </summary>
            public const int WM_MBUTTONDOWN = 0x207;
            /// <summary>
            /// 鼠标左键抬起(514)
            /// </summary>
            public const int WM_LBUTTONUP = 0x202;
            /// <summary>
            /// 鼠标右键抬起(517)
            /// </summary>
            public const int WM_RBUTTONUP = 0x205;
            /// <summary>
            /// 鼠标中键抬起(520)
            /// </summary>
            public const int WM_MBUTTONUP = 0x208;
            /// <summary>
            /// 鼠标左键双击(515)
            /// </summary>
            public const int WM_LBUTTONDBLCLK = 0x203;
            /// <summary>
            /// 鼠标右键双击(518)
            /// </summary>
            public const int WM_RBUTTONDBLCLK = 0x206;
            /// <summary>
            /// 鼠标中键双击(521)
            /// </summary>
            public const int WM_MBUTTONDBLCLK = 0x209;
            /// <summary>
            /// 鼠标滚轮滚动
            /// </summary>
            public const int WM_MOUSEROLL = 0x20A;
            /// <summary>
            /// 鼠标钩子
            /// </summary>
            public const int WH_MOUSE_LL = 14;
            /// <summary>
            /// 键盘钩子
            /// </summary>
            public const int WH_KEYBOARD_LL = 13;
        }

        /// <summary>
        /// 键盘键值
        /// </summary>
        public class KeyValue
        {
            /// <summary>
            /// CANCEL 键
            /// </summary>
            public const byte Cancel = 0x3;
            /// <summary>
            /// BACKSPACE 键
            /// </summary>
            public const byte Backspace = 0x8;
            /// <summary>
            /// TAB 键
            /// </summary>
            public const byte Tab = 0x9;
            /// <summary>
            ///  CLEAR 键
            /// </summary>
            public const byte Clear = 0xC;
            /// <summary>
            /// ENTER 键
            /// </summary>
            public const byte Enter = 0xD;
            /// <summary>
            /// SHIFT 键
            /// </summary>
            public const byte Shift = 0x10;
            /// <summary>
            /// CTRL 键
            /// </summary>
            public const byte Ctrl = 0x11;
            /// <summary>
            ///  Alt 键  (键码18)
            /// </summary>
            public const byte Alt = 18;
            /// <summary>
            ///  MENU 键
            /// </summary>
            public const byte Menu = 0x12;
            /// <summary>
            /// PAUSE 键
            /// </summary>
            public const byte Pause = 0x13;
            /// <summary>
            /// CAPS LOCK 键
            /// </summary>
            public const byte Casp = 0x14;
            /// <summary>
            ///  ESC 键
            /// </summary>
            public const byte Esc = 0x1B;
            /// <summary>
            /// SPACEBAR 键
            /// </summary>
            public const byte Space = 0x20;
            /// <summary>
            ///  PAGE UP 键
            /// </summary>
            public const byte PageUp = 0x21;
            /// <summary>
            /// End 键
            /// </summary>
            public const byte End = 0x23;
            /// <summary>
            /// HOME 键
            /// </summary>
            public const byte Home = 0x24;
            /// <summary>
            ///  LEFT ARROW 键
            /// </summary>
            public const byte Left = 0x25;
            /// <summary>
            ///  UP ARROW 键
            /// </summary>
            public const byte Up = 0x26;
            /// <summary>
            ///  RIGHT ARROW 键
            /// </summary>
            public const byte Right = 0x27;
            /// <summary>
            /// DOWN ARROW 键
            /// </summary>
            public const byte Down = 0x28;
            /// <summary>
            ///  Select 键
            /// </summary>
            public const byte Select = 0x29;
            /// <summary>
            /// PRINT SCREEN 键
            /// </summary>
            public const byte Prtsc = 0x2A;
            /// <summary>
            /// EXECUTE 键
            /// </summary>
            public const byte Execute = 0x2B;
            /// <summary>
            /// SNAPSHOT 键
            /// </summary>
            public const byte Snapshot = 0x2C;
            /// <summary>
            ///  Delete 键
            /// </summary>
            public const byte Delete = 0x2E;
            /// <summary>
            /// HELP 键
            /// </summary>
            public const byte Help = 0x2F;
            /// <summary>
            ///  NUM LOCK 键
            /// </summary>
            public const byte NumLock = 0x90;

            //常用键 字母键A到Z

            /// <summary>
            /// A
            /// </summary>
            public const byte A = 65;
            /// <summary>
            /// B
            /// </summary>
            public const byte B = 66;
            /// <summary>
            /// C
            /// </summary>
            public const byte C = 67;
            /// <summary>
            /// D
            /// </summary>
            public const byte D = 68;
            /// <summary>
            /// E
            /// </summary>
            public const byte E = 69;
            /// <summary>
            /// F
            /// </summary>
            public const byte F = 70;
            /// <summary>
            /// G
            /// </summary>
            public const byte G = 71;
            /// <summary>
            /// H
            /// </summary>
            public const byte H = 72;
            /// <summary>
            /// I
            /// </summary>
            public const byte I = 73;
            /// <summary>
            /// J
            /// </summary>
            public const byte J = 74;
            /// <summary>
            /// K
            /// </summary>
            public const byte K = 75;
            /// <summary>
            /// L
            /// </summary>
            public const byte L = 76;
            /// <summary>
            /// M
            /// </summary>
            public const byte M = 77;
            /// <summary>
            /// N
            /// </summary>
            public const byte N = 78;
            /// <summary>
            /// O
            /// </summary>
            public const byte O = 79;
            /// <summary>
            /// P
            /// </summary>
            public const byte P = 80;
            /// <summary>
            /// Q
            /// </summary>
            public const byte Q = 81;
            /// <summary>
            /// R
            /// </summary>
            public const byte R = 82;
            /// <summary>
            /// S
            /// </summary>
            public const byte S = 83;
            /// <summary>
            /// T
            /// </summary>
            public const byte T = 84;
            /// <summary>
            /// U
            /// </summary>
            public const byte U = 85;
            /// <summary>
            /// V
            /// </summary>
            public const byte V = 86;
            /// <summary>
            /// W
            /// </summary>
            public const byte W = 87;
            /// <summary>
            /// X
            /// </summary>
            public const byte X = 88;
            /// <summary>
            /// Y
            /// </summary>
            public const byte Y = 89;
            /// <summary>
            /// Z
            /// </summary>
            public const byte Z = 90;

            //数字键盘0到9

            /// <summary>
            ///  0
            /// </summary>
            public const byte NumberKey_0 = 48;
            /// <summary>
            /// 1
            /// </summary>
            public const byte NumberKey_1 = 49;
            /// <summary>
            /// 2
            /// </summary>
            public const byte NumberKey_2 = 50;
            /// <summary>
            /// 3
            /// </summary>
            public const byte NumberKey_3 = 51;
            /// <summary>
            /// 4
            /// </summary>
            public const byte NumberKey_4 = 52;
            /// <summary>
            /// 5
            /// </summary>
            public const byte NumberKey_5 = 53;
            /// <summary>
            /// 6
            /// </summary>
            public const byte NumberKey_6 = 54;
            /// <summary>
            /// 7
            /// </summary>
            public const byte NumberKey_7 = 55;
            /// <summary>
            /// 8
            /// </summary>
            public const byte NumberKey_8 = 56;
            /// <summary>
            /// 9
            /// </summary>
            public const byte NumberKey_9 = 57;

            /// <summary>
            /// 0
            /// </summary>
            public const byte Key_0 = 0x60;
            /// <summary>
            /// 1
            /// </summary>
            public const byte Key_1 = 0x61;
            /// <summary>
            /// 2
            /// </summary>
            public const byte Key_2 = 0x62;
            /// <summary>
            /// 3
            /// </summary>
            public const byte Key_3 = 0x63;
            /// <summary>
            /// 4
            /// </summary>
            public const byte Key_4 = 0x64;
            /// <summary>
            /// 5
            /// </summary>
            public const byte Key_5 = 0x65;
            /// <summary>
            /// 6
            /// </summary>
            public const byte Key_6 = 0x66;
            /// <summary>
            /// 7
            /// </summary>
            public const byte Key_7 = 0x67;
            /// <summary>
            /// 8
            /// </summary>
            public const byte Key_8 = 0x68;
            /// <summary>
            /// 9
            /// </summary>
            public const byte Key_9 = 0x69;
            /// <summary>
            /// MULTIPLICATIONSIGN(*)键
            /// </summary>
            public const byte Multiplication = 0x6A;
            /// <summary>
            /// PLUS SIGN(+) 键
            /// </summary>
            public const byte Add = 0x6B;
            /// <summary>
            /// ENTER 键
            /// </summary>
            public const byte Separator = 0x6C;
            /// <summary>
            ///  MINUS SIGN(-) 键
            /// </summary>
            public const byte Subtract = 0x6D;
            /// <summary>
            /// DECIMAL POINT(.) 键
            /// </summary>
            public const byte Point = 0x6E;
            /// <summary>
            ///  DIVISION SIGN(/) 键
            /// </summary>
            public const byte Divide = 0x6F;


            /// <summary>
            /// F1 键
            /// </summary>
            public const byte F1 = 0x70;
            /// <summary>
            /// F2 键
            /// </summary>
            public const byte F2 = 0x71;
            /// <summary>
            /// F3 键
            /// </summary>
            public const byte F3 = 0x72;
            /// <summary>
            /// F4 键
            /// </summary>
            public const byte F4 = 0x73;
            /// <summary>
            /// F5 键
            /// </summary>
            public const byte F5 = 0x74;
            /// <summary>
            /// F6 键
            /// </summary>
            public const byte F6 = 0x75;
            /// <summary>
            /// F7 键
            /// </summary>
            public const byte F7 = 0x76;
            /// <summary>
            /// F8 键
            /// </summary>
            public const byte F8 = 0x77;
            /// <summary>
            /// F9 键
            /// </summary>
            public const byte F9 = 0x78;
            /// <summary>
            /// F10 键
            /// </summary>
            public const byte F10 = 0x79;
            /// <summary>
            /// F11 键
            /// </summary>
            public const byte F11 = 0x7A;
            /// <summary>
            /// F12 键
            /// </summary>
            public const byte F12 = 0x7B;
        }

        /// <summary>
        /// Windows操作值
        /// </summary>
        public class WindowsCap
        {

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



            /// <summary>
            /// 显示窗体
            /// </summary>
            public const int WM_SHOWWINDOW = 0x40;
            /// <summary>
            /// 点击
            /// </summary>
            public const int BM_CLICK = 0xF5;
            /// <summary>
            /// 创建一个窗口
            /// </summary>
            public const int WM_CREATE = 0x01;
            /// <summary>
            /// 一个窗口被激活或失去激活状态
            /// </summary>
            public const int WM_ACTIVATE = 0x06;
            /// <summary>
            /// 一个窗口获得焦点
            /// </summary>
            public const int WM_SETFOCUS = 0x07;
            /// <summary>
            /// 一个窗口失去焦点
            /// </summary>
            public const int WM_KILLFOCUS = 0x08;
            /// <summary>
            /// 设置窗口是否能重画
            /// </summary>
            public const int WM_SETREDRAW = 0x0B;
            /// <summary>
            /// 要求一个窗口重画自己
            /// </summary>
            public const int WM_PAINT = 0x0F;
            /// <summary>
            /// 当用户选择结束对话框或程序自己调用ExitWindows函数
            /// </summary>
            public const int WM_QUERYENDSESSION = 0x11;
            /// <summary>
            /// 用来结束程序运行
            /// </summary>
            public const int WM_QUIT = 0x12;
            /// <summary>
            /// 当用户窗口恢复以前的大小位置时，把此消息发送给某个图标
            /// </summary>
            public const int WM_QUERYOPEN = 0x13;
            /// <summary>
            /// 当窗口背景必须被擦除时（例在窗口改变大小时）
            /// </summary>
            public const int WM_ERASEBKGND = 0x14;
            /// <summary>
            /// 当系统颜色改变时，发送此消息给所有顶级窗口
            /// </summary>
            public const int WM_SYSCOLORCHANGE = 0x15;
            /// <summary>
            /// 当系统进程发出WM_QUERYENDSESSION消息后，此消息发送给应用程序，通知它对话是否结束
            /// </summary>
            public const int WM_ENDSESSION = 0x16;
            /// <summary>
            /// 当隐藏或显示窗口是发送此消息给这个窗口
            /// </summary>
            public const int WM_SHOWWINDOWMESSAGE = 0x18;
            /// <summary>
            /// 发此消息给应用程序哪个窗口是激活的，哪个是非激活的
            /// </summary>
            public const int WM_ACTIVATEAPP = 0x1C;
            /// <summary>
            /// 当系统的字体资源库变化时发送此消息给所有顶级窗口
            /// </summary>
            public const int WM_FONTCHANGE = 0x1D;
            /// <summary>
            /// 当系统的时间变化时发送此消息给所有顶级窗口
            /// </summary>
            public const int WM_TIMECHANGE = 0x1E;
            /// <summary>
            /// 发送此消息来取消某种正在进行的摸态（操作）
            /// </summary>
            public const int WM_CANCELMODE = 0x1F;
            /// <summary>
            /// 如果鼠标引起光标在某个窗口中移动且鼠标输入没有被捕获时，就发消息给某个窗口
            /// </summary>
            public const int WM_SETCURSOR = 0x20;
            /// <summary>
            /// 当光标在某个非激活的窗口中而用户正按着鼠标的某个键发送此消息给//当前窗口
            /// </summary>
            public const int WM_MOUSEACTIVATE = 0x21;
            /// <summary>
            /// 发送此消息给MDI子窗口//当用户点击此窗口的标题栏，或//当窗口被激活，移动，改变大小
            /// </summary>
            public const int WM_CHILDACTIVATE = 0x22;
            /// <summary>
            /// 此消息由基于计算机的训练程序发送，通过WH_JOURNALPALYBACK的hook程序分离出用户输入消息
            /// </summary>
            public const int WM_QUEUESYNC = 0x23;
            /// <summary>
            /// 此消息发送给窗口当它将要改变大小或位置
            /// </summary>
            public const int WM_GETMINMAXINFO = 0x24;
            /// <summary>
            /// 发送给最小化窗口当它图标将要被重画
            /// </summary>
            public const int WM_PAINTICON = 0x26;
            /// <summary>
            /// 此消息发送给某个最小化窗口，仅//当它在画图标前它的背景必须被重画
            /// </summary>
            public const int WM_ICONERASEBKGND = 0x27;
            /// <summary>
            /// 发送此消息给一个对话框程序去更改焦点位置
            /// </summary>
            public const int WM_NEXTDLGCTL = 0x28;
            /// <summary>
            /// 每当打印管理列队增加或减少一条作业时发出此消息
            /// </summary>
            public const int WM_SPOOLERSTATUS = 0x2A;
            /// <summary>
            /// 当button，combobox，listbox，menu的可视外观改变时发送
            /// </summary>
            public const int WM_DRAWITEM = 0x2B;
            /// <summary>
            /// 当button, combo box, list box, list view control, or menu item 被创建时
            /// </summary>
            public const int WM_MEASUREITEM = 0x2C;
            ///此消息有一个LBS_WANTKEYBOARDINPUT风格的发出给它的所有者来响应WM_KEYDOWN消息 
            public const int WM_VKEYTOITEM = 0x2E;
            /// <summary>
            /// 此消息由一个LBS_WANTKEYBOARDINPUT风格的列表框发送给他的所有者来响应WM_CHAR消息
            /// </summary>
            public const int WM_CHARTOITEM = 0x2F;
            /// <summary>
            /// 当绘制文本时程序发送此消息得到控件要用的颜色
            /// </summary>
            public const int WM_SETFONT = 0x30;
            /// <summary>
            /// 应用程序发送此消息得到当前控件绘制文本的字体
            /// </summary>
            public const int WM_GETFONT = 0x31;
            ///应用程序发送此消息让一个窗口与一个热键相关连 
            public const int WM_SETHOTKEY = 0x32;
            /// <summary>
            /// 应用程序发送此消息来判断热键与某个窗口是否有关联
            /// </summary>
            public const int WM_GETHOTKEY = 0x33;
            /// <summary>
            /// 此消息发送给最小化窗口，当此窗口将要被拖放而它的类中没有定义图标，应用程序能返回一个图标或光标的句柄，当用户拖放图标时系统显示这个图标或光标
            /// </summary>
            public const int WM_QUERYDRAGICON = 0x37;
            /// <summary>
            /// 发送此消息来判定combobox或listbox新增加的项的相对位置
            /// </summary>
            public const int WM_COMPAREITEM = 0x39;
            /// <summary>
            /// 显示内存已经很少了
            /// </summary>
            public const int WM_COMPACTING = 0x41;
            /// <summary>
            /// 发送此消息给那个窗口的大小和位置将要被改变时，来调用setwindowpos函数或其它窗口管理函数
            /// </summary>
            public const int WM_WINDOWPOSCHANGING = 0x46;
            /// <summary>
            /// 发送此消息给那个窗口的大小和位置已经被改变时，来调用setwindowpos函数或其它窗口管理函数
            /// </summary>
            public const int WM_WINDOWPOSCHANGED = 0x47;
            /// <summary>
            /// 当系统将要进入暂停状态时发送此消息
            /// </summary>
            public const int WM_POWER = 0x48;
            /// <summary>
            /// 当一个应用程序传递数据给另一个应用程序时发送此消息
            /// </summary>
            public const int WM_COPYDATA = 0x4A;
            /// <summary>
            /// 当某个用户取消程序日志激活状态，提交此消息给程序
            /// </summary>
            public const int WM_CANCELJOURNA = 0x4B;
            /// <summary>
            /// 当用户选择某种输入语言，或输入语言的热键改变
            /// </summary>
            public const int WM_INPUTLANGCHANGEREQUEST = 0x50;
            /// <summary>
            /// 当平台现场已经被改变后发送此消息给受影响的最顶级窗口
            /// </summary>
            public const int WM_INPUTLANGCHANGE = 0x51;
            /// <summary>
            /// 当程序已经初始化windows帮助例程时发送此消息给应用程序
            /// </summary>
            public const int WM_TCARD = 0x52;
            /// <summary>
            /// 此消息显示用户按下了F1，如果某个菜单是激活的，就发送此消息个此窗口关联的菜单，否则就发送给有焦点的窗口，如果//当前都没有焦点，就把此消息发送给//当前激活的窗口
            /// </summary>
            public const int WM_HELP = 0x53;
            /// <summary>
            /// 当用户已经登入或退出后发送此消息给所有的窗口，//当用户登入或退出时系统更新用户的具体设置信息，在用户更新设置时系统马上发送此消息
            /// </summary>
            public const int WM_USERCHANGED = 0x54;
            /// <summary>
            /// 公用控件，自定义控件和他们的父窗口通过此消息来判断控件是使用ANSI还是UNICODE结构
            /// </summary>
            public const int WM_NOTIFYFORMAT = 0x55;
            /// <summary>
            /// 当调用SETWINDOWLONG函数将要改变一个或多个 窗口的风格时发送此消息给那个窗口
            /// </summary>
            public const int WM_STYLECHANGING = 0x7C;
            /// <summary>
            /// 当调用SETWINDOWLONG函数一个或多个 窗口的风格后发送此消息给那个窗口s
            /// </summary>
            public const int WM_STYLECHANGED = 0x7D;
            /// <summary>
            /// 当显示器的分辨率改变后发送此消息给所有的窗口
            /// </summary>
            public const int WM_DISPLAYCHANGE = 0x7E;
            /// <summary>
            /// 此消息发送给某个窗口来返回与某个窗口有关连的大图标或小图标的句柄
            /// 
            /// </summary>
            public const int WM_GETICON = 0x7F;
            /// <summary>
            /// 程序发送此消息让一个新的大图标或小图标与某个窗口关联
            /// </summary>
            public const int WM_SETICON = 0x80;
            /// <summary>
            /// 当某个窗口第一次被创建时，此消息在WM_CREATE消息发送前发送
            /// </summary>
            public const int WM_NCCREATE = 0x81;
            /// <summary>
            /// 此消息通知某个窗口，非客户区正在销毁 
            /// </summary>
            public const int WM_NCDESTROY = 0x82;
            /// <summary>
            /// 当某个窗口的客户区域必须被核算时发送此消息
            /// </summary>
            public const int WM_NCCALCSIZE = 0x83;
            /// <summary>
            /// 移动鼠标，按住或释放鼠标时发生
            /// </summary>
            public const int WM_NCHITTEST = 0x84;
            /// <summary>
            /// 程序发送此消息给某个窗口当它（窗口）的框架必须被绘制时
            /// </summary>
            public const int WM_NCPAINT = 0x85;
            /// <summary>
            /// 此消息发送给某个窗口仅当它的非客户区需要被改变来显示是激活还是非激活状态
            /// </summary>
            public const int WM_NCACTIVATE = 0x86;
            /// <summary>
            /// 发送此消息给某个与对话框程序关联的控件，widdows控制方位键和TAB键使输入进入此控件通过应
            /// </summary>
            public const int WM_GETDLGCODE = 0x87;
            /// <summary>
            /// 当光标在一个窗口的非客户区内移动时发送此消息给这个窗口 非客户区为：窗体的标题栏及窗 的边框体
            /// </summary>
            public const int WM_NCMOUSEMOVE = 0xA0;
            /// <summary>
            /// 当光标在一个窗口的非客户区同时按下鼠标左键时提交此消息
            /// </summary>
            public const int WM_NCLBUTTONDOWN = 0xA1;
            /// <summary>
            /// 当用户释放鼠标左键同时光标某个窗口在非客户区十发送此消息 
            /// </summary>
            public const int WM_NCLBUTTONUP = 0xA2;
            /// <summary>
            /// 当用户双击鼠标左键同时光标某个窗口在非客户区十发送此消息
            /// </summary>
            public const int WM_NCLBUTTONDBLCLK = 0xA3;
            /// <summary>
            /// 当用户按下鼠标右键同时光标又在窗口的非客户区时发送此消息
            /// </summary>
            public const int WM_NCRBUTTONDOWN = 0xA4;
            /// <summary>
            /// 当用户释放鼠标右键同时光标又在窗口的非客户区时发送此消息
            /// </summary>
            public const int WM_NCRBUTTONUP = 0xA5;
            /// <summary>
            /// 当用户双击鼠标右键同时光标某个窗口在非客户区十发送此消息
            /// </summary>
            public const int WM_NCRBUTTONDBLCLK = 0xA6;
            /// <summary>
            /// 当用户按下鼠标中键同时光标又在窗口的非客户区时发送此消息
            /// </summary>
            public const int WM_NCMBUTTONDOWN = 0xA7;
            /// <summary>
            /// 当用户释放鼠标中键同时光标又在窗口的非客户区时发送此消息
            /// </summary>
            public const int WM_NCMBUTTONUP = 0xA8;
            /// <summary>
            /// 当用户双击鼠标中键同时光标又在窗口的非客户区时发送此消息
            /// </summary>
            public const int WM_NCMBUTTONDBLCLK = 0xA9;
            /// <summary>
            /// 当用translatemessage函数翻译WM_KEYUP消息时发送此消息给拥有焦点的窗口
            /// </summary>
            public const int WM_DEADCHAR = 0x103;
            /// <summary>
            /// 当用户按住ALT键同时按下其它键时提交此消息给拥有焦点的窗口
            /// </summary>
            public const int WM_SYSKEYDOWN = 0x104;
            /// <summary>
            /// 当用户释放一个键同时ALT 键还按着时提交此消息给拥有焦点的窗口
            /// </summary>
            public const int WM_SYSKEYUP = 0x105;
            /// <summary>
            /// 当WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后提交此消息给拥有焦点的窗口
            /// </summary>
            public const int WM_SYSCHAR = 0x106;
            /// <summary>
            /// 当WM_SYSKEYDOWN消息被TRANSLATEMESSAGE函数翻译后发送此消息给拥有焦点的窗口
            /// </summary>
            public const int WM_SYSDEADCHAR = 0x107;
            /// <summary>
            /// 在一个对话框程序被显示前发送此消息给它，通常用此消息初始化控件和执行其它任务
            /// </summary>
            public const int WM_INITDIALOG = 0x110;
            /// <summary>
            /// 当用户选择系统菜单命令或点击最大/最小化/关闭按钮时收到此消息
            /// </summary>
            public const int WM_SYSCOMMAND = 0x112;
            /// <summary>
            /// 发生了定时器事件
            /// </summary>
            public const int WM_TIMER = 0x113;
            /// <summary>
            /// 当一个窗口标准水平滚动条产生一个滚动事件时发送此消息给那个窗口，也发送给拥有它的控件
            /// </summary>
            public const int WM_HSCROLL = 0x114;
            /// <summary>
            /// 当一个窗口标准垂直滚动条产生一个滚动事件时发送此消息给那个窗口也，发送给拥有它的控件
            /// </summary>
            public const int WM_VSCROLL = 0x115;
            /// <summary>
            /// 当一个菜单将要被激活时发送此消息，它发生在用户菜单条中的某项或按下某个菜单键，它允许程序在显示前更改菜单
            /// </summary>
            public const int WM_INITMENU = 0x116;
            /// <summary>
            /// 当一个下拉菜单或子菜单将要被激活时发送此消息，它允许程序在它显示前更改菜单，而不要改变全部
            /// </summary>
            public const int WM_INITMENUPOPUP = 0x117;
            /// <summary>
            /// 当用户选择一条菜单项时发送此消息给菜单的所有者（一般是窗口）
            /// </summary>
            public const int WM_MENUSELECT = 0x11F;
            /// <summary>
            /// 当菜单已被激活用户按下了某个键（不同于加速键），发送此消息给菜单的所有者
            /// </summary>
            public const int WM_MENUCHAR = 0x120;
            /// <summary>
            /// 当一个模态对话框或菜单进入空载状态时发送此消息给它的所有者，一个模态对话框或菜单进入空载状态就是在处理完一条或几条先前的消息后没有消息它的列队中等待
            /// </summary>
            public const int WM_ENTERIDLE = 0x121;
            /// <summary>
            /// 在windows绘制消息框前发送此消息给消息框的所有者窗口，通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置消息框的文本和背景颜色
            /// </summary>
            public const int WM_CTLCOLORMSGBOX = 0x132;
            /// <summary>
            /// 当一个编辑型控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置编辑框的文本和背景颜色
            /// </summary>
            public const int WM_CTLCOLOREDIT = 0x133;
            /// <summary>
            /// 当一个列表框控件将要被绘制前发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置列表框的文本和背景颜色
            /// </summary>
            public const int WM_CTLCOLORLISTBOX = 0x134;
            /// <summary>
            /// 当一个按钮控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置按纽的文本和背景颜色
            /// </summary>
            public const int WM_CTLCOLORBTN = 0x135;
            /// <summary>
            /// ./当一个对话框控件将要被绘制前发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置对话框的文本背景颜色
            /// </summary>
            public const int WM_CTLCOLORDLG = 0x136;
            /// <summary>
            /// 当一个滚动条控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以通过使用给定的相关显示设备的句柄来设置滚动条的背景颜色
            /// </summary>
            public const int WM_CTLCOLORSCROLLBAR = 0x137;
            /// <summary>
            /// 当一个静态控件将要被绘制时发送此消息给它的父窗口通过响应这条消息，所有者窗口可以 通过使用给定的相关显示设备的句柄来设置静态控件的文本和背景颜色
            /// </summary>
            public const int WM_CTLCOLORSTATIC = 0x138;
        }

        /// <summary>
        /// 设备操作值
        /// </summary>
        public class DeviceCap
        {

            /// <summary>系统命令：控制显示器电源</summary>
            public const int SC_MONITORPOWER = 0xF170;
            /// <summary>
            /// 设备驱动版本
            /// </summary>
            public const int DRIVVERVERSION = 0;
            /// <summary>
            /// 设备分类
            /// </summary>
            public const int TECHNOLOGY = 2;
            /// <summary>
            /// 以毫米为单位的水平尺寸
            /// </summary>
            public const int HORZSIZE = 4;
            /// <summary>
            /// 以毫米为单位的垂直尺寸
            /// </summary>
            public const int VERTSIZE = 6;
            /// <summary>
            /// 以像素为单位的水平宽度
            /// </summary>
            public const int LEVEL = 8;
            /// <summary>
            /// 以像素为单位的垂直高度
            /// </summary>
            public const int VERTRES = 10;
            /// <summary>
            /// 每个像素的位数
            /// </summary>
            public const int BITSPIXEL = 12;
            /// <summary>
            /// 平面数
            /// </summary>
            public const int PLANES = 14;
            /// <summary>
            /// 设备拥有的画笔数
            /// </summary>
            public const int NUMBRUSHES = 16;
            /// <summary>
            /// 设备拥有的笔数
            /// </summary>
            public const int NUMPENS = 18;
            /// <summary>
            /// 设备拥有的标记数量 | 设置桌面背景图
            /// </summary>
            public const int NUMMARKERS = 20;
            /// <summary>
            /// 设备拥有的字体数量
            /// </summary>
            public const int NUMFONTS = 22;
            /// <summary>
            /// 设备支持的颜色数
            /// </summary>
            public const int NUMCOLORS = 24;
            /// <summary>
            /// 设备描述符所需的大​​小
            /// </summary>
            public const int PDEVICESIZE = 26;
            /// <summary>
            /// 曲线能力
            /// </summary>
            public const int CURVECAPS = 28;
            /// <summary>
            /// 线能力
            /// </summary>
            public const int LINECAPS = 30;
            /// <summary>
            /// 多边形能力
            /// </summary>
            public const int POLYGONALCAPS = 32;
            /// <summary>
            /// 文本能力
            /// </summary>
            public const int TEXTCAPS = 34;
            /// <summary>
            /// 剪辑能力
            /// </summary>
            public const int CLIPCAPS = 36;
            /// <summary>
            /// Bitblt 能力
            /// </summary>
            public const int RASTERCAPS = 38;
            /// <summary>
            /// X 腿的长度
            /// </summary>
            public const int ASPECTX = 40;
            /// <summary>
            /// Y 腿的长度
            /// </summary>
            public const int ASPECTY = 42;
            /// <summary>
            /// 斜边的长度
            /// </summary>
            public const int ASPECTXY = 44;
            /// <summary>
            /// 着色和混合帽
            /// </summary>
            public const int SHADEBLENDCAPS = 45;
            /// <summary>
            /// X 中的逻辑像素英寸
            /// </summary>
            public const int LOGPIXELSX = 88;
            /// <summary>
            /// Y 轴上的逻辑像素英寸
            /// </summary>
            public const int LOGPIXELSY = 90;
            /// <summary>
            /// 物理调色板中的条目数
            /// </summary>
            public const int SIZEPALETTE = 104;
            /// <summary>
            /// 调色板中保留的条目数
            /// </summary>
            public const int NUMRESERVED = 106;
            /// <summary>
            /// 实际颜色分辨率
            /// </summary>
            public const int COLORRES = 108;
            // 打印相关的DeviceCaps。这些替换了适当的转义
            /// <summary>
            /// 设备单位的物理宽度
            /// </summary>
            public const int PHYSICALWIDTH = 110;
            /// <summary>
            /// 
            /// </summary>
            public const int PHYSICALHEIGHT = 111;
            /// <summary>
            /// 物理可打印区域 x 边距
            /// </summary>
            public const int PHYSICALOFFSETX = 112;
            /// <summary>
            /// 物理可打印区域 y 边距
            /// </summary>
            public const int PHYSICALOFFSETY = 113;
            /// <summary>
            /// 缩放因子 x
            /// </summary>
            public const int SCALINGFACTORX = 114;
            /// <summary>
            /// 缩放因子 y
            /// </summary>
            public const int SCALINGFACTORY = 115;
            /// <summary>
            /// 显示设备的当前垂直刷新率（仅适用于显示器），以 Hz 为单位
            /// </summary>
            public const int VREFRESH = 116;
            /// <summary>
            /// 整个桌面的垂直高度（以像素为单位）
            /// </summary>
            public const int DESKTOPVERTRES = 117;
            /// <summary>
            /// 的水平宽度整个桌面以像素为单位
            /// </summary>
            public const int DESKTOPHORZRES = 118;
            /// <summary>
            /// 首选 blt 对齐方式
            /// </summary>
            public const int BLTALIGNMENT = 119;
        }


        /// <summary>
        /// 控制台相关标志
        /// </summary>
        public class ConsoleCap
        {
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
            /// 标准错误输出（Standard Error）的设备句柄标识符
            /// </summary>
            public const int STD_ERROR_HANDLE = -12;
            /// <summary>
            /// 启用虚拟终端处理的控制台模式标志
            /// <para>允许控制台解析ANSI转义序列(如颜色代码)</para>
            /// </summary>
            public const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        }


        /// <summary>
        /// 内存管理相关标志
        /// </summary>
        public class MemoryCap

        {

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
            /// <summary>
            /// 对文件映射视图的完全访问权限
            /// </summary>
            public const uint FILE_MAP_ALL_ACCESS = 0xF001F;
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
            // ==================== 进程权限常量 ====================
            /// <summary>允许设置进程优先级</summary>
            public const uint PROCESS_SET_INFORMATION = 0x0200;

            /// <summary>允许查询进程信息</summary>
            public const uint PROCESS_QUERY_INFORMATION = 0x0400;

            // ==================== 优先级常量 ====================
            /// <summary>空闲优先级（系统空闲时运行）</summary>
            public const uint PRIORITY_IDLE = 0x00000040;

            /// <summary>正常优先级（默认）</summary>
            public const uint PRIORITY_NORMAL = 0x00000020;

            /// <summary>高优先级（可能影响系统响应）</summary>
            public const uint PRIORITY_HIGH = 0x00000080;

            /// <summary>实时优先级（慎用，可能导致系统冻结）</summary>
            public const uint PRIORITY_REALTIME = 0x00000100;

            /// <summary>低于正常优先级</summary>
            public const uint PRIORITY_BELOW_NORMAL = 0x00004000;

            /// <summary>高于正常优先级</summary>
            public const uint PRIORITY_ABOVE_NORMAL = 0x00008000;
            // ==================== 进程状态常量 ====================
            /// <summary>进程仍在运行的退出码</summary>
            public const uint STILL_ACTIVE = 259;

        }



        #region 



        #endregion
    }
}
