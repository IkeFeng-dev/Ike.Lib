using System.Runtime.InteropServices;
using System;

namespace Ike.Standard
{
    public partial class MouseHook
    {
        /// <summary>
        /// 钩子结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            /// <summary>
            /// 鼠标位置
            /// </summary>
            public POINT Point;
            /// <summary>
            /// 滚轮增量或 X 按钮标识,[7864320|0x00780000|120	向上滚动,4287102976|0xFF800000|-128	向下滚动]
            /// </summary>
            public uint MouseData;
            /// <summary>
            /// 事件标志
            /// </summary>
            public uint Flags;
            /// <summary>
            /// 时间戳
            /// </summary>
            public uint Time;
            /// <summary>
            /// 与消息关联的额外值
            /// </summary>
            public IntPtr DwExtraInfo;
        }
    }
}
