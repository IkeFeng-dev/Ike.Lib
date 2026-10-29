#if NET5_0_OR_GREATER
#nullable enable 

using System;

namespace Ike.Standard.Controller
{
    /// <summary>
    /// API请求结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ApiResult<T>
    {
        /// <summary>
        /// 成功时的返回数据，失败时通常为 null
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 失败时的错误描述信息，成功时为 null
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// 服务端执行耗时，单位：毫秒
        /// </summary>
        public long ElapsedMs { get; set; }

        /// <summary>
        /// 响应生成时间，UTC 时间
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
    }
}

#endif