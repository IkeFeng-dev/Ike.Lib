using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ike.Standard
{
    /// <summary>
    /// 反射类
    /// </summary>
    public class Reflection
    {
        #region  定义结构
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

        /// <summary>
        /// 表示类型的详细信息，包含类型全名、类型分类标识和方法集合
        /// </summary>
        /// <remarks>
        /// 此类封装了反射获取的类型元数据，提供对类型结构的完整描述
        /// <para>示例用法参见 <see cref="ReflectionUtility.GetTypeDetails"/> 方法</para>
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

        #endregion

        /// <summary>
        /// 反射工具类，提供程序集、类型和方法的动态操作功能
        /// 
        /// 注意事项:<br/><br/>
        /// <see langword="typeName"></see> 表示完整的名称,包括命名空间以及类名,子类需要以<see langword="+"/>连接,如:namespace.class+class 
        /// 
        /// </summary>
        public class ReflectionUtility : IDisposable
        {
            /// <summary>
            /// 当前程序集实例对象
            /// </summary>
            private Assembly assembly;
            /// <summary>
            /// 获取或设置当前实例中程序集对象
            /// </summary>
            public Assembly AssemblyObject
            {
                get
                {
                    if (assembly == null)
                    {
                        throw new InvalidOperationException("Please load the assembly first.");
                    }
                    return assembly;
                }
                set
                {
                    assembly = value;
                }
            }

            /// <summary>
            /// 表示所有类型标志
            /// </summary>
            public BindingFlags AllTypeBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            /// <summary>
            /// 类型缓存字典
            /// </summary>
            /// <remarks>
            /// 键：类型的完全限定名（包含命名空间）
            /// 值：对应的 <see cref="System.Type"/> 实例
            /// <para>用于提高频繁获取类型时的性能</para>
            /// </remarks>
            private readonly Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

            /// <summary>
            /// 方法缓存字典
            /// </summary>
            /// <remarks>
            /// 键：由类型全名、方法名和静态标识组成的复合键（格式："TypeName.MethodName_IsStatic"）
            /// 值：对应的 <see cref="System.Reflection.MethodInfo"/> 实例
            /// <para>缓存方法元数据以避免重复反射</para>
            /// </remarks>
            private readonly Dictionary<string, MethodInfo> _methodCache = new Dictionary<string, MethodInfo>();

            /// <summary>
            /// 实例缓存字典
            /// </summary>
            /// <remarks>
            /// 键：类型的完全限定名（包含命名空间）
            /// 值：已创建的类型实例
            /// <para>用于实现单例模式或重复使用已创建的实例</para>
            /// </remarks>
            private readonly Dictionary<string, object> _instanceCache = new Dictionary<string, object>();


            /// <summary>
            /// 提供 <see cref="EventDetails"/> 的自定义相等比较实现
            /// </summary>
            /// <remarks>
            /// 用于在集合操作中比较两个事件是否相同。
            /// 比较依据：
            /// <list type="bullet">
            /// <item><description>事件名称（<see cref="EventDetails.Name"/>）</description></item>
            /// <item><description>事件处理程序类型（<see cref="EventDetails.HandlerType"/>）</description></item>
            /// </list>
            /// </remarks>
            private class EventDetailsComparer : IEqualityComparer<EventDetails>
            {
                /// <summary>
                /// 确定两个 <see cref="EventDetails"/> 实例是否相等
                /// </summary>
                /// <param name="x">要比较的第一个对象</param>
                /// <param name="y">要比较的第二个对象</param>
                /// <returns>
                /// 如果指定的对象相等，则为 <see langword="true"/>；
                /// 否则为 <see langword="false"/>
                /// </returns>
                /// <remarks>
                /// 满足以下条件时认为两个事件相等：
                /// <list type="number">
                /// <item><description>事件名称相同（区分大小写）</description></item>
                /// <item><description>事件处理程序类型相同</description></item>
                /// </list>
                /// 注意：此方法已处理 <see langword="null"/> 值情况。
                /// </remarks>
                public bool Equals(EventDetails x, EventDetails y)
                {
                    return x?.Name == y?.Name && x?.HandlerType == y?.HandlerType;
                }

                /// <summary>
                /// 返回指定对象的哈希代码
                /// </summary>
                /// <param name="obj">要获取哈希代码的对象</param>
                /// <returns>对象的哈希代码</returns>
                /// <exception cref="ArgumentNullException">
                /// 当 <paramref name="obj"/> 为 <see langword="null"/> 时抛出
                /// </exception>
                /// <remarks>
                /// 哈希码计算规则：
                /// <code>
                /// obj.Name.GetHashCode() ^ (obj.HandlerType?.GetHashCode() ?? 0)
                /// </code>
                /// </remarks>
                public int GetHashCode(EventDetails obj)
                {
                    if (obj == null)
                    {
                        throw new ArgumentNullException(nameof(obj));
                    }

                    return obj.Name.GetHashCode() ^ (obj.HandlerType?.GetHashCode() ?? 0);
                }
            }

            /// <summary>
            /// 加载程序集,会占用文件句柄
            /// </summary>
            /// <param name="assemblyPath">程序集路径</param>
            /// <exception cref="FileNotFoundException">找不到程序集</exception>
            public Assembly LoadAssembly(string assemblyPath)
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException("The assembly cannot be found.", nameof(assemblyPath));
                }
                AssemblyObject = Assembly.LoadFrom(assemblyPath);
                return AssemblyObject;
            }

            /// <summary>
            /// 加载程序集,不占用文件句柄
            /// </summary>
            /// <param name="assemblyPath">程序集路径</param>
            /// <returns></returns>
            public Assembly LoadAssemblyFromMemory(string assemblyPath)
            {
                if (!File.Exists(assemblyPath))
                {
                    throw new FileNotFoundException("The assembly cannot be found.", nameof(assemblyPath));
                }
                byte[] bytes = File.ReadAllBytes(assemblyPath);
                AssemblyObject = Assembly.Load(bytes);
                return AssemblyObject;
            }

            /// <summary>
            /// 执行静态方法
            /// </summary>
            /// <param name="typeName">完整类型名称</param>
            /// <param name="bindingFlags">反射成员的方法类型</param>
            /// <param name="methodName">方法名</param>
            /// <param name="parameters">参数数组</param>
            public object InvokeStaticMethod(string typeName, string methodName, BindingFlags bindingFlags = default, params object[] parameters)
            {
                if (bindingFlags == default)
                {
                    bindingFlags = AllTypeBindingFlags;
                }
                var method = GetMethods(typeName, methodName, parameters, bindingFlags);
                return method.Invoke(null, parameters);
            }

            /// <summary>
            /// 创建实例并执行实例方法
            /// </summary>
            /// <param name="typeName">完整类型名称</param>
            /// <param name="methodName">方法名</param>
            /// <param name="bindingFlags">方法类型</param>
            /// <param name="constructorArgs">构造函数参数</param>
            /// <param name="methodParameters">方法参数</param>
            public object InvokeInstanceMethod(string typeName, string methodName, object[] constructorArgs, BindingFlags bindingFlags = default, params object[] methodParameters)
            {
                if (bindingFlags == default)
                {
                    bindingFlags = AllTypeBindingFlags;
                }
                var instance = CreateInstance(typeName, constructorArgs);
                var method = GetMethodFromInstance(instance, methodName, methodParameters, bindingFlags);
                return method.Invoke(instance, methodParameters);
            }

            /// <summary>
            /// 创建类型的实例
            /// </summary>
            /// <param name="typeName">完整类型名称</param>
            /// <param name="args">构造函数参数</param>
            public object CreateInstance(string typeName, params object[] args)
            {
                if (_instanceCache.TryGetValue(typeName, out var cachedInstance))
                    return cachedInstance;

                var type = GetTypeFromAssembly(typeName);
                var instance = Activator.CreateInstance(type, args);
                _instanceCache[typeName] = instance;
                return instance;
            }

            /// <summary>
            /// 创建泛型类型实例
            /// </summary>
            /// <param name="genericTypeDef">泛型类型定义（如List&lt;&gt;）</param>
            /// <param name="typeArgs">类型参数数组</param>
            /// <param name="constructorArgs">构造函数参数</param>
            /// <returns>构造的泛型实例</returns>
            public object CreateGenericInstance(Type genericTypeDef, Type[] typeArgs, params object[] constructorArgs)
            {
                if (!genericTypeDef.IsGenericTypeDefinition)
                {
                    throw new ArgumentException("Type must be generic definition");
                }
                var concreteType = genericTypeDef.MakeGenericType(typeArgs);
                return Activator.CreateInstance(concreteType, constructorArgs);
            }

            /// <summary>
            /// 创建委托
            /// </summary>
            /// <param name="eventInfo">事件类型</param>
            /// <param name="handler">处理方法</param>
            /// <returns>委托对象</returns>
            public Delegate CreateDelegate(EventInfo eventInfo, Action<dynamic> handler)
            {
                return Delegate.CreateDelegate(eventInfo.EventHandlerType, handler.Target, handler.Method);
            }

            /// <summary>
            /// 转换指定枚举值
            /// </summary>
            /// <param name="typeName">完整类型名称</param>
            /// <param name="enumValue">需要转换的枚举值,和枚举结构属性名相同</param>
            /// <returns>转换后的枚举值</returns>
            public object ConvertEnumValue(string typeName, string enumValue)
            {
                // 获取LogType枚举类型
                Type logTypeEnum = AssemblyObject.GetType(typeName);
                // 获取Info枚举值
                object infoEnumValue = Enum.Parse(logTypeEnum, enumValue);

                return infoEnumValue;
            }

            #region Get 

            /// <summary>
            /// 获取指定实例属性值
            /// </summary>
            /// <param name="instance">实例对象</param>
            /// <param name="propertyName">属性名称</param>
            /// <returns>获取的属性值</returns>
            public object GetProperty(object instance, string propertyName)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }
                return instance.GetType().GetProperty(propertyName);
            }

            /// <summary>
            /// 获取程序集中所有公开类型
            /// </summary>
            /// <returns>一个数组，用于表示此程序集内定义的、在程序集外部可见的类型</returns>
            /// <exception cref="InvalidOperationException"></exception>
            public IEnumerable<Type> GetAllPublicTypes()
            {
                return AssemblyObject.GetExportedTypes();
            }

            /// <summary>
            /// 获取指定类型中定义的所有方法
            /// </summary>
            /// <param name="typeName">要查找的类型的完全限定名（包括命名空间）</param>
            /// <param name="bindingFlags">表示要获取方法标志
            /// 此方法将返回类型中定义的所有方法，如：
            /// <list type="bullet">
            /// <item><description>继承方法</description></item>
            /// <item><description>私有方法（<see langword="private"/>）</description></item>
            /// <item><description>受保护方法（<see langword="protected"/>）</description></item>
            /// <item><description>内部方法（<see langword="internal"/>）</description></item>
            /// <item><description>公共方法（<see langword="public"/>）</description></item>
            /// <item><description>实例方法（instance）</description></item>
            /// <item><description>静态方法（<see langword="static"/>）</description></item>
            /// </list>
            /// </param>
            /// <returns>包含所有方法信息的 <see cref="MethodInfo"/> 集合</returns>
            /// <exception cref="ArgumentNullException">当 typeName 为 null 或空字符串时抛出</exception>
            /// <exception cref="TypeLoadException">当找不到指定类型时抛出</exception>
            public IEnumerable<MethodInfo> GetMethodsInType(string typeName, BindingFlags bindingFlags = default)
            {
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    throw new ArgumentNullException(nameof(typeName), "The type name cannot be empty.");
                }
                if (bindingFlags == default)
                {
                    bindingFlags = AllTypeBindingFlags;
                }
                var type = GetTypeFromAssembly(typeName);

                // 获取所有方法（包括公共和非公共、静态和实例）
                return type.GetMethods(bindingFlags);
            }

            /// <summary>
            /// 获取方法信息
            /// </summary>
            /// <param name="typeName">完整类型名称</param>
            /// <param name="methodName">方法名</param>
            /// <param name="parameters">方法参数</param>
            /// <param name="bindingFlags">方法类型</param>
            /// <returns>方法信息对象</returns>
            /// <exception cref="MissingMethodException"></exception>
            /// <exception cref="AmbiguousMatchException"></exception>
            public MethodInfo GetMethods(string typeName, string methodName, object[] parameters, BindingFlags bindingFlags = default)
            {
                if (bindingFlags == default)
                {
                    bindingFlags = AllTypeBindingFlags;
                }
                string cacheKey = $"{typeName}.{methodName}_{bindingFlags}";
                if (!_methodCache.TryGetValue(cacheKey, out MethodInfo method))
                {
                    var type = GetTypeFromAssembly(typeName);
                    // 尝试精确匹配参数类型
                    var paramTypes = parameters?.Select(p => p?.GetType() ?? typeof(object)).ToArray();
                    method = type.GetMethod(methodName, bindingFlags, null, paramTypes ?? Type.EmptyTypes, null);

                    if (method == null)
                    {
                        // 如果精确匹配失败，尝试匹配方法名
                        var methods = type.GetMethods(bindingFlags).Where(m => m.Name == methodName).ToList();

                        if (methods.Count == 0)
                        {
                            throw new MissingMethodException($"No method was found: {typeName}.{methodName}");
                        }
                        else if (methods.Count > 1)
                        {
                            throw new AmbiguousMatchException($"Find multiple methods with the same name: {typeName}.{methodName}");
                        }

                        method = methods[0];
                    }
                    _methodCache[cacheKey] = method;
                }
                return method;
            }

            /// <summary>
            /// 从实例获取方法
            /// </summary>
            /// <param name="instance">实例对象</param>
            /// <param name="methodName">方法名称</param>
            /// <param name="bindingFlags">方法类型</param>
            /// <param name="parameters">方法参数</param>
            /// <returns>方法实例</returns>
            public MethodInfo GetMethodFromInstance(object instance, string methodName, object[] parameters, BindingFlags bindingFlags = default)
            {
                if (bindingFlags == default)
                {
                    bindingFlags = AllTypeBindingFlags;
                }
                var paramTypes = parameters?.Select(p => p?.GetType() ?? typeof(object)).ToArray();
                return instance.GetType().GetMethod(methodName, bindingFlags, null, paramTypes ?? Type.EmptyTypes, null);
            }

            /// <summary>
            /// 从当前程序集获取类型
            /// </summary>
            /// <param name="typeName">完整类型名称</param>
            /// <returns></returns>
            /// <exception cref="InvalidOperationException"></exception>
            /// <exception cref="TypeLoadException"></exception>
            public Type GetTypeFromAssembly(string typeName)
            {
                if (_typeCache.TryGetValue(typeName, out Type cachedType))
                {
                    return cachedType;
                }
                var type = AssemblyObject.GetType(typeName) ?? AssemblyObject.GetTypes().FirstOrDefault(t => t.Name == typeName);
                _typeCache[typeName] = type ?? throw new TypeLoadException($"Type not found: {typeName}");
                return type;
            }

            /// <summary>
            /// 获取方法的详细信息
            /// </summary>
            /// <param name="method">方法属性</param>
            /// <returns></returns>
            public MethodDetails GetMethodDetails(MethodInfo method)
            {
                return new MethodDetails
                {
                    Name = method.Name,
                    IsStatic = method.IsStatic,
                    ReturnType = method.ReturnType,
                    Parameters = method.GetParameters().Select(p => new ParameterInfo
                    {
                        Name = p.Name,
                        Type = p.ParameterType,
                        IsOptional = p.IsOptional,
                        DefaultValue = p.DefaultValue
                    }).ToList()
                };
            }

            /// <summary>
            /// 获取类型的详细信息
            /// </summary>
            /// <param name="type">类型</param>
            /// <param name="bindingFlags">方法类型</param>
            /// <returns></returns>
            public TypeDetails GetTypeDetails(Type type, BindingFlags bindingFlags = default)
            {
                if (bindingFlags == default)
                {
                    bindingFlags = AllTypeBindingFlags;
                }
                return new TypeDetails
                {
                    FullName = type.FullName,
                    IsAbstract = type.IsAbstract,
                    IsClass = type.IsClass,
                    IsInterface = type.IsInterface,
                    IsPublic = type.IsPublic,
                    Methods = type.GetMethods(bindingFlags).Select(GetMethodDetails).ToList()
                };
            }

            /// <summary>
            /// 获取类型属性详细信息
            /// </summary>
            /// <param name="type">要反射的目标类型</param>
            /// <param name="bindingFlags">绑定约束标志</param>
            /// <returns>属性详细信息集合</returns>
            /// <remarks>
            /// 此方法将返回类型中定义的所有属性，包括：
            /// <list type="bullet">
            /// <item><description>公共/非公共属性</description></item>
            /// <item><description>实例/静态属性</description></item>
            /// <item><description>索引器属性</description></item>
            /// </list>
            /// </remarks>
            public IEnumerable<PropertyDetails> GetPropertyDetails(Type type, BindingFlags bindingFlags = default)
            {
                if (bindingFlags == default)
                {
                    bindingFlags |= AllTypeBindingFlags;
                }
                return type.GetProperties(bindingFlags).Select(p => new PropertyDetails
                {
                    Name = p.Name,
                    PropertyType = p.PropertyType,
                    CanRead = p.CanRead,
                    CanWrite = p.CanWrite
                });
            }

            /// <summary>
            /// 获取实例中指定事件
            /// </summary>
            /// <param name="instance">实例</param>
            /// <param name="eventName">事件名称</param>
            /// <returns><inheritdoc cref="EventInfo"/></returns>
            public EventInfo GetInstanceEvent(object instance, string eventName)
            {
                return instance.GetType().GetEvent(eventName);
            }

            /// <summary>
            /// 获取类型中定义的事件(仅当前类型直接定义的事件)
            /// </summary>
            /// <param name="typeName">完全限定类型名（包括命名空间）</param>
            /// <param name="bindingFlags">绑定约束标志</param>
            /// <returns>事件详细信息集合</returns>
            /// <exception cref="ArgumentNullException">当typeName为null或空时抛出</exception>
            /// <exception cref="TypeLoadException">当找不到指定类型时抛出</exception>
            /// <remarks>
            /// 示例用法：
            /// 注意：
            /// 1. 子类事件需要通过"+“符号指定，如"MyNamespace.Parent<see langword="+"/> Child"
            /// 2. 默认包含公共和非公共事件
            /// </remarks>
            public IEnumerable<EventDetails> GetEventsInType(string typeName, BindingFlags bindingFlags = default)
            {
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    throw new ArgumentNullException(nameof(typeName));
                }
                var type = Type.GetType(typeName) ?? Assembly.GetCallingAssembly().GetType(typeName) ?? throw new TypeLoadException($"Type {typeName} not found");
                if (bindingFlags == default)
                {
                    bindingFlags = AllTypeBindingFlags;
                }
                // 合并默认绑定标志
                var flags = bindingFlags | BindingFlags.DeclaredOnly;
                return type.GetEvents(flags).Select(e => new EventDetails
                {
                    Name = e.Name,
                    HandlerType = e.EventHandlerType,
                    IsStatic = (e.GetAddMethod(true)?.IsStatic ?? false) || (e.GetRemoveMethod(true)?.IsStatic ?? false),
                    AddMethod = e.GetAddMethod(true),
                    RemoveMethod = e.GetRemoveMethod(true)
                });
            }

            /// <summary>
            /// 获取类型及其所有基类中定义的事件（包括继承的事件并且自动去重）
            /// </summary>
            /// <param name="typeName">完全限定类型名</param>
            /// <param name="bindingFlags">绑定约束标志</param>
            /// <returns>包含继承事件在内的完整集合</returns>
            public IEnumerable<EventDetails> GetAllEventsInTypeHierarchy(string typeName, BindingFlags bindingFlags = default)
            {
                var type = Type.GetType(typeName) ?? Assembly.GetCallingAssembly().GetType(typeName) ?? throw new TypeLoadException($"Type {typeName} not found");
                var events = new List<EventDetails>();
                if (bindingFlags == default)
                {
                    bindingFlags = AllTypeBindingFlags;
                }
                // 遍历类型继承链
                while (type != null)
                {
                    var currentEvents = GetEventsInType(type.FullName, bindingFlags);
                    events.AddRange(currentEvents);
                    type = type.BaseType;
                }
                return events.Distinct(new EventDetailsComparer());
            }

            /// <summary>
            /// 获取当前实例中指定类的所有特性
            /// </summary>
            /// <param name="typeName">完全限定类型名</param>
            /// <returns></returns>
            public object[] GetCustomAttributes(string typeName)
            { 
               return GetTypeFromAssembly(typeName).GetCustomAttributes(true);
            }



            #endregion 

            /// <summary>
            /// 订阅/取消订阅事件
            /// </summary>
            /// <param name="instance">事件源对象,实例对象</param>
            /// <param name="eventInfo">事件类型,可通过<see cref="GetInstanceEvent"/>方法获取</param>
            /// <param name="delegateAction">委托事件处理方法,可通过<see cref="CreateDelegate"/>方法创建</param>
            /// <param name="isSubscribe">true为订阅，false为取消订阅</param>
            /// <exception cref="ArgumentException">当事件不存在时抛出</exception>
            [Obsolete]
            public void EventOperation(object instance, EventInfo eventInfo, Delegate delegateAction, bool isSubscribe = true)
            {
                if (isSubscribe)
                {
                    eventInfo.AddEventHandler(instance, delegateAction);
                }
                else
                {
                    eventInfo.RemoveEventHandler(instance, delegateAction);
                }
            }

           
            /// <summary>
            /// 销毁对象
            /// </summary>
            public void Dispose()
            {
                AssemblyObject = null;
                _typeCache.Clear();
                _methodCache.Clear();
                _instanceCache.Clear();
            }
        }

    }
}
