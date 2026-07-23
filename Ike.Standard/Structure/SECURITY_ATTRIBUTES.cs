using System.Runtime.InteropServices;
using System;

namespace Ike.Standard
{
    public partial class WinAPI
    {
        /// <summary>
		/// Windows API 中的一个结构体，用于指定对象的安全描述符和继承属性。它在创建可共享或可继承的 Windows 对象（如文件、进程、线程、互斥体等）时使用
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            /// <summary>
            /// 结构体大小（字节数）
            /// </summary>
            public int nLength;
            /// <summary>
            /// 安全描述符指针
            /// </summary>
            public IntPtr lpSecurityDescriptor;
            /// <summary>
            /// 是否可被继承
            /// </summary>
            public bool bInheritHandle;
        }
    }
}
