using System;
using System.Reflection;

namespace Ike.Standard
{
    public partial class ReflectionUtility
    {

        /// <summary>
        /// 事件详细信息
        /// </summary>
        public class EventDetails
        {
            /// <summary>
            /// 事件名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 事件处理程序类型
            /// </summary>
            public Type HandlerType { get; set; }

            /// <summary>
            /// 是否为静态事件
            /// </summary>
            public bool IsStatic { get; set; }

            /// <summary>
            /// 事件的添加方法
            /// </summary>
            public MethodInfo AddMethod { get; set; }

            /// <summary>
            /// 事件的移除方法
            /// </summary>
            public MethodInfo RemoveMethod { get; set; }
        }
    }
}
