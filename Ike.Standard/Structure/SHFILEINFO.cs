using System.Runtime.InteropServices;
using System;

namespace Ike.Standard
{
    public partial class WinAPI
    {
        /// <summary>
        /// 文件信息结构体，用于获取图标和文件类型信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            /// <summary>图标句柄（hIcon）</summary>
            public IntPtr hIcon;

            /// <summary>系统映像列表中的图标索引（iIcon）</summary>
            public int iIcon;

            /// <summary>文件属性标志（dwAttributes）</summary>
            public uint dwAttributes;

            /// <summary>文件的显示名称（szDisplayName）</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            /// <summary>文件类型描述（szTypeName）</summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

    }
}
