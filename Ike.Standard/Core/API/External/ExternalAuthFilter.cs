#if NET5_0_OR_GREATER
#nullable enable 

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Ike.Core.API.External
{
    /// <summary>
    /// 通用过滤器。动态从依赖注入（DI）容器中获取匹配的 <see cref="IExternalApiValidator"/> 实现类并执行校验
    /// </summary>
    public class ExternalAuthFilter : IAsyncActionFilter
    {
        private readonly Type _validatorType;

        /// <summary>
        /// 初始化 <see cref="ExternalAuthFilter"/> 类的新实例
        /// </summary>
        /// <param name="validatorType">具体的校验器类型（必须实现 <see cref="IExternalApiValidator"/> 接口）</param>
        public ExternalAuthFilter(Type validatorType)
        {
            _validatorType = validatorType;
        }

        /// <summary>
        /// 在 Action 执行前后拦截并进行动态校验
        /// </summary>
        /// <param name="context">Action 执行上下文</param>
        /// <param name="next">Action 执行委托</param>
        /// <returns>异步任务</returns>
        /// <exception cref="InvalidOperationException">当未在 DI 容器中注册对应类型的校验器时抛出</exception>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 从 DI 容器中动态获取指定的校验器实现类
            var validator = context.HttpContext.RequestServices.GetService(_validatorType) as IExternalApiValidator
                ?? throw new InvalidOperationException($"未注册类型为 [{_validatorType.FullName}] 的校验器！请在主项目 Program.cs 中将其注册到 DI 容器中");

            // 调用主项目的校验逻辑
            bool isValid = await validator.ValidateAsync(context.HttpContext);

            if (!isValid)
            {
                // 校验失败，返回 401
                context.Result = new JsonResult(new { message = "权限不足或认证失败" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            // 校验通过，放行进入控制器
            await next();
        }
    }
}
#endif
