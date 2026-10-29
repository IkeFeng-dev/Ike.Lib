using System;
using System.IO;

namespace Ike.ActionEngine
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class GlobalVariable
    {
        /// <summary>
        /// 程序数据目录
        /// </summary>
        public static string BaseDir { get; set; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        /// <summary>
        /// 日志目录
        /// </summary>
        public static string LogDir { get; set; } = Path.Combine(BaseDir, "lOG");
        /// <summary>
        /// 当前执行储存数据的数据目录
        /// </summary>
        public static string CurrentDataDir { get; set; } = Path.Combine(BaseDir, "Run", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
        /// <summary>
        /// 当前执行JSON文件储存路径
        /// </summary>
        public static string ActionJsonPath { get; set; } = Path.Combine(CurrentDataDir, "Actions.json");
        /// <summary>
        /// 当前执行输出LOG文件储存路径
        /// </summary>
        public static string ActionLogPath { get; set; } = Path.Combine(CurrentDataDir, "Log.log");
        /// <summary>
        /// 当前执行JS脚本储存路径
        /// </summary>
        public static string ActionJSPath { get; set; } = Path.Combine(CurrentDataDir, "Script.js");
        /// <summary>
        /// 当前执行C#代码储存路径
        /// </summary>
        public static string ActionCSharpPath { get; set; } = Path.Combine(CurrentDataDir, "CSharp.cs");
        /// <summary>
        /// 日志对象
        /// </summary>
        public static Standard.LOG Log { get; set; } = new Standard.LOG(LogDir);

        /// <summary> 当前步骤号 </summary>
        public static int Step { get; set; }

        /// <summary> 所有步骤号 </summary>
        public static int StepNumbers { get; set; }

        /// <summary> 当前动作名称 </summary>
        public static string ActionName { get; set; }

        /// <summary>
        /// 写入Action开始信息,返回Action名称
        /// </summary>
        /// <param name="type">Action类型</param>
        /// <returns></returns>
        public static string WriteActionLog(Type type)
        {
            string actionName = $"[{type.Name.Replace("Action", string.Empty)}]";
            Log.Verbose(string.Format("{0}{1}{0}Action  -> {2}{0}{1}", Environment.NewLine, Standard.Text.CopyString("-",45), actionName));
            return actionName;
        }
    }
}
