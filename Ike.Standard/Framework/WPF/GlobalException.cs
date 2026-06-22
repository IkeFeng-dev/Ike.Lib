#if NETFRAMEWORK && !NET47
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Ike.Framework.WPF
{
    /// <summary>
    /// 全局异常捕获
    /// </summary>
    public static class GlobalException
    {
        /// <summary>
        /// 初始化标志
        /// </summary>
        private static bool _initialized = false;
        /// <summary>
        /// LOG日志输出目录
        /// </summary>
        private static string _logDirectory = null;
        /// <summary>
        /// 表示是否阻止应用程序崩溃，UI 继续运行
        /// </summary>
        private static bool _handled = true;

        /// <summary>
        /// 初始化全局异常处理,必须在 <see cref="Application"/> 对象已经创建后调用此方法,例如在<see langword="App"/>.<see cref="Application.OnStartup(StartupEventArgs)"/> 中调用
        /// </summary>
        /// <param name="logDirectory">日志目录（<see langword="null"/> 表示使用当前目录）</param>
        /// <param name="handled">表示是否阻止应用程序崩溃，UI 继续运行</param>
        public static void Initialize(string logDirectory = null,bool handled = true)
        {
            if (_initialized) return;

            _logDirectory = logDirectory ?? AppDomain.CurrentDomain.BaseDirectory;
            _handled = handled;

            // 设置全局异常处理（所有线程）
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 设置 WPF UI 线程异常处理
            if (Application.Current != null)
            {
                Application.Current.DispatcherUnhandledException += Dispatcher_UnhandledException;
            }
            else
            {
                throw new InvalidOperationException( "GlobalException.Initialize 必须在 Application 对象创建后调用，建议在 App.xaml.cs 的 OnStartup 方法中调用");
            }

            _initialized = true;
        }

        /// <summary>
        /// 捕获 UI 线程异常
        /// </summary>
        private static void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception, "WPF UI线程异常");
            e.Handled = _handled;
        }

        /// <summary>
        /// 捕获整个应用程序域未被捕获的异常
        /// </summary>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogException(ex, $"应用程序域异常[终止: {e.IsTerminating}]");
        }

        private static void LogException(Exception ex, string source)
        {
            try
            {
                string logFile = Path.Combine(_logDirectory, $"{DateTime.Now:yyyyMMdd}_Error.log");
                string logContent = BuildLogContent(ex, source);
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
                File.AppendAllText(logFile, logContent);
            }
            catch { }
        }

        private static string BuildLogContent(Exception ex, string source)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{source}]\n" +
                   $"异常类型：{ex?.GetType().FullName ?? "Unknown"}\n" +
                   $"异常消息：{ex?.Message ?? "NA"}\n" +
                   $"堆栈跟踪：\n{ex?.StackTrace ?? "NA"}\n" +
                   $"----------------------------------------\n";
        }
    }
}

#endif