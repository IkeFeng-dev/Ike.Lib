using Ike.ActionEngine.Model;
using System;
using System.IO;
using System.IO.Compression;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 压缩文件或目录为 ZIP 格式
    /// </summary>
    public class CompressAction : UpdateAction
    {
        /// <summary>
        /// 要压缩的源文件或源目录路径
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// 生成的目标 ZIP 文件路径
        /// </summary>
        public string ZipPath { get; set; }

        /// <summary>
        /// 若目标 ZIP 文件已存在，是否进行覆盖（默认为 true）
        /// </summary>
        public bool Overwrite { get; set; } = true;

        /// <summary>
        /// 压缩目录时，是否在 ZIP 中包含源目录本身的名称作为根文件夹（默认为 false）
        /// </summary>
        public bool IncludeBaseDirectory { get; set; } = false;
        /// <summary>
        /// 执行压缩动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;

            string realSource = context.ResolvePath(SourcePath);
            string realZip = context.ResolvePath(ZipPath);

            GlobalVariable.Log.Debug($"{actionName} -> {realSource} -> {realZip}");

            bool isDirectory = Directory.Exists(realSource);
            bool isFile = File.Exists(realSource);

            if (!isDirectory && !isFile)
            {
                throw new FileNotFoundException($"找不到要压缩的源文件或目录：{realSource}");
            }

            // 自动确保目标 ZIP 文件所在的上级目录存在
            string targetDirectory = Path.GetDirectoryName(realZip);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // 处理目标 ZIP 文件已存在的情况
            if (File.Exists(realZip))
            {
                if (Overwrite)
                {
                    File.Delete(realZip);
                }
                else
                {
                    throw new IOException($"目标 ZIP 文件已存在且已配置禁止覆盖：{realZip}");
                }
            }

            // 根据源路径类型选择压缩逻辑
            if (isDirectory)
            {
                // 压缩目录
                ZipFile.CreateFromDirectory(realSource,realZip,CompressionLevel.Optimal,IncludeBaseDirectory);
            }
            else
            {
                // 压缩单个文件
                using (ZipArchive archive = ZipFile.Open(realZip, ZipArchiveMode.Create))
                {
                    string entryName = Path.GetFileName(realSource);
                    archive.CreateEntryFromFile(realSource, entryName, CompressionLevel.Optimal);
                }
            }
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }
}
