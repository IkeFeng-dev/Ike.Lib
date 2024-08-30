using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace Ike.AspNet
{
	/// <summary>
	/// 使用凭据登录的中间件
	/// </summary>
	public class BasicAuthMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly string _user;
		private readonly string _password;

		/// <summary>
		/// 初始化中间件实例
		/// </summary>
		/// <param name="next">下一个中间件委托</param>
		/// <param name="user">用户名</param>
		/// <param name="password">密码</param>
		public BasicAuthMiddleware(RequestDelegate next,string user,string password)
		{
			_next = next;
			_user = user;
			_password = password;
		}

		/// <summary>
		/// 中间件的主要处理逻辑
		/// </summary>
		/// <param name="context">HTTP上下文</param>
		/// <returns>任务实例</returns>
		public async Task InvokeAsync(HttpContext context)
		{
			string authHeader = context.Request.Headers["Authorization"];
			if (authHeader != null && authHeader.StartsWith("Basic "))
			{
				// 从请求头中获取凭据
				var header = AuthenticationHeaderValue.Parse(authHeader);

				if (header.Parameter is not null)
				{
					var inBytes = Convert.FromBase64String(header.Parameter);
					var credentials = Encoding.UTF8.GetString(inBytes).Split(':');
					var username = credentials[0];
					var password = credentials[1];
					// 验证凭据
					if (username.Equals(_user) && password.Equals(_password))
					{
						await _next.Invoke(context);
						return;
					}
				}
			}
			// 如果认证失败，设置响应头和状态码
			context.Response.Headers["WWW-Authenticate"] = "Basic";
			context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
		}

	}
}
