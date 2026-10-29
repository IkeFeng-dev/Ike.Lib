#if NET5_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 自定义控制器特性提供程序（Feature Provider）
    /// <para>用于在 ASP.NET Core 扫描控制器时，动态剔除本类库中未在配置中显式启用的 Controller</para>
    /// </summary>
    /// <remarks>
    /// 初始化 <see cref="SelectiveControllerFeatureProvider"/> 的新实例
    /// </remarks>
    /// <param name="options">包含已允许启用的 Controller 类型集合的配置项</param>
    public class SelectiveControllerFeatureProvider(CommonControllersOptions options) : IApplicationFeatureProvider<ControllerFeature>
    {
        /// <summary>
        /// 控制器启用选项配置
        /// </summary>
        private readonly CommonControllersOptions _options = options ?? throw new ArgumentNullException(nameof(options));

        /// <summary>
        /// 填充并过滤应用程序的控制器特性列表（由 MVC 框架在初始化时自动调用）
        /// </summary>
        /// <param name="parts">当前应用程序引用的应用部件列表</param>
        /// <param name="feature">框架扫描到的控制器特性集合，包含所有候选 Controller</param>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // 1. 获取当前 DLL（通用类库）所在的程序集
            var currentAssembly = typeof(SelectiveControllerFeatureProvider).Assembly;

            // 2. 找出所有属于当前 DLL，但【未在 Options 中被显式启用】的 Controller 类型
            var controllersToRemove = feature.Controllers
                .Where(c => c.Assembly == currentAssembly && !_options.EnabledControllers.Contains(c.AsType()))
                .ToList();

            // 3. 从框架的 Controller 注册列表中逐个移除未启用的 Controller
            // 被移除后，对应的路由规则不会生效，Swagger 文档中也不会显示这些接口
            foreach (var controller in controllersToRemove)
            {
                feature.Controllers.Remove(controller);
            }
        }
    }
}

#endif
