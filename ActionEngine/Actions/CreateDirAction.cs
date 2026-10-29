using Ike.ActionEngine.Model;
using System;
using System.IO;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 创建目录
    /// </summary>
    public class CreateDirAction : UpdateAction
    {
        /// <summary>
        /// 目录路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 执行创建目录动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string realPath = context.ResolvePath(Path);
            GlobalVariable.Log.Debug($"{actionName} -> {realPath}");
            Directory.CreateDirectory(realPath);
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }


}
