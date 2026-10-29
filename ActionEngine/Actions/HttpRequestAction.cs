using Ike.ActionEngine.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 通用 HTTP API 请求动作 (专用于 GET/POST/PUT/DELETE 数据提交与获取)
    /// </summary>
    public class HttpRequestAction : UpdateAction
    {
        /// <summary>
        /// 请求 URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// HTTP 方法 (GET, POST, PUT, DELETE, PATCH)，默认为 GET
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
        /// 请求体 Body (如 JSON 字符串)
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Body 内容类型，默认为 application/json
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// 超时时间（秒），默认 30 秒
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// 保存服务器响应文本的上下文变量名称 (存入 context.Variables)
        /// </summary>
        public string ResponseVariable { get; set; }

        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            string realUrl = context.ResolvePath(Url);
            if (string.IsNullOrWhiteSpace(realUrl))
                throw new ArgumentNullException(nameof(Url), "请求 URL 不能为空");

            // 1. 自动拼接 Query 参数
            realUrl = HttpHandle.BuildUrlWithQueryParams(realUrl, QueryParams, context);

            GlobalVariable.Log.Debug($"{actionName} [{Method}] -> {realUrl}");

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
                var httpMethod = new HttpMethod(Method.ToUpper());
                var request = new HttpRequestMessage(httpMethod, realUrl);

                // 2. 填充 Body（POST / PUT / PATCH 等方法）
                if (!string.IsNullOrWhiteSpace(Body) && httpMethod != HttpMethod.Get && httpMethod != HttpMethod.Head)
                {
                    string realBody = context.ResolvePath(Body);
                    request.Content = new StringContent(realBody, Encoding.UTF8, ContentType);
                }

                // 3. 填充 Headers
                HttpHandle.ApplyHeaders(request, Headers, context);

                // 4. 发送请求并读取响应文本
                using (var response = httpClient.SendAsync(request).GetAwaiter().GetResult())
                {
                    string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"HTTP 请求失败, 状态码: {response.StatusCode}, 响应: {responseBody}");
                    }

                    // 5. 保存到上下文变量供后续 Action 使用
                    if (!string.IsNullOrWhiteSpace(ResponseVariable))
                    {
                        context.Variables[ResponseVariable] = responseBody;
                        GlobalVariable.Log.Debug($"{actionName} -> 响应已保存至变量 [{ResponseVariable}]");
                    }
                    GlobalVariable.Log.Debug($"{actionName} -> 响应: {responseBody}");
                    GlobalVariable.Log.Success($"{actionName} -> OK");
                }
            }
        }


    }
}
