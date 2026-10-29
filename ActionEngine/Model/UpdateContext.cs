using Ike.ActionEngine.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Ike.ActionEngine.Model
{
    /// <summary>
    /// 上下文环境：用于解析路径占位符
    /// </summary>
    public class UpdateContext
    {


        /// <summary>
        /// 共享上下文变量字典，供 Action 或 Script 传递数据
        /// </summary>
        public Dictionary<string, object> Variables { get; } = new Dictionary<string, object>();
        /// <summary>
        /// 共享上下文宏变量字典
        /// Key: 宏名称
        /// Value: 返回字符串的委托（支持静态值和动态实时获取）
        /// </summary>
        public Dictionary<string, Func<string>> MacroVariables { get; } = new Dictionary<string, Func<string>>()
        {
            // ==================== 1. 应用与基础路径 ====================
            { "{AppDir}",        () => AppContext.BaseDirectory },
            { "{AppName}",       () => Assembly.GetEntryAssembly()?.GetName().Name ?? "App" },
            { "{AppVersion}",    () => Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0" },
            // ==================== 2. 系统常用目录 ====================
            { "{TempDir}",       () => Path.GetTempPath() },
            { "{AppData}",       () => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) },
            { "{LocalAppData}",  () => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) },
            { "{Desktop}",       () => Environment.GetFolderPath(Environment.SpecialFolder.Desktop) },
            { "{Documents}",     () => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
            { "{UserProfile}",   () => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) },
            // ==================== 3. 用户与设备信息 ====================
            { "{UserName}",      () => Environment.UserName },
            { "{MachineName}",   () => Environment.MachineName },
            // ==================== 4. 时间与日期 ====================
            { "{NowTime}",       () => DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "{Date}",          () => DateTime.Now.ToString("yyyyMMdd") },
            { "{Time}",          () => DateTime.Now.ToString("HHmmss") },
            { "{Year}",          () => DateTime.Now.ToString("yyyy") },
            { "{Month}",         () => DateTime.Now.ToString("MM") },
            { "{Day}",           () => DateTime.Now.ToString("dd") },
            { "{Timestamp}",     () => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
            // ==================== 5. 动态唯一标识 ====================
            { "{GUID}",          () => Guid.NewGuid().ToString() },
             // 不带短横线 32 位
            { "{GUID_N}",        () => Guid.NewGuid().ToString("N") },
        };

        /// <summary>
        /// 替换路径中的宏变量
        /// </summary>
        public string ResolvePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            // 遍历所有注册的宏变量
            foreach (var kv in MacroVariables)
            {
                if (path.Contains(kv.Key))
                {
                    // 1. 动态执行委托获取最新值
                    string realValue = kv.Value?.Invoke() ?? string.Empty;

                    // 2. 替换路径中的宏
                    path = path.Replace(kv.Key, realValue);
                }
            }
            // 替换两层重复的路径分级
            string r = "\\\\";
            while (path.Contains(r))
            {
                path = path.Replace(r, "\\");
            }
            return path;
        }
    }

    /// <summary>
    /// 多态 Action 基类
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(KillProcessAction), "killProcess")]
    [JsonDerivedType(typeof(CreateDirAction), "createDir")]
    [JsonDerivedType(typeof(ExtractAction), "extract")]
    [JsonDerivedType(typeof(DeleteAction), "delete")]
    [JsonDerivedType(typeof(ModifyJsonAction), "modifyJson")]
    [JsonDerivedType(typeof(ModifyIniAction), "modifyIni")]
    [JsonDerivedType(typeof(VerifyFileHashAction), "verifyFileHash")]
    [JsonDerivedType(typeof(BackupFileAction), "backupFile")]
    [JsonDerivedType(typeof(CopyFileAction), "copyFile")]
    [JsonDerivedType(typeof(StartProcessAction), "startProcess")]
    [JsonDerivedType(typeof(ScriptAction), "script")]
    [JsonDerivedType(typeof(CSharpAction), "cSharpScript")]
    [JsonDerivedType(typeof(DownloadFileAction), "downloadFile")] 
    [JsonDerivedType(typeof(UploadFileAction), "uploadFile")]
    [JsonDerivedType(typeof(HttpRequestAction), "httpRequest")]
    [JsonDerivedType(typeof(CreateVariablesAction), "createVariables")]
    public abstract class UpdateAction
    {
        /// <summary>
        /// 当前动作出错时，是否继续执行后续动作
        /// </summary>
        public bool ContinueOnError { get; set; } = true;
        /// <summary>
        /// 动作执行
        /// </summary>
        /// <param name="context"></param>
        public abstract void Execute(UpdateContext context);
    }
}
