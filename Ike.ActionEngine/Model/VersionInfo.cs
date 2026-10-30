using System;

namespace Ike.Updater.Model
{
    /// <summary>
    /// 程序版本详细信息数据模型
    /// </summary>
    public class VersionInfo
    {
        /// <summary>
        /// 数据库版本记录主键 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 程序名称（例："Test.exe"）
        /// </summary>
        public string ProgramName { get; set; }

        /// <summary>
        /// 版本号（例："1.0.0.8"）
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 上传时的原始文件名（例："Test_Setup_v1.0.0.8.exe"）
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        /// 程序文件大小（单位：字节 Byte）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 可读的文件大小展示字符串（例："1.5 MB" 或 "512.0 KB"）
        /// </summary>
        public string FileSizeDisplay { get; set; }

        /// <summary>
        /// 本版本的更新说明 / 日志
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 文件上传时间
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// 是否启用该版本（true: 正常启用; false: 已停用，停用后客户端将检测/下载不到此版本）
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 该版本的历史累计下载次数
        /// </summary>
        public int DownloadCount { get; set; }
    }
}
