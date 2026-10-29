#if NET5_0_OR_GREATER

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 通用 API 控制器的启用选项配置类
    /// <para>用于管理在宿主项目中允许暴露的类库 Controller 类型集合</para>
    /// </summary>
    public class CommonControllersOptions
    {
        /// <summary>
        /// 获取存储已被允许启用的 Controller 类型集合（哈希表）
        /// </summary>
        internal HashSet<Type> EnabledControllers { get; } = [];

        /// <summary>
        /// 按泛型类型显式启用指定的 Controller
        /// </summary>
        /// <typeparam name="TController">继承自 <see cref="ControllerBase"/> 的控制器类型</typeparam>
        /// <returns>返回当前配置选项实例，支持链式调用（Fluent API）</returns>
        public CommonControllersOptions Enable<TController>() where TController : ControllerBase
        {
            EnabledControllers.Add(typeof(TController));
            return this;
        }

        /* 💡 提示：后续如果在类库中新增了控制器，也可以在这里添加快捷启用方法，方便外部调用，例如：
        /// <summary>
        /// 启用系统信息控制器 (SystemInfoController)
        /// </summary>
        public CommonControllersOptions EnableSystemInfo() => Enable<Controllers.SystemInfoController>();
        */

        /// <summary>
        /// 启用文件管理控制器 <see cref="FileReleaseController"/>
        /// </summary>
        public CommonControllersOptions EnableFileRelease() => Enable<FileReleaseController>();
    }
}

#endif
