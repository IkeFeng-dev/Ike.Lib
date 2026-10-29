using Ike.ActionEngine.Model;
using System;
using System.IO;
using System.IO.Compression;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 解压缩文件
    /// </summary>
    public class ExtractAction : UpdateAction
    {
        /// <summary>
        /// ZIP 文件路径
        /// </summary>
        public string ZipPath { get; set; }
        /// <summary>
        /// 解压目标目录
        /// </summary>
        public string TargetDir { get; set; }
        /// <summary>
        /// 执行解压缩动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string realZip = context.ResolvePath(ZipPath);
            string realTarget = context.ResolvePath(TargetDir);
            GlobalVariable.Log.Debug($"{actionName} -> {realZip} -> {realTarget}");
            Directory.CreateDirectory(realTarget);
            string targetRoot = Path.GetFullPath(realTarget).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            StringComparison pathComparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            using (ZipArchive archive = ZipFile.OpenRead(realZip))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string destinationPath = Path.GetFullPath(Path.Combine(realTarget, entry.FullName));
                    // 防止 ZIP 条目使用 ../ 等路径逃逸到目标目录之外
                    if (!destinationPath.StartsWith(targetRoot, pathComparison))
                    {
                        throw new InvalidOperationException($"ZIP 条目路径超出目标目录：{entry.FullName}");
                    }
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    else
                    {
                        string destinationDirectory = Path.GetDirectoryName(destinationPath);
                        if (!string.IsNullOrEmpty(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }
                        entry.ExtractToFile(destinationPath, true);
                    }
                }
            }
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }

}
