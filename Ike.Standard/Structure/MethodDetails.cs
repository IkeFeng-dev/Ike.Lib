using System;
using System.Collections.Generic;

namespace Ike.Standard
{
    public partial class ReflectionUtility
    {
        /// <summary>
        /// 表示方法的详细信息，包含方法名称、静态标识、返回类型和参数列表
        /// </summary>
        /// <remarks>
        /// 此类用于反射获取的方法元数据封装，便于分析和调用方法
        /// </remarks>
        public class MethodDetails
        {
            /// <summary>
            /// 获取或设置方法的名称
            /// </summary>
            /// <value>方法的字符串名称，大小写敏感</value>
            public string Name { get; set; }

            /// <summary>
            /// 获取或设置指示方法是否为静态的值
            /// </summary>
            /// <value>如果方法是静态的则为 <c>true</c>；否则为 <c>false</c></value>
            public bool IsStatic { get; set; }

            /// <summary>
            /// 获取或设置方法的返回类型
            /// </summary>
            /// <value>表示返回类型的 <see cref="System.Type"/> 对象</value>
            public Type ReturnType { get; set; }

            /// <summary>
            /// 获取或设置方法的参数列表
            /// </summary>
            /// <value>包含所有参数信息的 <see cref="ParameterInfo"/> 集合</value>
            public List<ParameterInfo> Parameters { get; set; }
        }
    }
}
