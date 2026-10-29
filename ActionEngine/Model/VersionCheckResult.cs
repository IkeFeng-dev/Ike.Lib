using System;

namespace Ike.ActionEngine.Model
{
    /// <summary>
    /// 客户端版本检测结果数据模型
    /// </summary>
    public class VersionCheckResult
    {
        /// <summary>
        /// 是否存在比当前客户端更高的新版本
        /// </summary>
        public bool HasNewVersion { get; set; }

        /// <summary>
        /// 服务器上最新版本的版本号（例："1.0.0.8"）
        /// </summary>
        public string LatestVersion { get; set; }

        /// <summary>
        /// 程序名称（例："Test.exe"）
        /// </summary>
        public string ProgramName { get; set; }

        /// <summary>
        /// 最新版本程序文件的大小（单位：字节 Byte）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 可读的文件大小展示字符串（例："1.5 MB" 或 "512.0 KB"）
        /// </summary>
        public string FileSizeDisplay { get; set; }

        /// <summary>
        /// 最新版本的更新日志 / 说明信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 最新版本的上传发布时间
        /// </summary>
        public DateTime UploadTime { get; set; }
    }
}
