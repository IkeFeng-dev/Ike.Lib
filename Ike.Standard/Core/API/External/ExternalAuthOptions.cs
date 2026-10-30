#if NET5_0_OR_GREATER
#nullable enable 

using System;
using System.Collections.Generic;

namespace Ike.Core.API.External
{
    /// <summary>
    /// 外部控制器权限拦截规则映射项。
    /// </summary>
    public class ExternalAuthRule
    {
        /// <summary>
        /// 获取或设置需要拦截的控制器名称（例如 "Test"）
        /// </summary>
        public string ControllerName { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置需要拦截的 Action 名称（若为 null，则对整个 Controller 生效）
        /// </summary>
        public string? ActionName { get; set; }

        /// <summary>
        /// 获取或设置绑定的校验器类型（必须实现 <see cref="IExternalApiValidator"/>）
        /// </summary>
        public Type ValidatorType { get; set; } = null!;
    }

    /// <summary>
    /// 外部 API 权限验证配置选项
    /// </summary>
    public class ExternalAuthOptions
    {
        /// <summary>
        /// 获取匹配规则映射列表。
        /// </summary>
        public List<ExternalAuthRule> Rules { get; } = [];

        /// <summary>
        /// 为指定的外部 Controller 绑定权限校验器
        /// </summary>
        /// <typeparam name="TValidator">使用的校验器实现类</typeparam>
        /// <param name="controllerName">外部控制器名称（不包含 Controller 后缀，如 "Test"）</param>
        public void MapController<TValidator>(string controllerName) where TValidator : IExternalApiValidator
        {
            Rules.Add(new ExternalAuthRule
            {
                ControllerName = controllerName,
                ValidatorType = typeof(TValidator)
            });
        }

        /// <summary>
        /// 为指定的外部 Action 绑定特定的权限校验器
        /// </summary>
        /// <typeparam name="TValidator">使用的校验器实现类</typeparam>
        /// <param name="controllerName">外部控制器名称（如 "Test"）</param>
        /// <param name="actionName">外部 Action 名称（如 "Index"）</param>
        public void MapAction<TValidator>(string controllerName, string actionName) where TValidator : IExternalApiValidator
        {
            Rules.Add(new ExternalAuthRule
            {
                ControllerName = controllerName,
                ActionName = actionName,
                ValidatorType = typeof(TValidator)
            });
        }
    }
}
#endif
