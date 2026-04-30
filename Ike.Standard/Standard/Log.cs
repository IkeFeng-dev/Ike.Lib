using System;
using System.IO;
using System.Text;
using System.Threading;
   

namespace Ike.Standard
{
    /// <summary>
    /// 日志输出
    /// </summary>
    public class LOG
    {
        /// <summary>
        /// 是否为控制台环境
        /// </summary>
        private readonly bool isonsoleEnv;
        /// <summary>
        /// 是否输出线程ID
        /// </summary>
        private readonly bool isOutThreadId;
        /// <summary>
        /// 文件名格式
        /// </summary>
        private readonly string fileNameFormat;
        /// <summary>
        /// 时间戳格式
        /// </summary>
        private readonly string timestamp;
        /// <summary>
        /// 日志目录
        /// </summary>
        public string LogDirectory { get; private set; }
        /// <summary>
        /// 异常日志目录
        /// </summary>
        public string ErrorLogDirectory { get; private set; }

        /// <summary>
        /// 控制台输出计数
        /// </summary>
        private int consoleCount = 0;
        /// <summary>
        /// 控制台输出最大行数
        /// </summary>
        private readonly int consoleMaxRow;
        /// <summary>
        /// 日志队列
        /// </summary>
        private readonly DynamicQueue<LogStruct> queue;
        /// <summary>
        /// 日志输出到控制台的颜色
        /// </summary>
        private readonly LogColors logColors;
        /// <summary>
        /// 用于委托日志输出事件
        /// </summary>
        /// <param name="log"></param>
        public delegate void LogEventHandler(LogStruct log);
        /// <summary>
        /// 日志事件,可用于订阅此事件后同步在自定义方法中输出到UI,输出前提是定义了<see cref="LogStruct.WriteToEvent"/> = <see langword="true"/>
        /// </summary>
        public event LogEventHandler LogEvent;

        /// <summary>
        /// 定义日志输出颜色
        /// </summary>
        /// <summary>
        /// 定义日志输出颜色
        /// </summary>
        public class LogColors
        {
            /// <summary>
            /// 日志类型字符串颜色，默认颜色<see langword="#808080"/>(中性灰)
            /// </summary>
            public string Type = "#ffffff";

            /// <summary>
            /// <inheritdoc cref="LogType.Verbose"/>，默认颜色<see langword="#c0c0c0"/>(浅银灰)
            /// </summary>
            public string Verbose = "#c0c0c0";

            /// <summary>
            /// <inheritdoc cref="LogType.Debug"/>，默认颜色<see langword="#e4e4e4"/>(灰白色)
            /// </summary>
            public string Debug = "#e4e4e4";

            /// <summary>
            /// <inheritdoc cref="LogType.Info"/>，默认颜色<see langword="#afd7ff"/>(淡天蓝)
            /// </summary>
            public string Info = "#afd7ff";

            /// <summary>
            /// <inheritdoc cref="LogType.Warning"/>，默认颜色<see langword="#ffffaf"/>(浅米黄)
            /// </summary>
            public string Warning = "#ffffaf";

            /// <summary>
            /// <inheritdoc cref="LogType.Error"/>，默认颜色<see langword="#ff5f87"/>(粉红色)
            /// </summary>
            public string Error = "#ff5f87";

            /// <summary>
            /// <inheritdoc cref="LogType.Fatal"/>，默认颜色<see langword="#ff0000"/>(纯红色)
            /// </summary>
            public string Fatal = "#ff0000";

            /// <summary>
            /// <inheritdoc cref="LogType.Success"/>，默认颜色<see langword="#87d7af"/>(浅蓝绿色)
            /// </summary>
            public string Success = "#87d7af";
        }

        /// <summary>
        /// 表示日志的类型，帮助区分不同的日志级别或类别。
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// 详细信息,记录详细的调试信息或其他较为冗长的输出
            /// </summary>
            Verbose,
            /// <summary>
            /// 一般调试信息
            /// </summary>
            Debug,
            /// <summary>
            /// 信息日志
            /// </summary>
            Info,
            /// <summary>
            /// 警告日志
            /// </summary>
            Warning,
            /// <summary>
            /// 错误日志
            /// </summary>
            Error,
            /// <summary>
            /// 致命错误日志
            /// </summary>
            Fatal,
            /// <summary>
            /// 成功信息
            /// </summary>
            Success
        }

        /// <summary>
        /// 日志队列结构
        /// </summary>
        public class LogStruct
        {
            /// <summary>
            /// 日志时间
            /// </summary>
            public DateTime Time { get; set; }
            /// <summary>
            /// 日志信息
            /// </summary>
            public string Info { get; set; }
            /// <summary>
            /// 日志类型
            /// </summary>
            public LogType LogType { get; set; }
            /// <summary>
            /// 是否输出到控制台
            /// </summary>
            public bool WriteToConsole { get; set; }
            /// <summary>
            /// 是否输出到文件
            /// </summary>
            public bool WriteToFile { get; set; }
            /// <summary>
            /// 是否输出到事件
            /// </summary>
            public bool WriteToEvent { get; set; }
            /// <summary>
            /// 线程ID
            /// </summary>
            public int ThreadID { get; set; }
        }

        /// <summary>
        /// 构造日志实例,如需订阅输出事件,可通过<see cref="LogEvent"/>进行订阅
        /// </summary>
        /// <param name="logDirectory">日志目录</param>
        /// <param name="consoleMaxRow">控制台输出的最大行</param>
        /// <param name="fileNameFormat">文件名格式</param>
        /// <param name="timestamp">时间戳格式</param>
        /// <param name="isOutThreadId">是否输出线程ID</param>
        /// <param name="logColors">定义控制台输出的颜色</param>

        public LOG(string logDirectory, int consoleMaxRow = 5000, string fileNameFormat = "yyyy-MM-dd", string timestamp = "yyyy-MM-dd HH:mm:ss", bool isOutThreadId = true, LogColors logColors = default)
        {
            isonsoleEnv = Console.IsConsoleEnv();
            this.timestamp = timestamp; if (isonsoleEnv)
            {
                WinMethod.EnableAnsiSupport();
            }
            LogDirectory = logDirectory;
            this.consoleMaxRow = consoleMaxRow;
            this.fileNameFormat = fileNameFormat;
            this.isOutThreadId = isOutThreadId;
            if (logColors == default)
            {
                logColors = new LogColors();
            }
            this.logColors = logColors;
            ErrorLogDirectory = Path.Combine(logDirectory, "Error");
            queue = new DynamicQueue<LogStruct>(OutLog);
            queue.StartConsuming();
        }


        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="logStruct"></param>
        private void OutLog(LogStruct logStruct)
        {
            try
            {
                string time = logStruct.Time.ToString(timestamp);
                string typeStr = logStruct.LogType.ToString().Substring(0, 3).ToUpper();
                if (logStruct.WriteToFile)
                {
                    ToFile(logStruct.Info, logStruct.ThreadID, typeStr, time);
                }
                if (logStruct.WriteToEvent)
                {
                    LogEvent?.Invoke(logStruct);
                }
                if (logStruct.WriteToConsole)
                {
                    ToConsole(logStruct.Info, logStruct.LogType, logStruct.ThreadID, typeStr, time);
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
        }

        /// <summary>
        /// 日志输出
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="logType">日志类型</param>
        /// <param name="writeToConsole">是否输出到控制台</param>
        /// <param name="writeToFile">是否输出到文件</param>
        /// <param name="writeToEvent">是否输出到UI</param>
        public void Log(string info, LogType logType, bool writeToConsole, bool writeToFile, bool writeToEvent = true)
        {
            int currentThreadID = 0;
            if (isOutThreadId)
            {
                currentThreadID = Thread.CurrentThread.ManagedThreadId;
            }
            queue.Add(new LogStruct()
            {
                Time = DateTime.Now,
                Info = info,
                LogType = logType,
                WriteToConsole = writeToConsole,
                WriteToEvent = writeToEvent,
                WriteToFile = writeToFile,
                ThreadID = currentThreadID
            });
        }



        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="currentThreadID">当前线程ID</param>
        /// <param name="typeStr">日志类型字符串</param>
        /// <param name="time">时间戳</param>
        private void ToFile(string info, int currentThreadID, string typeStr,string time)
        {
            var sb = new StringBuilder();
            if (isOutThreadId)
            {
                sb.Append(DateTime.Now.ToString(timestamp))
                  .Append("  :[")
                  .Append(currentThreadID)
                  .Append("]-[")
                  .Append(typeStr)
                  .Append("]  ")
                  .Append(info)
                  .AppendLine();
            }
            else
            {
                sb.Append(time)
                  .Append("  :[")
                  .Append(typeStr)
                  .Append("]  ")
                  .Append(info)
                  .AppendLine();
            }
            string filePath = Path.Combine(LogDirectory, DateTime.Now.ToString(fileNameFormat) + ".log");
            Directory.CreateDirectory(LogDirectory);
            File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
        }


        /// <summary>
        /// 写入控制台
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="currentThreadID">当前线程ID</param>
        /// <param name="typeStr">日志类型字符串</param>
        /// <param name="time">时间戳</param>
        /// <param name="hexColor">自定义输出颜色文本</param>
        public void ToConsole(string info, int currentThreadID, string typeStr,string time,string hexColor)
        {
            if (!isonsoleEnv)
            {
                return;
            }
            if (consoleCount > consoleMaxRow)
            {
                System.Console.Clear();
                consoleCount = 0;
            }
            string timeRGB = logColors.Verbose;
            Console.Write(string.Format("{0} => [{1}]:  ",time, currentThreadID),timeRGB);
            if (isOutThreadId)
            {
                Console.Write(string.Format("[{0}]  ", typeStr), logColors.Type);
            }
            else
            {
                Console.Write(string.Format("[{0}]  ", typeStr), logColors.Type);
            }
            Console.WriteLine(info, hexColor);
            consoleCount++;
        }


        /// <summary>
        /// 写入控制台
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="type">日志类型</param>
        /// <param name="currentThreadID">当前线程ID</param>
        /// <param name="typeStr">日志类型字符串</param>
        /// <param name="time">时间戳</param>
        private void ToConsole(string info, LogType type, int currentThreadID, string typeStr,string time)
        {
            if (!isonsoleEnv)
            {
                return;
            }
            if (consoleCount > consoleMaxRow)
            {
                System.Console.Clear();
                consoleCount = 0;
            }
            string timeRGB = logColors.Verbose;
            string infoRGB = logColors.Verbose;
            switch (type)
            {
                case LogType.Debug:
                    infoRGB = logColors.Debug;
                    break;
                case LogType.Info:
                    infoRGB = logColors.Info;
                    break;
                case LogType.Warning:
                    infoRGB = logColors.Warning;
                    break;
                case LogType.Error:
                    infoRGB = logColors.Error;
                    break;
                case LogType.Fatal:
                    infoRGB = logColors.Fatal;
                    break;
                case LogType.Success:
                    infoRGB = logColors.Success;
                    break;
            }
            Console.Write(string.Format("{0} => [{1}]:  ",time, currentThreadID),timeRGB);
            if (isOutThreadId)
            {
                Console.Write(string.Format("[{0}]  ", typeStr), logColors.Type);
            }
            else
            {
                Console.Write(string.Format("[{0}]  ", typeStr), logColors.Type);
            }
            Console.WriteLine(info, infoRGB);
            consoleCount++;
        }


        /// <summary>
        /// 输出信息到调试器中(中文会乱码)
        /// </summary>
        /// <param name="text">输出的信息</param>
        public void ToDebugger(string text)
        {
            WinAPI.OutputDebugString(text);
        }




        /// <summary>
        /// 输出<see cref="LogType.Verbose"/>类型日志
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="writeToEvent">是否输出到UI页面</param>
        /// <param name="writeToConsole">是否输出到控制台</param>
        /// <param name="writeToFile">是否输出到文件</param>
        public void Verbose(string info, bool writeToEvent = true, bool writeToConsole = true, bool writeToFile = true)
        {
            Log(info, LogType.Verbose, writeToConsole, writeToFile, writeToEvent);
        }


        /// <summary>
        /// 输出<see cref="LogType.Debug"/>类型日志
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="writeToEvent">是否输出到UI页面</param>
        /// <param name="writeToConsole">是否输出到控制台</param>
        /// <param name="writeToFile">是否输出到文件</param>
        public void Debug(string info, bool writeToEvent = true, bool writeToConsole = true, bool writeToFile = true)
        {
            Log(info, LogType.Debug, writeToConsole, writeToFile, writeToEvent);
        }


        /// <summary>
        /// 输出<see cref="LogType.Info"/>类型日志
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="writeToEvent">是否输出到UI页面</param>
        /// <param name="writeToConsole">是否输出到控制台</param>
        /// <param name="writeToFile">是否输出到文件</param>
        public void Info(string info, bool writeToEvent = true, bool writeToConsole = true, bool writeToFile = true)
        {
            Log(info, LogType.Info, writeToConsole, writeToFile, writeToEvent);
        }


        /// <summary>
        /// 输出<see cref="LogType.Warning"/>类型日志
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="writeToEvent">是否输出到UI页面</param>
        /// <param name="writeToConsole">是否输出到控制台</param>
        /// <param name="writeToFile">是否输出到文件</param>
        public void Warning(string info, bool writeToEvent = true, bool writeToConsole = true, bool writeToFile = true)
        {
            Log(info, LogType.Warning, writeToConsole, writeToFile, writeToEvent);
        }


        /// <summary>
        /// 输出<see cref="LogType.Success"/>类型日志
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="writeToEvent">是否输出到UI页面</param>
        /// <param name="writeToConsole">是否输出到控制台</param>
        /// <param name="writeToFile">是否输出到文件</param>
        public void Success(string info, bool writeToEvent = true, bool writeToConsole = true, bool writeToFile = true)
        {
            Log(info, LogType.Success, writeToConsole, writeToFile, writeToEvent);
        }


        /// <summary>
        /// 输出<see cref="LogType.Error"/>类型日志
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="writeToEvent">是否输出到UI页面</param>
        /// <param name="writeToConsole">是否输出到控制台</param>
        /// <param name="writeToFile">是否输出到文件</param>
        public void Error(string info, bool writeToEvent = true, bool writeToConsole = true, bool writeToFile = true)
        {
            Log(info, LogType.Error, writeToConsole, writeToFile, writeToEvent);
        }


        /// <summary>
        /// 输出<see cref="LogType.Fatal"/>类型日志
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="writeToEvent">是否输出到UI页面</param>
        /// <param name="writeToConsole">是否输出到控制台</param>
        /// <param name="writeToFile">是否输出到文件</param>
        public void Fatal(string info, bool writeToEvent = true, bool writeToConsole = true, bool writeToFile = true)
        {
            Log(info, LogType.Fatal, writeToConsole, writeToFile, writeToEvent);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception">捕获异常</param>
        /// <param name="writeToEvent">是否输出到UI页面</param>
        /// <param name="writeToConsole">是否输出到控制台</param>
        /// <param name="writeToFile">是否输出到文件</param>
        public void Exception(Exception exception, bool writeToEvent = true, bool writeToConsole = true, bool writeToFile = true)
        {
            Log(exception.Message, LogType.Error, writeToConsole, writeToFile, writeToEvent);
            RecordException(exception);
        }




        /// <summary>
        /// 记录程序异常信息
        /// </summary>
        /// <param name="exception">捕获的异常</param>
        private void RecordException(Exception exception)
        {
            var sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString(timestamp))
              .Append("  :[")
              .Append(Thread.CurrentThread.ManagedThreadId)
              .Append("]  ")
              .Append(exception.Message)
              .AppendLine()
              .Append(exception.ToString())
              .AppendLine()
              .AppendLine();

            string filePath = Path.Combine(ErrorLogDirectory, DateTime.Now.ToString(fileNameFormat) + ".log");
            Directory.CreateDirectory(ErrorLogDirectory);
            File.AppendAllText(filePath, sb.ToString(), Encoding.UTF8);
        }




    }
}
