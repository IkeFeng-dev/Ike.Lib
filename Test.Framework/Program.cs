using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Mono.Cecil;
using Mono.Cecil.Cil;
using OpCodes = Mono.Cecil.Cil.OpCodes;


namespace Test.Framework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Ike.Standard.WinMethod.EnableAnsiSupport();
            try
            {


                //var client = new Ike.Standard.DataInteraction.NetworkSocket("127.0.0.1", 44444, 1082, 1);

                //client.LogEvent += (s) =>
                //{
                //    Console.WriteLine("LogEvent => " + s);
                //};

                //client.ReceiveEvent += (s) =>
                //{
                //    Console.WriteLine("ReceiveEvent => " + s);
                //    client.Send(s + "[OK]");
                //};


                var client = new Ike.Standard.DataInteraction.NetworkSocket("127.0.0.1", 44444, 55555, 1,null);
                client.LogEvent += (s) =>
                {
                    Console.WriteLine("LogEvent => " + s);
                };

                client.ReceiveEvent += (s) =>
                {
                    Console.WriteLine("ReceiveEvent => " + s);
                };
                bool r = client.WaitReceiveMessage(out string result,6000);

                Console.WriteLine(r + "  :" + result);

                client.Dispose();


                //string dllPath = @"C:\Users\D21044623\source\repos\Ike.Lib\Ike.Standard\bin\Debug\netstandard2.0\Ike.Standard.dll";
                //var ru = new Ike.Standard.Reflection.ReflectionUtility();
                //ru.LoadAssemblyFromMemory(dllPath);
                //string typeName = "Ike.Standard.MyClass";
                //Type type = ru.GetTypeFromAssembly(typeName);
                //object[] attrs = ru.GetCustomAttributes(typeName);
                //object instance = ru.CreateInstance(typeName);



                //int a = 0;


                //// 加载程序集
                //var assemblyPath = dllPath;
                //var assembly = Mono.Cecil.AssemblyDefinition.ReadAssembly(assemblyPath);

                //// 找到目标类型和方法
                //var type = assembly.MainModule.GetType("Ike.Standard.MyClass");
                //var method = type.Methods.First(m => m.Name == "MyMethod");

                //// 创建IL处理器
                //var ilProcessor = method.Body.GetILProcessor();
                //var instructions = method.Body.Instructions.ToList(); // 复制现有指令

                //// 找到插入点 - 在第一个Console.WriteLine之后
                //var insertAfter = instructions.FirstOrDefault(i =>
                //    i.OpCode == OpCodes.Call &&
                //    i.Operand is MethodReference mr &&
                //    mr.Name == "WriteLine" &&
                //    mr.DeclaringType.FullName == "Ike.Standard.Console");

                //if (insertAfter != null)
                //{
                //    // 创建要插入的新指令
                //    var newInstructions = new List<Instruction>
                //    {
                //        // 加载参数值 (param)
                //        ilProcessor.Create(OpCodes.Ldarg_1), // 加载第一个参数

                //        // 加载要追加的字符串
                //        ilProcessor.Create(OpCodes.Ldstr, "->OK")
                //    };

                //    // 调用string.Concat
                //    var concatMethod = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
                //    newInstructions.Add(ilProcessor.Create(
                //        OpCodes.Call,
                //        assembly.MainModule.ImportReference(concatMethod)));

                //    // 将结果存回参数变量
                //    newInstructions.Add(ilProcessor.Create(OpCodes.Starg_S, method.Parameters[0]));

                //    // 在目标指令后插入新指令
                //    var insertIndex = instructions.IndexOf(insertAfter) + 1;
                //    foreach (var instr in newInstructions)
                //    {
                //        ilProcessor.InsertAfter(insertAfter, instr);
                //        insertAfter = instr; // 更新插入点
                //    }
                //}

                //// 保存修改后的程序集
                //assembly.Write("ModifiedAssembly.dll");

                //Type attributeType = ru.GetTypeFromAssembly("Ike.Standard.MyCustomAttribute");



                //var types = helper.GetMethodsInType("Ike.Standard.LOG", helper.AllTypeBindingFlags);
                //// 获取LogType枚举类型
                //Type logTypeEnum = helper.GetTypeFromAssembly("Ike.Standard.LOG+LogType");
                //// 获取Info枚举值
                //object infoEnumValue = Enum.Parse(logTypeEnum, "Info");
                //// 获取LogType枚举类型
                //var colorInstance = helper.CreateInstance("Ike.Standard.LOG+LogColors");
                //// 构造方法参数
                //object[] constructorArgs = new object[] { @"F:\Desktop\Logs", 5000, "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", true, colorInstance };
                //// 创建方法参数结构体(Ike.Standard.LOG+LogType)
                //object[] methodParams = new object[] { "Test to create class instances and execute the specified methods.", infoEnumValue, true, true, false };
                //// 类名称
                //string name = "Ike.Standard.LOG";
                //// 创建类实例并执行指定方法
                //helper.InvokeInstanceMethod(name, "Log", constructorArgs, default, methodParams);



                //MethodInfo method = helper.GetMethods(name, "Log", methodParams, helper.AllTypeBindingFlags);

                //var meinfo = helper.GetMethodDetails(method);
                //// 获取指定结构
                //Type type = helper.GetTypeFromAssembly("Ike.Standard.LOG+LogStruct");

                //Ike.Standard.Reflection.TypeDetails temp1 = helper.GetTypeDetails(type);

                //var temp2 = helper.GetPropertyDetails(type).ToList();


                //var temp3 = helper.GetAllEventsInTypeHierarchy(name);

                //var instance = helper.CreateInstance(name, constructorArgs);

                //Type targetType = instance.GetType();
                //string eventName = "LogEvent";
                //// 使用 dynamic 类型处理
                //EventInfo eventInfo = helper.GetInstanceEvent(instance, eventName);

                //// 创建处理方法，使用 dynamic 接收参数
                //var handler = (Action<dynamic>)((log) =>
                //{
                //    // 通过动态访问属性
                //    Console.WriteLine($"LogEvent: {log.Info}");
                //});
                //// 创建委托
                //Delegate delegateObj = helper.CreateDelegate(eventInfo,handler);
                //// 订阅事件
                //helper.EventOperation(instance, eventInfo, delegateObj, true);

                //methodParams = new object[] { "This is a test log", infoEnumValue, true, true, true };

                //MethodInfo getMethod = helper.GetMethodFromInstance(instance, "Log", methodParams, default);
                ////System.Diagnostics.Debugger.Launch();
                //object re = getMethod.Invoke(instance, methodParams);
                //helper.EventOperation(instance, eventInfo, delegateObj, false);
                //re = getMethod.Invoke(instance, methodParams);
            }
            catch (Exception ex)
            {
                Ike.Standard.Console.WriteLine(ex.ToString(), "#ff6024");
            }
            Console.ReadLine();
        }




        //}

        // 比较文本中不同
        //class Program
        //{
        //    private static Dictionary<string, string> fileHashes = new Dictionary<string, string>();
        //    private static FileSystemWatcher watcher;
        //    private static readonly InlineDiffBuilder diffBuilder = new InlineDiffBuilder(new Differ());

        //    static void Main(string[] args)
        //    {
        //        Console.InputEncoding = Encoding.UTF8;
        //        Console.OutputEncoding = Encoding.UTF8;

        //        Console.WriteLine("精确文件修改监控程序");
        //        Console.WriteLine("---------------------");

        //        // 设置要监控的目录路径
        //        string directoryPath = @"F:\Desktop\Test"; // 替换为实际路径

        //        if (!Directory.Exists(directoryPath))
        //        {
        //            Console.WriteLine($"目录不存在: {directoryPath}");
        //            return;
        //        }

        //        InitializeFileHashes(directoryPath, "*.txt");

        //        watcher = new FileSystemWatcher
        //        {
        //            Path = directoryPath,
        //            Filter = "*.txt",
        //            IncludeSubdirectories = true,
        //            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        //        };

        //        watcher.Changed += OnFileChanged;
        //        watcher.Created += OnFileChanged;
        //        watcher.Deleted += OnFileDeleted;
        //        watcher.Renamed += OnFileRenamed;

        //        watcher.EnableRaisingEvents = true;

        //        Console.WriteLine($"正在精确监控目录: {directoryPath}");
        //        Console.ReadLine();

        //        watcher.EnableRaisingEvents = false;
        //        watcher.Dispose();
        //    }

        //    private static void InitializeFileHashes(string directoryPath, string filter)
        //    {
        //        foreach (var filePath in Directory.GetFiles(directoryPath, filter, SearchOption.AllDirectories))
        //        {
        //            fileHashes[filePath] = CalculateFileHash(filePath);
        //        }
        //    }

        //    private static void OnFileChanged(object sender, FileSystemEventArgs e)
        //    {
        //        try
        //        {
        //            // 暂停监控防止重复触发
        //            watcher.EnableRaisingEvents = false;

        //            // 增加等待时间确保文件可访问
        //            System.Threading.Thread.Sleep(500);

        //            if (File.Exists(e.FullPath))
        //            {
        //                string currentContent = File.ReadAllText(e.FullPath);
        //                string newHash = CalculateFileHash(e.FullPath);

        //                // 如果文件已记录在字典中
        //                if (fileHashes.TryGetValue(e.FullPath, out var oldRecord))
        //                {
        //                    // oldRecord现在直接存储旧内容而不仅是哈希
        //                    if (newHash != CalculateFileHashFromContent(oldRecord))
        //                    {
        //                        Console.WriteLine($"\n文件已修改: {e.FullPath}");
        //                        Console.WriteLine($"修改时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        //                        var diffModel = diffBuilder.BuildDiffModel(oldRecord, currentContent);

        //                        if (diffModel.Lines.Any(x => x.Type != ChangeType.Unchanged))
        //                        {
        //                            Console.WriteLine("精确修改内容:");
        //                            PrintPreciseDifferences(diffModel);
        //                        }
        //                        else
        //                        {
        //                            Console.WriteLine("(文件哈希改变但内容差异未检测到)");
        //                        }

        //                        Console.WriteLine("---------------------");
        //                    }
        //                }
        //                else
        //                {
        //                    Console.WriteLine($"\n新文件已创建: {e.FullPath}");
        //                    Console.WriteLine("---------------------");
        //                }

        //                // 更新记录为当前内容
        //                fileHashes[e.FullPath] = currentContent;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"处理文件更改时出错: {ex.Message}");
        //        }
        //        finally
        //        {
        //            watcher.EnableRaisingEvents = true;
        //        }
        //    }

        //    private static string CalculateFileHashFromContent(string content)
        //    {
        //        using (var md5 = MD5.Create())
        //        {
        //            byte[] contentBytes = Encoding.UTF8.GetBytes(content);
        //            byte[] hashBytes = md5.ComputeHash(contentBytes);
        //            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        //        }
        //    }

        //    private static void PrintPreciseDifferences(DiffPaneModel diffModel)
        //    {
        //        foreach (var line in diffModel.Lines)
        //        {
        //            switch (line.Type)
        //            {
        //                case ChangeType.Inserted:
        //                    Console.ForegroundColor = ConsoleColor.Green;
        //                    Console.Write("[+] ");
        //                    Console.WriteLine(line.Text);
        //                    break;
        //                case ChangeType.Deleted:
        //                    Console.ForegroundColor = ConsoleColor.Red;
        //                    Console.Write("[-] ");
        //                    Console.WriteLine(line.Text);
        //                    break;
        //                case ChangeType.Modified:
        //                    Console.ForegroundColor = ConsoleColor.Yellow;
        //                    Console.Write("[~] ");
        //                    Console.WriteLine(line.Text);
        //                    break;
        //                case ChangeType.Unchanged:
        //                    // 不显示未更改的内容以保持简洁
        //                    // Console.ForegroundColor = ConsoleColor.Gray;
        //                    // Console.Write("[ ] ");
        //                    // Console.WriteLine(line.Text);
        //                    break;
        //            }
        //            Console.ResetColor();
        //        }
        //    }

        //    private static void OnFileDeleted(object sender, FileSystemEventArgs e)
        //    {
        //        Console.WriteLine($"\n文件已删除: {e.FullPath}");
        //        Console.WriteLine("---------------------");

        //        fileHashes.Remove(e.FullPath);
        //    }

        //    private static void OnFileRenamed(object sender, RenamedEventArgs e)
        //    {
        //        Console.WriteLine($"\n文件已重命名: 从 {e.OldFullPath} 到 {e.FullPath}");
        //        Console.WriteLine("---------------------");

        //        if (fileHashes.TryGetValue(e.OldFullPath, out var hash))
        //        {
        //            fileHashes.Remove(e.OldFullPath);
        //            fileHashes[e.FullPath] = hash;
        //        }
        //    }

        //    private static string CalculateFileHash(string filePath)
        //    {
        //        try
        //        {
        //            using (var md5 = MD5.Create())
        //            using (var stream = File.OpenRead(filePath))
        //            {
        //                byte[] hashBytes = md5.ComputeHash(stream);
        //                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        //            }
        //        }
        //        catch
        //        {
        //            return string.Empty;
        //        }
        //    }
        //}


    }
}
