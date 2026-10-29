#if NET5_0_OR_GREATER

using System.Collections.Generic;

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 目录List
    /// </summary>
    public class DirectoryListDto
    {
        /// <summary>
        /// 当前路径
        /// </summary>
        public string CurrentPath { get; set; } = string.Empty;
        /// <summary>
        /// 文件集合
        /// </summary>
        public List<FileSystemItemDto> Items { get; set; } = [];
    }
}

#endif