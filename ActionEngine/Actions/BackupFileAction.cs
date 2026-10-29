using Ike.ActionEngine.Model;
using System.IO;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 备份文件
    /// </summary>
    public class BackupFileAction : UpdateAction
    {
        /// <summary>
        /// 原文件路径
        /// </summary>
        public string SourcePath { get; set; }
        /// <summary>
        /// 备份文件路径
        /// </summary>
        public string BackupPath { get; set; }
        /// <summary>
        /// 执行文件备份
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string sourcePath = context.ResolvePath(SourcePath);
            string backupPath = context.ResolvePath(BackupPath);
            GlobalVariable.Log.Debug($"{actionName} -> {sourcePath} -> {backupPath}");
            if (!File.Exists(sourcePath))
            {
                GlobalVariable.Log.Warning($"{actionName} -> 源文件不存在，跳过：{sourcePath}");
                return;
            }
            string backupDirectory = System.IO.Path.GetDirectoryName(backupPath);
            if (!string.IsNullOrEmpty(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }
            File.Copy(sourcePath, backupPath, true);
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }
}
