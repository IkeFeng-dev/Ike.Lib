using System;
using System.Reflection;

namespace Ike.Standard
{
    public partial class ObjectProperty
    {
        /// <summary>
        /// 当前库信息
        /// </summary>
        public struct LibraryInfo
        {
            /// <summary>
            /// 命名空间名称
            /// </summary>
            public string NamespaceName;
            /// <summary>
            /// 类名称
            /// </summary>
            public string ClassName;
            /// <summary>
            /// 类全称
            /// </summary>
            public string ClassFullName;
            /// <summary>
            /// 方法名称
            /// </summary>
            public string MethodName;
            /// <summary>
            /// 方法句柄
            /// </summary>
            public RuntimeMethodHandle MethodHandle;
            /// <summary>
            /// 元数据元素
            /// </summary>
            public int MetadataToken;
            /// <summary>
            /// 是否为静态方法
            /// </summary>
            public bool IsStatic;
            /// <summary>
            /// 是否为虚拟函数
            /// </summary>
            public bool IsIsVirtual;
            /// <summary>
            /// 是否为公共函数
            /// </summary>
            public bool IsPublic;
            /// <summary>
            /// 是否为私有函数
            /// </summary>
            public bool IsPrivate;
            /// <summary>
            /// 是否为构造函数
            /// </summary>
            public bool IsConstructor;
            /// <summary>
            /// 是否为限定函数
            /// </summary>
            public bool IsFinal;
            /// <summary>
            /// 方法本身
            /// </summary>
            public MethodBase Base;
        }
    }
}
