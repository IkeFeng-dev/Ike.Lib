using System;
using System.Text;

namespace Ike.Standard
{
    public partial class LOG
    {

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

            /// <summary>
            /// 转日志格式字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                StringBuilder sb = new();
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                .Append("  :[")
                .Append(ThreadID)
                .Append("]-[")
                .Append(LogType.ToString().Substring(0, 3).ToUpper())
                .Append("]  ")
                .Append(Info)
                .AppendLine();
                return sb.ToString();
            }
        }
    }
}
