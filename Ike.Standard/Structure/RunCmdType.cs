#if NETFRAMEWORK

namespace Ike.Standard
{
    public partial class Process
    {
        /// <summary>
		/// CMD命令执行类型
		/// </summary>
		public enum RunCmdType
        {
            /// <summary>
            /// 启用新窗口独立运行这个命令,返回<see cref="string.Empty"/>
            /// </summary>
            IndependentOperation,
            /// <summary>
            /// 启用新窗口独立运行这个命令,显示这个窗口,按任意键关闭它,返回<see cref="string.Empty"/>
            /// </summary>
            ShowWindow,
            /// <summary>
            /// 获取输出文本
            /// </summary>
            GetOutputText,
            /// <summary>
            /// 结果直接输出到调用程序控制台中,返回<see cref="string.Empty"/>
            /// </summary>
            Association,
            /// <summary>
            /// 后台执行,不输出结果,不显示窗体,返回<see cref="string.Empty"/>
            /// </summary>
            BackgroundExecution,
        }
    }
}
#endif