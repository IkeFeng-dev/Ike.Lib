using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace Ike.AspNet
{
	/// <summary>
	/// 嵌入式静态文件中间件,使用 <see langword="new "/> <see cref="Ike.AspNet.EmbeddedStaticFilesMiddleware"/>() 可查看调用示例
	/// </summary>
	public class EmbeddedStaticFilesMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly Assembly _assembly;
		private readonly string _namespace;

		private readonly string _outPath;
		private readonly string _inPath;

		/// <summary>
		/// 映射内嵌文件到静态文件的,根据资源文件后缀名返回指定MIME类型
		/// </summary>
		/// <param name="next"><inheritdoc cref="RequestDelegate"/></param>
		/// <param name="assembly">引用项目的程序集对象,用于获取内嵌资源,调用<see cref="Assembly.GetExecutingAssembly()"/>获取对象</param>
		/// <param name="namespace">资源文件所在的命名空间名称</param>
		/// <param name="outPath">输出到请求访问的地址,使用'/'分割</param>
		/// <param name="inPath">内嵌资源地址,使用'.'分割</param>
		/// <remarks>
		///  调用示例
		///  <br></br>
		///  <see langword="var"/> app = builder.Build();
		///  <br></br>
		///  app.UseMiddleware&lt;<see cref="EmbeddedStaticFilesMiddleware"/>&gt;(<see cref="Assembly.GetExecutingAssembly()"/>, "ProjectNamespace","/res",".res");
		/// <br></br> <br></br>
		/// <paramref name="outPath"/> / <paramref name="inPath"/>示例
		///  <br></br>
		///  命名空间:ProjectNamespace
		///  <br></br>
		/// 前端访问地址:http://127.0.0.1:5169/res/test.png
		/// <br></br>
		/// 资源文件路径: ProjectNamespace.res.test.png
		/// <br></br>
		/// 前端即可通过当前中间件访问到内嵌的res目录下test.png资源
		/// </remarks>

		public EmbeddedStaticFilesMiddleware(RequestDelegate next, Assembly assembly, string @namespace,string outPath = "/res",string inPath = ".res")
		{
			_next = next;
			_assembly = assembly;
			_namespace = @namespace;
			_outPath = outPath;
			_inPath = inPath;
		}

		/// <summary>
		/// 资源获取
		/// </summary>
		/// <param name="context">请求信息</param>
		/// <returns></returns>
		public async Task Invoke(HttpContext context)
		{
			var path = context.Request.Path.Value;
			if (path is not null && path.StartsWith(_outPath, StringComparison.OrdinalIgnoreCase))
			{
				var resourcePath = _namespace + path.Replace(_outPath, _inPath).Replace('/', '.');
				using var stream = _assembly.GetManifestResourceStream(resourcePath);
				if (stream != null)
				{
					var contentType = GetContentType(path);
					context.Response.ContentType = contentType;
					await stream.CopyToAsync(context.Response.Body);
					return;
				}
			}
			await _next(context);
		}

		/// <summary>
		/// 获取内容类型
		/// </summary>
		/// <param name="path">路径</param>
		/// <returns>MIME类型</returns>
		private static string GetContentType(string path)
		{
			var ext = Path.GetExtension(path).ToLowerInvariant();
			return ext switch
			{
				// Text Files
				".html" => "text/html",
				".htm" => "text/html",
				".css" => "text/css",
				".js" => "application/javascript",
				".json" => "application/json",
				".xml" => "application/xml",
				".txt" => "text/plain",
				".csv" => "text/csv",
				// Images
				".jpg" => "image/jpeg",
				".jpeg" => "image/jpeg",
				".png" => "image/png",
				".gif" => "image/gif",
				".bmp" => "image/bmp",
				".ico" => "image/x-icon",
				".svg" => "image/svg+xml",
				".webp" => "image/webp",
				// Audio/Video
				".mp3" => "audio/mpeg",
				".wav" => "audio/wav",
				".ogg" => "audio/ogg",
				".mp4" => "video/mp4",
				".webm" => "video/webm",
				// Fonts
				".woff" => "font/woff",
				".woff2" => "font/woff2",
				".ttf" => "font/ttf",
				".otf" => "font/otf",
				".eot" => "application/vnd.ms-fontobject",
				// Archives
				".zip" => "application/zip",
				".rar" => "application/x-rar-compressed",
				".tar" => "application/x-tar",
				".gz" => "application/gzip",
				// Others
				".pdf" => "application/pdf",
				".doc" => "application/msword",
				".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
				".xls" => "application/vnd.ms-excel",
				".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
				".ppt" => "application/vnd.ms-powerpoint",
				".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
				// Default
				_ => "application/octet-stream"
			};
		}
	}
}
