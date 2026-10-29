#if NET5_0_OR_GREATER

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 创建目录
    /// </summary>
    public class CreateFolderDto
    {
        /// <summary>
        /// 目录路径
        /// </summary>
        public string Path { get; set; } = string.Empty;
    }
}

#endif