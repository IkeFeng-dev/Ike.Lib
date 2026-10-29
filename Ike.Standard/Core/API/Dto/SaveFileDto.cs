#if NET5_0_OR_GREATER
#nullable enable 

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 保存文件内容
    /// </summary>
    public class SaveFileDto
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Path { get; set; } = string.Empty;
        /// <summary>
        /// 文件内容
        /// </summary>
        public string? Content { get; set; }
    }

}

#endif