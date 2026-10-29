using Ike.ActionEngine.Model;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 修改 JSON 文档
    /// </summary>
    public class ModifyJsonAction : UpdateAction
    {
        /// <summary>
        /// JSON 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// JSON 属性路径。
        /// 每个数组元素代表一层属性，属性名可以包含点号、斜杠等字符。
        /// </summary>
        public string[] JsonPath { get; set; }

        /// <summary>
        /// 要设置的新值。
        /// 当前按 JSON 字符串写入。
        /// </summary>
        public string NewValue { get; set; }

        /// <summary>
        /// 执行修改 JSON 文档动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string tempPath = null;

            try
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                string realPath = context.ResolvePath(FilePath);

                GlobalVariable.Log.Debug($"{actionName} -> " + realPath + ", Path: " + FormatJsonPath(JsonPath) + ", Value: " + NewValue);

                if (string.IsNullOrWhiteSpace(realPath))
                {
                    throw new InvalidOperationException("JSON 文件路径不能为空");
                }

                if (!File.Exists(realPath))
                {
                    throw new FileNotFoundException("JSON 配置文件不存在", realPath);
                }

                ValidateJsonPath(JsonPath);

                string jsonText = File.ReadAllText(realPath);
                JsonNode rootNode = JsonNode.Parse(jsonText) ?? throw new InvalidDataException("JSON 文档内容为空");
                JsonObject rootObject = rootNode as JsonObject ?? throw new InvalidDataException("JSON 根节点必须是对象");

                // 从根对象逐层查找到目标属性的父对象。
                JsonObject currentObject = rootObject;

                for (int i = 0; i < JsonPath.Length - 1; i++)
                {
                    string key = JsonPath[i];
                    JsonNode childNode = currentObject[key] ?? throw new InvalidOperationException("JSON 属性不存在：" + FormatJsonPath(JsonPath, i + 1));
                    JsonObject childObject = childNode as JsonObject ?? throw new InvalidOperationException("JSON 路径中的节点不是对象：" + FormatJsonPath(JsonPath, i + 1));
                    currentObject = childObject;
                }

                // 最后一项是要修改的目标属性名。
                string targetKey = JsonPath[JsonPath.Length - 1];

                // 这里将 NewValue 按 JSON 字符串写入。
                // 例如 "6.6.6" 会成为 JSON 字符串 "6.6.6"。
                currentObject[targetKey] = JsonValue.Create(NewValue);

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string updatedJson = rootNode.ToJsonString(options);
                string directory = Path.GetDirectoryName(realPath);

                if (string.IsNullOrEmpty(directory))
                {
                    directory = AppDomain.CurrentDomain.BaseDirectory;
                }

                // 在原文件同目录生成临时文件，然后替换原文件，降低写入中断导致文件损坏的风险。
                tempPath = Path.Combine(directory, Path.GetFileName(realPath) + "." + Guid.NewGuid().ToString("N") + ".tmp");

                File.WriteAllText(tempPath, updatedJson, new UTF8Encoding(false));

                // 原文件存在时使用 Replace 替换。
                File.Replace(tempPath, realPath, null);
                tempPath = null;

                GlobalVariable.Log.Success($"{actionName} -> OK");
            }
            finally
            {
                // 如果执行失败，尽可能清理临时文件。
                if (!string.IsNullOrEmpty(tempPath) && File.Exists(tempPath))
                {
                    try
                    {
                        File.Delete(tempPath);
                    }
                    catch (Exception ex)
                    {
                        GlobalVariable.Log.Error($"{actionName} 清理临时文件失败 -> " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 校验 JSON 属性路径
        /// </summary>
        /// <param name="path">属性路径</param>
        private static void ValidateJsonPath(string[] path)
        {
            if (path == null || path.Length == 0)
            {
                throw new InvalidOperationException("JsonPath 不能为空");
            }
            for (int i = 0; i < path.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(path[i]))
                {
                    throw new InvalidOperationException("JsonPath 第 " + (i + 1) + " 项不能为空");
                }
            }
        }

        /// <summary>
        /// 将路径数组格式化为便于日志显示的字符串
        /// </summary>
        /// <param name="path">属性路径</param>
        /// <param name="length">要显示的路径项数；小于等于零时显示完整路径</param>
        /// <returns>用于日志显示的路径字符串</returns>
        private static string FormatJsonPath(string[] path, int length = 0)
        {
            if (path == null)
            {
                return string.Empty;
            }
            int count = length <= 0 || length > path.Length ? path.Length : length;
            return string.Join(".", path, 0, count);
        }
    }
}
