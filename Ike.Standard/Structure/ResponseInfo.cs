using System;

namespace Ike.Standard
{
    public partial class Ftp
    {
        /// <summary>
        /// 资源信息结构
        /// </summary>
        public struct ResponseInfo
        {
            /// <summary>
            /// 是否文件
            /// </summary>
            public bool IsFile;
            /// <summary>
            /// 权限
            /// </summary>
            public string Limits;
            /// <summary>
            /// 大小
            /// </summary>
            public long Size;
            /// <summary>
            /// 修改时间
            /// </summary>
            public DateTime Modification;
            /// <summary>
            /// 名称
            /// </summary>
            public string Name;
        }

    }
}
