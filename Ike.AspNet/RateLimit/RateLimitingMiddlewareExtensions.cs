using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Ike.AspNet.RateLimit
{
    /// <summary>
    /// 速率限制中间件扩展类，用于将速率限制中间件添加到应用程序构建器中
    /// </summary>
    public static class RateLimitingMiddlewareExtensions
    {
        /// <summary>
        /// 使用默认的时间窗口和最大请求数来配置速率限制中间件
        /// </summary>
        /// <param name="builder">应用程序构建器</param>
        /// <param name="apiPassword">API接口访问密码</param>
        /// <param name="timeWindowInSeconds">时间窗口（秒）</param>
        /// <param name="maxRequests">最大请求数</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder,string apiPassword, uint timeWindowInSeconds, uint maxRequests)
        {
            Flag.Password = apiPassword;
            // 获取服务提供者
            var services = builder.ApplicationServices;
            // 获取速率限制配置服务实例
            var configService = services.GetRequiredService<IRateLimitConfigService>();
            // 配置默认的路径限制
            var pathsToLimit = new Dictionary<string, PathConfig>
            {
                { "*", new PathConfig { Name = "*", Path = "/*", TimeWindowInSeconds = (int)timeWindowInSeconds, MaxRequests = (int)maxRequests } }
            };
            // 更新路径限制配置
            configService.UpdatePaths(pathsToLimit);
            // 使用速率限制中间件
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }

        /// <summary>
        /// 使用指定的路径配置来配置速率限制中间件
        /// </summary>
        /// <param name="builder">应用程序构建器</param>
        /// <param name="apiPassword">API接口访问密码</param>
        /// <param name="pathsToLimit">需要限制的路径配置</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder, string apiPassword, List<PathConfig> pathsToLimit)
        {
            Flag.Password = apiPassword;
            // 获取服务提供者
            var services = builder.ApplicationServices;
            // 获取速率限制配置服务实例
            var configService = services.GetRequiredService<IRateLimitConfigService>();
            // 如果路径配置不为空，更新路径限制配置
            if (pathsToLimit != null)
            {
                IDictionary<string,PathConfig> keyValuePairs = new Dictionary<string, PathConfig>();
                foreach (var path in pathsToLimit) 
                {
                    keyValuePairs.TryAdd(path.Name,path);
				}
                configService.UpdatePaths(keyValuePairs);
            }
            // 使用速率限制中间件
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }

        /// <summary>
        /// 使用指定的JSON配置文件来配置速率限制中间件
        /// </summary>
        /// <param name="builder">应用程序构建器</param>
        /// <param name="apiPassword">API接口访问密码</param>
        /// <param name="jsonFilePath">JSON配置文件路径</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder, string apiPassword, string jsonFilePath)
        {
            Flag.Password = apiPassword;
            // 获取服务提供者
            var services = builder.ApplicationServices;
            // 获取速率限制配置服务实例
            var configService = services.GetRequiredService<IRateLimitConfigService>();

            // 检查配置文件是否存在
            if (!File.Exists(jsonFilePath))
            {
                throw new FileNotFoundException($"指定的配置文件未找到: {jsonFilePath}");
            }

            // 读取JSON配置文件内容
            var jsonContent = File.ReadAllText(jsonFilePath);
            // 反序列化JSON内容为路径配置字典
            var pathsToLimit = JsonSerializer.Deserialize<Dictionary<string, PathConfig>>(jsonContent);

            // 如果路径配置不为空，更新路径限制配置
            if (pathsToLimit != null)
            {
                configService.UpdatePaths(pathsToLimit);
            }

            // 使用速率限制中间件
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}
