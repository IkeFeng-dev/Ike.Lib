using Ike.ActionEngine.Model;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 校验文件 SHA-256
    /// </summary>
    public class VerifyFileHashAction : UpdateAction
    {
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 预期的 SHA-256 值
        /// </summary>
        public string ExpectedHash { get; set; }
        /// <summary>
        /// 执行哈希校验
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string realPath = context.ResolvePath(FilePath);
            GlobalVariable.Log.Debug($"{actionName} -> {realPath}");
            if (!File.Exists(realPath))
            {
                throw new FileNotFoundException("待校验文件不存在", realPath);
            }
            string actualHash;
            using (SHA256 sha256 = SHA256.Create())
            using (FileStream stream = File.OpenRead(realPath))
            {
                byte[] hashBytes = sha256.ComputeHash(stream);
                StringBuilder builder = new StringBuilder();
                foreach (byte hashByte in hashBytes)
                {
                    builder.Append(hashByte.ToString("x2"));
                }
                actualHash = builder.ToString();
            }
            if (!string.Equals(actualHash, ExpectedHash, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"SHA-256 校验失败，预期：{ExpectedHash}，实际：{actualHash}");
            }
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }
    }
}
