#if NET5_0_OR_GREATER

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 通用 API 控制器扩展类，用于按需注册类库中封装的 Controller
    /// </summary>
    public static class CommonApiExtensions
    {
        /// <summary>
        /// 注册并配置通用 API 控制器（按需开启指定的 Controller）
        /// </summary>
        /// <param name="builder">MVC 构建器（通常为 <c>builder.Services.AddControllers()</c> 的返回值）</param>
        /// <param name="configure">控制器配置委托，用于指定哪些 Controller 需要被启用</param>
        /// <returns>返回 <see cref="IMvcBuilder"/> 实例，支持链式调用</returns>
        public static IMvcBuilder AddCommonControllers(this IMvcBuilder builder, Action<CommonControllersOptions> configure)
        {
            // 1. 实例化通用控制器的配置选项对象
            var options = new CommonControllersOptions();

            // 2. 执行外部传入的配置 Action，将使用者开启的 Controller 记录到 options 中
            configure(options);

            // 3. 配置应用部件管理器（ApplicationPartManager）
            // 将自定义的控制器过滤器（FeatureProvider）加入到 MVC 框架的处理链条中
            // 框架在识别 Controller 时会触发此 Provider，从而动态剔除未启用的 Controller
            builder.ConfigureApplicationPartManager(apm =>
            {
                apm.FeatureProviders.Add(new SelectiveControllerFeatureProvider(options));
            });

            // 4. 返回 builder 以支持链式调用（Fluent API）
            return builder;
        }
    }
}

#endif
