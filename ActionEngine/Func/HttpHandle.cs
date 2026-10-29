using Ike.ActionEngine.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Ike.ActionEngine
{

    public class HttpHandle
    {
        /// <summary>
        /// 拼接 URL 查询参数（Query Parameters）
        /// <para>自动处理 URL 分隔符（'?' 与 '&amp;'）、解析宏变量替换并进行 URL 编码转义。</para>
        /// </summary>
        /// <param name="baseUrl">基础 URL 路径</param>
        /// <param name="queryParams">查询参数字典（Key-Value）</param>
        /// <param name="context">上下文环境（用于替换参数中的宏变量，如 {AppDir}）</param>
        /// <returns>拼接并编码转义后的完整 URL 字符串</returns>
        public static string BuildUrlWithQueryParams(string baseUrl, Dictionary<string, string> queryParams, UpdateContext context)
        {
            // 如果未配置查询参数或参数字典为空，直接返回原始基础 URL
            if (queryParams == null || queryParams.Count == 0) return baseUrl;

            var sb = new StringBuilder(baseUrl);
            // 检查基础 URL 中是否已经包含 '?' 标记（例如原始 URL 已经带有其他 query 参数）
            bool hasQuery = baseUrl.Contains("?");

            foreach (var kvp in queryParams)
            {
                // 若已包含 '?'，后续参数使用 '&' 拼接；否则首个参数使用 '?' 拼接
                sb.Append(hasQuery ? "&" : "?");
                hasQuery = true;

                // 步骤 1：先解析 Key 和 Value 中的宏变量（如 {AppDir}、{TempDir}）
                // 步骤 2：对 Key 和 Value 进行 URL 转义/编码，防止特殊字符（如空格、中文、&、= 等）导致 URL 格式非法
                string key = Uri.EscapeDataString(context.ResolvePath(kvp.Key));
                string value = Uri.EscapeDataString(context.ResolvePath(kvp.Value));

                sb.Append($"{key}={value}");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 应用自定义 HTTP 请求头（Request Headers）
        /// <para>支持宏变量替换，并能自动区分/适配请求头（Request Headers）与内容头（Content Headers）。</para>
        /// </summary>
        /// <param name="request">HTTP 请求消息对象 (HttpRequestMessage)</param>
        /// <param name="headers">自定义请求头字典（Key-Value）</param>
        /// <param name="context">上下文环境（用于替换 Header 中的宏变量）</param>
        public static void ApplyHeaders(HttpRequestMessage request, Dictionary<string, string> headers, UpdateContext context)
        {
            // 如果未配置请求头，直接跳过处理
            if (headers == null) return;

            foreach (var kvp in headers)
            {
                // 解析 Header 的 Key 和 Value 中可能存在的宏变量（例如："Authorization": "Bearer {Token}"）
                string key = context.ResolvePath(kvp.Key);
                string val = context.ResolvePath(kvp.Value);

                // 使用 TryAddWithoutValidation 避免非标准格式 Header 抛出 System.FormatException
                // 1. 优先尝试作为 Request 级别的请求头添加
                // 2. 如果添加失败（说明该 Header 属于 Content 级别，如 Content-Type、Content-Disposition、Content-Length），则添加到 Content.Headers 中
                if (!request.Headers.TryAddWithoutValidation(key, val))
                {
                    request.Content?.Headers.TryAddWithoutValidation(key, val);
                }
            }
        }

    }
}
