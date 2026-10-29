using System.Windows;

namespace Ike.ActionEngine
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 重写 OnStartup 方法，当应用启动时会自动执行这里面的代码
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            Standard.WPF.GlobalException.Initialize(GlobalVariable.LogDir);
            base.OnStartup(e);
        }
    }
}
