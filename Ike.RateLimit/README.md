# RateLimit 项目文档

## 项目简介

`RateLimit` 项目是一个用于在 ASP.NET Core 应用程序中实现速率限制的库。它允许你通过配置来限制每个IP地址对特定路径的API调用次数，从而防止过载和滥用。

## 平台

- .NET Core 6.0

## 必要引用

- `Microsoft.AspNetCore.Http`
- `Microsoft.AspNetCore.Mvc`

## 项目结构

- **RateLimitLibrary**
  - `PathConfig.cs`
  - `IRateLimitConfigService.cs`
  - `RateLimitConfigService.cs`
  - `RateLimitController.cs`
  - `ServiceCollectionExtensions.cs`

## 引用中间件

### Program.cs

```csharp
 public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 向容器中添加控制器服务
        builder.Services.AddControllers();

        // 注册速率限制配置服务
        builder.Services.AddSingleton<IRateLimitConfigService, RateLimitConfigService>();

        // 配置Swagger/OpenAPI以便生成API文档
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // 初始化需要限制的路径及其配置
        var pathsToLimit = new Dictionary<string, PathConfig>
        {
            { "api", new PathConfig { Name = "api", Path = "/api", TimeWindowInSeconds = 10, MaxRequests = 5 } },
            { "res", new PathConfig { Name = "res", Path = "/res/*.png", TimeWindowInSeconds = 20, MaxRequests = 2 } },
            { "*", new PathConfig { Name = "*", Path = "/*", TimeWindowInSeconds = 60, MaxRequests = 10 } }
        };

        // 使用速率限制中间件并传递API接口访问密码以及路径限制配置
        app.UseRateLimiting("API_Password",pathsToLimit);

        // 配置HTTP请求管道
        if (app.Environment.IsDevelopment())
        {
            // 如果是开发环境，使用开发者异常页面和Swagger
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"));
        }
        // 启用授权中间件
        app.UseAuthorization();

        // 映射控制器路由
        app.MapControllers();

        // 运行应用程序
        app.Run();
    }
}

