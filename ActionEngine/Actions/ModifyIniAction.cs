using Ike.ActionEngine.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 修改 INI 文档
    /// </summary>
    public class ModifyIniAction : UpdateAction
    {
        /// <summary>
        /// INI 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 配置节名称
        /// </summary>
        public string Section { get; set; }
        /// <summary>
        /// 配置项名称
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 配置项的新值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 执行修改 INI 文档动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string realPath = context.ResolvePath(FilePath);
            GlobalVariable.Log.Debug($"{actionName} -> {realPath}, [{Section}] {Key}={Value}");

            if (!File.Exists(realPath))
            {
                throw new FileNotFoundException("INI 配置文件不存在", realPath);
            }
            string[] lines = File.ReadAllLines(realPath);
            List<string> newLines = new List<string>();
            string currentSection = string.Empty;
            bool keyFound = false;
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                }
                int equalsIndex = trimmedLine.IndexOf('=');
                string currentKey = equalsIndex >= 0 ? trimmedLine.Substring(0, equalsIndex).Trim() : string.Empty;
                if (currentSection.Equals(Section, StringComparison.OrdinalIgnoreCase) && currentKey.Equals(Key, StringComparison.OrdinalIgnoreCase))
                {
                    newLines.Add($"{Key}={Value}");
                    keyFound = true;
                }
                else
                {
                    newLines.Add(line);
                }
            }
            if (!keyFound)
            {
                int sectionIndex = newLines.FindIndex(
                    line => line.Trim().Equals($"[{Section}]", StringComparison.OrdinalIgnoreCase));
                if (sectionIndex >= 0)
                {
                    newLines.Insert(sectionIndex + 1, $"{Key}={Value}");
                }
                else
                {
                    newLines.Add($"[{Section}]");
                    newLines.Add($"{Key}={Value}");
                }
            }
            File.WriteAllLines(realPath, newLines);
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }


    }
}
