using Ike.ActionEngine.Model;
using System;
using System.Diagnostics;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 结束进程
    /// </summary>
    public class KillProcessAction : UpdateAction
    {
        /// <summary>
        /// 进程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 等待进程退出事件(毫秒)
        /// </summary>
        public int WaitForExit { get; set; } = 5000;
        /// <summary>
        /// 执行停止进程动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            foreach (var proc in Process.GetProcessesByName(ProcessName))
            {
                GlobalVariable.Log.Debug($"{actionName} -> {ProcessName} {proc.Id}");
                proc.Kill();
                proc.WaitForExit(WaitForExit);
                GlobalVariable.Log.Success($"{actionName} -> OK");
            }
        }
    }

}
