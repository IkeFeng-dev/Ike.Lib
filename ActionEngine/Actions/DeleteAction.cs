using Ike.ActionEngine.Model;
using System;
using System.IO;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 删除文件或者目录
    /// </summary>
    public class DeleteAction : UpdateAction
    {
        /// <summary>
        /// 文件或目录路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 是否为目录
        /// </summary>
        public bool IsFolder { get; set; }
        /// <summary>
        /// 执行删除动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string realPath = context.ResolvePath(Path);
            GlobalVariable.Log.Debug($"{actionName} -> {realPath}");
            if (IsFolder)
            {
                if (Directory.Exists(realPath))
                {
                    Directory.Delete(realPath, true);
                }
            }
            else
            {
                if (File.Exists(realPath))
                {
                    File.Delete(realPath);
                }
            }
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }
}
