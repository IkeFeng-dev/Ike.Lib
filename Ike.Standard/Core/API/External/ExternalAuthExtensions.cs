#if NET5_0_OR_GREATER
#nullable enable 

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ike.Core.API.External
{
    /// <summary>
    /// 外部控制器权限验证注册扩展类
    /// </summary>
    public static class ExternalAuthExtensions
    {
        /// <summary>
        /// 开启外部控制器/接口权限拦截支持
        /// </summary>
        /// <param name="mvcOptions">MVC 配置选项 <see cref="MvcOptions"/></param>
        /// <param name="configure">拦截规则配置委托</param>
        /// <returns>配置后的 <see cref="MvcOptions"/></returns>
        public static MvcOptions AddExternalActionAuth(this MvcOptions mvcOptions, Action<ExternalAuthOptions> configure)
        {
            var options = new ExternalAuthOptions();
            configure(options);

            mvcOptions.Conventions.Add(new ExternalActionAuthConvention(options));
            return mvcOptions;
        }
    }
}
#endif
