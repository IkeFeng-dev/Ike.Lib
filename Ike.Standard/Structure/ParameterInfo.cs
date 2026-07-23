using System;

namespace Ike.Standard
{
    public partial class ReflectionUtility
    {
        /// <summary>
        /// 表示方法的参数信息，包含参数名、类型、可选性标识和默认值
        /// </summary>
        /// <remarks>
        /// 此类用于描述方法的单个参数特性，通常作为 <see cref="MethodDetails.Parameters"/> 集合的元素
        /// </remarks>
        public class ParameterInfo
        {
            /// <summary>
            /// 获取或设置参数的名称
            /// </summary>
            /// <value>参数的字符串名称</value>
            public string Name { get; set; }

            /// <summary>
            /// 获取或设置参数的类型
            /// </summary>
            /// <value>表示参数类型的 <see cref="System.Type"/> 对象</value>
            public Type Type { get; set; }

            /// <summary>
            /// 获取或设置指示参数是否为可选的值
            /// </summary>
            /// <value>如果参数是可选的则为 <c>true</c>；否则为 <c>false</c></value>
            public bool IsOptional { get; set; }

            /// <summary>
            /// 获取或设置参数的默认值
            /// </summary>
            /// <value>参数的默认值对象，如果无默认值则为 <c>null</c></value>
            /// <remarks>
            /// 对于非可选参数，此属性通常为 <c>null</c>
            /// </remarks>
            public object DefaultValue { get; set; }
        }
    }
}
