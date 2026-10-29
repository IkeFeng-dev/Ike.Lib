using Ike.ActionEngine.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 通用文件下载动作
    /// </summary>
    public class DownloadFileAction : UpdateAction
    {
        /// <summary>
        /// 请求 URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 本地保存绝对路径（支持 {AppDir} / {TempDir} 等宏变量）
        /// </summary>
        public string SavePath { get; set; }

        /// <summary>
        /// 请求方法 (GET, POST, PUT)，默认为 GET
        /// </summary>
        public string Method { get; set; } = "GET";

        /// <summary>
        /// 请求头 Headers (如 Authorization: Bearer xxx)
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// URL 查询参数 QueryParams
        /// </summary>
        public Dictionary<string, string> QueryParams { get; set; }

        /// <summary>
        /// 请求体 (如果下载接口需要 POST 携带 JSON 参数)
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 请求体类型，默认 application/json
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// 超时时间（秒），默认 120 秒
        /// </summary>
        public int TimeoutSeconds { get; set; } = 120;

        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string realUrl = context.ResolvePath(Url);
            string realSavePath = context.ResolvePath(SavePath);

            if (string.IsNullOrWhiteSpace(realUrl))
                throw new ArgumentNullException(nameof(Url), "下载 URL 不能为空");

            // 1. 自动拼接 Query 参数
            realUrl = HttpHandle.BuildUrlWithQueryParams(realUrl, QueryParams, context);
            GlobalVariable.Log.Debug($"{actionName} -> {realUrl} -> {realSavePath}");
            // 确保目标目录存在
            string saveDir = Path.GetDirectoryName(realSavePath);
            if (!string.IsNullOrEmpty(saveDir) && !Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
                // 2. 构造 HttpRequestMessage
                var httpMethod = new HttpMethod(Method.ToUpper());
                var request = new HttpRequestMessage(httpMethod, realUrl);
                // 3. 填充 Body
                if (!string.IsNullOrWhiteSpace(Body) && (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put))
                {
                    string realBody = context.ResolvePath(Body);
                    request.Content = new StringContent(realBody, Encoding.UTF8, ContentType);
                }
                // 4. 填充 Headers
                HttpHandle.ApplyHeaders(request, Headers, context);
                // 5. 发送请求并流式写入文件
                using (var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
                {
                    response.EnsureSuccessStatusCode();

                    using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                    using (var fileStream = new FileStream(realSavePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }

    }
}
