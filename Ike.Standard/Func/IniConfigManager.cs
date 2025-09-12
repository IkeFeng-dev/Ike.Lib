using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ike.Standard.Ini
{
    /// <summary>
    /// 定义节名特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IniSectionAttribute : Attribute
    {
        /// <summary>
        /// 节点名称
        /// </summary>
        public string SectionName { get; }
        /// <summary>
        /// 定义节名特性
        /// </summary>
        /// <param name="sectionName"></param>
        public IniSectionAttribute(string sectionName)
        {
            SectionName = sectionName;
        }
    }

    /// <summary>
    /// 重定向节点键名特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IniKeyAttribute : Attribute
    {
        /// <summary>
        /// 键名称
        /// </summary>
        public string KeyName { get; }
        /// <summary>
        /// 重定向节点键名特性
        /// </summary>
        /// <param name="keyName"></param>
        public IniKeyAttribute(string keyName)
        {
            KeyName = keyName;
        }
    }

    /// <summary>
    /// Ini配置档读写器
    /// </summary>
    /// <typeparam name="T">配置档实例</typeparam>
    public class IniConfigManager<T> where T : new()
    {
        /// <summary>
        /// 配置档对象实例,每次调用<see cref="Load()"/>时会同步更新此实例,调用<see cref="Save()"/>则保持此实例到文件
        /// </summary>
        public T Data { get; private set; }
        /// <summary>
        /// 数组分隔符
        /// </summary>
        private readonly char split = '|';
        /// <summary>
        /// 配置档路径
        /// </summary>
        private readonly string _filePath;
        /// <summary>
        /// 属性映射关系
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, PropertyInfo>> _sectionMappings;
        /// <summary>
        /// 配置文档读写编码
        /// </summary>
        private readonly Encoding encoding;
        /// <summary>
        /// 支持的数据类型
        /// </summary>
        private readonly Type[] types = new Type[]
            {
                // 基本类型
                typeof(string),
                typeof(int),
                typeof(long),
                typeof(short),
                typeof(decimal),
                typeof(bool),
                typeof(double),
                typeof(float),
                typeof(byte),
                typeof(char),
                typeof(DateTime),
                typeof(TimeSpan),
                typeof(Guid),
                typeof(Enum),
                
                // 数组类型
                typeof(int[]),
                typeof(long[]),
                typeof(short[]),
                typeof(double[]),
                typeof(float[]),
                typeof(decimal[]),
                typeof(bool[]),
                typeof(byte[]),
                typeof(char[]),
                typeof(DateTime[]),
                typeof(TimeSpan[]),
                typeof(Guid[]),
                typeof(string[]),
                typeof(Enum[]),
            };

        /// <summary>
        /// INI配置管理器构造函数
        /// <example>
        /// <para>1. ini配置文档结构
        /// <code>
        /// [Test]
        /// IP=127.0.0.1
        /// Port=8080
        /// Open=True
        /// TestIntArr = 2,4,6,8,12,34,45,0,-1
        /// 
        /// [TestKey]
        /// KeyName=The Key in the file is KeyName
        /// </code>
        /// </para>
        /// <para>2. 定义配置类(结构与Ini保持文档一致)：</para>
        /// <code>
        /// public class IniData
        /// {
        ///     [IniSection("Test")]
        ///     <see langword="public"/> string IP <see langword="get"/>; <see langword="set"/>; } = "127.0.0.1";
        /// 
        ///     [IniSection("Test")]
        ///     <see langword="public"/> int Port { <see langword="get"/>; <see langword="set"/>; } = 5200;
        /// 
        ///     [IniSection("Test")]
        ///     <see langword="public"/> bool Open { <see langword="get"/>; <see langword="set"/>; } = true;
        /// 
        ///     [IniSection("Test")]
        ///     <see langword="public"/> int[] TestIntArr { <see langword="get"/>; <see langword="set"/>; } = new int[] { 2, 4, 6, 8, 12, 34, 45, 0, -1 };
        /// 
        ///     [IniSection("TestKey")]
        ///     [IniKey("KeyName")]
        ///     <see langword="public"/> string Temp { <see langword="get"/>; <see langword="set"/>; } = "The Key in the file is KeyName";
        /// }
        /// </code>
        /// 
        /// <para>3. 使用示例：</para>
        /// <code>
        /// // 初始化配置管理器
        /// IniConfigManager&lt;IniData&gt; configManager = new IniConfigManager&lt;IniData&gt;("F:\\Desktop\\test.ini");
        /// // 加载配置
        /// configManager.Load();
        /// // 修改配置数据
        /// configManager.Data.Port = 5555;
        /// // 保存配置
        /// configManager.Save();
        /// 
        ///  <para>4. 自动更新：</para>
        ///  如果实例类实现了INotifyPropertyChanged接口,则更新字段时自动写入文档
        /// 
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="filePath">配置档路径</param>
        /// <param name="encoding">配置档读写编码,默认使用<see cref="Encoding.UTF8"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public IniConfigManager(string filePath, Encoding encoding = default)
        {
            this.encoding = encoding ?? Encoding.UTF8;
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _sectionMappings = BuildSectionMappings();

            Load();
            // 如果T实现了INotifyPropertyChanged，注册事件
            if (Data is INotifyPropertyChanged notifyObj)
            {
                notifyObj.PropertyChanged += (sender, args) =>
                {
                    // 属性变更时自动保存
                    Save();
                };
            }

        }


        /// <summary>
        /// 构建INI文件节与类属性的映射关系
        /// </summary>
        private Dictionary<string, Dictionary<string, PropertyInfo>> BuildSectionMappings()
        {
            var mappings = new Dictionary<string, Dictionary<string, PropertyInfo>>(StringComparer.OrdinalIgnoreCase);
            foreach (var prop in typeof(T).GetProperties())
            {
                // 获取属性所在节的名称（默认为类名）
                var sectionName = typeof(T).Name;
                // 检查是否有自定义节名特性
                var sectionAttr = prop.GetCustomAttribute<IniSectionAttribute>();
                if (sectionAttr != null)
                {
                    sectionName = sectionAttr.SectionName;
                }
                // 获取键名（默认为属性名）
                var keyName = prop.Name;
                var keyAttr = prop.GetCustomAttribute<IniKeyAttribute>();
                if (keyAttr != null)
                {
                    keyName = keyAttr.KeyName;
                }
                // 添加到映射字典
                if (!mappings.ContainsKey(sectionName))
                {
                    mappings[sectionName] = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
                }
                mappings[sectionName][keyName] = prop;
            }
            return mappings;
        }

        

        /// <summary>
        /// 获取属性值并格式化为INI文件可接受的字符串
        /// </summary>
        private string GetPropertyValueForIni(PropertyInfo property, T config)
        {
            var value = property.GetValue(config);
            if (value == null)
            {
                return string.Empty;
            }
            // 处理数组类型
            if (property.PropertyType.IsArray)
            {
                return Convert.ConvertArrayToString(value, split.ToString());
            }
            // 处理其他类型
            return value.ToString();
        }


        /// <summary>
        /// 设置属性值（支持类型转换）
        /// </summary>
        private void SetPropertyValue(T config, PropertyInfo property, string value)
        {
            if (property.PropertyType == typeof(string))
            {
                property.SetValue(config, value);
            }
            else if (property.PropertyType == typeof(int))
            {
                property.SetValue(config, int.Parse(value));
            }
            else if (property.PropertyType == typeof(long))
            {
                property.SetValue(config, long.Parse(value));
            }
            else if (property.PropertyType == typeof(short))
            {
                property.SetValue(config, short.Parse(value));
            }
            else if (property.PropertyType == typeof(decimal))
            {
                property.SetValue(config, decimal.Parse(value));
            }
            else if (property.PropertyType == typeof(bool))
            {
                property.SetValue(config, Convert.ConvertToBoolean(value));
            }
            else if (property.PropertyType == typeof(double))
            {
                property.SetValue(config, double.Parse(value));
            }
            else if (property.PropertyType == typeof(float))
            {
                property.SetValue(config, float.Parse(value));
            }
            else if (property.PropertyType == typeof(byte))
            {
                property.SetValue(config, byte.Parse(value));
            }
            else if (property.PropertyType == typeof(char))
            {
                property.SetValue(config, char.Parse(value));
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                property.SetValue(config, DateTime.Parse(value));
            }
            else if (property.PropertyType == typeof(TimeSpan))
            {
                property.SetValue(config, TimeSpan.Parse(value));
            }
            else if (property.PropertyType == typeof(Guid))
            {
                property.SetValue(config, Guid.Parse(value));
            }
            else if (property.PropertyType.IsEnum)
            {
                property.SetValue(config, Enum.Parse(property.PropertyType, value, true));
            }
            // 可以继续添加其他类型的支持...
            else if (property.PropertyType.IsArray)
            {
                if (property.PropertyType == typeof(int[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, int.Parse));
                }
                else if (property.PropertyType == typeof(long[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, long.Parse));
                }
                else if (property.PropertyType == typeof(short[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, short.Parse));
                }
                else if (property.PropertyType == typeof(double[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, double.Parse));
                }
                else if (property.PropertyType == typeof(float[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, float.Parse));
                }
                else if (property.PropertyType == typeof(decimal[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, decimal.Parse));
                }
                else if (property.PropertyType == typeof(bool[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, Convert.ConvertToBoolean));
                }
                else if (property.PropertyType == typeof(byte[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, byte.Parse));
                }
                else if (property.PropertyType == typeof(char[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, char.Parse));
                }
                else if (property.PropertyType == typeof(DateTime[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, DateTime.Parse));
                }
                else if (property.PropertyType == typeof(TimeSpan[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, TimeSpan.Parse));
                }
                else if (property.PropertyType == typeof(Guid[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, Guid.Parse));
                }
                else if (property.PropertyType == typeof(string[]))
                {
                    property.SetValue(config, Array.ConvertArray(value, split, s => s));
                }
                else if (property.PropertyType.IsArray && property.PropertyType.GetElementType().IsEnum)
                {
                    Type enumType = property.PropertyType.GetElementType();
                    property.SetValue(config, Array.ConvertArray(value, split, s => Enum.Parse(enumType, s, true)));
                }
                // 可以继续添加其他数组类型的支持...
            }
        }




        //private bool IsGenericCollection(Type type)
        //{
        //    if (!type.IsGenericType) return false;

        //    Type genericType = type.GetGenericTypeDefinition();
        //    return genericType == typeof(List<>) ||
        //           genericType == typeof(IList<>) ||
        //           genericType == typeof(ICollection<>) ||
        //           genericType == typeof(IEnumerable<>);
        //}



        /// <summary>
        /// 支持的数据类型
        /// </summary>
        /// <returns>数据类型集合</returns>
        public Type[] SupportedTypes() => types;


        /// <summary>
        /// 保存配置到INI文件
        /// </summary>
        /// <param name="config">配置档实例对象</param>
        public void Save(T config)
        {
            var sb = new StringBuilder();
            // 按节分组属性
            var sections = _sectionMappings.Keys.OrderBy(s => s);
            foreach (var section in sections)
            {
                sb.AppendLine($"[{section}]");

                var props = _sectionMappings[section];
                foreach (var kvp in props)
                {
                    var value = GetPropertyValueForIni(kvp.Value, config);
                    sb.AppendLine($"{kvp.Key}={value}");
                }
                sb.AppendLine();
            }
            File.WriteAllText(_filePath, sb.ToString(), encoding);
        }


        /// <summary>
        /// 保存当前配置档实例到INI文件
        /// </summary>
        public void Save()
        {
            Save(Data);
        }

        /// <summary>
        /// 从指定INI文件加载配置
        /// </summary>
        /// <param name="filePath">配置档路径</param>
        /// <returns>配置档实例对象</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public T Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("INI file not found", filePath);
            }
            var config = new T();
            var currentSection = string.Empty;
            var lines = File.ReadAllLines(filePath, encoding);
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // 跳过空行和注释
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";") || trimmedLine.StartsWith("#"))
                {
                    continue;
                }
                // 处理节
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    continue;
                }
                // 处理键值对
                var equalSignIndex = trimmedLine.IndexOf('=');
                if (equalSignIndex <= 0)
                {
                    continue;
                }
                var key = trimmedLine.Substring(0, equalSignIndex).Trim();
                var value = trimmedLine.Substring(equalSignIndex + 1).Trim();
                // 查找对应的属性并设置值
                if (_sectionMappings.TryGetValue(currentSection, out var sectionProps) && sectionProps.TryGetValue(key, out var property))
                {
                    SetPropertyValue(config, property, value);
                }
            }

            return config;
        }

        /// <summary>
        /// 从当前绑定的INI文件加载配置
        /// </summary>
        /// <returns></returns>
        public T Load()
        {
            Data = Load(_filePath);
            return Data;
        }

    }
}
