using System.Collections.Generic;

namespace Ike.Standard
{
    public partial class ReflectionUtility
    { /// <summary>
      /// 表示类型的详细信息，包含类型全名、类型分类标识和方法集合
      /// </summary>
      /// <remarks>
      /// 此类封装了反射获取的类型元数据，提供对类型结构的完整描述
      /// <para>示例用法参见 <see cref="Reflection.GetTypeDetails"/> 方法</para>
      /// </remarks>
        public class TypeDetails
        {
            /// <summary>
            /// 获取或设置类型的完全限定名
            /// </summary>
            /// <value>包含命名空间的类型全名</value>
            public string FullName { get; set; }

            /// <summary>
            /// 获取或设置指示类型是否为抽象的值
            /// </summary>
            /// <value>如果类型是抽象的则为 <c>true</c>；否则为 <c>false</c></value>
            public bool IsAbstract { get; set; }

            /// <summary>
            /// 获取或设置指示类型是否为类的值
            /// </summary>
            /// <value>如果类型是类则为 <c>true</c>；否则为 <c>false</c></value>
            public bool IsClass { get; set; }

            /// <summary>
            /// 获取或设置指示类型是否为接口的值
            /// </summary>
            /// <value>如果类型是接口则为 <c>true</c>；否则为 <c>false</c></value>
            public bool IsInterface { get; set; }

            /// <summary>
            /// 获取或设置指示类型是否为公共的值
            /// </summary>
            /// <value>如果类型是公共可见的则为 <c>true</c>；否则为 <c>false</c></value>
            public bool IsPublic { get; set; }

            /// <summary>
            /// 获取或设置类型中定义的方法集合
            /// </summary>
            /// <value>包含所有方法信息的 <see cref="MethodDetails"/> 集合</value>
            /// <example>
            /// 以下示例展示如何访问类型的方法列表：
            /// <code>
            /// var typeDetails = reflectionHelper.GetTypeDetails(typeof(MyClass));
            /// foreach (var method in typeDetails.Methods)
            /// {
            ///     Console.WriteLine(method.Name);
            /// }
            /// </code>
            /// </example>
            public List<MethodDetails> Methods { get; set; }
        }
    }
}
