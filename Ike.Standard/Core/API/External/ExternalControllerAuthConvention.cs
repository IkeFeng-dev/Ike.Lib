#if NET5_0_OR_GREATER
#nullable enable 

using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Linq;

namespace Ike.Core.API.External
{
    /// <summary>
    /// 外部 API Action 级别的权限验证应用模型约定
    /// <para>
    /// 用于在启动阶段扫描外部控制器的 Action，并根据 <see cref="ExternalAuthOptions"/> 中的映射规则,动态挂载注入了特定校验器类型的 <see cref="ExternalAuthFilter"/>
    /// </para>
    /// </summary>
    /// <remarks>
    /// 初始化 <see cref="ExternalActionAuthConvention"/> 类的新实例
    /// </remarks>
    /// <param name="options">外部控制器权限配置选项</param>
    public class ExternalActionAuthConvention(ExternalAuthOptions options) : IActionModelConvention
    {
        private readonly ExternalAuthOptions _options = options;

        /// <summary>
        /// 当 ASP.NET Core MVC 构建 Action 模型时调用此方法
        /// </summary>
        /// <param name="action">当前正在处理的 Action 模型 <see cref="ActionModel"/></param>
        public void Apply(ActionModel action)
        {
            string controllerName = action.Controller.ControllerName;
            string actionName = action.ActionName;

            // 优先查找精确匹配 Action 的规则；若无，再查找针对 Controller 的规则
            var matchedRule = _options.Rules.FirstOrDefault(r =>
                r.ControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrEmpty(r.ActionName) &&
                r.ActionName.Equals(actionName, StringComparison.OrdinalIgnoreCase));

            if (matchedRule == null)
            {
                matchedRule = _options.Rules.FirstOrDefault(r =>
                    r.ControllerName.Equals(controllerName, StringComparison.OrdinalIgnoreCase) &&
                    string.IsNullOrEmpty(r.ActionName));
            }

            // 如果匹配到了针对该 Action 或 Controller 的规则，添加带有所需 ValidatorType 的 Filter
            if (matchedRule != null)
            {
                action.Filters.Add(new ExternalAuthFilter(matchedRule.ValidatorType));
            }
        }
    }
}
#endif
