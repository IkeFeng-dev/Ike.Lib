#if NET5_0_OR_GREATER
#nullable enable 

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Ike.Core.API.External
{
    /// <summary>
    /// 外部 API 权限校验器接口
    /// <para>主项目需要实现此接口，用于编写具体的权限/Cookie/Database/Config校验逻辑</para>
    /// </summary>
    public interface IExternalApiValidator
    {
        /// <summary>
        /// 执行异步校验逻辑
        /// </summary>
        /// <param name="context">当前请求的 HTTP 上下文，可从中获取 Request、Cookies、Headers 等信息</param>
        /// <returns>返回 <c>true</c> 表示校验通过并放行，返回 <c>false</c> 表示校验失败并直接拒绝访问</returns>
        Task<bool> ValidateAsync(HttpContext context);
    }
}
#endif
