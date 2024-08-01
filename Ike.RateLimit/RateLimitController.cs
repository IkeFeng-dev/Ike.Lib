using Microsoft.AspNetCore.Mvc;

namespace Ike.RateLimit
{
    /// <summary>
    /// 速率限制控制器，用于管理速率限制配置
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RateLimitController : ControllerBase
    {
        private readonly IRateLimitConfigService _configService;

        /// <summary>
        /// 初始化速率限制控制器实例
        /// </summary>
        /// <param name="configService">速率限制配置服务实例</param>
        public RateLimitController(IRateLimitConfigService configService)
        {
            _configService = configService;
        }

        /// <summary>
        /// 添加需要限制的路径和配置
        /// </summary>
        /// <param name="pathConfig">包含路径和配置的对象</param>
        /// <param name="password">访问密码</param>
        /// <returns>操作结果</returns>
        [HttpPost("Add")]
        public IActionResult AddPath([FromBody] PathConfig pathConfig, [FromQuery] string password)
        {
            if (!password.Equals(Flag.Password))
            {
                return BadRequest("Unauthorized access.");
            }
            _configService.AddPath(pathConfig);
            return Ok();
        }

        /// <summary>
        /// 移除指定名称的路径配置
        /// </summary>
        /// <param name="name">路径配置的名称</param>
        /// <param name="password">访问密码</param>
        /// <returns>操作结果</returns>
        [HttpPost("Remove")]
        public IActionResult RemovePath([FromBody] string name, [FromQuery] string password)
        {
            if (!password.Equals(Flag.Password))
            {
                return BadRequest("Unauthorized access.");
            }
            _configService.RemovePath(name);
            return Ok();
        }

        /// <summary>
        /// 更新指定路径的配置
        /// </summary>
        /// <param name="pathConfig">包含更新后路径和配置的对象</param>
        /// <param name="password">访问密码</param>
        /// <returns>操作结果</returns>
        [HttpPost("Update")]
        public IActionResult UpdatePath([FromBody] PathConfig pathConfig, [FromQuery] string password)
        {
            if (!password.Equals(Flag.Password))
            {
                return BadRequest("Unauthorized access.");
            }
            _configService.UpdatePath(pathConfig);
            return Ok();
        }

        /// <summary>
        /// 批量更新路径配置
        /// </summary>
        /// <param name="paths">包含多个路径和对应配置的字典</param>
        /// <param name="password">访问密码</param>
        /// <returns>操作结果</returns>
        [HttpPost("UpdateAll")]
        public IActionResult UpdatePaths([FromBody] IDictionary<string, PathConfig> paths, [FromQuery] string password)
        {
            if (!password.Equals(Flag.Password))
            {
                return BadRequest("Unauthorized access.");
            }
            _configService.UpdatePaths(paths);
            return Ok();
        }

        /// <summary>
        /// 获取所有需要限制的路径和配置
        /// </summary>
        /// <param name="password">访问密码</param>
        /// <returns>包含路径和对应配置的字典</returns>
        [HttpGet("Paths")]
        public IActionResult GetPaths([FromQuery] string password)
        {
            if (!password.Equals(Flag.Password))
            {
                return BadRequest("Unauthorized access.");
            }
            return Ok(_configService.GetPathsToLimit());
        }
    }
}
