using Ike.ActionEngine.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
namespace Ike.ActionEngine
{
    /// <summary>
    /// 客户端程序更新SDK主类
    /// </summary>
    public class AppUpdater : IDisposable
    {
        /// <summary>
        /// HTTP请求对象
        /// </summary>
        private readonly HttpClient _httpClient;
        /// <summary>
        /// API 地址
        /// </summary>
        private readonly string _baseUrl;
        /// <summary>
        /// 主程序集对象
        /// </summary>
        private readonly Assembly _assembly;
        /// <summary>
        /// JSON解析
        /// </summary>
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// 下载进度变更事件
        /// </summary>
        public event EventHandler<DownloadProgressArgs> DownloadProgressChanged;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseUrl">服务器接口基础地址，如 "http://127.0.0.1:5295/api/ProgramVersionManagement"</param>
        /// <param name="timeoutSeconds">HTTP请求超时时间（秒），默认30秒</param>
        /// <param name="assembly">主程序对象,如果传递后续调用方法可默认使用此对象</param>
        public AppUpdater(string baseUrl, int timeoutSeconds = 30, Assembly assembly = null)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl), "接口基础地址不能为空");

            _baseUrl = baseUrl.TrimEnd('/');
            _assembly = assembly;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                // 忽略 JSON 字段大小写
                PropertyNameCaseInsensitive = true
            };
        }

        #region 公开 API 方法

        /// <summary>
        /// 检查当前程序是否有可用更新
        /// </summary>
        /// <param name="assembly">程序集对象,构造函数传递了程序集此处可不传递,默认引用构造时的对象</param>
        /// <returns>检测结果</returns>
        public async Task<VersionCheckResult> CheckUpdateAsync(Assembly assembly = default)
        {

            if (assembly == null)
            {
                assembly = _assembly;
            }
            string name = GetProgramName(assembly);
            string ver = GetProgramVersion(assembly);
            return await CheckUpdateAsync(name, ver);
        }

        /// <summary>
        /// 检查当前程序是否有可用更新
        /// </summary>
        /// <param name="programName">程序名称（如 "Test.exe"）</param>
        /// <param name="currentVersion">当前客户端版本号（如 "1.0.0.1"）</param>
        /// <returns>检测结果</returns>
        public async Task<VersionCheckResult> CheckUpdateAsync(string programName, string currentVersion)
        {
            if (string.IsNullOrWhiteSpace(programName))
                throw new ArgumentNullException(nameof(programName));
            if (string.IsNullOrWhiteSpace(currentVersion))
                throw new ArgumentNullException(nameof(currentVersion));
            string url = $"{_baseUrl}/CheckUpdate?programName={Uri.EscapeDataString(programName)}&currentVersion={Uri.EscapeDataString(currentVersion)}";
            return await PostAsync<VersionCheckResult>(url);
        }

        /// <summary>
        /// 获取当前程序的最新版本信息
        /// </summary>
        /// <param name="assembly">程序集对象,构造函数传递了程序集此处可不传递,默认引用构造时的对象</param>
        /// <returns>版本详细信息</returns>
        public async Task<VersionInfo> GetLatestVersionAsync(Assembly assembly = default)
        {
            if (assembly == null)
            {
                assembly = _assembly;
            }
            string name = GetProgramName(assembly);
            return await GetLatestVersionAsync(name);
        }

        /// <summary>
        /// 获取当前程序的最新版本信息
        /// </summary>
        /// <param name="programName">程序名称（如 "Test.exe"）</param>
        public async Task<VersionInfo> GetLatestVersionAsync(string programName)
        {
            if (string.IsNullOrWhiteSpace(programName))
                throw new ArgumentNullException(nameof(programName));
            string url = $"{_baseUrl}/CheckLatestVersion?programName={Uri.EscapeDataString(programName)}";
            return await PostAsync<VersionInfo>(url);
        }

        /// <summary>
        /// 获取当前程序的所有可用版本列表
        /// </summary>
        /// <param name="assembly">程序集对象,构造函数传递了程序集此处可不传递,默认引用构造时的对象</param>
        /// <returns>所有可用版本信息</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<VersionInfo>> GetVersionListAsync(Assembly assembly = default)
        {
            if (assembly == null)
            {
                assembly = _assembly;
            }
            string name = GetProgramName(assembly);
            return await GetVersionListAsync(name);
        }

        /// <summary>
        /// 获取当前程序的所有可用版本列表
        /// </summary>
        /// <param name="programName">程序名称（如 "Test.exe"）</param>
        public async Task<List<VersionInfo>> GetVersionListAsync(string programName)
        {
            if (string.IsNullOrWhiteSpace(programName))
                throw new ArgumentNullException(nameof(programName));
            string url = $"{_baseUrl}/GetVersionList?programName={Uri.EscapeDataString(programName)}";
            return await PostAsync<List<VersionInfo>>(url);
        }

        /// <summary>
        /// 下载程序更新包
        /// </summary>
        /// <param name="assembly">程序集对象,构造函数传递了程序集此处可不传递,默认引用构造时的对象</param>
        /// <param name="saveFilePath">本地保存全路径（如 @"C:\Updates\Test_v1.0.0.8.exe"）</param>
        /// <param name="version">指定下载的版本号（为空则默认下载最新版）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否下载成功</returns>
        public async Task<bool> DownloadUpdateAsync(string saveFilePath, Assembly assembly = default, string version = default, CancellationToken cancellationToken = default)
        {
            if (assembly == null)
            {
                assembly = _assembly;
            }
            string name = GetProgramName(assembly);
            if (string.IsNullOrWhiteSpace(version))
            {
                version = GetProgramVersion(assembly);
            }

            return await DownloadUpdateAsync(name, saveFilePath, version, cancellationToken);
        }


        /// <summary>
        /// 下载程序更新包
        /// </summary>
        /// <param name="programName">程序名称（如 "Test.exe"）</param>
        /// <param name="saveFilePath">本地保存全路径（如 @"C:\Updates\Test_v1.0.0.8.exe"）</param>
        /// <param name="version">指定下载的版本号（为空则默认下载最新版）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>是否下载成功</returns>
        public async Task<bool> DownloadUpdateAsync(string programName, string saveFilePath, string version = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(programName))
                throw new ArgumentNullException(nameof(programName));
            if (string.IsNullOrWhiteSpace(saveFilePath))
                throw new ArgumentNullException(nameof(saveFilePath));

            string url = $"{_baseUrl}/Download?programName={Uri.EscapeDataString(programName)}";
            if (!string.IsNullOrWhiteSpace(version))
            {
                url += $"&version={Uri.EscapeDataString(version)}";
            }

            // 自动创建保存目录
            string directory = Path.GetDirectoryName(saveFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    throw new Exception($"文件下载失败: [HTTP {(int)response.StatusCode}] {errorMsg}");
                }

                long totalBytes = response.Content.Headers.ContentLength ?? -1L;

                using (var downloadStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var buffer = new byte[8192];
                    long totalBytesRead = 0;
                    int bytesRead;

                    while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                        totalBytesRead += bytesRead;
                        OnDownloadProgressChanged(totalBytesRead, totalBytes);
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// 快捷方法：自动检测更新，若有最新版则直接下载到指定路径
        /// </summary>
        /// <param name="assembly">程序集对象,构造函数传递了程序集此处可不传递,默认引用构造时的对象</param>
        /// <param name="currentVersion">当前版本</param>
        /// <param name="saveFilePath">保存路径</param>
        /// <returns>若触发并完成了下载返回 true，无更新返回 false</returns>
        public async Task<bool> CheckAndDownloadIfAvailableAsync(string saveFilePath, Assembly assembly = default, string currentVersion = default)
        {
            if (assembly == null)
            {
                assembly = _assembly;
            }
            string name = GetProgramName(assembly);
            if (string.IsNullOrWhiteSpace(currentVersion))
            {
                currentVersion = GetProgramVersion(assembly);
            }
            return await CheckAndDownloadIfAvailableAsync(name, currentVersion, saveFilePath);
        }


        /// <summary>
        /// 快捷方法：自动检测更新，若有最新版则直接下载到指定路径
        /// </summary>
        /// <param name="programName">程序名称</param>
        /// <param name="currentVersion">当前版本</param>
        /// <param name="saveFilePath">保存路径</param>
        /// <returns>若触发并完成了下载返回 true，无更新返回 false</returns>
        public async Task<bool> CheckAndDownloadIfAvailableAsync(string programName, string currentVersion, string saveFilePath)
        {
            var check = await CheckUpdateAsync(programName, currentVersion);
            if (check != null && check.HasNewVersion)
            {
                return await DownloadUpdateAsync(programName, saveFilePath, check.LatestVersion);
            }
            return false;
        }

        #endregion



        #region 私有辅助方法


        /// <summary>
        /// 获取程序集文件名
        /// </summary>
        /// <param name="assembly">程序集对象</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string GetProgramName(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new InvalidOperationException("assembly is null");
            }
            string fileName = Path.GetFileName(assembly.Location);
            return fileName;
        }


        /// <summary>
        /// 获取程序版本号
        /// </summary>
        /// <param name="assembly">程序集对象</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private string GetProgramVersion(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new InvalidOperationException("assembly is null");
            }
            return assembly.GetName().Version.ToString();
        }


        /// <summary>
        /// POST请求
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <returns>请求结果</returns>
        /// <exception cref="Exception"></exception>
        private async Task<T> PostAsync<T>(string url)
        {
            var response = await _httpClient.PostAsync(url, null);
            string json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // 解析服务端返回的错误信息
                try
                {
                    var errResult = JsonSerializer.Deserialize<ApiResult<object>>(json, _jsonOptions);
                    throw new Exception(errResult?.Error ?? $"请求失败，HTTP 状态码: {response.StatusCode}");
                }
                catch (JsonException)
                {
                    throw new Exception($"请求失败 [{response.StatusCode}]: {json}");
                }
            }

            var result = JsonSerializer.Deserialize<ApiResult<T>>(json, _jsonOptions) ?? throw new Exception("服务端响应格式异常，反序列化结果为空");
            if (!string.IsNullOrEmpty(result.Error))
            {
                throw new Exception($"服务器返回错误: {result.Error}");
            }

            return result.Data;
        }


        /// <summary>
        /// 现在进度
        /// </summary>
        /// <param name="bytesReceived">接收数据长度</param>
        /// <param name="totalBytes">数据总长度</param>
        private void OnDownloadProgressChanged(long bytesReceived, long totalBytes)
        {
            if (DownloadProgressChanged == null) return;

            double percentage = 0;
            if (totalBytes > 0)
            {
                percentage = Math.Round((double)bytesReceived / totalBytes * 100, 2);
            }

            DownloadProgressChanged.Invoke(this, new DownloadProgressArgs
            {
                BytesReceived = bytesReceived,
                TotalBytes = totalBytes,
                ProgressPercentage = percentage
            });
        }


        /// <summary>
        /// 对消销毁
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        #endregion
    }
}
