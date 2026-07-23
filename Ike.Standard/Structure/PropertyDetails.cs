using System;

namespace Ike.Standard
{
    public partial class ReflectionUtility
    {
        /// <summary>
        /// 属性元数据
        /// </summary>
        public class PropertyDetails
        {
            /// <summary>
            /// 属性名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 属性类型
            /// </summary>
            public Type PropertyType { get; set; }

            /// <summary>
            /// 是否可读
            /// </summary>
            public bool CanRead { get; set; }

            /// <summary>
            /// 是否可写
            /// </summary>
            public bool CanWrite { get; set; }
        }

    }
}
