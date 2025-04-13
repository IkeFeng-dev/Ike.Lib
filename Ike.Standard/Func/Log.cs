using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Ike.Standard
{
	/// <summary>
	/// 日志类
	/// </summary>
	public class Log
	{
        /// <summary>
        /// 表示日志的类型
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// 详细信息,记录详细的调试信息或其他较为冗长的输出
            /// </summary>
            Verbose,
            /// <summary>
            /// 调试信息,用于诊断和调试目的
            /// </summary>
            Debug,
            /// <summary>
            /// 提供程序状态信息,用于跟踪程序的执行情况
            /// </summary>
            Information,
            /// <summary>
            /// 表示成功
            /// </summary>
            Succeed,
            /// <summary>
            /// 警告信息,表示潜在的问题或不符合预期的情况,但不会影响程序的正常执行
            /// </summary>
            Warning,
            /// <summary>
            /// 错误信息,用于指示程序出现的错误,但不会导致程序终止执行
            /// </summary>
            Error,
            /// <summary>
            /// 严重错误信息,用于指示程序出现的严重错误,可能会导致程序终止执行或无法继续正常运行
            /// </summary>
            Critical
        }

        /// <summary>
        /// 日志锁
        /// </summary>
        private readonly object _lock = new { };
		/// <summary>
		/// 当前日志文件的文件流
		/// </summary>
		private FileStream _logFileStream;
		/// <summary>
		/// 当前日志文件流的写入流
		/// </summary>
		private StreamWriter _logStreamWriter;
		/// <summary>
		/// 控制台日志实例,同步输出需要设置属性<see cref="OutputToConsole"/>=<see langword="true"></see>
		/// </summary>
		public readonly Console.Log ConsoleLog = new Console.Log();
		/// <summary>
		/// 日志目录
		/// </summary>
		private readonly string targetDirectory;
		/// <summary>
		/// 获取或设置日志写入的时间戳格式
		/// </summary>
		public string TimestampFormat { get; set; }
		/// <summary>
		/// 获取或设置日志文件的文件名格式,根据格式设定文件有效周期
		/// </summary>
		public string FileNameFormat { get; set; }
		/// <summary>
		/// 获取或设置写入日志的最低级别
		/// </summary>
		public LogType RequiredLevel { get; set; } = LogType.Information;
		/// <summary>
		/// 获取或设置编码格式
		/// </summary>
		public Encoding EncodingFormat { get; set; } = Encoding.UTF8;
		/// <summary>
		/// 获取或设置是否同步输出到控制台
		/// </summary>
		public bool OutputToConsole { get; set; } = false;
		/// <summary>
		/// 获取当前日志文件的路径
		/// </summary>
		public string FilePath { get { return Path.Combine(targetDirectory, DateTime.Now.ToString(FileNameFormat) + ".log"); } }

		/// <summary>
		/// 构造实例
		/// </summary>
		/// <param name="timestampFormat">时间戳格式</param>
		/// <param name="fileNameFormat">文件名格式</param>
		/// <param name="targetDirectory">日志输出目录</param>
		public Log(string timestampFormat = "yyyy-MM-dd HH:mm:ss.fff", string fileNameFormat = "yyyy-MM-dd", string targetDirectory = "")
		{
			TimestampFormat = timestampFormat;
			FileNameFormat = fileNameFormat;
			ConsoleLog.TimestampFormat = TimestampFormat;
			if (string.IsNullOrEmpty(targetDirectory))
			{
				targetDirectory = Environment.CurrentDirectory;
			}
			else
			{
				this.targetDirectory = targetDirectory;
			}
			if (!Directory.Exists(targetDirectory))
			{
				Directory.CreateDirectory(targetDirectory);
			}
		}

		/// <summary>
		/// 创建文件流
		/// </summary>
		/// <param name="logPath">日志文件路径</param>
		private void CreateStream(string logPath)
		{
			if (_logFileStream is null)
			{
				_logFileStream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Read);
			}
			if (_logStreamWriter is null)
			{
				_logStreamWriter = new StreamWriter(_logFileStream, EncodingFormat)
				{
					AutoFlush = true
				};
			}
		}

		/// <summary>
		/// 写入日志
		/// </summary>
		/// <param name="log">日志内容</param>
		/// <param name="logType">日志类型</param>
		/// <returns>
		/// 结果返回值
		/// <list type="bullet">
		/// <item>0 = 未满足写入级别,跳过日志写入,可通过属性<see cref="RequiredLevel"/>修改</item>
		/// <item>1 = 日志写入成功</item>
		/// <item>2 = 日志写入引发异常</item>
		/// </list>
		/// </returns>
		public int Write(string log, LogType logType)
		{
			bool lockTaken = false;
			try
			{
				if (OutputToConsole)
				{
					ConsoleLog.Write(log, logType);
				}
				if (logType < RequiredLevel)
				{
					return 0;
				}
				string logPath = FilePath;
				if (!File.Exists(logPath))
				{
					Close();
				}
				CreateStream(logPath);
				StringBuilder sb = new StringBuilder(log.Length + TimestampFormat.Length + 10);
				sb.Append(DateTime.Now.ToString(TimestampFormat));
				sb.Append(":  | ");
				sb.Append(logType.ToString().Substring(0, 3));
				sb.Append(" |  ");
				sb.Append(log);
				Monitor.Enter(_lock, ref lockTaken);
				_logStreamWriter.WriteLine(sb.ToString());
				return 1;
			}
			catch
			{
				return -1;
			}
			finally
			{
				if (lockTaken)
				{
					Monitor.Exit(_lock);
				}
			}
		}

		/// <summary>
		/// 关闭文件流,释放日志文件句柄
		/// </summary>
		public void Close()
		{
			if (_logStreamWriter != null)
			{
				_logStreamWriter.Close();
				_logStreamWriter.Dispose();
				_logStreamWriter = null;
			}
			if (_logFileStream != null)
			{
				_logFileStream.Close();
				_logFileStream.Dispose();
				_logFileStream = null;
			}
		}




	}
}
