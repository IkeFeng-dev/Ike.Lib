using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Ike.Standard.Ini;

namespace Test.Framework
{
    //internal class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        Console.InputEncoding = Encoding.UTF8;
    //        Console.OutputEncoding = Encoding.UTF8;
    //        Ike.Standard.WinMethod.EnableAnsiSupport();
    //        try
    //        {
    //            Test();
    //        }
    //        catch (Exception ex)
    //        {
    //            Ike.Standard.Console.WriteLine(ex.ToString(), "#ff6024");
    //        }
    //        Console.ReadKey();
    //    }




    //}


    class Program
    {
        private static Dictionary<string, string> fileHashes = new Dictionary<string, string>();
        private static FileSystemWatcher watcher;
        private static readonly InlineDiffBuilder diffBuilder = new InlineDiffBuilder(new Differ());

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("精确文件修改监控程序");
            Console.WriteLine("---------------------");

            // 设置要监控的目录路径
            string directoryPath = @"F:\Desktop\Test"; // 替换为实际路径

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine($"目录不存在: {directoryPath}");
                return;
            }

            InitializeFileHashes(directoryPath, "*.txt");

            watcher = new FileSystemWatcher
            {
                Path = directoryPath,
                Filter = "*.txt",
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };

            watcher.Changed += OnFileChanged;
            watcher.Created += OnFileChanged;
            watcher.Deleted += OnFileDeleted;
            watcher.Renamed += OnFileRenamed;

            watcher.EnableRaisingEvents = true;

            Console.WriteLine($"正在精确监控目录: {directoryPath}");
            Console.ReadLine();

            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        private static void InitializeFileHashes(string directoryPath, string filter)
        {
            foreach (var filePath in Directory.GetFiles(directoryPath, filter, SearchOption.AllDirectories))
            {
                fileHashes[filePath] = CalculateFileHash(filePath);
            }
        }

        private static void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                // 暂停监控防止重复触发
                watcher.EnableRaisingEvents = false;

                // 增加等待时间确保文件可访问
                System.Threading.Thread.Sleep(500);

                if (File.Exists(e.FullPath))
                {
                    string currentContent = File.ReadAllText(e.FullPath);
                    string newHash = CalculateFileHash(e.FullPath);

                    // 如果文件已记录在字典中
                    if (fileHashes.TryGetValue(e.FullPath, out var oldRecord))
                    {
                        // oldRecord现在直接存储旧内容而不仅是哈希
                        if (newHash != CalculateFileHashFromContent(oldRecord))
                        {
                            Console.WriteLine($"\n文件已修改: {e.FullPath}");
                            Console.WriteLine($"修改时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                            var diffModel = diffBuilder.BuildDiffModel(oldRecord, currentContent);

                            if (diffModel.Lines.Any(x => x.Type != ChangeType.Unchanged))
                            {
                                Console.WriteLine("精确修改内容:");
                                PrintPreciseDifferences(diffModel);
                            }
                            else
                            {
                                Console.WriteLine("(文件哈希改变但内容差异未检测到)");
                            }

                            Console.WriteLine("---------------------");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\n新文件已创建: {e.FullPath}");
                        Console.WriteLine("---------------------");
                    }

                    // 更新记录为当前内容
                    fileHashes[e.FullPath] = currentContent;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理文件更改时出错: {ex.Message}");
            }
            finally
            {
                watcher.EnableRaisingEvents = true;
            }
        }

        private static string CalculateFileHashFromContent(string content)
        {
            using (var md5 = MD5.Create())
            {
                byte[] contentBytes = Encoding.UTF8.GetBytes(content);
                byte[] hashBytes = md5.ComputeHash(contentBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        private static void PrintPreciseDifferences(DiffPaneModel diffModel)
        {
            foreach (var line in diffModel.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("[+] ");
                        Console.WriteLine(line.Text);
                        break;
                    case ChangeType.Deleted:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("[-] ");
                        Console.WriteLine(line.Text);
                        break;
                    case ChangeType.Modified:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("[~] ");
                        Console.WriteLine(line.Text);
                        break;
                    case ChangeType.Unchanged:
                        // 不显示未更改的内容以保持简洁
                        // Console.ForegroundColor = ConsoleColor.Gray;
                        // Console.Write("[ ] ");
                        // Console.WriteLine(line.Text);
                        break;
                }
                Console.ResetColor();
            }
        }

        private static void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"\n文件已删除: {e.FullPath}");
            Console.WriteLine("---------------------");

            fileHashes.Remove(e.FullPath);
        }

        private static void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"\n文件已重命名: 从 {e.OldFullPath} 到 {e.FullPath}");
            Console.WriteLine("---------------------");

            if (fileHashes.TryGetValue(e.OldFullPath, out var hash))
            {
                fileHashes.Remove(e.OldFullPath);
                fileHashes[e.FullPath] = hash;
            }
        }

        private static string CalculateFileHash(string filePath)
        {
            try
            {
                using (var md5 = MD5.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }


}
