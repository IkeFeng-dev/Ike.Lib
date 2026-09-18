#if NETFRAMEWORK && !NET47

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using Expression = System.Linq.Expressions.Expression;

namespace Ike.Standard.WPF
{
    /// <summary>
    /// WPF ini配置映射数据结构
    /// </summary>
    class IniMapStructure
    {
        /// <summary>
        /// WPF控件对象
        /// </summary>
        internal FrameworkElement Control;

        /// <summary>
        /// 控件属性对象
        /// </summary>
        internal PropertyInfo ControlpPropertyInfo;

        /// <summary>
        /// 映射数据的属性对象
        /// </summary>
        internal PropertyInfo MapPropertyInfo;

        /// <summary>
        /// 控件事件对象
        /// </summary>
        private EventInfo ControlEventInfo;

        /// <summary>
        /// 控件事件委托
        /// </summary>
        private Delegate ControlEventDelegate;

        /// <summary>
        /// 添加当前控件委托事件并绑定事件
        /// </summary>
        internal void AddEventInfo(EventInfo eventInfo, Delegate eventDelegate)
        {
            ControlEventInfo = eventInfo;
            ControlEventDelegate = eventDelegate;
            // 绑定事件
            ControlEventInfo.AddEventHandler(Control, ControlEventDelegate);
        }

        /// <summary>
        /// 移除当前控件的委托事件
        /// </summary>
        internal void RemoveEventDelegate()
        {
            if (ControlEventInfo == null || ControlEventDelegate == null || Control == null)
            {
                return;
            }
            // 移除事件
            ControlEventInfo.RemoveEventHandler(Control, ControlEventDelegate);
        }
    }

    /// <summary>
    /// Ini配置与WPF控件映射管理器,实现WPF控件与INI配置的自动双向绑定
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
        /// [<see langword="2026-9-18 14:02:01" />]
        /// </remarks>
        public IniMap(string iniPath, bool fileAutoUpdate)
        {
            maps = new List<IniMapStructure>();
            iniConfig = new IniConfigManager<T>(iniPath, fileAutoUpdate);
            if (!File.Exists(iniPath))
            {
                iniConfig.Save();
            }

            if (fileAutoUpdate)
            {
                iniConfig.UpdateEvent += () =>
                {
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
        /// 析构函数
        /// </summary>
        ~IniMap()
        {
            Dispose(false);
        }

        /// <summary>
        /// 实现 <see cref="IDisposable"/>,释放该对象资源
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
                Type targetType = item.ControlpPropertyInfo.PropertyType;

                if (value != null && targetType.IsAssignableFrom(value.GetType()))
                {
                    SetPropertyValue(item.Control, item.ControlpPropertyInfo, value);
                    continue;
                }

                object convertedValue = ConvertValueToType(value, targetType);
                SetPropertyValue(item.Control, item.ControlpPropertyInfo, convertedValue);
            }
        }

        /// <summary>
        /// 调用WPF Dispatcher设置控件属性值
        /// </summary>
        private void SetPropertyValue(FrameworkElement control, PropertyInfo property, object value)
        {
            if (control == null) return;

            try
            {
                // 检查Dispatcher状态
                if (control.Dispatcher.HasShutdownStarted || control.Dispatcher.HasShutdownFinished)
                    return;

                if (!control.Dispatcher.CheckAccess())
                {
                    // WPF Dispatcher 异步调用
                    control.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        property.SetValue(control, value);
                    }));
                }
                else
                {
                    property.SetValue(control, value);
                }
            }
            catch (Exception) { /* 忽略 UI 已经关闭或卸载时的异常 */ }
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
                if (targetType.IsValueType && !IsNullableType(targetType))
                {
                    return Activator.CreateInstance(targetType);
                }
                return null;
            }

            if (targetType == typeof(string))
            {
                return value.ToString();
            }

            if (IsNullableType(targetType))
            {
                var underlyingType = Nullable.GetUnderlyingType(targetType);
                if (underlyingType != null)
                {
                    if (value is string strValue && string.IsNullOrEmpty(strValue))
                    {
                        return null;
                    }
                    return System.Convert.ChangeType(value, underlyingType);
                }
            }

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
        private void MapToData(FrameworkElement control)
        {
            foreach (var item in maps)
            {
                if (!ReferenceEquals(control, item.Control))
                    continue;

                object controlValue = GetControlValue(item);

                if (controlValue == null)
                {
                    if (item.MapPropertyInfo.PropertyType.IsValueType && !IsNullableType(item.MapPropertyInfo.PropertyType))
                    {
                        controlValue = Activator.CreateInstance(item.MapPropertyInfo.PropertyType);
                    }
                }
                else
                {
                    controlValue = ConvertValueToPropertyType(controlValue, item.MapPropertyInfo.PropertyType);
                }

                item.MapPropertyInfo.SetValue(Data, controlValue);
                return;
            }
        }

        /// <summary>
        /// 获取WPF控件属性的值，支持跨线程Dispatcher访问
        /// </summary>
        private object GetControlValue(IniMapStructure item)
        {
            if (item.Control == null) return null;

            object value = null;
            try
            {
                if (item.Control.Dispatcher.HasShutdownStarted || item.Control.Dispatcher.HasShutdownFinished)
                    return null;

                if (!item.Control.Dispatcher.CheckAccess())
                {
                    item.Control.Dispatcher.Invoke(new Action(() =>
                    {
                        value = item.ControlpPropertyInfo.GetValue(item.Control);
                    }));
                }
                else
                {
                    value = item.ControlpPropertyInfo.GetValue(item.Control);
                }
            }
            catch (Exception) { /* 忽略异常 */ }
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

            if (targetType.IsAssignableFrom(valueType))
                return value;

            if (value is string strValue)
            {
                if (string.IsNullOrEmpty(strValue))
                {
                    if (IsNullableType(targetType))
                        return null;

                    if (targetType == typeof(string))
                        return string.Empty;

                    if (targetType.IsValueType)
                        throw new InvalidCastException($"无法将空字符串转换为值类型: {targetType.Name}");
                }

                var converter = TypeDescriptor.GetConverter(targetType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return converter.ConvertFrom(strValue);
                }
            }

            if (IsNullableType(targetType))
            {
                Type underlyingType = Nullable.GetUnderlyingType(targetType);
                if (underlyingType != null)
                {
                    object innerValue = System.Convert.ChangeType(value, underlyingType);
                    return Activator.CreateInstance(targetType, innerValue);
                }
            }

            return System.Convert.ChangeType(value, targetType);
        }

        /// <summary>
        /// 关联绑定WPF控件与属性
        /// </summary>
        /// <param name="control">WPF控件对象 (如 TextBox, CheckBox 等)</param>
        /// <param name="controlPropertyName">控件读写显示的属性名，如 Text, IsChecked 等</param>
        /// <param name="mapPropertyName">映射的配置属性名称</param>
        /// <param name="eventName">控件值修改触发的事件名称，如 TextChanged, Checked, Unchecked 等</param>
        public void Binding(FrameworkElement control, string controlPropertyName, string mapPropertyName, string eventName)
        {
            if (string.IsNullOrWhiteSpace(controlPropertyName))
            {
                throw new ArgumentNullException(nameof(controlPropertyName));
            }
            if (string.IsNullOrWhiteSpace(mapPropertyName))
            {
                throw new ArgumentNullException(nameof(mapPropertyName));
            }

            PropertyInfo controlpPropertyInfo = control.GetType().GetProperty(controlPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            CheckObjectNull(controlpPropertyInfo, nameof(controlpPropertyInfo));
            CheckObjectNull(control, nameof(control));

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

            // 初始化数据到控件
            MapToControl();

            // 事件绑定处理
            if (!string.IsNullOrWhiteSpace(eventName))
            {
                var eventInfo = control.GetType().GetEvent(eventName);
                if (eventInfo == null)
                {
                    throw new Exception($"事件对象'{eventName}'获取为null");
                }

                // 核心突破点：为 WPF 多样化的 Event Delegate 适配，使用表达式树构建通用委托
                Delegate dynamicHandler = CreateDynamicEventHandler(eventInfo, (sender) =>
                {
                    if (_isUpdatingFromFile) return;

                    if (sender is FrameworkElement eventControl)
                    {
                        MapToData(eventControl);
                        iniConfig.Save();
                    }
                });

                item.AddEventInfo(eventInfo, dynamicHandler);
            }
        }

        /// <summary>
        /// 使用表达式树动态创建兼容 WPF 多种 Delegate (如 RoutedEventHandler, TextChangedEventHandler) 的事件委托
        /// </summary>
        private Delegate CreateDynamicEventHandler(EventInfo eventInfo, Action<object> callback)
        {
            Type handlerType = eventInfo.EventHandlerType;
            MethodInfo invokeMethod = handlerType.GetMethod("Invoke");
            ParameterInfo[] parameterInfos = invokeMethod.GetParameters();

            // 获取事件委托的所有入参 (例如 sender, e)
            ParameterExpression[] parameters = parameterInfos.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();

            // 构建调用 callback(sender) 的表达式 (sender 即第一个参数)
            MethodInfo actionInvoke = typeof(Action<object>).GetMethod("Invoke");
            MethodCallExpression body = Expression.Call(
                Expression.Constant(callback),
                actionInvoke,
                parameters[0] // 传入 sender
            );

            // 编译为目标事件委托类型
            return Expression.Lambda(handlerType, body, parameters).Compile();
        }

        /// <summary>
        /// 移除控件绑定
        /// </summary>
        /// <param name="control">控件对象</param>
        /// <returns>移除结果</returns>
        public bool RemoveBinding(FrameworkElement control)
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
        /// UI数据保存到文件
        /// </summary>
        public void SaveToFile()
        {
            foreach (var item in maps)
            {
                MapToData(item.Control);
            }
            iniConfig.Save();
        }


        /// <summary>
        /// 文件加载到UI
        /// </summary>
        public void LoadToUI()
        {
            iniConfig.Load();
            MapToControl();
        }
    }
}

#endif
