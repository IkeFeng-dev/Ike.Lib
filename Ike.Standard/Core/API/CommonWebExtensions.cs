#if NET5_0_OR_GREATER


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ike.Standard.Controller
{
    /// <summary>
    /// Web API 通用功能与常用设置配置扩展类
    /// </summary>
    public static class CommonWebExtensions
    {
        /// <summary>
        /// 类库默认使用的跨域策略名称
        /// </summary>
        public const string DefaultCorsPolicy = "DefaultCorsPolicy";

        /// <summary>
        /// 配置【全开放】跨域策略（允许任何域名、请求头和 HTTP 方法，适合开发调试或内部 API）
        /// </summary>
        /// <param name="services">依赖注入服务集合 <see cref="IServiceCollection"/></param>
        /// <param name="policyName">自定义策略名称，默认使用 <see cref="DefaultCorsPolicy"/></param>
        /// <returns>返回 <see cref="IServiceCollection"/> 实例，支持链式调用</returns>
        public static IServiceCollection AddAllowAllCorsFast(this IServiceCollection services, string policyName = DefaultCorsPolicy)
        {
            return services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        /// <summary>
        /// 配置【指定域名】跨域策略（适合生产环境安全管控，只允许特定的前端域名访问）
        /// </summary>
        /// <param name="services">依赖注入服务集合 <see cref="IServiceCollection"/></param>
        /// <param name="origins">允许访问的域名列表（例如：<c>new[] { "https://example.com", "http://localhost:8080" }</c>）</param>
        /// <param name="policyName">自定义策略名称，默认使用 <see cref="DefaultCorsPolicy"/></param>
        /// <param name="allowCredentials">是否允许携带 Cookie/凭据，默认允许</param>
        /// <returns>返回 <see cref="IServiceCollection"/> 实例，支持链式调用</returns>
        public static IServiceCollection AddCorsForOriginsFast(this IServiceCollection services, string[] origins, string policyName = DefaultCorsPolicy, bool allowCredentials = true)
        {
            if (origins == null || origins.Length == 0)
            {
                throw new ArgumentNullException(nameof(origins), "指定的跨域域名列表 origins 不能为空");
            }

            return services.AddCors(options =>
            {
                options.AddPolicy(policyName, builder =>
                {
                    var policyBuilder = builder.WithOrigins(origins)
                                               .AllowAnyMethod()
                                               .AllowAnyHeader();

                    if (allowCredentials)
                    {
                        policyBuilder.AllowCredentials();
                    }
                });
            });
        }

        /// <summary>
        /// 一键开启默认跨域策略（等同于调用 <see cref="AddAllowAllCorsFast"/>，最简调用方式）
        /// </summary>
        /// <param name="services">依赖注入服务集合 <see cref="IServiceCollection"/></param>
        /// <param name="policyName">自定义策略名称，默认使用 <see cref="DefaultCorsPolicy"/></param>
        /// <returns>返回 <see cref="IServiceCollection"/> 实例，支持链式调用</returns>
        public static IServiceCollection AddEasyCorsFast(this IServiceCollection services, string policyName = DefaultCorsPolicy)
        {
            return services.AddAllowAllCorsFast(policyName);
        }


        /// <summary>
        /// 一键启用跨域中间件（需在 <c>app.UseRouting()</c> 之后，<c>app.UseAuthorization()</c> 之前调用）
        /// </summary>
        /// <param name="app">应用构建器 <see cref="IApplicationBuilder"/></param>
        /// <param name="policyName">使用的策略名称，必须与注册时的策略名称完全一致</param>
        /// <returns>返回 <see cref="IApplicationBuilder"/> 实例，支持链式调用</returns>
        public static IApplicationBuilder UseEasyCorsFast(this IApplicationBuilder app, string policyName = DefaultCorsPolicy)
        {
            return app.UseCors(policyName);
        }



        /// <summary>
        /// 配置 Multipart Form 表单上传大小限制
        /// </summary>
        /// <param name="services">依赖注入服务集合 <see cref="IServiceCollection"/></param>
        /// <param name="megabytes">允许最大上传的限制大小（单位：MB，默认值：1024 MB）</param>
        /// <returns>返回 <see cref="IServiceCollection"/> 实例，支持链式调用</returns>
        public static IServiceCollection AddFormUploadLimitMBFast(this IServiceCollection services, int megabytes = 1024)
        {
            long maxBytes = (long)megabytes * 1024 * 1024;

            return services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = maxBytes;
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
        }

        /// <summary>
        /// 配置 Kestrel Web 服务器底层请求体大小限制（解决默认 > 30MB 被拦截问题）
        /// </summary>
        /// <param name="services">依赖注入服务集合 <see cref="IServiceCollection"/></param>
        /// <param name="megabytes">允许最大上传的限制大小（单位：MB，默认值：1024 MB）</param>
        /// <returns>返回 <see cref="IServiceCollection"/> 实例，支持链式调用</returns>
        public static IServiceCollection AddKestrelUploadLimitMBFast(this IServiceCollection services, int megabytes = 1024)
        {
            long maxBytes = (long)megabytes * 1024 * 1024;

            return services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = maxBytes;
            });
        }

        /// <summary>
        /// 配置 IIS 托管服务器底层请求体大小限制（适用于 Windows IIS 部署环境）
        /// </summary>
        /// <param name="services">依赖注入服务集合 <see cref="IServiceCollection"/></param>
        /// <param name="megabytes">允许最大上传的限制大小（单位：MB，默认值：1024 MB）</param>
        /// <returns>返回 <see cref="IServiceCollection"/> 实例，支持链式调用</returns>
        public static IServiceCollection AddIISUploadLimitMBFast(this IServiceCollection services, int megabytes = 1024)
        {
            long maxBytes = (long)megabytes * 1024 * 1024;

            return services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = maxBytes;
            });
        }

        /// <summary>
        /// 一键配置全局文件上传大小限制（同时配置 Form 表单、Kestrel 与 IIS 底层限制）
        /// </summary>
        /// <param name="services">依赖注入服务集合 <see cref="IServiceCollection"/></param>
        /// <param name="megabytes">允许最大上传的限制大小（单位：MB，默认值：1024 MB）</param>
        /// <returns>返回 <see cref="IServiceCollection"/> 实例，支持链式调用</returns>
        /// <remarks>
        /// 本方法内部会依次调用以下 3 个子方法：
        /// <list type="number">
        ///   <item><description><see cref="AddFormUploadLimitMBFast"/></description></item>
        ///   <item><description><see cref="AddKestrelUploadLimitMBFast"/></description></item>
        ///   <item><description><see cref="AddIISUploadLimitMBFast"/></description></item>
        /// </list>
        /// </remarks>
        public static IServiceCollection AddFileUploadLimitMBFast(this IServiceCollection services, int megabytes = 1024)
        {
            return services
                .AddFormUploadLimitMBFast(megabytes)
                .AddKestrelUploadLimitMBFast(megabytes)
                .AddIISUploadLimitMBFast(megabytes);
        }


        /// <summary>
        /// 映射单条 HTTP 重定向规则,传统网页重定向、老旧 URL 变更通知、跳外部链接
        /// </summary>
        /// <param name="endpoints">路由构建器</param>
        /// <param name="fromPath">源路径 (例如: "/test")</param>
        /// <param name="toPath">目标路径 (例如: "/test.html")</param>
        /// <param name="permanent">是否为 301 永久重定向 (默认 false，即 302 临时重定向)</param>
        public static IEndpointRouteBuilder MapEasyRedirectFast(this IEndpointRouteBuilder endpoints, string fromPath, string toPath, bool permanent = false)
        {
            endpoints.MapGet(fromPath, context =>
            {
                context.Response.Redirect(toPath, permanent);
                return System.Threading.Tasks.Task.CompletedTask;
            });
            return endpoints;
        }

        /// <summary>
        /// 批量映射 HTTP 重定向规则,传统网页重定向、老旧 URL 变更通知、跳外部链接 (批量配置)
        /// </summary>
        /// <param name="endpoints">路由构建器</param>
        /// <param name="rules">重定向规则字典 (Key: 源路径, Value: 目标路径)</param>
        /// <param name="permanent">是否为 301 永久重定向 (默认 false)</param>
        public static IEndpointRouteBuilder MapEasyRedirectsFast(this IEndpointRouteBuilder endpoints, IDictionary<string, string> rules, bool permanent = false)
        {
            if (rules != null)
            {
                foreach (var rule in rules)
                {
                    endpoints.MapEasyRedirectFast(rule.Key, rule.Value, permanent);
                }
            }
            return endpoints;
        }

        /// <summary>
        /// 启用单条 URL 内部重写中间件,美化 URL、单页应用 (SPA) 路由伪静态映射
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <param name="fromPath">源路径 (例如: "/test")</param>
        /// <param name="toPath">目标路径 (例如: "/test.html")</param>
        public static IApplicationBuilder UseEasyRewriteFast(this IApplicationBuilder app, string fromPath, string toPath)
        {
            return app.UseEasyRewriteFast(new Dictionary<string, string> { { fromPath, toPath } });
        }

        /// <summary>
        /// 批量启用 URL 内部重写中间件,美化 URL、单页应用 (SPA) 路由伪静态映射  (批量配置)
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <param name="rules">重写规则字典 (Key: 源路径, Value: 目标路径)</param>
        public static IApplicationBuilder UseEasyRewriteFast(this IApplicationBuilder app, IDictionary<string, string> rules)
        {
            if (rules == null || rules.Count == 0) return app;
            var options = new RewriteOptions();
            foreach (var rule in rules)
            {
                // 去掉首尾斜杠以匹配 Rewrite 正则规则
                var from = rule.Key.TrimStart('/');
                var to = rule.Value.TrimStart('/');
                // 正则精准匹配整条路径
                options.AddRewrite($"^{from}$", to, skipRemainingRules: true);
            }
            return app.UseRewriter(options);
        }


        /// <summary>
        /// 快捷挂载本地静态文件目录（物理路径不存在时会自动创建目录,如访问 /files/abc.jpg -> 本地 /wwwroot/abc.jpg）
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <param name="folderName">物理文件夹名称，默认项目根目录下的 wwwroot 文件夹</param>
        /// <param name="requestPath">URL 访问前缀（例如: "/files"），留空则直接映射到根路径</param>
        public static IApplicationBuilder UseEasyStaticFilesFast(this IApplicationBuilder app, string folderName = "wwwroot", string requestPath = "/files")
        {
            var physicalPath = System.IO.Path.Combine(System.AppContext.BaseDirectory, folderName);
            // 自动创建不存在的物理目录
            if (!System.IO.Directory.Exists(physicalPath))
            {
                System.IO.Directory.CreateDirectory(physicalPath);
            }
            var options = new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(physicalPath),
                RequestPath = new Microsoft.AspNetCore.Http.PathString(requestPath)
            };
            return app.UseStaticFiles(options);
        }



        /// <summary>
        /// 映射简单的心跳/健康检查接口（如 GET /api/ping，返回服务器时间与机器名）
        /// </summary>
        /// <param name="endpoints">路由构建器</param>
        /// <param name="pattern">路由路径（例如: "api/ping"）</param>
        public static IEndpointRouteBuilder MapEasyPingFast(this IEndpointRouteBuilder endpoints, string pattern = "api/ping")
        {
            endpoints.MapGet(pattern, async context =>
            {
                context.Response.ContentType = "application/json; charset=utf-8";

                var data = new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.Now,
                    Machine = Environment.MachineName,
                    OS = Environment.OSVersion.ToString()
                };

                await context.Response.WriteAsJsonAsync(data);
            });

            return endpoints;
        }



        /// <summary>
        /// 修复 Nginx / Docker / 反向代理下无法获取真实客户端 IP 和 HTTPS 协议的问题
        /// </summary>
        /// <param name="app">应用构建器</param>
        public static IApplicationBuilder UseEasyForwardedHeadersFast(this IApplicationBuilder app)
        {
            // 注意：ForwardedHeadersOptions 在 Microsoft.AspNetCore.Builder 命名空间下
            var options = new Microsoft.AspNetCore.Builder.ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };

            // 清空已知网络与代理限制（适用于容器化/集群部署环境）
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();

            return app.UseForwardedHeaders(options);
        }



        /// <summary>
        /// 快捷配置 Controller 返回的 JSON 格式化选项
        /// </summary>
        /// <param name="builder">MVC 构建器（AddControllers 的返回值）</param>
        /// <param name="keepPascalCase">是否保持 C# 原有的首字母大写属性名（默认 false，即转化为驼峰写法）</param>
        /// <param name="ignoreNull">是否在返回的 JSON 中忽略 null 值的字段（默认 false）</param>
        public static IMvcBuilder AddEasyJsonOptionsFast(this IMvcBuilder builder, bool keepPascalCase = false,bool ignoreNull = false)
        {
            return builder.AddJsonOptions(options =>
            {
                // 1. 属性大小写命名策略
                if (keepPascalCase)
                {
                    // 保持 C# 原始名称
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; 
                }

                // 2. 忽略 null 值属性
                if (ignoreNull)
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                }

#if NET6_0_OR_GREATER
                // 3. 允许循环引用不报错（仅在 .NET 6.0 及以上版本生效）
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
#endif
            });
        }


    }

}

#endif