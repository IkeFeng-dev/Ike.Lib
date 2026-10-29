#if NET5_0_OR_GREATER

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 复制文件
    /// </summary>
    public class CopyFileDto
    {
        /// <summary>
        /// 原文件路径
        /// </summary>
        public string SourcePath { get; set; } = string.Empty;
        /// <summary>
        /// 复制的目标路径
        /// </summary>
        public string TargetDirectory { get; set; } = string.Empty;
    }
}

#endif