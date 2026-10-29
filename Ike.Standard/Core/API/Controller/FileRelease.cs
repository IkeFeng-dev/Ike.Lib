#if NET5_0_OR_GREATER
#nullable enable 

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using static Ike.Standard.Controller.UniversalMethod;

namespace Ike.Standard.Controller
{
    /// <summary>
    /// 文件管理控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FileReleaseController(IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// 配置档
        /// </summary>
        private readonly IConfiguration _configuration = configuration;

        #region 私有辅助方法

        /// <summary>
        /// 获取配置的根目录
        /// </summary>
        private string GetRootDirectory()
        {
            var dir = _configuration["wwwrootDIr"];
            if (string.IsNullOrEmpty(dir))
            {
                dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return Path.GetFullPath(dir);
        }

        /// <summary>
        /// 解析并校验相对路径，防止越界访问基目录之外的文件 (防目录遍历漏洞)
        /// </summary>
        private string GetSafeFullPath(string? relativePath)
        {
            var rootDir = GetRootDirectory();
            var combined = Path.Combine(rootDir, relativePath?.TrimStart('/', '\\') ?? string.Empty);
            var fullPath = Path.GetFullPath(combined);

            if (!fullPath.StartsWith(rootDir, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("非法访问：越界超出基目录范围！");
            }

            return fullPath;
        }

        /// <summary>
        /// 递归复制文件夹及其子文件
        /// </summary>
        private static void CopyDirectoryRecursive(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFilePath = Path.Combine(destinationDir, Path.GetFileName(file));
                System.IO.File.Copy(file, targetFilePath, overwrite: true);
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string targetSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                CopyDirectoryRecursive(subDir, targetSubDir);
            }
        }

        #endregion

        #region 接口实现

        /// <summary>
        /// 1. 获取指定目录下的文件和文件夹列表
        /// </summary>
        [HttpGet("list")]
        public IActionResult GetList([FromQuery] string? path = "")
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var targetPath = GetSafeFullPath(path);
                if (!Directory.Exists(targetPath))
                {
                    
                    return ApiResponse.Fail(StatusCodes.Status404NotFound, "指定目录不存在", StopAndGetMilliseconds(sw));
                }

                var rootDir = GetRootDirectory();
                var dirInfo = new DirectoryInfo(targetPath);
                var items = new List<FileSystemItemDto>();

                // 获取子文件夹
                foreach (var dir in dirInfo.GetDirectories())
                {
                    items.Add(new FileSystemItemDto
                    {
                        Name = dir.Name,
                        RelativePath = Path.GetRelativePath(rootDir, dir.FullName).Replace('\\', '/'),
                        IsDirectory = true,
                        Size = 0,
                        LastModified = dir.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }

                // 获取文件
                foreach (var file in dirInfo.GetFiles())
                {
                    items.Add(new FileSystemItemDto
                    {
                        Name = file.Name,
                        RelativePath = Path.GetRelativePath(rootDir, file.FullName).Replace('\\', '/'),
                        IsDirectory = false,
                        Size = file.Length,
                        LastModified = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
                    });
                }

                var data = new DirectoryListDto
                {
                    CurrentPath = path ?? "",
                    Items = items
                };

                
                return ApiResponse.Ok(data, StopAndGetMilliseconds(sw));
            }
            catch (UnauthorizedAccessException ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status403Forbidden, ex.Message, StopAndGetMilliseconds(sw));
            }
            catch (Exception ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status400BadRequest, ex.Message, StopAndGetMilliseconds(sw));
            }
        }

        /// <summary>
        /// 2. 读取文本文件内容
        /// </summary>
        [HttpGet("content")]
        public async Task<IActionResult> GetContent([FromQuery] string path)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var targetPath = GetSafeFullPath(path);
                if (!System.IO.File.Exists(targetPath))
                {
                    
                    return ApiResponse.Fail(StatusCodes.Status404NotFound, "文件不存在", StopAndGetMilliseconds(sw));
                }

                string content = await System.IO.File.ReadAllTextAsync(targetPath);
                var data = new FileContentDto
                {
                    Path = path,
                    Content = content
                };

                
                return ApiResponse.Ok(data, StopAndGetMilliseconds(sw));
            }
            catch (UnauthorizedAccessException ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status403Forbidden, ex.Message, StopAndGetMilliseconds(sw));
            }
            catch (Exception ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status400BadRequest, ex.Message, StopAndGetMilliseconds(sw));
            }
        }

        /// <summary>
        /// 3. 保存/更新文本文件内容
        /// </summary>
        [HttpPost("save")]
        public async Task<IActionResult> SaveContent([FromBody] SaveFileDto dto)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var targetPath = GetSafeFullPath(dto.Path);

                var dir = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                await System.IO.File.WriteAllTextAsync(targetPath, dto.Content ?? string.Empty);

                
                return ApiResponse.Ok("文件保存成功！", StopAndGetMilliseconds(sw));
            }
            catch (UnauthorizedAccessException ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status403Forbidden, ex.Message, StopAndGetMilliseconds(sw));
            }
            catch (Exception ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status400BadRequest, ex.Message, StopAndGetMilliseconds(sw));
            }
        }

        /// <summary>
        /// 4. 新建文件夹
        /// </summary>
        [HttpPost("create-folder")]
        public IActionResult CreateFolder([FromBody] CreateFolderDto dto)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var targetPath = GetSafeFullPath(dto.Path);
                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                
                return ApiResponse.Ok("文件夹创建成功！", StopAndGetMilliseconds(sw));
            }
            catch (UnauthorizedAccessException ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status403Forbidden, ex.Message, StopAndGetMilliseconds(sw));
            }
            catch (Exception ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status400BadRequest, ex.Message, StopAndGetMilliseconds(sw));
            }
        }

        /// <summary>
        /// 5. 上传文件到指定目录
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string? path = "")
        {
            var sw = Stopwatch.StartNew();
            try
            {
                if (file == null || file.Length == 0)
                {
                    
                    return ApiResponse.Fail(StatusCodes.Status400BadRequest, "请选择要上传的文件", StopAndGetMilliseconds(sw));
                }

                var targetDir = GetSafeFullPath(path);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                var filePath = Path.Combine(targetDir, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                
                return ApiResponse.Ok("文件上传成功！", StopAndGetMilliseconds(sw));
            }
            catch (UnauthorizedAccessException ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status403Forbidden, ex.Message, StopAndGetMilliseconds(sw));
            }
            catch (Exception ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status400BadRequest, ex.Message, StopAndGetMilliseconds(sw));
            }
        }

        /// <summary>
        /// 6. 删除文件或文件夹
        /// </summary>
        [HttpPost("delete")]
        public IActionResult Delete([FromQuery] string path)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var targetPath = GetSafeFullPath(path);

                if (Directory.Exists(targetPath))
                {
                    Directory.Delete(targetPath, recursive: true);
                    
                    return ApiResponse.Ok("文件夹删除成功！", StopAndGetMilliseconds(sw));
                }

                if (System.IO.File.Exists(targetPath))
                {
                    System.IO.File.Delete(targetPath);
                    
                    return ApiResponse.Ok("文件删除成功！", StopAndGetMilliseconds(sw));
                }

                
                return ApiResponse.Fail(StatusCodes.Status404NotFound, "文件或目录不存在", StopAndGetMilliseconds(sw));
            }
            catch (UnauthorizedAccessException ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status403Forbidden, ex.Message, StopAndGetMilliseconds(sw));
            }
            catch (Exception ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status400BadRequest, ex.Message, StopAndGetMilliseconds(sw));
            }
        }

        /// <summary>
        /// 7. 重命名文件或文件夹
        /// </summary>
        [HttpPost("rename")]
        public IActionResult Rename([FromBody] RenameFileDto dto)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var sourcePath = GetSafeFullPath(dto.Path);
                var parentDir = Path.GetDirectoryName(sourcePath);
                if (string.IsNullOrEmpty(parentDir))
                {
                    
                    return ApiResponse.Fail(StatusCodes.Status400BadRequest, "无效的源路径", StopAndGetMilliseconds(sw));
                }

                var targetPath = GetSafeFullPath(Path.Combine(parentDir, dto.NewName));

                if (System.IO.File.Exists(sourcePath))
                {
                    if (System.IO.File.Exists(targetPath))
                    {
                        
                        return ApiResponse.Fail(StatusCodes.Status400BadRequest, "已存在同名文件", StopAndGetMilliseconds(sw));
                    }
                    System.IO.File.Move(sourcePath, targetPath);
                    
                    return ApiResponse.Ok("文件重命名成功！", StopAndGetMilliseconds(sw));
                }

                if (Directory.Exists(sourcePath))
                {
                    if (Directory.Exists(targetPath))
                    {
                        
                        return ApiResponse.Fail(StatusCodes.Status400BadRequest, "已存在同名目录", StopAndGetMilliseconds(sw));
                    }
                    Directory.Move(sourcePath, targetPath);
                    
                    return ApiResponse.Ok("目录重命名成功！", StopAndGetMilliseconds(sw));
                }

                
                return ApiResponse.Fail(StatusCodes.Status404NotFound, "源文件或目录不存在", StopAndGetMilliseconds(sw));
            }
            catch (UnauthorizedAccessException ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status403Forbidden, ex.Message, StopAndGetMilliseconds(sw));
            }
            catch (Exception ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status400BadRequest, ex.Message, StopAndGetMilliseconds(sw));
            }
        }

        /// <summary>
        /// 8. 复制文件或文件夹到指定目录
        /// </summary>
        [HttpPost("copy")]
        public IActionResult Copy([FromBody] CopyFileDto dto)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var sourcePath = GetSafeFullPath(dto.SourcePath);
                var targetDir = GetSafeFullPath(dto.TargetDirectory);

                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                var name = Path.GetFileName(sourcePath);
                var destPath = Path.Combine(targetDir, name);

                if (System.IO.File.Exists(sourcePath))
                {
                    System.IO.File.Copy(sourcePath, destPath, overwrite: true);
                    
                    return ApiResponse.Ok("文件复制成功！", StopAndGetMilliseconds(sw));
                }

                if (Directory.Exists(sourcePath))
                {
                    if (destPath.StartsWith(sourcePath, StringComparison.OrdinalIgnoreCase))
                    {
                        
                        return ApiResponse.Fail(StatusCodes.Status400BadRequest, "无法将目录复制到自身或其子目录中", StopAndGetMilliseconds(sw));
                    }

                    CopyDirectoryRecursive(sourcePath, destPath);
                    
                    return ApiResponse.Ok("文件夹复制成功！", StopAndGetMilliseconds(sw));
                }

                
                return ApiResponse.Fail(StatusCodes.Status404NotFound, "源文件或目录不存在", StopAndGetMilliseconds(sw));
            }
            catch (UnauthorizedAccessException ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status403Forbidden, ex.Message, StopAndGetMilliseconds(sw));
            }
            catch (Exception ex)
            {
                
                return ApiResponse.Fail(StatusCodes.Status400BadRequest, ex.Message, StopAndGetMilliseconds(sw));
            }
        }

        #endregion
    }

}


#endif