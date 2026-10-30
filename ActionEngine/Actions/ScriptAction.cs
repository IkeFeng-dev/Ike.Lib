using Jint;
using Ike.ActionEngine.Model;
using System;
using System.IO;
using System.Net.Http;
using Jint.Runtime.Interop;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 执行动态 JavaScript 脚本动作
    /// </summary>
    public class ScriptAction : UpdateAction
    {
        /// <summary>
        /// 内嵌脚本内容
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// 外部脚本 Url 路径
        /// </summary>
        public string ScriptUrl { get; set; }

        /// <summary>
        /// 外部脚本文件路径
        /// </summary>
        public string ScriptFile { get; set; }

        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string code = null;

            // 1. 优先级 1：从 ScriptUrl 获取远程脚本
            if (!string.IsNullOrWhiteSpace(ScriptUrl))
            {
                string realUrl = context.ResolvePath(ScriptUrl);
                GlobalVariable.Log.Debug($"{actionName} -> 正在从 URL 下载脚本: {realUrl}");

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    code = httpClient.GetStringAsync(realUrl).GetAwaiter().GetResult();
                }
            }
            // 2. 优先级 2：从 ScriptFile 读取本地脚本
            else if (!string.IsNullOrWhiteSpace(ScriptFile))
            {
                string realScriptPath = context.ResolvePath(ScriptFile);
                GlobalVariable.Log.Debug($"{actionName} -> 正在读取本地脚本文件: {realScriptPath}");

                if (!File.Exists(realScriptPath))
                {
                    throw new FileNotFoundException($"找不到指定的脚本文件: {realScriptPath}");
                }

                code = File.ReadAllText(realScriptPath);
            }
            // 3. 优先级 3：使用内嵌 Script 内容
            else
            {
                code = Script;
            }

            // 保留代码到文件
            Directory.CreateDirectory(GlobalVariable.CurrentDataDir);
            if (File.Exists(GlobalVariable.ActionJSPath))
            {
                File.AppendAllText(GlobalVariable.ActionJSPath, string.Format("{0}{1}{0}", Standard.Text.CopyString(Environment.NewLine, 10), Standard.Text.CopyString("=", 30)));
            }
            File.AppendAllText(GlobalVariable.ActionJSPath, code);

            if (string.IsNullOrWhiteSpace(code))
            {
                GlobalVariable.Log.Warning($"{actionName} -> 脚本内容为空，跳过执行");
                return;
            }

            // 创建 Jint 4.17.0 引擎并配置成员解析逻辑
            var engine = new Engine(options =>
            {
                // 1. 允许访问 CLR
                options.AllowClr();

                // 2. 注册全新的 TypeResolver，设置成员比较器为忽略大小写
                options.SetTypeResolver(new TypeResolver
                {
                    MemberNameComparer = StringComparer.OrdinalIgnoreCase
                });
            })
            .SetValue("context", context)
            .SetValue("log", GlobalVariable.Log);

            // 执行脚本
            engine.Execute(code);

            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }
}
