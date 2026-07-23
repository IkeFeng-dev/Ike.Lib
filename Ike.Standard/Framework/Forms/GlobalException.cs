#if NETFRAMEWORK
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Ike.Standard.Forms
{
    /// <summary>
    /// 全局异常捕获
    /// </summary>
    public static class GlobalException
    {
        /// <summary>
        /// 表示是否初始化标志
        /// </summary>
        private static bool _initialized = false;
        
        /// <summary>
        /// 异常LOG储存目录
        /// </summary>
        private static string _logDirectory = null;

        /// <summary>
        /// 初始化全局异常处理
        /// </summary>
        /// <param name="logDirectory">日志目录（null表示使用当前目录）</param>
        public static void Initialize(string logDirectory = null)
        {
            if (_initialized) return;

            _logDirectory = logDirectory ?? AppDomain.CurrentDomain.BaseDirectory;

            // 设置全局异常处理
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
            
            _initialized = true;
        }

        /// <summary>
        /// 捕获整个应用程序域未被捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            LogException(ex, $"应用程序域异常[终止: {e.IsTerminating}]");
        }


        /// <summary>
        /// 捕获UI线程异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogException(e.Exception, "UI线程异常");
        }


        /// <summary>
        /// 记录异常LOG
        /// </summary>
        /// <param name="ex">捕获的异常</param>
        /// <param name="source">说明</param>
        private static void LogException(Exception ex, string source)
        {
            try
            {
                string logFile = Path.Combine(_logDirectory, $"{DateTime.Now:yyyyMMdd}_Error.Log");
                string logContent = BuildLogContent(ex, source);
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
                File.AppendAllText(logFile, logContent);
            }
            catch {}
        }


        /// <summary>
        /// 拼接LOG内容
        /// </summary>
        /// <param name="ex">捕获的异常</param>
        /// <param name="source">说明</param>
        /// <returns></returns>
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