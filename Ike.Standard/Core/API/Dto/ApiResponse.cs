#if NET5_0_OR_GREATER
#nullable enable 

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ike.Standard.Controller
{
    /// <summary>
    /// API响应体
    /// </summary>
    public static class ApiResponse
    {
        /// <summary>
        /// 构建成功响应
        /// </summary>
        /// <param name="data">返回的业务数据</param>
        /// <param name="elapsedMs">服务端执行耗时，单位：毫秒</param>
        /// <param name="statusCode">实际 HTTP 响应状态码</param>
        public static IActionResult Ok<T>(T? data, long elapsedMs, int statusCode = StatusCodes.Status200OK)
        {
            var result = new ApiResult<T>
            {
                Data = data,
                ElapsedMs = elapsedMs
            };

            return new ObjectResult(result)
            {
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// 构建失败响应
        /// </summary>
        /// <param name="statusCode">实际 HTTP 响应状态码</param>
        /// <param name="error">错误描述信息</param>
        /// <param name="elapsedMs">服务端执行耗时，单位：毫秒</param>
        public static IActionResult Fail(int statusCode, string error, long elapsedMs)
        {
            var result = new ApiResult<object?>
            {
                Error = error,
                ElapsedMs = elapsedMs
            };

            return new ObjectResult(result)
            {
                StatusCode = statusCode
            };
        }
    }
}


#endif