namespace Ike.Standard
{
    public partial class LOG
    {
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

    }
}
