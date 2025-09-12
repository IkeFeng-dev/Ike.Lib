using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Iced.Intel;
using Ike.Standard.Ini;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpCodes = Mono.Cecil.Cil.OpCodes;


namespace Test.Framework
{
    /// <summary>
    /// 入库请求
    /// </summary>
    public class RuninRequest
    {
        /// <summary>
        /// 放料次数
        /// </summary>
        public SocketMessageTimes Times { get; set; }

        /// <summary>
        /// <inheritdoc cref="SocketMessageActionFlag"/>
        /// </summary>
        public SocketMessageActionFlag Flag { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// <inheritdoc cref="SocketMessageDestination "/>
        /// </summary>
        public SocketMessageDestination Destination { get; set; }

        /// <summary>
        /// 入库数据
        /// </summary>
        public Value Values { get; set; }
    }

    /// <summary>
    /// 出库响应
    /// </summary>
    public class RuninResponse
    {
        /// <summary>
        /// <inheritdoc cref="SocketMessageActionFlag"/>
        /// </summary>
        public SocketMessageActionFlag Flag { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// <inheritdoc cref="SocketMessageDestination "/>
        /// </summary>
        public SocketMessageDestination Destination { get; set; }

        /// <summary>
        /// 出库数据
        /// </summary>
        public List<Value> Values { get; set; }
    }


    public class Value
    {

        /// <summary>
        /// 机台编码
        /// </summary>
        public string MachineCode { get; set; }

        /// <summary>
        /// 载具编码
        /// </summary>
        public string CarrierCode { get; set; }

        /// <summary>
        /// 货架编码
        /// </summary>
        public string ShelfCode { get; set; }

        /// <summary>
        /// 包装线别
        /// </summary>
        public string Line { get; set; }
    }

    /// <summary>
    /// 发送次数
    /// </summary>
    public enum SocketMessageTimes
    {
        /// <summary>
        /// 第一次
        /// </summary>
        First = 1,
        /// <summary>
        /// 第二次
        /// </summary>
        Second = 2,
        /// <summary>
        /// 第三次
        /// </summary>
        Third = 3
    }

    /// <summary>
    /// Socket数据结构消息动作标志
    /// </summary>
    public enum SocketMessageActionFlag
    {
        /// <summary>
        /// 入料
        /// </summary>
        Input = 0,
        /// <summary>
        /// 出料
        /// </summary>
        Output = 1,
        /// <summary>
        /// 完成
        /// </summary>
        Complete = 2,
        /// <summary>
        /// NG
        /// </summary>
        NG = 3,
    }

    /// <summary>
    /// Socket数据结构目的地描述
    /// </summary>
    public enum SocketMessageDestination
    {
        /// <summary>
        /// 包装
        /// </summary>
        Packing = 0,
        /// <summary>
        /// 回流
        /// </summary>
        Reflow = 1,
        /// <summary>
        /// NG
        /// </summary>
        NG = 2,
        /// <summary>
        /// 无
        /// </summary>
        NA = 3
    }


    internal class Program
    {

        /// <summary>
        /// 重定向架位编号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static string ConvertJWNumber(string input)
        {
            // 使用正则表达式提取数字部分
            Match match = Regex.Match(input, @"JW-(\d+)-(\d+)");
            if (!match.Success)
                throw new ArgumentException("无效的编号格式");
            int lastNumber = int.Parse(match.Groups[2].Value);
            int column = lastNumber % 3;
            if (column == 0) column = 3;
            int row = (lastNumber - 1) / 3 + 1;
            // 构造新编号
            string newMiddle = column.ToString("D4");
            string newLast = row.ToString("D2");
            return $"JW-{newMiddle}-{newLast}";
        }


        private static void Handle(string info, Ike.Standard.Sockets.MessageType messageType)
        {
            info = DateTime.Now.ToString("HH:mm:ss.fff") + ": " + info;
            switch (messageType)
            {
                case Ike.Standard.Sockets.MessageType.Error:
                    Ike.Standard.Console.WriteLine(info, "#e94242");
                    break;
                case Ike.Standard.Sockets.MessageType.Receive:
                    Ike.Standard.Console.WriteLine(info, "#a5cd71");
                    break;
                case Ike.Standard.Sockets.MessageType.Info:
                    Ike.Standard.Console.WriteLine(info, "#006cc2");
                    break;
            }
        }


        /// <summary>
        /// 处理接收的信息并且回复
        /// </summary>
        /// <param name="info">接收信息</param>
        /// <returns></returns>
        private static Task<string> ReplyHandle(string info)
        {
            var data = JsonConvert.DeserializeObject<RuninRequest>(info);
            // 入库
            if (data.Flag == SocketMessageActionFlag.Input)
            {
                string jw = ConvertJWNumber(data.Values.ShelfCode);
                string query = $"[Query]{jw}";
                string queryResult = Ike.Standard.Sockets.SendAndReceive("127.0.0.1", 16800, query, 2500);
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(queryResult);
                bool isOK = dic[jw].Substring(0, 1).Equals("T");
                data.Flag = isOK ? SocketMessageActionFlag.Complete : SocketMessageActionFlag.NG;
            }
            return Task.FromResult(JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings { Converters = new List<JsonConverter> { new StringEnumConverter() } }));
        }



        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            Ike.Standard.WinMethod.EnableAnsiSupport();
            try
            {


                //Ike.Standard.Ini.IniConfigManager<IniData> iniConfig = new Ike.Standard.Ini.IniConfigManager<IniData>(@"F:\Desktop\1.txt");





                //iniConfig.Load();



                //return;


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
                


                Ike.Standard.Sockets.SocketListen socketListen = new Ike.Standard.Sockets.SocketListen(16700,50, Handle, ReplyHandle);
                Task.Run(socketListen.Start);



                Console.ReadLine();



                //var client = new Ike.Standard.DataInteraction.NetworkSocket("127.0.0.1", 44444, 55555, 1,null);
                //client.LogEvent += (s) =>
                //{
                //    Console.WriteLine("LogEvent => " + s);
                //};

                //client.ReceiveEvent += (s) =>
                //{
                //    Console.WriteLine("ReceiveEvent => " + s);
                //};

                //Console.ReadLine();

                //bool r = client.WaitReceiveMessage(out string result,6000);

                //Console.WriteLine(r + "  :" + result);

                //client.Dispose();


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
