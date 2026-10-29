#if NET5_0_OR_GREATER

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class FileSystemItemDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelativePath { get; set; } = string.Empty;
        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool IsDirectory { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public string LastModified { get; set; } = string.Empty;
    }
}

#endif