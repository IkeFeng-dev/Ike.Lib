using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ike.Standard.Func
{
    /// <summary>
    /// 对象属性操作类
    /// </summary>
    /// <remarks>
    /// [<see langword="2025年12月26日10:11:17" />]
    /// </remarks>
    public static class ObjectPropertyOperator
    {

        /// <summary>
        /// 比较两个对象的所有属性值是否相同
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj1">第一个对象</param>
        /// <param name="obj2">第二个对象</param>
        /// <param name="ignoreProperties">要忽略的属性名数组</param>
        /// <returns>如果所有属性值相同返回true，否则返回false</returns>
        /// <remarks>
        /// [<see langword="2025年12月26日16:28:50" />]
        /// </remarks>
        public static bool CompareAllProperties<T>(T obj1, T obj2, params string[] ignoreProperties)
        {
            // 处理null情况
            if (ReferenceEquals(obj1, obj2)) return true;
            if (obj1 == null || obj2 == null) return false;

            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                // 检查是否在忽略列表中
                bool shouldIgnore = false;
                if (ignoreProperties != null)
                {
                    for (int i = 0; i < ignoreProperties.Length; i++)
                    {
                        if (string.Equals(property.Name, ignoreProperties[i], StringComparison.OrdinalIgnoreCase))
                        {
                            shouldIgnore = true;
                            break;
                        }
                    }
                }
                if (shouldIgnore) continue;

                // 获取属性值
                object value1 = property.GetValue(obj1);
                object value2 = property.GetValue(obj2);

                // 处理基本类型和字符串的直接比较
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                {
                    if (!object.Equals(value1, value2))
                        return false;
                }
                // 处理引用类型的递归比较
                else
                {
                    if (value1 == null && value2 == null)
                        continue;

                    if (value1 == null || value2 == null)
                        return false;

                    // 递归比较嵌套对象
                    if (!CompareAllProperties(value1, value2, ignoreProperties))
                        return false;
                }
            }
            return true;
        }



        /// <summary>
        /// 严格类型属性复制（源和目标必须是相同类型）,将<paramref name="source"/>属性值赋值到<paramref name="target"/>属性中
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="ignoreProperties">需要忽略的属性名（不区分大小写）</param>
        /// <exception cref="ArgumentNullException">参数为null时抛出</exception>
        /// <remarks>
        /// [<see langword="2025年12月26日13:47:32" />]
        /// </remarks>
        public static void CopyPropertiesStrict<T>(T source, T target, params string[] ignoreProperties) where T : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var ignoreSet = new HashSet<string>(
                ignoreProperties?.Select(p => p.ToLowerInvariant()) ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            foreach (var prop in typeof(T).GetProperties())
            {
                if (prop.CanRead && prop.CanWrite && !ignoreSet.Contains(prop.Name.ToLowerInvariant()))
                {
                    prop.SetValue(target, prop.GetValue(source));
                }
            }
        }


        /// <summary>
        /// 灵活类型属性复制（允许源和目标类型不同）,将<paramref name="source"/>属性值赋值到<paramref name="target"/>属性中
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TTarget">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="ignoreProperties">需要忽略的属性名（不区分大小写）</param>
        /// <exception cref="ArgumentNullException">参数为null时抛出</exception>
        /// <remarks>
        /// [<see langword="2025年12月26日13:47:32" />]
        /// </remarks>
        public static void CopyPropertiesFlexible<TSource, TTarget>(TSource source, TTarget target, params string[] ignoreProperties) where TSource : class where TTarget : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (target == null) throw new ArgumentNullException(nameof(target));

            var ignoreSet = new HashSet<string>(
                ignoreProperties?.Select(p => p.ToLowerInvariant()) ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            var targetProps = typeof(TTarget).GetProperties()
                .Where(p => p.CanWrite)
                .ToDictionary(p => p.Name.ToLowerInvariant());

            foreach (var srcProp in typeof(TSource).GetProperties().Where(p => p.CanRead))
            {
                var propNameLower = srcProp.Name.ToLowerInvariant();
                if (ignoreSet.Contains(propNameLower)) continue;

                if (targetProps.TryGetValue(propNameLower, out var tgtProp))
                {
                    if (IsTypeCompatible(srcProp.PropertyType, tgtProp.PropertyType))
                    {
                        tgtProp.SetValue(target, srcProp.GetValue(source));
                    }
                }
            }
        }


        /// <summary>
        /// 检查源类型是否可以安全地转换为目标类型
        /// </summary>
        /// <param name="sourceType">要转换的源类型</param>
        /// <param name="targetType">要转换到的目标类型</param>
        /// <returns>
        ///   <see  langword="true"/> 如果满足以下任一条件：
        ///   <list type="bullet">
        ///     <item><description>目标类型可以接受源类型的实例（包括继承关系和接口实现）</description></item>
        ///     <item><description>两者都是基本类型（Primitive）</description></item>
        ///     <item><description>目标类型是源类型的可空版本（如 int 到 int?）</description></item>
        ///     <item><description>源类型是目标类型的可空版本（如 int? 到 int）</description></item>
        ///   </list>
        ///   否则返回<see  langword="false"/>
        /// </returns>
        /// <remarks>
        /// [<see langword="2025年12月26日13:47:32" />]
        /// </remarks>
        public static bool IsTypeCompatible(Type sourceType, Type targetType)
        {
            var realTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var realSourceType = Nullable.GetUnderlyingType(sourceType) ?? sourceType;
            return realTargetType.IsAssignableFrom(realSourceType) || (realTargetType.IsPrimitive && realSourceType.IsPrimitive);
        }


    }
}
