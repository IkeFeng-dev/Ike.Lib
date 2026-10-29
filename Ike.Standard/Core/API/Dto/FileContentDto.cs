#if NET5_0_OR_GREATER

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 文件内容
    /// </summary>
    public class FileContentDto
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 文件内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}

#endif