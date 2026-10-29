using Ike.ActionEngine.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 创建变量
    /// </summary>
    public class CreateVariablesAction : UpdateAction
    {
        /// <summary>
        /// 上下文共享变量变量集合
        /// </summary>
        public Dictionary<string, string> ContextVariables { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 宏变量,用于路径
        /// </summary>
        public Dictionary<string, string> MacroVariables { get; set; } = new Dictionary<string, string>();
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            foreach (var kv in ContextVariables)
            {
                GlobalVariable.Log.Debug($"{actionName} -> {kv.Key}={kv.Value}");
                context.Variables[kv.Key] = kv.Value;
            }
            foreach (var kv in MacroVariables)
            {
                GlobalVariable.Log.Debug($"{actionName} -> {kv.Key}={kv.Value}");
                context.MacroVariables[kv.Key] = () => kv.Value;
            }
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }
}
