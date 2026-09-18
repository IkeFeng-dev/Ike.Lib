#if NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Ike.Standard
{

    /// <summary>
    /// ini配置映射数据结构
    /// </summary>
    class IniMapStructure
    {
        /// <summary>
        /// 控件对象
        /// </summary>
        internal Control Control;
        /// <summary>
        /// 控件属性对象
        /// </summary>
        internal PropertyInfo ControlpPropertyInfo;
        /// <summary>
        /// 映射数据的属性对象
        /// </summary>
        internal PropertyInfo MapPropertyInfo;
        /// <summary>
        /// 控件事件对象,储存控件事件绑定信息以便后续移除使用
        /// </summary>
        private EventInfo ControlEventInfo;
        /// <summary>
        /// 控件事件委托,储存控件事件绑定信息以便后续移除使用
        /// </summary>
        private Delegate ControlEventDelegate;
        /// <summary>
        /// 添加当前控件委托事件并绑定事件
        /// </summary>
        /// <param name="eventInfo">控件事件对象</param>
        /// <param name="eventDelegate">控件事件委托</param>
        internal void AddEventInfo(EventInfo eventInfo,Delegate eventDelegate)
        { 
            ControlEventInfo = eventInfo;
            ControlEventDelegate = eventDelegate;
            // 绑定事件
            ControlEventInfo.AddEventHandler(Control, ControlEventDelegate);
        }

        /// <summary>
        /// 移除当前控件的委托事件
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        internal void RemoveEventDelegate() 
        {
            if (ControlEventInfo == null || ControlEventDelegate == null)
            {
                return;
            }
            // 移除事件
            ControlEventInfo.RemoveEventHandler(Control, ControlEventDelegate);
        }
    }


    /// <summary>
    /// Ini配置与控件映射管理器,实现WinForms控件与INI配置的自动双向绑定
    /// </summary>
    /// <typeparam name="T">INI配置类</typeparam>
    public class IniMap<T> : IDisposable where T : new()
    {
        /// <summary>
        /// 是否已释放对象
        /// </summary>
        private bool disposed = false;
        /// <summary>
        /// 配置当操作对象
        /// </summary>
        private readonly IniConfigManager<T> iniConfig;
        /// <summary>
        /// 数据映射对象关联
        /// </summary>
        private readonly List<IniMapStructure> maps;
        /// <summary>
        /// 获取配置档数据对象
        /// </summary>
        public T Data { get { return iniConfig.Data; } }
        /// <summary>
        /// 获取配置档操作对象实例
        /// </summary>
        public IniConfigManager<T> IniConfigManager { get { return iniConfig; } }
        /// <summary>
        /// 防止控件事件与文件更新互相触发的重入锁
        /// </summary>
        private volatile bool _isUpdatingFromFile = false;
        /// <summary>
        /// 构造映射操作对象
        /// </summary>
        /// <param name="iniPath">配置档路径</param>
        /// <param name="fileAutoUpdate">是否检测配置档外部修改自动更新</param>
        /// <remarks>
        /// [<see langword="2026-9-18 10:50:14" />]
        /// </remarks>
        public IniMap(string iniPath, bool fileAutoUpdate)
        {
            maps = new List<IniMapStructure>();
            iniConfig = new IniConfigManager<T>(iniPath, fileAutoUpdate);
            if (!File.Exists(iniPath))
            { 
               iniConfig.Save();
            }
            // 判断是否监听文件修改
            if (fileAutoUpdate)
            {
                // 监听修改并更新到UI
                iniConfig.UpdateEvent += () =>
                {
                    // 标记是来自文件更新，避免控件事件回调触发Save
                    _isUpdatingFromFile = true;
                    try
                    {
                        MapToControl();
                    }
                    finally
                    {
                        _isUpdatingFromFile = false;
                    }
                };
            }
        }

        /// <summary>
        /// 析构函数（安全网）
        /// </summary>
        ~IniMap()
        {
            Dispose(false);
        }


        /// <summary>
        /// 实现 IDisposable,释放该对象资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放改对象资源
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                foreach (var item in maps)
                {
                    item.RemoveEventDelegate();
                }
                maps.Clear();
                iniConfig.Dispose();
            }
            disposed = true;
        }




        /// <summary>
        /// 检测对象是否为null,如果是则抛出异常
        /// </summary>
        /// <param name="obj">数据对象</param>
        /// <param name="name">名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        private void CheckObjectNull(object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }


        /// <summary>
        /// 映射数据到控件显示
        /// </summary>
        /// <exception cref="Exception">映射属性为null</exception>
        private void MapToControl()
        {
            foreach (var item in maps)
            {
                object value = item.MapPropertyInfo.GetValue(Data);

                // 获取控件属性需要的类型
                Type targetType = item.ControlpPropertyInfo.PropertyType;

                // 如果已经是目标类型，直接赋值
                if (value != null && targetType.IsAssignableFrom(value.GetType()))
                {
                    SetPropertyValue(item.Control, item.ControlpPropertyInfo, value);
                    continue;
                }

                // 转换值到目标类型
                object convertedValue = ConvertValueToType(value, targetType);

                // 设置转换后的值
                SetPropertyValue(item.Control, item.ControlpPropertyInfo, convertedValue);
            }
        }


        /// <summary>
        /// 调用UI线程设置控件属性值
        /// </summary>
        /// <param name="control">控件</param>
        /// <param name="property">属性</param>
        /// <param name="value">值</param>
        private void SetPropertyValue(Control control, PropertyInfo property, object value)
        {
            // 控件已释放则跳过
            if (control == null || control.IsDisposed) return;
            try
            {
                if (control.InvokeRequired)
                {
                    // 使用 BeginInvoke 异步调用，避免死锁
                    control.BeginInvoke(new Action(() =>
                    {
                        if (!control.IsDisposed)
                        {
                            property.SetValue(control, value);
                        }
                    }));
                }
                else
                {
                    property.SetValue(control, value);
                }
            }
            catch (ObjectDisposedException) { /* 控件已释放，忽略 */ }
            catch (InvalidOperationException) { /* 句柄未创建，忽略 */ }
        }


        /// <summary>
        /// 转换值类型
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>转换后的值</returns>
        private object ConvertValueToType(object value, Type targetType)
        {
            if (value == null)
            {
                // 处理null值
                if (targetType.IsValueType && !IsNullableType(targetType))
                {
                    // 值类型的默认值
                    return Activator.CreateInstance(targetType); 
                }
                return null;
            }

            // 特殊处理：字符串类型
            if (targetType == typeof(string))
            {
                return value.ToString();
            }

            // 处理可空类型
            if (IsNullableType(targetType))
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                if (underlyingType != null)
                {
                    // 如果值是空字符串，返回null
                    if (value is string strValue && string.IsNullOrEmpty(strValue))
                    {
                        return null;
                    }

                    // 尝试转换到可空类型的底层类型
                    return System.Convert.ChangeType(value, underlyingType);
                }
            }

            // 尝试通用的类型转换（这里会抛出异常如果转换失败）
            return System.Convert.ChangeType(value, targetType);
        }


        /// <summary>
        /// 判断可空类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>返回是否为可空类型</returns>
        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }


        /// <summary>
        /// 控件值映射到数据结构
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <exception cref="Exception"></exception>
        private void MapToData(Control control)
        {
            foreach (var item in maps)
            {
                if (!ReferenceEquals(control, item.Control))
                    continue;

                object controlValue = GetControlValue(item);

                if (controlValue == null)
                {
                    // 根据目标属性类型处理null值
                    if (item.MapPropertyInfo.PropertyType.IsValueType && !IsNullableType(item.MapPropertyInfo.PropertyType))
                    {
                        // 对于非可空值类型，使用默认值
                        controlValue = Activator.CreateInstance(item.MapPropertyInfo.PropertyType);
                    }
                }
                else
                {
                    // 如果需要类型转换
                    controlValue = ConvertValueToPropertyType(controlValue, item.MapPropertyInfo.PropertyType);
                }

                item.MapPropertyInfo.SetValue(Data, controlValue);
                return;
            }
        }


        /// <summary>
        /// 获取控件属性的值,处理跨线程调用
        /// </summary>
        /// <param name="item">结构对象</param>
        /// <returns>获取的数据</returns>
        /// <remarks>
        /// [<see langword="2026-9-18 10:49:42" />]
        /// </remarks>
        private object GetControlValue(IniMapStructure item)
        {
            if (item.Control == null || item.Control.IsDisposed) return null;

            object value = null;
            try
            {
                if (item.Control.InvokeRequired)
                {
                    item.Control.Invoke(new Action(() =>
                    {
                        if (!item.Control.IsDisposed)
                            value = item.ControlpPropertyInfo.GetValue(item.Control);
                    }));
                }
                else
                {
                    value = item.ControlpPropertyInfo.GetValue(item.Control);
                }
            }
            catch (ObjectDisposedException) { /* 忽略 */ }
            return value;
        }



        /// <summary>
        /// 将值转换为数据属性的类型
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>转换后的值</returns>
        /// <exception cref="InvalidCastException"></exception>
        private object ConvertValueToPropertyType(object value, Type targetType)
        {
            if (value == null)
                return null;

            Type valueType = value.GetType();

            // 如果类型兼容，直接返回
            if (targetType.IsAssignableFrom(valueType))
                return value;

            // 特殊处理：字符串到其他类型
            if (value is string strValue)
            {
                // 处理空字符串
                if (string.IsNullOrEmpty(strValue))
                {
                    if (IsNullableType(targetType))
                        return null;

                    if (targetType == typeof(string))
                        return string.Empty;

                    if (targetType.IsValueType)
                        throw new InvalidCastException($"无法将空字符串转换为值类型: {targetType.Name}");
                }
                // 尝试使用TypeConverter转换
                var converter = TypeDescriptor.GetConverter(targetType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return converter.ConvertFrom(strValue);
                }
            }

            // 处理可空类型
            if (IsNullableType(targetType))
            {
                Type underlyingType = Nullable.GetUnderlyingType(targetType);
                if (underlyingType != null)
                {
                    object innerValue = System.Convert.ChangeType(value, underlyingType);
                    return Activator.CreateInstance(targetType, innerValue);
                }
            }
            // 通用转换
            return System.Convert.ChangeType(value, targetType);
        }




        /// <summary>
        /// 关联绑定控件与属性
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <param name="controlPropertyName">控件读写显示的属性,使用<see  langword="nameof()"/> 获取属性名</param>
        /// <param name="mapPropertyName">映射的属性名称,使用<see  langword="nameof()"/> 获取属性名,注:实际对象只能是属性,如果是字段则无法进行对象反射</param>
        /// <param name="eventName">控件值修改触的事件名称,使用<see  langword="nameof()"/> 获取事件属性名,如果传递空字符串或者<see langword="null"/> 时不会进行控件事件监听,则无法实现即时更新数据到配置档,并且事件源只能为<see cref="EventInfo"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        /// <remarks>
        /// [<see langword="2026-9-18 10:55:47" />]
        /// </remarks>
        public void Binding(Control control, string controlPropertyName, string mapPropertyName, string eventName)
        {
            if (string.IsNullOrWhiteSpace(controlPropertyName))
            {
                throw new ArgumentNullException(nameof(controlPropertyName));
            }
            if (string.IsNullOrWhiteSpace(mapPropertyName))
            {
                throw new ArgumentNullException(nameof(mapPropertyName));
            }
            if (control.IsDisposed)
            {
                throw new Exception("该控件已经被释放");
            }
            PropertyInfo controlpPropertyInfo = control.GetType().GetProperty(controlPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            CheckObjectNull(controlpPropertyInfo, nameof(controlpPropertyInfo));
            CheckObjectNull(control, nameof(control));
            CheckObjectNull(mapPropertyName, nameof(mapPropertyName));
            if (maps.Any(i => ReferenceEquals(i.Control, control)))
            {
                throw new Exception("无法重复绑定控件对象");
            }
            Type type = typeof(T);
            PropertyInfo mapPropertyInfo = type.GetProperty(mapPropertyName) ?? throw new Exception($"'{mapPropertyName}'属性对象获取为null");

            IniMapStructure item = new IniMapStructure()
            {
                Control = control,
                ControlpPropertyInfo = controlpPropertyInfo,
                MapPropertyInfo = mapPropertyInfo
            };
            maps.Add(item);
            MapToControl();
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                var eventInfo = control.GetType().GetEvent(eventName);
                if (eventInfo == null)
                {
                    throw new Exception($"事件对象'{eventName}'获取为null");
                }
                else
                {
                    // 创建事件处理程序
                    EventHandler handler = (sender, e) =>
                    {
                        // 文件更新触发UI时，忽略控件事件
                        if (_isUpdatingFromFile) return;  

                        if (sender is Control eventControl && !eventControl.IsDisposed)
                        {
                            MapToData(eventControl);
                            iniConfig.Save();
                        }
                    };
                    // 保存绑定信息
                    item.AddEventInfo(eventInfo, handler);
                }
            }
        }
        



        /// <summary>
        /// 移除绑定
        /// </summary>
        /// <param name="control">要移除的控件对象</param>
        /// <returns>移除结果</returns>
        public bool RemoveBinding(Control control)
        {
            IniMapStructure temp = null;
            foreach (var item in maps)
            {
                if (ReferenceEquals(control, item.Control))
                {
                    temp = item;
                    break;
                }
            }
            if (temp == null)
            {
                return false;
            }
            temp.RemoveEventDelegate();
            maps.Remove(temp);
            return true;
        }


        /// <summary>
        /// 保存当前控件映射值到配置档文件中
        /// </summary>
        public void SaveToFile()
        {
            foreach (var item in maps)
            { 
                //映射控件数据到数据类中
                MapToData(item.Control);
            }
            //保存数据类修改到配置档文件
            iniConfig.Save();
        }



        /// <summary>
        /// 加载配置档文件中数据到控件中显示
        /// </summary>
        public void LoadToUI()
        {
            iniConfig.Load();
            MapToControl();
        }

    }
}

#endif