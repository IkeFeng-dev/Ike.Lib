#if NET5_0_OR_GREATER

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 重命名文件
    /// </summary>
    public class RenameFileDto
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 新名称
        /// </summary>
        public string NewName { get; set; } = string.Empty;
    }
}

#endif