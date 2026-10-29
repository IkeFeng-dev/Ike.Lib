using Ike.ActionEngine.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Ike.ActionEngine
{
    /// <summary>
    /// 解释器核心
    /// </summary>
    public class ManifestEngine
    {
        /// <summary>
        /// Action名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Action描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// 执行动作集合
        /// </summary>
        public List<UpdateAction> Actions { get; set; } = new List<UpdateAction>();
        

        /// <summary>
        /// 运行解释器
        /// </summary>
        /// <param name="manifestJsonPath">json内容</param>
        /// <exception cref="FileNotFoundException"></exception>
        public static void Run(string manifestJsonPath)
        {
            string actionName = GlobalVariable.WriteActionLog(typeof(ManifestEngine));
            GlobalVariable.ActionName = actionName;
            if (!File.Exists(manifestJsonPath))
            {
                throw new FileNotFoundException("脚本文件不存在！", manifestJsonPath);
            }

            string jsonContent = File.ReadAllText(manifestJsonPath);
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            // 1. 解析 JSON 为结构化的对象树
            var engine = JsonSerializer.Deserialize<ManifestEngine>(jsonContent, options);

            if (engine == null || engine.Actions == null)
            {
                GlobalVariable.Log.Warning($"{actionName} -> 配置文件解析失败或没有可执行的操作");
                return;
            }
            GlobalVariable.StepNumbers = engine.Actions.Count;
            GlobalVariable.Log.Info($"{actionName} -> 开始执行动作, 动作名称: {engine.Name}");
            GlobalVariable.Log.Info(engine.Description);
            var context = new UpdateContext();

            // 2. 依次执行每一个 Action
            for (int i = 0; i < engine.Actions.Count; i++)
            {
                var action = engine.Actions[i];
                try
                {
                    GlobalVariable.Step = i + 1;
                    GlobalVariable.Log.Info($"{action} -> 步骤 [{GlobalVariable.Step}/{engine.Actions.Count}]");
                    action.Execute(context);
                }
                catch(Exception ex)
                {
                    GlobalVariable.Log.Exception(ex);
                    if (!action.ContinueOnError)
                    {
                        GlobalVariable.Log.Warning("动作执行发生异常,当前动作已配置异常停止执行,立即执行退出");
                        return;
                    }
                }
            }
            GlobalVariable.Log.Debug($"End  -> {actionName}");
        }
    }

}
