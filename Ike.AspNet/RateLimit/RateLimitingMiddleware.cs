using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace Ike.AspNet.RateLimit
{
    /// <summary>
    /// 速率限制中间件，用于限制每个IP地址的API调用次数
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRateLimitConfigService _configService;
        private static readonly ConcurrentDictionary<string, RateLimitInfo> _clients = new();

        /// <summary>
        /// 初始化速率限制中间件实例
        /// </summary>
        /// <param name="next">下一个中间件委托</param>
        /// <param name="configService">速率限制配置服务实例</param>
        public RateLimitingMiddleware(RequestDelegate next, IRateLimitConfigService configService)
        {
            _next = next;
            _configService = configService;
        }

        /// <summary>
        /// 中间件的主要处理逻辑
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>任务实例</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // 获取请求路径和IP地址
            var path = context.Request.Path.ToString().ToLower();
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // 如果IP地址为空，返回400错误
            if (ipAddress == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("无法确定IP地址");
                return;
            }

            // 获取需要限制的路径配置
            var pathsToLimit = _configService.GetPathsToLimit();
            var matchedConfig = pathsToLimit.Values.FirstOrDefault(p => MatchesPath(path, p.Path));

            if (matchedConfig != null)
            {
                var (timeWindowInSeconds, maxRequests) = (matchedConfig.TimeWindowInSeconds, matchedConfig.MaxRequests);
                var clientKey = $"{ipAddress}:{matchedConfig.Path}";

                // 尝试从字典中获取该IP和路径的调用信息，如果没有则创建新的
                if (!_clients.TryGetValue(clientKey, out var client))
                {
                    client = new RateLimitInfo { LastRequestTime = DateTime.UtcNow, RequestCount = 0 };
                    _clients[clientKey] = client;
                }

                // 锁定该IP的调用信息，防止并发修改
                lock (client)
                {
                    var now = DateTime.UtcNow;
                    // 如果超过了时间周期，则重置请求计数
                    if (now - client.LastRequestTime > TimeSpan.FromSeconds(timeWindowInSeconds))
                    {
                        client.RequestCount = 0;
                        client.LastRequestTime = now;
                    }

                    // 增加请求计数
                    client.RequestCount++;
                    // 如果请求次数超过限制，返回429状态码
                    if (client.RequestCount > maxRequests)
                    {
                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        return;
                    }
                }
            }

            // 调用下一个中间件
            await _next(context);
        }

        /// <summary>
        /// 检查请求路径是否匹配指定的模式
        /// </summary>
        /// <param name="requestPath">请求路径</param>
        /// <param name="pattern">匹配模式</param>
        /// <returns>如果匹配，则返回true；否则返回false</returns>
        private static bool MatchesPath(string requestPath, string pattern)
        {
            if (string.IsNullOrEmpty(requestPath) || string.IsNullOrEmpty(pattern))
            {
                return false;
            }

            if (pattern.EndsWith("*"))
            {
                return requestPath.StartsWith(pattern.TrimEnd('*'), StringComparison.OrdinalIgnoreCase);
            }
            return string.Equals(requestPath, pattern, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 用于存储IP调用信息的内部类
        /// </summary>
        private class RateLimitInfo
        {
            /// <summary>
            /// 上次请求的时间
            /// </summary>
            public DateTime LastRequestTime { get; set; }

            /// <summary>
            /// 当前时间周期内的请求计数
            /// </summary>
            public int RequestCount { get; set; }
        }
    }
}
