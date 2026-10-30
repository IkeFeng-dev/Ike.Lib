using Ike.ActionEngine.Model;
using Ike.Standard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// FTP 操作支持的具体动作类型
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FtpCommand
    {
        /// <summary>创建 FTP 目录</summary>
        CreateDirectory,
        /// <summary>上传单个文件</summary>
        UploadFile,
        /// <summary>上传整个本地目录</summary>
        UploadDirectory,
        /// <summary>下载单个文件</summary>
        DownloadFile,
        /// <summary>下载整个 FTP 目录</summary>
        DownloadDirectory,
        /// <summary>删除 FTP 文件</summary>
        DeleteFile,
        /// <summary>删除 FTP 目录</summary>
        DeleteDirectory,
        /// <summary>重命名 FTP 文件/目录</summary>
        Rename
    }

    /// <summary>
    /// FTP 单个步骤定义
    /// </summary>
    public class FtpStep
    {
        /// <summary>
        /// 操作类型（命令）
        /// </summary>
        public FtpCommand Command { get; set; }

        /// <summary>
        /// 本地文件或目录路径
        /// </summary>
        public string LocalPath { get; set; }

        /// <summary>
        /// FTP 远端文件或目录路径
        /// </summary>
        public string FtpPath { get; set; }

        /// <summary>
        /// 重命名时的目标新名称（仅当 Command = Rename 时生效）
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// 上传目录时的匹配通配符，默认为 "*.*" 或 "*"
        /// </summary>
        public string SearchPattern { get; set; } = "*";
    }

    /// <summary>
    /// 统一的 FTP 动作执行器
    /// </summary>
    public class FtpAction : UpdateAction
    {
        /// <summary>
        /// FTP 服务器地址/IP（例: 192.168.1.100:2121 或 ftp://192.168.1.100）
        /// </summary>
        public string ServerIP { get; set; }

        /// <summary>
        /// FTP 登录用户名
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// FTP 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 要按顺序执行的 FTP 步骤列表
        /// </summary>
        public List<FtpStep> Steps { get; set; } = new List<FtpStep>();

        /// <summary>
        /// 执行 FTP 动作
        /// </summary>
        /// <param name="context">上下文环境</param>
        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;

            if (string.IsNullOrWhiteSpace(ServerIP))
            {
                throw new ArgumentException("FTP 服务器 IP 地址未配置", nameof(ServerIP));
            }

            if (Steps == null || Steps.Count == 0)
            {
                GlobalVariable.Log.Debug($"{actionName} -> 没有可执行的 FTP 步骤，略过。");
                return;
            }

            // 初始化 FTP 操作对象
            Ftp ftp = new Ftp(ServerIP, UserID, Password);
            GlobalVariable.Log.Debug($"{actionName} -> 已连接服务器 [{ServerIP}]，开始执行 {Steps.Count} 个步骤...");

            int index = 1;
            foreach (var step in Steps)
            {
                ExecuteStep(context, ftp, step, index++);
            }

            GlobalVariable.Log.Success($"{actionName} -> OK");
        }

        /// <summary>
        /// 执行单个 FTP 步骤
        /// </summary>
        private void ExecuteStep(UpdateContext context, Ftp ftp, FtpStep step, int stepIndex)
        {
            // 解析路径（若配置了本地路径或远端路径，支持上下文变量解析）
            string realLocalPath = !string.IsNullOrEmpty(step.LocalPath) ? context.ResolvePath(step.LocalPath) : null;
            string realFtpPath = !string.IsNullOrEmpty(step.FtpPath) ? context.ResolvePath(step.FtpPath) : null;

            GlobalVariable.Log.Debug($"  [Step {stepIndex}] 执行命令: {step.Command} | Local: '{realLocalPath}' | Ftp: '{realFtpPath}'");

            bool success;

            switch (step.Command)
            {
                case FtpCommand.CreateDirectory:
                    if (string.IsNullOrEmpty(realFtpPath)) throw new ArgumentException("CreateDirectory 必须指定 FtpPath");
                    if (!ftp.CheckDirectory(realFtpPath))
                    {
                        success = ftp.CreateDirectory(realFtpPath);
                    }
                    else
                    {
                        success = true;
                    }
                    break;

                case FtpCommand.UploadFile:
                    if (string.IsNullOrEmpty(realLocalPath)) throw new ArgumentException("UploadFile 必须指定 LocalPath");
                    if (!File.Exists(realLocalPath)) throw new FileNotFoundException($"找不到要上传的本地文件：{realLocalPath}");
                    string targetFtpDir = realFtpPath ?? "/";
                    success = ftp.Upload(realLocalPath, targetFtpDir);
                    break;

                case FtpCommand.UploadDirectory:
                    if (string.IsNullOrEmpty(realLocalPath)) throw new ArgumentException("UploadDirectory 必须指定 LocalPath");
                    if (!Directory.Exists(realLocalPath)) throw new DirectoryNotFoundException($"找不到要上传的本地目录：{realLocalPath}");
                    string targetDir = realFtpPath ?? "/";
                    string pattern = string.IsNullOrWhiteSpace(step.SearchPattern) ? "*" : step.SearchPattern;
                    success = ftp.UploadDirectory(realLocalPath, targetDir, pattern);
                    break;

                case FtpCommand.DownloadFile:
                    if (string.IsNullOrEmpty(realFtpPath)) throw new ArgumentException("DownloadFile 必须指定 FtpPath");
                    if (string.IsNullOrEmpty(realLocalPath)) throw new ArgumentException("DownloadFile 必须指定 LocalPath (本地下载路径)");
                    success = ftp.Download(realFtpPath, realLocalPath);
                    break;

                case FtpCommand.DownloadDirectory:
                    if (string.IsNullOrEmpty(realFtpPath)) throw new ArgumentException("DownloadDirectory 必须指定 FtpPath");
                    if (string.IsNullOrEmpty(realLocalPath)) throw new ArgumentException("DownloadDirectory 必须指定 LocalPath (本地保存目录)");
                    success = ftp.DownloadDirectory(realFtpPath, realLocalPath);
                    break;

                case FtpCommand.DeleteFile:
                    if (string.IsNullOrEmpty(realFtpPath)) throw new ArgumentException("DeleteFile 必须指定 FtpPath");
                    // isCurrenWorkDir 传 false 保证使用 realFtpPath 完整路径
                    success = ftp.DeleteFile(realFtpPath, isCurrenWorkDir: false);
                    break;

                case FtpCommand.DeleteDirectory:
                    if (string.IsNullOrEmpty(realFtpPath)) throw new ArgumentException("DeleteDirectory 必须指定 FtpPath");
                    success = ftp.DeleteDirectory(realFtpPath);
                    break;

                case FtpCommand.Rename:
                    if (string.IsNullOrEmpty(realFtpPath)) throw new ArgumentException("Rename 必须指定 FtpPath");
                    if (string.IsNullOrEmpty(step.NewName)) throw new ArgumentException("Rename 必须指定 NewName");
                    success = ftp.RenameFile(realFtpPath, step.NewName, isCurrenWorkDir: false);
                    break;

                default:
                    throw new NotSupportedException($"暂不支持的 FTP 命令类型: {step.Command}");
            }

            if (!success)
            {
                throw new InvalidOperationException($"[Step {stepIndex}] 执行 FTP 命令 '{step.Command}' 失败。");
            }
        }
    }
}
