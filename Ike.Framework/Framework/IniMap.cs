using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Ike.Framework
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
        private readonly Standard.Ini.IniConfigManager<T> iniConfig;
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
        public Standard.Ini.IniConfigManager<T> IniConfigManager { get { return iniConfig; } }
        /// <summary>
        /// 构造映射操作对象
        /// </summary>
        /// <param name="iniPath">配置档路径</param>
        /// <param name="fileAutoUpdate">是否检测配置档外部修改自动更新</param>
        public IniMap(string iniPath, bool fileAutoUpdate)
        {
            maps = new List<IniMapStructure>();
            iniConfig = new Standard.Ini.IniConfigManager<T>(iniPath, fileAutoUpdate);
            if (!File.Exists(iniPath))
            { 
               iniConfig.Save();
            }
            // 判断是否监听文件修改
            if (fileAutoUpdate)
            {
                // 监听修改并更新到UI
                iniConfig.UpdateEvent += () => {
                    MapToControl();
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

                // 检查是否需要跨线程调用
                if (item.Control.InvokeRequired)
                {
                    // 在 UI 线程上执行设置操作
                    item.Control.Invoke(new Action(() =>
                    {
                        item.ControlpPropertyInfo.SetValue(item.Control, value);
                    }));
                }
                else
                {
                    // 如果已经在 UI 线程，直接设置
                    item.ControlpPropertyInfo.SetValue(item.Control, value);
                }
            }
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
                if (ReferenceEquals(control, item.Control))
                {
                    object value = string.Empty;
                    // 检查是否需要跨线程调用
                    if (item.Control.InvokeRequired)
                    {
                        // 在 UI 线程上执行设置操作
                        item.Control.Invoke(new Action(() =>
                        {
                            value = item.ControlpPropertyInfo.GetValue(item.Control);
                        }));
                    }
                    else
                    {
                        // 如果已经在 UI 线程，直接设置
                        value = item.ControlpPropertyInfo.GetValue(item.Control);
                    }
                    if (value == null)
                    {
                        throw new Exception($"获取控件属性值'{item.ControlpPropertyInfo.Name}'为null");
                    }
                    item.MapPropertyInfo.SetValue(Data, value);
                    return;
                }

            }
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
                        if (sender is Control eventControl && !eventControl.IsDisposed)
                        {
                            //映射控件数据到数据类中
                            MapToData(eventControl);
                            //保存数据类修改到配置档文件
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

