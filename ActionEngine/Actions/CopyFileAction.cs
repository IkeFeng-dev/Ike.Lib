using Ike.ActionEngine.Model;
using System.IO;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 复制文件
    /// </summary>
    public class CopyFileAction : UpdateAction
    {
        /// <summary>
        /// 源文件路径
        /// </summary>
        public string SourcePath { get; set; }
        /// <summary>
        /// 目标文件路径
        /// </summary>
        public string TargetPath { get; set; }
        /// <summary>
        /// 目标文件已存在时是否覆盖
        /// </summary>
        public bool Overwrite { get; set; } = true;
        /// <summary>
        /// 执行复制文件动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string sourcePath = context.ResolvePath(SourcePath);
            string targetPath = context.ResolvePath(TargetPath);
            GlobalVariable.Log.Debug($"{actionName} -> {sourcePath} -> {targetPath}");
            if (!File.Exists(sourcePath))
            {
                throw new FileNotFoundException("源文件不存在", sourcePath);
            }
            string targetDirectory = System.IO.Path.GetDirectoryName(targetPath);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                GlobalVariable.Log.Debug($"{actionName} -> 创建目标目录'{targetDirectory}'");
                Directory.CreateDirectory(targetDirectory);
            }
            File.Copy(sourcePath, targetPath, Overwrite);
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }
}
