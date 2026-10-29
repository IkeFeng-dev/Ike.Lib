using Ike.ActionEngine.Model;
using System.Diagnostics;
using System.IO;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 启动程序
    /// </summary>
    public class StartProcessAction : UpdateAction
    {
        /// <summary>
        /// 可执行文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 启动参数
        /// </summary>
        public string Arguments { get; set; }
        /// <summary>
        /// 工作目录；为空时使用程序所在目录
        /// </summary>
        public string WorkingDirectory { get; set; }
        /// <summary>
        /// 执行启动程序动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string realFilePath = context.ResolvePath(FilePath);
            if (!File.Exists(realFilePath))
            {
                throw new FileNotFoundException("待启动程序不存在", realFilePath);
            }
            string realWorkingDirectory = string.IsNullOrWhiteSpace(WorkingDirectory) ? System.IO.Path.GetDirectoryName(realFilePath) : context.ResolvePath(WorkingDirectory);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = realFilePath,
                Arguments = Arguments ?? string.Empty,
                WorkingDirectory = realWorkingDirectory,
                UseShellExecute = false
            };
            Process.Start(startInfo);
            GlobalVariable.Log.Debug($"{actionName} -> Path: {realFilePath}");
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }

}
