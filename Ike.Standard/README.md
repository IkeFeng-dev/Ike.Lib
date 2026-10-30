# Ike.Standard

> 面向 .NET 应用的 C# 通用工具类库，包含文件、文本、配置、日志、网络、加解密等常用能力，并为部分目标框架提供桌面及 ASP.NET Core 辅助功能。

[![NuGet](https://img.shields.io/nuget/v/Ike.Standard)](https://www.nuget.org/packages/Ike.Standard)

## 目录

- [项目简介](#项目简介)
- [目标框架与功能差异](#目标框架与功能差异)
- [安装与引用](#安装与引用)
- [快速开始](#快速开始)
- [功能大纲](#功能大纲)
- [代码结构导航树](#代码结构导航树)
- [类型与成员详细索引](#类型与成员详细索引)
- [各目标框架使用指南](#各目标框架使用指南)
- [常见注意事项](#常见注意事项)
- [许可证](#许可证)

## 项目简介

`Ike.Standard` 将一组可复用的 C# 工具和辅助类整理为一个多目标框架类库。大部分基础工具可以直接用于所有已列出的目标框架；桌面 UI、Windows API 和 ASP.NET Core 辅助功能则按目标框架分别编译。

- **程序集 / NuGet 包：** `Ike.Standard`
- **当前包版本：** `7.0.0`
- **主要命名空间：** `Ike.Standard`
- **ASP.NET Core 扩展命名空间：** `Ike.Standard.Controller`
- **外部 API 验证命名空间：** `Ike.Core.API.External`

> 本文档只介绍 `Ike.Standard` 项目。具体公开成员及其参数以当前所选目标框架生成的程序集和 XML API 文档为准。

## 目标框架与功能差异

项目在 `Ike.Standard.csproj` 中声明了以下目标框架。引用 NuGet 包时，NuGet 会选择与调用方兼容的程序集；多目标项目建议明确指定应用本身的 `TargetFramework`。

| 目标框架 | 适用场景 | 特定功能 |
| --- | --- | --- |
| `netstandard2.0` | 兼容面较广的 .NET Standard 应用 | 通用工具；不包含本文所述的 .NET Framework 桌面扩展或 ASP.NET Core 扩展 |
| `netstandard2.1` | 支持 .NET Standard 2.1 的运行时 | 通用工具；不包含上述桌面或 ASP.NET Core 扩展 |
| `net46` | .NET Framework 4.6 | 通用工具及 .NET Framework 条件编译的 Windows / 桌面扩展 |
| `net461` | .NET Framework 4.6.1 | 通用工具及 .NET Framework 条件编译的 Windows / 桌面扩展 |
| `net462` | .NET Framework 4.6.2 | 通用工具及 .NET Framework 条件编译的 Windows / 桌面扩展 |
| `net47` | .NET Framework 4.7 | 通用工具及 .NET Framework 条件编译的 Windows / 桌面扩展 |
| `net471` | .NET Framework 4.7.1 | 通用工具及 .NET Framework 条件编译的 Windows / 桌面扩展 |
| `net472` | .NET Framework 4.7.2 | 通用工具及 .NET Framework 条件编译的 Windows / 桌面扩展 |
| `net48` | .NET Framework 4.8 | 通用工具及 .NET Framework 条件编译的 Windows / 桌面扩展 |
| `net481` | .NET Framework 4.8.1 | 通用工具及 .NET Framework 条件编译的 Windows / 桌面扩展 |
| `netcoreapp3.1` | .NET Core 3.1 应用 | 通用工具；ASP.NET Core 扩展使用 `NET5_0_OR_GREATER` 条件，不在此目标中提供 |
| `net5.0` | .NET 5 应用 | 通用工具、ASP.NET Core 扩展 |
| `net6.0` | .NET 6 应用 | 通用工具、ASP.NET Core 扩展，以及仅在 `net6.0` 条件下编译的 RSA 辅助方法 |
| `net7.0` | .NET 7 应用 | 通用工具、ASP.NET Core 扩展 |
| `net8.0` | .NET 8 应用 | 通用工具、ASP.NET Core 扩展 |

### 按框架选择功能

| 功能类别 | `netstandard2.0` / `netstandard2.1` | .NET Framework (`net46`–`net481`) | `netcoreapp3.1` | `net5.0`–`net8.0` |
| --- | :---: | :---: | :---: | :---: |
| 基础工具（文件、文本、转换、集合、校验、日志、常用网络等） | ✓ | ✓ | ✓ | ✓ |
| WinForms、WPF、注册表、Windows Hook 等桌面 / Windows 扩展 | — | ✓ | — | — |
| `Ike.Standard.Controller` 中的 ASP.NET Core 扩展 | — | — | — | ✓ |
| `Ike.Core.API.External` 外部 API 验证扩展 | — | — | — | ✓ |
| `Cryption.RsaEncryption` / `Cryption.RsaDecryption` | — | — | — | 仅 `net6.0` |

**重要说明**

1. “通用工具”表示该代码按项目目标编译；具体操作仍可能受操作系统、运行时权限、网络环境或目标框架 API 限制。
2. WinForms、WPF、注册表、鼠标 / 键盘钩子等使用 Windows 专属 API，只在 .NET Framework 目标中提供。项目的 `net6.0` 是普通 TFM，不是 `net6.0-windows`。
3. ASP.NET Core 扩展在 `net5.0`、`net6.0`、`net7.0`、`net8.0` 编译，并通过 `Microsoft.AspNetCore.App` 框架引用提供相关类型；它们不属于 `netstandard*`、`.NET Framework` 或 `netcoreapp3.1` 构建。
4. RSA 辅助实现受源码中的 `NET6_0` 条件控制，因此不要假定它在 `net7.0` 或更高目标中也可用。
5. 本项目当前没有声明 `net9.0` 或更高目标。需要其它 TFM 时，请先确认包中是否提供兼容资产。

## 安装与引用

### NuGet

```shell
dotnet add package Ike.Standard --version 7.0.0
```

也可以在 Visual Studio 的 NuGet 包管理器中搜索并安装 `Ike.Standard`。

### 命名空间

基础工具通常引用：

```csharp
using Ike.Standard;
```

使用 ASP.NET Core 通用控制器时还需引用：

```csharp
using Ike.Standard.Controller;
```

外部 API 验证扩展使用：

```csharp
using Ike.Core.API.External;
```

后两组 API 仅适用于上文列出的 `net5.0`–`net8.0` 目标。

## 快速开始

下面的示例展示基础日志、文件哈希和防抖器的用法。示例中的 API 属于 `Ike.Standard` 基础工具。

```csharp
using Ike.Standard;

public static class Program
{
    public static void Main()
    {
        var log = new LOG();
        log.Info("应用已启动");

        string sha256 = FileDir.GetFileSHA256(@"C:\data\example.txt");
        System.Console.WriteLine(sha256);

        var debouncer = new Debouncer("save", 500);
        if (debouncer.QueryDebouncer("save"))
        {
            debouncer.TriggerDebouncer("save");
        }
    }
}
```

> `LOG` 的默认行为会将日志写入文件并输出到控制台；不需要的输出通道可以通过对应日志方法的参数关闭。

## 功能大纲

以下按源码中的主要类型整理。类型默认位于 `Ike.Standard`，除非特别标注。

### 1. 文件、目录与配置

| 类型 / API | 用途 |
| --- | --- |
| `FileDir` | 文件和目录辅助操作；包括文件 SHA-256、文件编码识别、文本追加等 |
| `IniConfigManager<T>` | 将带有属性的配置对象与 INI 文件读写、解析；支持 `Load`、`Save` 和可选的自动更新 |
| `IniSectionAttribute`、`IniKeyAttribute` | 为配置属性指定 INI 节名和键名 |
| `FTP` (`Ftp`) | 通过 FTP 列出文件 / 目录、上传、下载、创建目录、删除和重命名 |

#### INI 配置示例

```csharp
using Ike.Standard;
using System;

public class AppSettings
{
    [IniSection("server")]
    [IniKey("host")]
    public string Host { get; set; } = "localhost";

    [IniSection("server")]
    [IniKey("port")]
    public int Port { get; set; } = 8080;
}

public static class SettingsExample
{
    public static void SaveAndLoad()
    {
        using (var settings = new IniConfigManager<AppSettings>(
            @"C:\app\settings.ini",
            autoUpdate: false))
        {
            settings.Data.Port = 9000;
            settings.Save();

            settings.Load();
            System.Console.WriteLine(settings.Data.Host);
        }
    }
}
```

`IniConfigManager<T>` 要求 `T` 有无参构造函数，并实现 `IDisposable`。如启用自动更新或文件监听，请在使用完成后释放实例。支持的数据类型以 `SupportedTypes()` 返回值和当前实现为准。

#### FTP 示例

```csharp
using Ike.Standard;

public static class FtpExample
{
    public static string[] ListFiles()
    {
        var ftp = new Ftp("192.168.1.20:21", "username", "password");
        ftp.CurrentDirectory = "/uploads";
        return ftp.GetFileList();
    }
}
```

FTP 服务器地址、认证信息、目录和网络连接需要由应用按实际环境配置。该类型使用 FTP 请求 API；在较新的 .NET 目标下请注意运行时对相关 API 的兼容性及弃用提示。

### 2. 日志、文本与常用工具

| 类型 | 主要用途 |
| --- | --- |
| `LOG`、`LogType`、`LogStruct` | 按详细、调试、信息、警告、成功、错误、致命等级别记录日志；可控制文件、控制台及事件输出 |
| `Text` | 文本处理辅助 |
| `Convert` | 常见数据及数组转换 |
| `Array` | 数组处理辅助 |
| `Calculate` | 常见计算、时间和百分比辅助 |
| `Validator` | 常用格式和数据校验 |
| `GetInfo` | 主机、网络及运行环境信息 |
| `ObjectProperty`、`ReflectionUtility` | 对象属性操作和反射辅助 |
| `Extensions` | 常用扩展方法 |

#### 日志示例

```csharp
using Ike.Standard;

public static class LogExample
{
    public static void Run()
    {
        var log = new LOG(
            logDirectory: @"C:\app\logs",
            consoleMaxRow: 2000);

        log.Info("已连接");
        log.Warning("配置项使用默认值", writeToFile: true);
        log.Error("处理失败", writeToConsole: false);
    }
}
```

`LOG` 构造函数还支持日期文件名格式、时间戳格式、线程 ID 和控制台颜色配置。可订阅 `LogEvent` 将日志转发到自定义 UI；按时间段记录日志时可使用 `StartRecordLog()` 与 `GetRecordLog()`。

### 3. 网络、队列与进程间通信

| 类型 | 主要用途 |
| --- | --- |
| `Sockets` | TCP Socket 消息发送及请求 / 接收；包含同步和异步方法 |
| `NetworkSocket` | 封装网络消息发送、等待接收和回调处理 |
| `DynamicQueue<T>` | 基于并发集合的 FIFO / LIFO 消费队列，可配置消费回调及容量 |
| `SharedMemory` | .NET Framework 条件编译的共享内存通信辅助 |
| `Network` | 常用网络相关辅助 |
| `Debouncer`、`DebouncerExecute` | 防抖状态及防抖执行辅助 |

#### Socket 示例

```csharp
using Ike.Standard;

public static class SocketExample
{
    public static async System.Threading.Tasks.Task<string> SendAsync()
    {
        return await Sockets.SendAndReceiveAsync(
            ip: "127.0.0.1",
            port: 9000,
            message: "ping",
            receiveTimeout: 5000);
    }
}
```

`Sockets` 的简单消息方法以 UTF-8 编码消息。调用方仍需按服务端协议处理消息边界、响应长度、超时和连接失败等情况。

### 4. 加解密

`Cryption` 提供整数异或、随机种子、AES 字符串和文件加解密；`net6.0` 目标另提供 RSA 字符串加解密方法。

```csharp
using Ike.Standard;

public static class EncryptionExample
{
    public static string EncryptAndDecrypt()
    {
        string cipherText = Cryption.AesEncrypt("示例文本", "0123456789abcdef");
        return Cryption.AesDecrypt(cipherText, "0123456789abcdef");
    }
}
```

AES 密钥参数按 **UTF-8 字节数** 校验，必须为 16、24 或 32 字节。上述现有字符串加密实现使用 ECB 模式，**不适用于需要现代安全保证的新设计或敏感数据保护**；ECB 不提供完整性认证，也可能泄漏数据模式。生产环境请优先选择经审查的认证加密方案，并妥善管理和轮换密钥。不要将密钥硬编码到源码或提交到版本控制。

`net6.0` 下的 RSA 方法为 `Cryption.RsaEncryption` / `Cryption.RsaDecryption`，密钥类为 `Cryption.RSASecretKey`。私钥必须保密，并在加密和解密两侧使用匹配的密钥对。

### 5. Windows 桌面和系统 API（仅 .NET Framework）

以下类型受 `NETFRAMEWORK` 等条件编译控制，不应在 `netstandard*`、`netcoreapp3.1` 或 `net5.0`–`net8.0` 目标中引用：

| 类型 / 命名空间 | 用途 |
| --- | --- |
| `Ike.Standard` 下的 `WinAPI`、`WinMethod`、`Keyboard`、`MouseHook` | Windows API、键盘 / 鼠标钩子及相关操作 |
| `Ike.Standard` 下的 `Regedit`、`Image`、`IniMap<T>` | 注册表、图像及 Windows 桌面配置辅助 |
| `Ike.Standard` 下的 `Forms` 相关类型 | WinForms 动画、图形、布局、Chart、进度及异常处理辅助 |
| `Ike.Standard.WPF` | WPF 控件查找、异常 / 日志、INI 映射及 `ViewModelBase` 等辅助 |

具体控件 API 和可用类型会随 .NET Framework 目标及对应桌面程序集引用而异。桌面项目请使用对应 Windows UI 项目类型，并在所选目标框架上编译确认。

### 6. ASP.NET Core 扩展（仅 .NET 5–8）

ASP.NET Core 扩展位于 `Ike.Standard.Controller`，可以按需启用类库控制器、构造统一 API 响应，并配置若干常见 Web 行为。

#### 按需启用通用控制器

以下示例启用类库中的文件管理控制器。调用 `MapControllers()` 后，控制器才会参与路由映射。

```csharp
using Ike.Standard.Controller;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddCommonControllers(options => options.EnableFileRelease());

var app = builder.Build();
app.MapControllers();
app.Run();
```

可通过 `CommonControllersOptions.Enable<TController>()` 显式启用其它控制器类型。只启用应用需要的控制器，并按宿主应用的认证、授权和文件访问策略配置文件管理接口。

#### 统一 API 响应

控制器可以使用 `ApiResponse.Ok<T>(data, elapsedMs, statusCode)` 和 `ApiResponse.Fail(statusCode, error, elapsedMs)` 创建 `IActionResult`：

```csharp
using Ike.Standard.Controller;
using Microsoft.AspNetCore.Mvc;

public class StatusController : ControllerBase
{
    [HttpGet("api/status")]
    public IActionResult Get()
    {
        return ApiResponse.Ok(new { Status = "ready" }, elapsedMs: 1);
    }
}
```

#### 可用的 Web 辅助扩展

`CommonWebExtensions` 还提供以下类别的扩展方法（使用前检查目标框架及方法签名）：

- CORS 策略注册与启用；
- 表单、Kestrel、IIS 及组合文件上传大小限制；
- 简单重定向与 URL 重写；
- 静态文件映射、Ping 端点和转发头；
- MVC JSON 序列化选项。

其中“允许所有来源”的 CORS 配置会放宽跨域访问范围，不应未经评估用于生产环境。上传限制及转发头配置也应结合宿主服务器和可信代理设置。

#### 外部 API 验证

`Ike.Core.API.External` 提供 `IExternalApiValidator`、`ExternalAuthOptions` 及 `AddExternalActionAuth`。通过 `MapController<TValidator>()` 或 `MapAction<TValidator>()` 将外部控制器 / Action 映射到验证器；验证器类型需要实现 `IExternalApiValidator`。此功能依赖 ASP.NET Core MVC，且只在 `net5.0`–`net8.0` 构建中提供。

## 代码结构导航树

本节按 **命名空间 → 类型 → 嵌套类型 / 成员** 展开，便于先定位 API，再跳转到下方的详细索引。各节点是否可用由目标框架条件编译决定，并非每个目标框架的程序集都包含树中的全部节点。请结合[目标框架与功能差异](#目标框架与功能差异)选择。

> **命名空间不等同于文件夹。** 例如源码文件位于 `Standard\DataInteraction`、`Standard\Socket` 下的 `NetworkSocket`、`SharedMemory`、`Sockets` 等类型，实际命名空间仍为 `Ike.Standard`；`Structure` 文件夹中的部分类型则是父类型的嵌套 enum / struct。

### `Ike.Standard` — 基础工具与 Windows API

- **文件、目录和配置**
  - [`FileDir`](#api-filedir)（静态 partial）
    - 文件：`GetFileEncodeType`、`GetFileSHA256`、`GetFileHashValue`、`CompareFileContent`
    - 文件 / 目录：`ReadinessFileDirectory`、`GetFileCreationTimeOrder`、`GetFileWriteTimeOrder`、`GetFileLastWriteTimeOrder`、`DirectoryCopy`
    - 内存映射 / 指针：`AppendTextMMF`、`IntPtrToFile`、`LoadBinFile`
    - INI：`GetIniSectionNames`、`GetIniSectionKeyValues`、`GetIniAllKeyValues`、`IniRead`、`ReadIniValue`、`IniReadToInt`、`IniReadToDouble`、`IniWrite`
    - 仅 .NET Framework：`GetDirectoriesByDepth`、`LoadFileToClipboard`、`SelectFolder`、`SelectFile`
  - [`IniConfigManager<T>`](#api-iniconfigmanager)（`IDisposable`）
    - 属性 / 事件：`Data`、`UpdateEvent`
    - 生命周期 / 查询：构造函数、`Dispose`、`SupportedTypes`
    - 序列化 / 解析：`Save`（3 个重载）、`Load`（3 个重载）、`LoadFromString`、`AnalyseData`（3 个重载）
  - [`IniSectionAttribute`](#api-ini-attributes)：属性 `SectionName`
  - [`IniKeyAttribute`](#api-ini-attributes)：属性 `KeyName`
  - [`Ftp`](#api-ftp)：属性 `CurrentDirectory`；目录切换 `CDIn` / `CDOut`；列表、上传、下载、目录创建 / 删除及文件重命名
    - 嵌套结构：`Ftp.ResponseInfo`
- **日志与输出**
  - [`LOG`](#api-log)：属性 `LogDirectory`、`ErrorLogDirectory`、`QueueNumbers`；事件 `LogEvent`
    - 配置 / 子类型：`LOG.LogColors`、`LOG.LogStruct`、`LOG.LogType`、`LOG.LogEventHandler`
    - 输出：`Log`、`Verbose`、`Debug`、`Info`、`Warning`、`Success`、`Error`、`Fatal`、`Exception`
    - 记录 / 转发：`StartRecordLog`、`GetRecordLog`、`ToConsole`、`ToDebugger`
  - [`Console`](#api-base-utilities)：彩色 `Write` / `WriteLine`、`Foreground`、`ResetColor`、`InputPassword`、`IsConsoleEnv`
- **文本、数据与校验**
  - [`Text`](#api-base-utilities)：填充、路径模板、模式匹配、组合生成、字符串提取、字符提取
  - [`Array`](#api-base-utilities)：CSV 解析、数组转换 / 合并 / 比较 / 查找、二维数组行删除
  - [`Convert`](#api-base-utilities)：数组 / 文件 / 字节 / 颜色 / 枚举转换
  - [`Calculate`](#api-base-utilities)：平均时间、时间区间比较、百分比计算
  - [`Validator`](#api-base-utilities)：文件名 / 字符、UTF-8、IP / 链接、数字、日期、字符及邮件校验
  - [`Extensions`](#api-base-utilities)：枚举 / 字节 / 颜色 / 字符串转换，集合安全计数和判空，原子整数操作，数组追加
- **对象、反射与环境**
  - [`ObjectProperty`](#api-base-utilities)：方法位置、整数类型检测、属性比较 / 复制、类型兼容、枚举值解析
    - 嵌套结构：`ObjectProperty.LibraryInfo`
  - [`ReflectionUtility`](#api-base-utilities)：程序集资源查询、程序集加载及类型 / 成员信息查询
    - 嵌套类型：`ReflectionUtility.Reflection`
    - 反射结果类型：`TypeDetails`、`MethodDetails`、`ParameterInfo`、`PropertyDetails`、`EventDetails`
  - [`GetInfo`](#api-base-utilities)：主机名、系统 / 进程位数、路径、IPv4 / IPv6 和网络适配器
- **网络、队列及进程间通信**
  - [`Sockets`](#api-sockets)：同步 / 异步消息发送与请求响应
    - 嵌套类型：`Sockets.SocketListen`、`Sockets.SocketSend`、`Sockets.MessageType`
  - [`NetworkSocket`](#api-sockets)（`IDisposable`）：`Send`、`SendWaitReceive`、`WaitReceiveMessage`；事件 `ReceiveEvent`、`LogEvent`
  - [`DynamicQueue<T>`](#api-dynamicqueue)：属性 `Count`、`PauseAdd`；事件 `ItemConsumed`；`Add`、`StartConsuming`、`CompleteAdding`、`SetConsumerAction`
    - 嵌套枚举：`DynamicQueue<T>.Type`（FIFO / LIFO）
  - [`Network`](#api-sockets)：`PingIp`、`IsPortOpen`、`IsPrivateIPAddress`
  - `SharedMemory`（仅 .NET Framework，`IDisposable`）：`Write`、`Read`、`WaitRead`、`WaitReceive`、`Clear`、`StartListener` / `StopListener`
    - 属性 / 事件 / 枚举：`MapName`、`ReceiveEvent`、`SharedMemory.Identity`
  - [`Debouncer`](#api-debouncer)：`AddDebouncer`、`RemoveDebouncer`、`QueryDebouncer`、`TriggerDebouncer`
  - [`DebouncerExecute`](#api-base-utilities)（`IDisposable`）：构造函数、`Invoke`、`Dispose`
- **加解密**
  - [`Cryption`](#api-cryption)：`XOREncryption`、`RandomSeed`、`AesEncrypt` / `AesDecrypt`、`AesFileEncrypt` / `AesFileDecrypt`
  - `Cryption.RSASecretKey`、`RsaEncryption`、`RsaDecryption`：**仅 `net6.0`**
- **Windows API 与互操作（仅 .NET Framework）**
  - [`WinAPI`](#api-structures)：Windows 原生 API 封装；委托 `HookProc`、`EnumWindowsProc`
    - 嵌套结构 / 枚举：`SHFILEINFO`、`SYSTEMTIME`、`SYSTEM_INFO`、`SECURITY_ATTRIBUTES`、`ExecutionState`
  - [`WinMethod`](#api-structures)：窗口查找 / 焦点 / 状态、窗口几何信息、控件读写、任务栏、电源及显示器操作
    - 相关枚举：`WindowsState`
  - [`Keyboard`](#api-structures)：组合键判断和按键模拟
    - `Keyboard.Hook`：属性 `Enabled`、事件 `KeyEvents`、`InstallHook` / `UninstallHook`
    - 嵌套类型：`Keyboard.CombinationKey`、`Keyboard.Hook.KeyBoardHookStruct`
  - [`MouseHook`](#api-structures)：属性 `Enabled`、`IsIntercept`、事件 `MouseEvents`、`InstallHook` / `UninstallHook`
    - 嵌套类型：`MouseKey`、`MSLLHOOKSTRUCT`、`POINT`
  - [`Regedit`](#api-structures)：注册表读写 / 菜单关联；嵌套类型 `Regedit.Paths`
  - [`Image`](#api-structures)：`ImageCompression`
  - [`Process`](#api-structures)：`GetProcesses`、`KillProcess`、`GetProcessState`、`GetProcessNumber`、`OpenProcess`、`RunCmd`、`RunBat`
    - 嵌套枚举：`Process.RunCmdType`
  - [`WinMessages`](#api-structures)：Windows 消息及键值常量分组
  - [`Structure`](#api-structures)：嵌套互操作结构 `RGB`、`RECT`、`Point`、`SystemTime`

### [`Ike.Standard.Forms`](#api-forms-wpf) — WinForms 辅助（仅 .NET Framework）

- `AnimationOperation`：`Zoom`、`ClickControl`、`ClearData`
  - 嵌套枚举：`ClickSelect`、`ZoomEdge`、`ZoomSelect`
- `AutoAdaptWindowsSize`（`IDisposable`）：`ScaleX`、`ScaleY`、`CalculateLayout`、`CreateAutoUpdater`、`Dispose`
- `ChartOperation`：图表轴 / 标签 / 数据格式设置与数据清理
  - 嵌套枚举：`Axis`、`AxisAttribute`、`ValueType`、`ValueTypeFormat`
- `GlobalException`：WinForms 全局异常处理
- `GraphicsOperation`：WinForms 绘图辅助
- `Layout`：窗体布局辅助
  - 嵌套枚举：`Alignment`、`Direction`、`FillDirection`、`Priority`、`RelativePosition`

### [`Ike.Standard.WPF`](#api-forms-wpf) — WPF 辅助（仅 .NET Framework）

- `ControlHandle`：遍历 / 查找 WPF 可视树控件
- `GlobalException`：WPF 全局异常处理
- `IniMap<T>`：WPF 配置映射 / 更新（`IDisposable`）
- `Log`：WPF 日志辅助；嵌套枚举 `Log.LogType`
- `ViewModelBase`：`PropertyChanged`、`NotifyPropertyChanged`
  - 嵌套命令：`ViewModelBase.RelayCommand`，实现 `ICommand`

### [`Ike.Standard.Controller`](#api-controller) — ASP.NET Core（仅 .NET 5–8）

- `CommonApiExtensions`：`AddCommonControllers`
- `CommonControllersOptions`：`Enable<TController>`、`EnableFileRelease`
- `SelectiveControllerFeatureProvider`：MVC 扫描控制器时按启用选项过滤库内控制器
- `CommonWebExtensions`：常量 `DefaultCorsPolicy`；CORS、上传大小、重定向 / 重写、静态文件、Ping、转发头、JSON 选项扩展
- `ApiResponse`：`Ok<T>`、`Fail`
- `ApiResult<T>`：属性 `Data`、`Error`、`ElapsedMs`、`Timestamp`
- `FileReleaseController`：`GetList`、`GetContent`、`SaveContent`、`CreateFolder`、`Upload`、`Delete`、`Rename`、`Copy`
- 文件管理 DTO：
  - `DirectoryListDto`：`CurrentPath`、`Items`
  - `FileSystemItemDto`：`Name`、`RelativePath`、`IsDirectory`、`Size`、`LastModified`
  - `FileContentDto`：`Path`、`Content`
  - `SaveFileDto`：`Path`、`Content`
  - `CreateFolderDto`：`Path`
  - `RenameFileDto`：重命名所需路径 / 名称
  - `CopyFileDto`：`SourcePath`、`TargetDirectory`
- `UniversalMethod`：通用 Web / API 方法

### [`Ike.Core.API.External`](#api-external) — 外部 API 验证（仅 .NET 5–8）

- `IExternalApiValidator`：验证器接口
- `ExternalAuthExtensions`：MVC 扩展 `AddExternalActionAuth`
- `ExternalAuthOptions`：规则集合 `Rules`、`MapController<TValidator>`、`MapAction<TValidator>`
- `ExternalAuthRule`：`ControllerName`、`ActionName`、`ValidatorType`
- `ExternalActionAuthConvention`：将配置规则映射到 MVC Action
- `ExternalAuthFilter`：异步 Action 验证过滤器

## 类型与成员详细索引

以下索引从“知道类型名后如何用”进一步列出主要公开成员与行为。类型的完整重载、边界条件和 XML 注释以所选 TFM 的 IntelliSense / XML 文档为准。

### <a id="api-filedir"></a>`Ike.Standard.FileDir`

`FileDir` 是静态 partial 类；`Standard\FileDir.cs` 提供跨目标的基础文件方法，`Framework\FileDir.cs` 仅为 .NET Framework 补充桌面交互方法。

| 成员 | 说明 |
| --- | --- |
| `GetFileEncodeType(filename)` | 检查文件并返回检测到的 `Encoding` |
| `GetFileSHA256(filePath)` | 计算文件 SHA-256 十六进制摘要 |
| `GetFileHashValue(filePath)` | 获取文件哈希值 |
| `CompareFileContent(sourceFile, compareFile)` | 比较两个文件内容 |
| `GetFileCreationTimeOrder` / `GetFileWriteTimeOrder` / `GetFileLastWriteTimeOrder` | 按文件时间排序查询目录文件 |
| `ReadinessFileDirectory(filePath)` | 准备文件或目录路径 |
| `DirectoryCopy(sourceDirName, destDirName, copySubDirs)` | 复制目录，可指定是否包含子目录 |
| `GetIniSectionNames(filePath)` | 获取 INI 节名称 |
| `GetIniSectionKeyValues(filePath, sectionName)` | 获取指定节的键值 |
| `GetIniAllKeyValues(filePath)` | 获取所有节和键值 |
| `IniRead(...)` / `ReadIniValue(...)` | 读取 INI 字符串值；`IniRead` 提供编码、缓冲区及默认值写回选项重载 |
| `IniReadToInt(...)` / `IniReadToDouble(...)` | 将 INI 值读取为数值 |
| `IniWrite(...)` | 写入 INI 键值 |
| `AppendTextMMF(text, filePath, encoding)` | 通过内存映射方式追加文本 |
| `IntPtrToFile(intPtr, length, filePath)` / `LoadBinFile(filePath)` | 指针与二进制文件间复制；`LoadBinFile` 返回的非托管内存须由调用方释放 |
| `.NET Framework: GetDirectoriesByDepth(...)` | 按指定深度查找目录 |
| `.NET Framework: LoadFileToClipboard(...)`、`SelectFolder(...)`、`SelectFile(...)` | 剪贴板、目录选择和文件选择 UI |

```csharp
using Ike.Standard;

public static class FileExample
{
    public static void ReadFileInfo()
    {
        string sha256 = FileDir.GetFileSHA256(@"C:\data\payload.bin");
        var sections = FileDir.GetIniSectionNames(@"C:\app\settings.ini");
    }
}
```

### <a id="api-iniconfigmanager"></a>`Ike.Standard.IniConfigManager<T>`

泛型配置读写器要求 `T : new()`，通过 `Data` 维护当前配置对象。构造参数为文件路径、是否启用自动更新以及可选编码。

| 成员 | 说明 |
| --- | --- |
| `Data` | 当前配置对象；调用 `Load` 后更新，调用 `Save` 时写入文件 |
| `UpdateEvent` | 自动更新功能触发时通知订阅者 |
| `SupportedTypes()` | 返回当前实现支持的值类型集合 |
| `Load()` / `Load(filePath)` / `Load(config)` | 从默认路径、指定路径或已有对象加载 |
| `LoadFromString(iniText)` | 从 INI 文本加载 |
| `Save()` / `Save(config)` / `Save(iniConfig)` | 保存当前对象、指定对象或文本 |
| `AnalyseData(config)` / `AnalyseData(iniText)` / `AnalyseData(lines)` | 在对象和 INI 文本 / 行之间解析转换 |
| `Dispose()` | 释放文件监听等资源 |

用于映射的属性可通过 `[IniSection("section")]` 和 `[IniKey("key")]` 指定节名与键名。自动更新模式会使用文件监听，应使用 `using` 或及时调用 `Dispose()`。

### <a id="api-ini-attributes"></a>`IniSectionAttribute` 与 `IniKeyAttribute`

这两个 Attribute 只应用于配置类的属性，用来把 C# 属性映射到 INI 的节名和键名。

| Attribute | 构造函数 | 只读属性 |
| --- | --- | --- |
| `IniSectionAttribute(sectionName)` | 指定 INI 节名 | `SectionName` |
| `IniKeyAttribute(keyName)` | 指定 INI 键名 | `KeyName` |

```csharp
using Ike.Standard;

public class AppSettings
{
    [IniSection("database")]
    [IniKey("connectionString")]
    public string ConnectionString { get; set; }
}
```

### <a id="api-log"></a>`Ike.Standard.LOG`、`LogType` 与 `LogStruct`

`LOG` 构造函数可配置日志目录、控制台最大行数、文件名日期格式、时间戳格式、线程 ID 和 `LogColors`。实例包含文件、控制台及事件输出的开关参数。

| 成员 | 说明 |
| --- | --- |
| `LogDirectory` / `ErrorLogDirectory` | 日志和错误日志目录，只读属性 |
| `QueueNumbers` | 当前待输出日志数量 |
| `LogEvent` | 日志事件；通过 `LogEventHandler(LogStruct)` 订阅 |
| `Verbose`、`Debug`、`Info`、`Warning`、`Success`、`Error`、`Fatal` | 各日志级别快捷方法；参数可分别控制事件、控制台、文件输出 |
| `Exception(exception, ...)` | 记录异常 |
| `Log(info, logType, writeToConsole, writeToFile, writeToEvent)` | 通用日志入口 |
| `StartRecordLog()` / `GetRecordLog(isStopRecordLog)` | 开始收集与读取一段时间的日志文本 |
| `ToConsole(...)` / `ToDebugger(text)` | 显式写入控制台或调试输出 |
| `LogColors` | 可配置 `Type`、`Verbose`、`Debug`、`Info`、`Warning`、`Error`、`Fatal`、`Success` 等级颜色 |
| `LogType` | `Verbose`、`Debug`、`Info`、`Warning`、`Error`、`Fatal`、`Success` 等日志等级 |
| `LogStruct` | 单条日志事件数据；包含日志文本、等级、线程及输出相关字段 |

```csharp
using Ike.Standard;

public static class LogExample
{
    public static void WriteLogs()
    {
        var log = new LOG(logDirectory: @"C:\app\logs");
        log.Info("服务启动");
        log.Error("只写文件，不写控制台", writeToConsole: false);
    }
}
```

### <a id="api-ftp"></a>`Ike.Standard.Ftp` 与 `Ftp.ResponseInfo`

通过 `new Ftp(server, user, password)` 初始化；`CurrentDirectory` 表示工作目录。目录浏览使用 `GetFileList` / `GetDirectoryList`，传输使用 `Upload` / `Download`，另提供目录批量传输与管理操作。

| 成员组 | 成员 |
| --- | --- |
| 路径 | `CurrentDirectory`、`CheckPath`、`CDIn`、`CDOut`、`UpdateUser` |
| 查询 | `GetResponseInfo`、`GetFileList`、`GetDirectoryList`、`GetResponse`、`CheckDirectory` |
| 上传 / 下载 | `Upload`（3 个重载）、`UploadDirectory`、`Download`（3 个重载）、`DownloadDirectory` |
| 目录 / 文件管理 | `CreateDirectory`、`DeleteFile`、`DeleteDirectory`、`RenameFile` |
| 响应结构 | `Ftp.ResponseInfo`：`IsFile`、`Limits`、`Size`、`Modification`、`Name` |

```csharp
using Ike.Standard;

public static class FtpUploadExample
{
    public static bool Upload()
    {
        var ftp = new Ftp("ftp.example.com:21", "user", "password");
        ftp.CurrentDirectory = "/incoming";
        return ftp.Upload(@"C:\data\report.csv");
    }
}
```

文件传输会访问远程服务器并读写本地路径；应检查返回值，并处理认证、目录权限、网络失败等情况。

### <a id="api-sockets"></a>`Ike.Standard.Sockets` 与 TCP 类型

`Sockets` 静态方法使用 UTF-8 字符串消息，既可简单发送，也可发送并等待一段响应。

| 成员 | 说明 |
| --- | --- |
| `SendMessage(ip, port, message)` / `SendMessageAsync(...)` | 连接 TCP 端点并发送，不读取响应 |
| `SendAndReceive(ip, port, message, receiveTimeout)` / `SendAndReceiveAsync(...)` | 发送后读取一次响应，默认接收超时 5000ms |
| `SocketListen` | TCP 监听器；`Start()` 开始监听，`Stop()` 停止；构造函数可接收消息处理及回复回调；属性 `ReceiveStatus` 表示监听状态 |
| `SocketSend` | TCP 发送端；`Send(message)` 或 `SendToServer(ip, port, message)` 发送消息 |
| `MessageType` | 监听 / 发送回调中用于区分消息类别的枚举 |
| `Network` | `PingIp(ip)`、`IsPortOpen(ip, port)`、`IsPrivateIPAddress(ipAddress)` |
| `NetworkSocket` | 构造参数包含目标 / 本地端口、连接数和接收回调；事件 `ReceiveEvent` / `LogEvent`；`Send`、`SendWaitReceive`、`WaitReceiveMessage`；必须释放 |
| `SharedMemory` | 属性 `MapName`；事件 `ReceiveEvent`；`StartListener`、`StopListener`、`Write`、`Read`、`WaitRead`、`WaitReceive`、`Clear`（2 重载）、`GetProperty`、`Dispose` |

TCP 是字节流协议；简单 `SendAndReceive` 仅读取一次，不会自动实现应用层分帧或完整消息协议。若消息可能超过单次读取缓冲区，应由通信协议明确长度 / 结束标志并自行循环读取。

### <a id="api-dynamicqueue"></a>`Ike.Standard.DynamicQueue<T>`

并发队列支持 FIFO / LIFO、可选最大容量和消费委托。默认容量 `0` 表示不限制，默认顺序为 `DynamicQueue<T>.Type.FIFO`。

| 成员 | 说明 |
| --- | --- |
| `DynamicQueue(consumerAction, type, boundedCapacity)` | 创建队列；可以传入消费函数或订阅 `ItemConsumed` |
| `Add(item)` | 尝试添加元素；暂停添加或有界队列无法加入时返回 `false` |
| `StartConsuming()` | 启动后台消费 |
| `SetConsumerAction(consumerAction)` | 设置 / 更换消费委托 |
| `CompleteAdding()` | 标记不再接收新元素并结束消费枚举 |
| `Count` / `PauseAdd` | 查询队列数量 / 暂停新增 |
| `ItemConsumed` | 消费一个项目后触发 |
| `Type.FIFO` / `Type.LIFO` | 队列消费顺序 |

```csharp
using Ike.Standard;

public static class QueueExample
{
    public static bool Enqueue()
    {
        var queue = new DynamicQueue<string>(
            item => System.Console.WriteLine(item),
            DynamicQueue<string>.Type.FIFO,
            boundedCapacity: 100);
        queue.StartConsuming();
        return queue.Add("job-001");
    }
}
```

### <a id="api-debouncer"></a>`Ike.Standard.Debouncer`

可在同一实例中使用多个 `id` 独立维护防抖间隔。`QueryDebouncer` 仅查询是否已到间隔；成功执行目标操作后调用 `TriggerDebouncer` 重置计时。

| 成员 | 说明 |
| --- | --- |
| `Debouncer(id, millisecond)` | 创建并注册默认防抖项 |
| `AddDebouncer(id, millisecond)` | 新增或更新间隔；要求毫秒数大于 0 |
| `RemoveDebouncer(id)` | 移除指定项；省略 ID 时尝试移除首项 |
| `QueryDebouncer(id)` | 判断当前项是否已达到防抖间隔 |
| `TriggerDebouncer(id)` | 重置对应项时间戳 |

### <a id="api-cryption"></a>`Ike.Standard.Cryption`

| 成员 | 适用范围 / 说明 |
| --- | --- |
| `XOREncryption<T>(number, key)` | 基础工具；仅整数泛型类型 |
| `RandomSeed()` | 返回随机种子 |
| `AesEncrypt(text, key)` / `AesDecrypt(ciphertext, key)` | 字符串 AES；密钥 UTF-8 字节数须为 16 / 24 / 32 |
| `AesFileEncrypt(sourceFile, outputFile, secretKey)` / `AesFileDecrypt(...)` | AES 文件加解密，返回文件操作结果 |
| `RSASecretKey` | 仅 `net6.0`；创建 RSA 密钥对，分别提供 `PublicKey` / `PrivateKey` |
| `RsaEncryption(text, publicKey)` / `RsaDecryption(ciphertext, privateKey)` | 仅 `net6.0`；使用 RSA OAEP-SHA256 |

> 字符串 AES 实现使用 ECB 模式，仅为保持与现有实现一致而记录于此；不要将其当作新系统的安全加密设计。密钥应使用安全存储，私钥不可公开。

### <a id="api-base-utilities"></a>`Ike.Standard` 基础工具成员索引

| 类型 | 成员 / 分组说明 |
| --- | --- |
| `Array` | `MultipleCsvFilePathToArray`、`CsvToArray`（2 重载）、`ConvertArray<T>`、`CombineArrays<T>`、`AddElementsToArray<T>`、`DeleteRowFrom2DArray<T>`、`ArrayTrimEmpty`、`ArrayReversal<T>`、`IntArrayToDoubleArray`、`DoubleToInt`、`CompareArraySameContent<T>`、`CompareArrayDifferentContent<T>`、`GetArrayDifferentContent<T>`、`FindContentIndex<T>` |
| `Text` | 字段 `SpecialCharacters`；`PadString`、`ResolvePathWithDateTime`、`MatchPattern`（字符串 / 集合重载）、`GetAllCombinations`（2 重载）、`ExtractString`、`ExtractAllStrings`、`CopyString`、`ContainsChinese`、`ExtractNumber`、`ExtractLowerLetter`、`ExtractUpperLetter`、`ExtractLetter` |
| `Convert` | `ConvertArrayToString`、`ConvertToBoolean`、`HexToRGB`、`RgbToHex`（2 重载）、`ToHex(RGB)`、`FileToBytes`、`FileToStream`、`ColorToHex`、`StringToBytes`、`ValueToEnum<T>`、`NameToEnum<T>` |
| `Calculate` | `GetAverageTime`、`CompareTime`、`CalculationPercentage`（`int` / `double` 重载） |
| `Console` | `IsConsoleEnv`、彩色 `Write` / `WriteLine`（`RGB` / RGB 字节 / 十六进制颜色重载）、`Foreground`、`ResetColor`、`InputPassword` |
| `Validator` | `IsValidFileName`、`IsValidFileChars`、`IsUTF8Bytes`、`IsValidPLCAddress`、`IsPAddress`、`IsLink`、`ContainNumber`、`ContainLowerLetter`、`ContainUpperLetter`、`ContainUpperAndLowerLetter`、`IsNumeric`、`StringIsDate`、`StringIsTime`、`StringIsDateTime`、`StringIsDouble`、`StringIsChineseCharacters`、`StringIsNumber`、`StringContainChineseCharacters`、`StringIsMail` |
| `GetInfo` | 属性 `HostName`、`OperatingSystem`、`HostAddress`、`SystemEncoding`（返回当前线程 Culture 名称）、`MachineName`、`Soft_Bit`、`OperatingSystemBit`、`EnvDirectory`、`BaseDirectory`、`DesktopPath`、`GetCurrentProgramPath`；方法 `GetAdaptersName`、`GetIPv6Address`、`GetIPv4Address` |
| `ObjectProperty` | `GetExceptionLine`、`GetMethodInfo`（4 重载）、`IsIntegerType`、`CompareAllProperties<T>`、`CopyPropertiesStrict<T>`、`CopyPropertiesFlexible<TSource,TTarget>`、`IsTypeCompatible`、`GetEnumFlagsAsStringArray<T>`、`GetEnumFlagsAsEnumArray<T>`、`GetAllEnumValues<T>`、`GetAllEnumValuesToString<T>` |
| `Extensions` | 字符串 / 整数 `ToEnum<T>`、`ToBytes`、颜色 `ToHex`、`ExtractString`、`ConvertToBoolean`、日期 `Between`、集合 `SafeCount` / `IsNullOrEmpty`、整数原子加减 / 递增递减、`AddElements<T>` |
| `DebouncerExecute` | 构造函数 `DebouncerExecute(action, waitTimeMs, leadingExecution)`；`Invoke()` 排队执行；`Dispose()` 释放定时资源 |
| `ReflectionUtility` | 静态资源方法 `GetResourcesInfo` / `ReadEmbeddedResource` / `GetResourceToFile`；嵌套 `Reflection` 提供反射 API；实现 `IDisposable` |
| `ReflectionUtility.Reflection`：程序集与调用 | `LoadAssemblyFromFile`、`LoadAssemblyByte`、`GetAllClassTypeNames`、`GetGenericConstraints`、`InvokeStaticMethod`、`InvokeInstanceMethod`、`CreateInstance`、`CreateGenericInstance`、`CreateDelegate`、`ConvertEnumValue` |
| `ReflectionUtility.Reflection`：类型与成员 | `GetProperty`、`GetAllPublicTypes`、`GetMethodsInType`、`GetMethods`、`GetMethodFromInstance`、`GetTypeFromAssembly`、`GetMethodDetails`、`GetTypeDetails`、`GetPropertyDetails`、`GetInstanceEvent`、`GetEventsInType`、`GetAllEventsInTypeHierarchy`、`GetCustomAttributes`、`EventOperation` |

```csharp
using Ike.Standard;

using Ike.Standard;

public static class UtilityExample
{
    public static void Run()
    {
        double percent = Calculate.CalculationPercentage(250, 50);
        bool isValidIp = Validator.IsPAddress("127.0.0.1");
        string[] tokens = Text.ExtractAllStrings("a[one]b[two]", "[", "]");
    }
}
```

> `Ike.Standard.Convert`、`Ike.Standard.Console` 与 BCL 的 `System.Convert`、`System.Console` 同名。发生名称冲突时使用完整名称，如 `System.Console.WriteLine(...)`。

### <a id="api-controller"></a>`Ike.Standard.Controller` API 成员索引

以下成员只在 `net5.0`–`net8.0` 可用。

| 类型 | 成员 / 数据 |
| --- | --- |
| `CommonApiExtensions` | `AddCommonControllers(IMvcBuilder, configure)`：按需注册库内控制器 |
| `CommonControllersOptions` | `Enable<TController>()`、`EnableFileRelease()` |
| `SelectiveControllerFeatureProvider` | `PopulateFeature(parts, feature)`：移除未显式启用的库内控制器 |
| `CommonWebExtensions` | 常量 `DefaultCorsPolicy`；`AddAllowAllCorsFast`、`AddCorsForOriginsFast`、`AddEasyCorsFast`、`UseEasyCorsFast`；上传大小限制系列；`MapEasyRedirectFast` / `MapEasyRedirectsFast`；`UseEasyRewriteFast`；`UseEasyStaticFilesFast`；`MapEasyPingFast`；`UseEasyForwardedHeadersFast`；`AddEasyJsonOptionsFast` |
| `ApiResponse` | `Ok<T>(data, elapsedMs, statusCode)`、`Fail(statusCode, error, elapsedMs)` |
| `ApiResult<T>` | `Data`、`Error`、`ElapsedMs`、`Timestamp` |
| `FileReleaseController` | `GetList`、`GetContent`、`SaveContent`、`CreateFolder`、`Upload`、`Delete`、`Rename`、`Copy` |
| DTO | `DirectoryListDto`、`FileSystemItemDto`、`FileContentDto`、`SaveFileDto`、`CreateFolderDto`、`RenameFileDto`（`Path`、`NewName`）、`CopyFileDto`（`SourcePath`、`TargetDirectory`）各自的路径、内容、条目名称、类型和大小属性 |
| `UniversalMethod` | 通用 API / Web 支持方法；查看当前 TFM 的 XML 注释获取重载与参数 |

**宿主接入示例：**

```csharp
builder.Services
    .AddControllers()
    .AddCommonControllers(options => options.EnableFileRelease());
```

启用文件管理控制器会暴露文件系统相关操作；必须在宿主侧限制根目录、认证授权与可操作范围，不要直接向不可信网络开放。

### <a id="api-external"></a>`Ike.Core.API.External` API 成员索引

此命名空间属于外部 API 验证扩展（仅 .NET 5–8），与 `Ike.Standard.Controller` 的通用响应 / 控制器注册 API 分开。

| 类型 | 成员 / 说明 |
| --- | --- |
| `IExternalApiValidator` | `Task<bool> ValidateAsync(HttpContext context)`；返回 `true` 放行，返回 `false` 拒绝 |
| `ExternalAuthExtensions` | `AddExternalActionAuth(mvcOptions, configure)`：注册规则到 MVC 约定 |
| `ExternalAuthOptions` | `Rules`；`MapController<TValidator>(controllerName)`；`MapAction<TValidator>(controllerName, actionName)` |
| `ExternalAuthRule` | `ControllerName`、`ActionName`、`ValidatorType` |
| `ExternalActionAuthConvention` | 将规则附加到匹配的 MVC Action |
| `ExternalAuthFilter` | 调用验证器并继续 / 拒绝 Action 执行 |

```csharp
using Ike.Core.API.External;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public sealed class PartnerApiValidator : IExternalApiValidator
{
    public Task<bool> ValidateAsync(HttpContext context)
    {
        bool hasToken = context.Request.Headers.ContainsKey("X-Partner-Token");
        return Task.FromResult(hasToken);
    }
}
```

上例仅演示接口接入形状；实际验证应核对令牌签名、有效期和授权范围，不能只判断请求头是否存在。

### <a id="api-forms-wpf"></a>`Ike.Standard.Forms` 和 `Ike.Standard.WPF` 成员索引

此两组只用于 .NET Framework Windows 桌面目标；它们不是 .NET 5–8 Windows TFM API。

| 类型 | 主要成员 / 嵌套类型 |
| --- | --- |
| `Forms.AnimationOperation` | `Zoom`、`ClearData`、`ClickControl`；枚举 `ClickSelect`、`ZoomSelect`、`ZoomEdge` |
| `Forms.AutoAdaptWindowsSize` | `ScaleX`、`ScaleY`、`CalculateLayout`、`CreateAutoUpdater`、`Dispose` |
| `Forms.ChartOperation` | `CurrentChart`、轴属性 / 数据标签配置方法；枚举 `Axis`、`AxisAttribute`、`ValueType`、`ValueTypeFormat` |
| `Forms.GraphicsOperation` | `GetRandomColor`、`GetDarkerColor`、`GetStringWidth`、`DrawText`、`DrawGradient`、`DrawBorder`、`Draworder`、`DrawSolidBorder`、`DrawRectangle`（2 重载）、`DrawFilletGraphicsPath`、`GetRectangleFFillet`、`DrawRoundRect`（2 重载） |
| `Forms.Layout` | `FillMax`、`Move`、`SetFixedMargin`、`SetArrange`、`SetRelativePosition`、`SetAlignment`、`SetControlsCenterHorizontal`、`SetControlsScaleSize`、`SetBatchSort`（2 重载）；枚举 `Alignment`、`Direction`、`FillDirection`、`Priority`、`RelativePosition` |
| `Forms.GlobalException` | `Initialize(logDirectory, showError)`：WinForms 全局异常挂接 |
| `WPF.ControlHandle` | `GetAllChildControls<T>` 可视树查找 |
| `WPF.IniMap<T>` | 属性 `Data`、`IniConfigManager`；`Binding`、`RemoveBinding`、`SaveToFile`、`LoadToUI`、`Dispose` |
| `WPF.Log` | 颜色属性 `NormalColor`、`InfoColor`、`WarnColor`、`ErrorColor`、`SuccessColor`、`CustomColor`；`WriteLine`（2 重载）、`Normal`、`Info`、`Warn`、`Error`、`Success`、`Custom`；枚举 `LogType` |
| `WPF.ViewModelBase` | `PropertyChanged`、`NotifyPropertyChanged`；嵌套 `RelayCommand` 的 `CanExecute` / `Execute` / `RaiseCanExecuteChanged` |
| `WPF.GlobalException` | `Initialize(logDirectory, handled)`：WPF 全局异常挂接 |

### <a id="api-structures"></a>结构体、枚举与互操作数据类型

下列类型主要用于方法参数 / 返回值或 Windows 互操作；Windows 互操作条目仅限 .NET Framework。

| 所属类型 | 嵌套 enum / struct | 用途 |
| --- | --- | --- |
| `Structure` | `RGB`、`RECT`、`Point`、`SystemTime` | 颜色、矩形、坐标、系统时间 |
| `ObjectProperty` | `LibraryInfo` | 程序集 / 方法位置等信息 |
| `ReflectionUtility` | `TypeDetails`、`MethodDetails`、`ParameterInfo`、`PropertyDetails`、`EventDetails` | 反射查询结果模型 |
| `DynamicQueue<T>` | `Type` | FIFO / LIFO 队列模式 |
| `Ftp` | `ResponseInfo` | FTP 响应条目 |
| `Sockets` | `MessageType` | Socket 消息类别 |
| `LOG` | `LogType`、`LogStruct`、`LogColors` | 日志级别、日志记录和颜色 |
| `Keyboard` / `Keyboard.Hook` | `CombinationKey` / `KeyBoardHookStruct` | 按键组合及钩子数据 |
| `MouseHook` | `MouseKey`、`MSLLHOOKSTRUCT`、`POINT` | 鼠标按键及钩子数据 |
| `WinAPI` | `ExecutionState`、`SHFILEINFO`、`SYSTEMTIME`、`SYSTEM_INFO`、`SECURITY_ATTRIBUTES` | Windows API 标志和原生结构 |
| `WinMethod` | `WindowsState` | 窗口显示状态参数 |
| `Process` | `RunCmdType` | 命令运行模式 |
| `Regedit` | `Paths` | 注册表路径信息 |
| `WinMessages` | `MouseCap`、`HookCap`、`KeyValue`、`WindowsCap`、`DeviceCap`、`ConsoleCap`、`MemoryCap` | Windows 消息 / 常量分组 |
| `Forms.Layout` | `Alignment`、`Direction`、`FillDirection`、`Priority`、`RelativePosition` | 布局参数 |
| `Forms.AnimationOperation` | `ClickSelect`、`ZoomEdge`、`ZoomSelect` | 动画 / 控件操作参数 |
| `Forms.ChartOperation` | `Axis`、`AxisAttribute`、`ValueType`、`ValueTypeFormat` | 图表轴和数据格式参数 |

`Structure` 中常用字段：

| 结构 | 字段 |
| --- | --- |
| `Structure.RGB` | `R`、`G`、`B` |
| `Structure.RECT` | `Left`、`Top`、`Right`、`Bottom` |
| `Structure.Point` | `X`、`Y` |
| `Ftp.ResponseInfo` | `IsFile`、`Limits`、`Size`、`Modification`、`Name` |
| `WinAPI.SYSTEMTIME` | `Year`、`Month`、`DayOfWeek`、`Day`、`Hour`、`Minute`、`Second`、`Miliseconds` |
| `WinAPI.SYSTEM_INFO` | 处理器架构、页面大小、地址范围、处理器数量和分配粒度等原生字段 |

> 类型是否为顶层类型或嵌套类型会影响 C# 的完整名称。例如队列模式写为 `DynamicQueue<string>.Type.FIFO`，而不是单独的 `Type.FIFO`。

## 各目标框架使用指南

### .NET Standard 2.0 / 2.1

适合希望由多个 .NET 应用共享基础逻辑的类库。添加 `Ike.Standard` 包后使用 `Ike.Standard` 命名空间中的通用工具。不要在这些目标中依赖 WinForms、WPF、注册表、Windows Hook 或 `Ike.Standard.Controller` 的 ASP.NET Core 扩展。

选择 `netstandard2.0` 可覆盖较广的旧运行时；只有当消费端运行时支持 .NET Standard 2.1 时才选择 `netstandard2.1`。

### .NET Framework 4.6–4.8.1

项目列出的每个 .NET Framework TFM 均提供通用工具和对应的 .NET Framework 条件编译代码。桌面 / Windows 专用类型也只会在这组目标中出现。根据应用的 UI 技术选择 WinForms 或 WPF 项目，并在实际目标 TFM 下检查引用和编译结果。

### .NET Core 3.1

可使用基础工具；不要将 `net5.0` 及以上 ASP.NET Core 扩展的可用性假定为适用于 `netcoreapp3.1`。该目标面向仍在维护的 .NET Core 3.1 应用；请同时评估运行时生命周期和升级计划。

### .NET 5 / 6 / 7 / 8

这些目标可使用基础工具和 `Ike.Standard.Controller` / `Ike.Core.API.External` 中的 Web 扩展。ASP.NET Core 功能依赖宿主应用正确配置 MVC / 中间件，并由宿主应用负责身份验证、授权、CORS 和文件访问安全。

只有项目目标为 **`net6.0`** 时，当前库源码才包含 `Cryption` 中的 RSA 帮助方法；不可因 .NET 7 或 8 版本更高而假设该部分 API 已包含。

## 常见注意事项

1. **按目标框架构建：** 多目标包中的 API 并非在每个 TFM 中都相同。升级或更换项目目标后，重新检查 `using`、API 可用性和编译警告。
2. **平台能力：** 文件系统权限、Windows 专属操作、Socket 连接和服务器行为取决于部署环境；捕获并处理调用可能产生的异常。
3. **释放资源：** 对实现 `IDisposable` 的实例（例如 `IniConfigManager<T>`、`NetworkSocket`、`SharedMemory`、`DebouncerExecute`）使用 `using` 或显式调用 `Dispose()`。
4. **网络和文件接口：** FTP、Socket、静态文件和文件管理控制器应结合认证、访问控制、输入校验及部署网络策略使用。运行 `Process.RunCmd` / `RunBat` 时，不要将未验证的外部输入拼接进命令行。
5. **敏感数据：** 不要将密码、令牌、私钥或生产密钥写入示例、配置仓库或日志；加密 API 的具体限制请参阅[加解密](#4-加解密)。
6. **API 文档：** 项目启用了 XML 文档生成。需要了解完整参数、返回值和异常信息时，查看所安装包对应目标框架的 XML 文档或 IDE IntelliSense。

## 许可证

本项目许可证信息见仓库中的 [`LICENSE.txt`](../LICENSE.txt)。
