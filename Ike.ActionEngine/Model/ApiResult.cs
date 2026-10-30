using System;

namespace Ike.Updater.Model
{
    /// <summary>
    /// 服务端统一响应结构
    /// </summary>
    internal class ApiResult<T>
    {
        /// <summary>
        /// API请求数据
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// API错误内容
        /// </summary>
        public string Error { get; set; }
        /// <summary>
        /// API请求耗时
        /// </summary>
        public long ElapsedMs { get; set; }
        /// <summary>
        /// API请求时间戳
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
    }
}
