using System.Runtime.InteropServices;
using System;

namespace Ike.Standard
{
    public partial class WinAPI
    {
        /// <summary>
        /// 系统信息结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            /// <summary>处理器架构</summary>
            public ushort wProcessorArchitecture;

            /// <summary>保留字段</summary>
            public ushort wReserved;

            /// <summary>页面大小</summary>
            public uint dwPageSize;

            /// <summary>最小应用程序地址</summary>
            public IntPtr lpMinimumApplicationAddress;

            /// <summary>最大应用程序地址</summary>
            public IntPtr lpMaximumApplicationAddress;

            /// <summary>处理器掩码</summary>
            public UIntPtr dwActiveProcessorMask;

            /// <summary>处理器数量</summary>
            public uint dwNumberOfProcessors;

            /// <summary>处理器类型（已弃用）</summary>
            public uint dwProcessorType;

            /// <summary>分配粒度</summary>
            public uint dwAllocationGranularity;

            /// <summary>处理器级别</summary>
            public ushort wProcessorLevel;

            /// <summary>处理器版本</summary>
            public ushort wProcessorRevision;
        }

    }
}
