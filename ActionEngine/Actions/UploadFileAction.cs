using Ike.ActionEngine.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Ike.ActionEngine.Actions
{
    /// <summary>
    /// 通用文件上传动作
    /// </summary>
    public class UploadFileAction : UpdateAction
    {
        /// <summary>
        /// 请求 URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 待上传的本地文件路径（支持 {AppDir} / {TempDir} 等宏变量）
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// HTTP 请求方法 (POST / PUT)，默认为 POST
        /// </summary>
        public string Method { get; set; } = "POST";

        /// <summary>
        /// 上传模式："Multipart" (表单, 默认) 或 "Binary" (二进制流/octet-stream)
        /// </summary>
        public string UploadType { get; set; } = "Multipart";

        /// <summary>
        /// 表单上传时的文件字段名称 (Multipart 模式下生效)，如 "file", "media"
        /// </summary>
        public string FileParamName { get; set; } = "file";

        /// <summary>
        /// 自定义上传文件名 (可选，若不指定则取真实文件名)
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 请求头 Headers
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// URL 查询参数 QueryParams
        /// </summary>
        public Dictionary<string, string> QueryParams { get; set; }

        /// <summary>
        /// 附加的文本表单数据 (Multipart 模式下随文件一起提交)
        /// </summary>
        public Dictionary<string, string> FormData { get; set; }

        /// <summary>
        /// 超时时间（秒），默认 120 秒
        /// </summary>
        public int TimeoutSeconds { get; set; } = 120;

        /// <summary>
        /// 接收服务器响应内容的上下文变量名称 (可选，存入 context.Variables)
        /// </summary>
        public string ResponseVariable { get; set; }

        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            try
            {
                string realUrl = context.ResolvePath(Url);
                string realFilePath = context.ResolvePath(FilePath);

                if (!File.Exists(realFilePath))
                    throw new FileNotFoundException($"找不到要上传的文件: {realFilePath}");

                // 1. 自动拼接 Query 参数
                realUrl = HttpHandle.BuildUrlWithQueryParams(realUrl, QueryParams, context);

                GlobalVariable.Log.Debug($"{actionName} -> {realFilePath} -> {realUrl}");

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(TimeoutSeconds);
                    var httpMethod = new HttpMethod(Method.ToUpper());
                    var request = new HttpRequestMessage(httpMethod, realUrl);

                    string uploadFileName = string.IsNullOrWhiteSpace(FileName)
                        ? Path.GetFileName(realFilePath)
                        : context.ResolvePath(FileName);

                    // 2. 根据 UploadType 构建 HttpContent
                    if (UploadType.Equals("Binary", StringComparison.OrdinalIgnoreCase))
                    {
                        // 二进制流直传模式 (application/octet-stream)
                        byte[] fileBytes = File.ReadAllBytes(realFilePath);
                        var byteContent = new ByteArrayContent(fileBytes);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        request.Content = byteContent;
                    }
                    else
                    {
                        // Multipart/form-data 表单上传模式
                        var multipartContent = new MultipartFormDataContent();

                        // A. 填充附加文本字段
                        if (FormData != null)
                        {
                            foreach (var kvp in FormData)
                            {
                                string fieldKey = context.ResolvePath(kvp.Key);
                                string fieldValue = context.ResolvePath(kvp.Value);
                                multipartContent.Add(new StringContent(fieldValue, Encoding.UTF8), fieldKey);
                            }
                        }

                        // B. 填充文件流
                        byte[] fileBytes = File.ReadAllBytes(realFilePath);
                        var fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                        string paramName = string.IsNullOrWhiteSpace(FileParamName) ? "file" : context.ResolvePath(FileParamName);
                        multipartContent.Add(fileContent, paramName, uploadFileName);

                        request.Content = multipartContent;
                    }

                    // 3. 填充 Headers
                    HttpHandle.ApplyHeaders(request, Headers, context);

                    // 4. 发送请求
                    using (var response = httpClient.SendAsync(request).GetAwaiter().GetResult())
                    {
                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (!response.IsSuccessStatusCode)
                        {
                            throw new HttpRequestException($"上传失败, HTTP 状态码: {response.StatusCode}, 响应: {responseBody}");
                        }

                        // 5. 将响应写入上下文变量 (如果配置了 ResponseVariable)
                        if (!string.IsNullOrWhiteSpace(ResponseVariable))
                        {
                            context.Variables[ResponseVariable] = responseBody;
                            GlobalVariable.Log.Debug($"{actionName} -> Response saved to Variable[{ResponseVariable}]");
                        }

                        GlobalVariable.Log.Debug($"{actionName} -> Response: {responseBody}");
                        GlobalVariable.Log.Success($"{actionName} -> OK");
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalVariable.Log.Exception(ex);
                throw;
            }
        }


    }
}
