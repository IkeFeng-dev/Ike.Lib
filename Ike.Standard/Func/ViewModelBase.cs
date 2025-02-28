using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Ike.Standard
{
    /// <summary>
    /// WPF MVVM 模式,实现<see cref="INotifyPropertyChanged"/> 接口的基类
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 实现INotifyPropertyChanged接口,用于通知属性更新事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 通知属性更新
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 设置属性值并通知更改
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="field">字段引用</param>
        /// <param name="value">新值</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>如果值已更改则返回true，否则返回false</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        /// <summary>
        /// 手动调用此方法以触发属性更改通知
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        public void NotifyPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// 命令系统
        /// MVVM模式中，RelayCommand允许将命令的处理逻辑从视图模型中分离出来，使得视图模型不需要知道命令的具体执行逻辑，从而实现了视图模型和命令处理逻辑的解耦
        /// </summary>
        public class RelayCommand : ICommand
        {
            /// <summary>
            /// 当<see cref="CanExecute"/> 的返回值可能发生更改时，应引发此事件。WPF 或其他框架会自动订阅此事件，以在命令的可执行状态发生变化时更新 UI 元素的启用状态
            /// </summary>
            public event EventHandler CanExecuteChanged;

            private Action<object> excute { get; set; }

            private Predicate<object> canExecute { get; set; }


            /// <summary>
            /// 构造函数，初始化 <see cref="RelayCommand"/> 类的新实例
            /// </summary>
            /// <param name="excute">命令的执行逻辑, 该委托定义了命令被调用时需要执行的操作</param>
            /// <param name="canExecute">判断命令是否可以执行的逻辑, 该委托定义了命令是否可以在当前状态下执行。如果为 <see langword="null"/>，则默认认为命令始终可以执行。</param>
            public RelayCommand(Action<object> excute, Predicate<object> canExecute)
            {
                this.excute = excute;
                this.canExecute = canExecute;
            }

            /// <summary>
            /// <see cref="ICommand.CanExecute(object)"/> 方法通常用于确定命令是否可以执行
            /// 当命令绑定到UI元素（如Button）时，WPF 会自动调用 CanExecute 方法来确定该UI元素（如按钮）是否应该被启用（IsEnabled 属性为 true）或禁用（IsEnabled 属性为 false）
            /// </summary>
            /// <param name="parameter">此参数通常由 UI 元素传递，用于在执行命令时提供额外的上下文信息</param>
            /// <returns></returns>
            public bool CanExecute(object parameter = null)
            {
                if (canExecute == null)
                    return true;
                else
                    return canExecute(parameter);
            }

            /// <summary>
            /// 当调用此命令时，应执行的操作
            /// </summary>
            /// <param name="parameter">此参数通常由 UI 元素传递，用于在执行命令时提供额外的上下文信息</param>
            public void Execute(object parameter = null)
            {
                excute(parameter);
            }

            /// <summary>
            /// 手动触发<see cref="CanExecuteChanged"/> 事件
            /// </summary>
            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
