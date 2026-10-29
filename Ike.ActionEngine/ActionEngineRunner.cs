using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ike.ActionEngine
{
    /// <summary>
    /// 动作解析器运行
    /// </summary>
    public static class ActionEngineRunner
    {
        /// <summary>
        /// 基目录
        /// </summary>
        private static readonly string BaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tool");
        /// <summary>
        /// JSON目录
        /// </summary>
        private static readonly string JsonDir = Path.Combine(BaseDir, "Json");
        /// <summary>
        /// 获取输出目录中 Tool\ActionEngine.exe 的绝对路径
        /// </summary>
        public static string ExePath => Path.Combine(BaseDir, "ActionEngine.exe");

        /// <summary>
        /// 执行 ActionEngine 工具
        /// </summary>
        /// <param name="arguments">传递给 exe 的命令行参数</param>
        /// <returns>进程退出代码（0 代表成功）</returns>
        public static void Run(string json)
        {
            if (!File.Exists(ExePath))
            {
                throw new FileNotFoundException($"未找到工具引擎，请确认文件是否存在: {ExePath}");
            }
            Directory.CreateDirectory(JsonDir);
            string jsonPath = Path.Combine(JsonDir, DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".json");
            File.WriteAllText(jsonPath, json, Encoding.UTF8);
            var startInfo = new ProcessStartInfo
            {
                FileName = ExePath,
                Arguments = json,
                // 关键点 1：必须为 true，让 Windows Shell (explorer.exe) 启动它，解耦父子进程关系
                UseShellExecute = true,
                // 关键点 2：不能进行任何输出重定向，断开管道依赖
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                // 关键点 3：工作目录设为工具所在目录或当前目录
                WorkingDirectory = BaseDir
            };

            // 启动独立进程
            Process.Start(startInfo);
        }

        #region JsonBuilder Web Server

        private static HttpListener _listener;
        private static readonly object _serverLock = new object();
        private static bool _isServerRunning = false;
        private static string _currentServerUrl;

        /// <summary>
        /// 开启 JsonBuilder Web 访问服务
        /// </summary>
        /// <param name="openBrowser">是否自动在默认浏览器中打开链接</param>
        /// <param name="port">指定端口号，默认为 0（自动随机获取可用空闲端口）</param>
        /// <returns>返回可访问的 URL 路径</returns>
        public static string StartBuilderServer(bool openBrowser = false, int port = 0)
        {
            lock (_serverLock)
            {
                // 1. 防重复开启校验
                if (_isServerRunning || (_listener != null && _listener.IsListening))
                {
                    throw new InvalidOperationException($"JsonBuilder 服务已在运行中，请勿重复开启！当前访问地址: {_currentServerUrl}");
                }

                // 2. 动态分配可用端口号
                if (port <= 0)
                {
                    port = GetFreeTcpPort();
                }

                _currentServerUrl = $"http://localhost:{port}/";

                // 3. 初始化并启动 HttpListener
                _listener = new HttpListener();
                _listener.Prefixes.Add(_currentServerUrl);

                try
                {
                    _listener.Start();
                    _isServerRunning = true;
                }
                catch (Exception ex)
                {
                    _listener?.Close();
                    _listener = null;
                    _isServerRunning = false;
                    _currentServerUrl = null;
                    throw new InvalidOperationException($"启动 JsonBuilder Web 服务失败: {ex.Message}", ex);
                }

                // 4. 开启后台异步监听 HTTP 请求
                Task.Run(() => ListenRequestsAsync(_listener));

                // 5. 判断是否自动打开浏览器
                if (openBrowser)
                {
                    OpenBrowser(_currentServerUrl);
                }

                return _currentServerUrl;
            }
        }

        /// <summary>
        /// 关闭 JsonBuilder Web 访问服务
        /// </summary>
        public static void StopBuilderServer()
        {
            lock (_serverLock)
            {
                if (!_isServerRunning && _listener == null)
                {
                    return; // 未开启则直接返回
                }

                try
                {
                    if (_listener != null && _listener.IsListening)
                    {
                        _listener.Stop();
                        _listener.Close();
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _listener = null;
                    _isServerRunning = false;
                    _currentServerUrl = null;
                }
            }
        }

        /// <summary>
        /// 异步循环处理 HTTP 请求
        /// </summary>
        private static async Task ListenRequestsAsync(HttpListener listener)
        {
            while (listener != null && listener.IsListening)
            {
                try
                {
                    var context = await listener.GetContextAsync();
                    ProcessRequest(context);
                }
                catch (HttpListenerException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 读取并响应内嵌 HTML 内容
        /// </summary>
        private static void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                var response = context.Response;
                byte[] htmlBytes = GetEmbeddedHtmlBytes();

                response.ContentType = "text/html; charset=utf-8";
                response.ContentLength64 = htmlBytes.Length;

                using (var output = response.OutputStream)
                {
                    output.Write(htmlBytes, 0, htmlBytes.Length);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 读取内嵌资源 JsonBuilder.html 字节流
        /// </summary>
        private static byte[] GetEmbeddedHtmlBytes()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = null;

            // 自动查找名称以后缀 JsonBuilder.html 结尾的资源
            foreach (var name in assembly.GetManifestResourceNames())
            {
                if (name.EndsWith("JsonBuilder1.html", StringComparison.OrdinalIgnoreCase))
                {
                    resourceName = name;
                    break;
                }
            }

            if (string.IsNullOrEmpty(resourceName))
            {
                string errorMsg = RenderErrorPage(
                    code: "RESOURCE_NOT_FOUND",
                    title: "未找到内嵌资源 JsonBuilder.html",
                    description: "系统无法读取到前端工具界面的 HTML 模板文件",
                    suggestion: "/"
                );
                return Encoding.UTF8.GetBytes(errorMsg);
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    string errorMsg = RenderErrorPage(
                        code: "STREAM_READ_FAILED",
                        title: "无法读取内嵌资源流",
                        description: "找到了资源定义，但尝试读取 <code>JsonBuilder.html</code> 文件流时失败",
                        suggestion: "请检查该 HTML 文件权限或重新尝试重新生成（Rebuild）项目"
                    );
                    return Encoding.UTF8.GetBytes(errorMsg);
                }

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }

        }



        /// <summary>
        /// 渲染简约美观的错误提示 HTML 页面
        /// </summary>
        private static string RenderErrorPage(string code, string title, string description, string suggestion)
        {
            return $@"<!DOCTYPE html>
            <html lang=""zh-CN"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Ike.ActionEngine - {title}</title>
                <style>
                    * {{ box-sizing: border-box; margin: 0; padding: 0; }}
                    body {{
                        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
                        background-color: #f8fafc;
                        color: #1e293b;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        min-height: 100vh;
                        padding: 20px;
                    }}
                    .card {{
                        background: #ffffff;
                        border-radius: 16px;
                        box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.01);
                        border: 1px solid #e2e8f0;
                        max-width: 500px;
                        width: 100%;
                        padding: 36px 32px;
                        text-align: center;
                    }}
                    .icon-box {{
                        width: 56px;
                        height: 56px;
                        background-color: #fef2f2;
                        color: #ef4444;
                        border-radius: 50%;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        margin: 0 auto 20px;
                    }}
                    .icon-box svg {{
                        width: 28px;
                        height: 28px;
                    }}
                    .badge {{
                        display: inline-block;
                        font-size: 11px;
                        font-weight: 700;
                        color: #94a3b8;
                        background-color: #f1f5f9;
                        padding: 3px 10px;
                        border-radius: 12px;
                        margin-bottom: 12px;
                        letter-spacing: 0.8px;
                    }}
                    h1 {{
                        font-size: 20px;
                        font-weight: 600;
                        color: #0f172a;
                        margin-bottom: 10px;
                    }}
                    p {{
                        font-size: 14px;
                        line-height: 1.6;
                        color: #64748b;
                        margin-bottom: 24px;
                    }}
                    .suggestion-box {{
                        background-color: #fff1f2;
                        border-left: 4px solid #f43f5e;
                        padding: 14px 16px;
                        text-align: left;
                        border-radius: 0 10px 10px 0;
                        font-size: 13px;
                        color: #9f1239;
                        line-height: 1.6;
                    }}
                    code {{
                        font-family: ui-monospace, SFMono-Regular, Menlo, Monaco, Consolas, monospace;
                        background-color: rgba(255, 255, 255, 0.7);
                        color: #be123c;
                        padding: 2px 6px;
                        border-radius: 4px;
                        font-weight: 600;
                        font-size: 90%;
                        border: 1px solid rgba(225, 29, 72, 0.2);
                    }}
                </style>
            </head>
            <body>
                <div class=""card"">
                    <div class=""icon-box"">
                        <svg fill=""none"" stroke=""currentColor"" viewBox=""0 0 24 24"" xmlns=""http://www.w3.org/2000/svg"">
                            <path stroke-linecap=""round"" stroke-linejoin=""round"" stroke-width=""2"" d=""M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z""></path>
                        </svg>
                    </div>
                    <div class=""badge"">{code}</div>
                    <h1>{title}</h1>
                    <p>{description}</p>
                    <div class=""suggestion-box"">
                        <strong>建议排查：</strong><br>
                        {suggestion}
                    </div>
                </div>
            </body>
            </html>";
        }


        /// <summary>
        /// 获取当前系统闲置可用的 TCP 端口
        /// </summary>
        private static int GetFreeTcpPort()
        {
            var tcpListener = new TcpListener(IPAddress.Loopback, 0);
            tcpListener.Start();
            int freePort = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
            tcpListener.Stop();
            return freePort;
        }

        /// <summary>
        /// 调用系统默认浏览器打开指定的 URL
        /// </summary>
        private static void OpenBrowser(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        #endregion

    }
}
