using System;

namespace Ike.Updater.Model
{
    /// <summary>
    /// 下载进度参数
    /// </summary>
    public class DownloadProgressArgs : EventArgs
    {
        /// <summary>已下载字节数</summary>
        public long BytesReceived { get; set; }

        /// <summary>文件总字节数</summary>
        public long TotalBytes { get; set; }

        /// <summary>百分比 (0 - 100)</summary>
        public double ProgressPercentage { get; set; }
    }
}
