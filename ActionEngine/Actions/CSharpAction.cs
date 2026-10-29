using Ike.ActionEngine.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Ike.ActionEngine.Actions
{
    public class CSharpAction : UpdateAction
    {
        /// <summary>
        /// C# 代码内容
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// C# 代码文件 Url 路径
        /// </summary>
        public string ScriptUrl { get; set; }

        /// <summary>
        /// C# 代码文件路径
        /// </summary>
        public string ScriptFile { get; set; }



        public override void Execute(UpdateContext context)
        {
            string actionName = GlobalVariable.WriteActionLog(GetType());
            GlobalVariable.ActionName = actionName;
            // 1. 获取 C# 代码 (下载或读取本地文件)
            string code = null;

            // 优先级 1：从 ScriptUrl 获取远程脚本
            if (!string.IsNullOrWhiteSpace(ScriptUrl))
            {
                string realUrl = context.ResolvePath(ScriptUrl);
                GlobalVariable.Log.Debug($"{actionName} -> 正在从 URL 下载脚本: {realUrl}");

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    code = httpClient.GetStringAsync(realUrl).GetAwaiter().GetResult();
                }
            }
            // 优先级 2：从 ScriptFile 读取本地脚本
            else if (!string.IsNullOrWhiteSpace(ScriptFile))
            {
                string realScriptPath = context.ResolvePath(ScriptFile);
                GlobalVariable.Log.Debug($"{actionName} -> 正在读取本地脚本文件: {realScriptPath}");

                if (!File.Exists(realScriptPath))
                {
                    throw new FileNotFoundException($"找不到指定的脚本文件: {realScriptPath}");
                }

                code = File.ReadAllText(realScriptPath);
            }
            // 优先级 3：使用内嵌 Script 内容
            else
            {
                code = Script;
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                GlobalVariable.Log.Warning($"{actionName} -> 脚本内容为空，跳过执行");
                return;
            }

            // 保留代码到文件
            Directory.CreateDirectory(GlobalVariable.CurrentDataDir);
            if (File.Exists(GlobalVariable.ActionCSharpPath))
            {
                File.AppendAllText(GlobalVariable.ActionCSharpPath, string.Format("{0}{1}{0}", Standard.Text.CopyString(Environment.NewLine, 10), Standard.Text.CopyString("=", 30)));
            }
            File.AppendAllText(GlobalVariable.ActionCSharpPath, code);

            // 2. 调用 CompileCode 方法进行内存编译
            Assembly asm = CompileCode(code, "DynamicAction_" + Guid.NewGuid().ToString("N"), out Exception ex);
            if (asm == null)
            {
                GlobalVariable.Log.Error($"{actionName} -> C# 动态编译失败: {ex?.Message}");
                throw ex;
            }

            // 3. 寻找继承自 UpdateAction 的类并实例化执行
            Type actionType = asm.GetTypes().FirstOrDefault(t => typeof(UpdateAction).IsAssignableFrom(t) && !t.IsAbstract)
                ?? throw new InvalidOperationException("编译成功，但在程序集中未找到继承自 UpdateAction 的实现类");

            // 4. 创建对象实例并运行 Execute 方法,必须按照特定的格式去写代码,否则无法找到
            UpdateAction instance = (UpdateAction)Activator.CreateInstance(actionType);
            instance.Execute(context);
            GlobalVariable.Log.Success($"{actionName} -> OK");
        }


        /// <summary>
        /// 编译执行 C# 代码
        /// </summary>
        internal static Assembly CompileCode(string code, string assemblyName, out Exception exception)
        {
            try
            {
                exception = null;
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code);

                var references = new List<MetadataReference>();
                var loadedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                // 1. 收集磁盘上已有的常规程序集引用
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location) && File.Exists(assembly.Location))
                        {
                            if (loadedPaths.Add(assembly.Location))
                            {
                                references.Add(MetadataReference.CreateFromFile(assembly.Location));
                            }
                        }
                    }
                    catch { }
                }

                // 2. 自动扫描主程序集 (EXE) 内部 Costura.Fody 打包的所有内嵌 DLL 资源并解压加载
                var mainAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                if (mainAssembly != null)
                {
                    foreach (string resName in mainAssembly.GetManifestResourceNames())
                    {
                        string lowerRes = resName.ToLowerInvariant();
                        // Costura 固定的内嵌资源名称前缀格式：costura.xxx.dll.compressed 或 costura.xxx.dll
                        if (lowerRes.Contains("costura.") && (lowerRes.EndsWith(".dll.compressed") || lowerRes.EndsWith(".dll")))
                        {
                            try
                            {
                                using (Stream stream = mainAssembly.GetManifestResourceStream(resName))
                                {
                                    if (stream == null) continue;

                                    var memStream = new MemoryStream();

                                    // 若带 .compressed 后缀，使用 DeflateStream 进行内存解压缩
                                    if (lowerRes.EndsWith(".compressed"))
                                    {
                                        using (var decompressStream = new DeflateStream(stream, CompressionMode.Decompress))
                                        {
                                            decompressStream.CopyTo(memStream);
                                        }
                                    }
                                    else
                                    {
                                        stream.CopyTo(memStream);
                                    }

                                    memStream.Position = 0;
                                    references.Add(MetadataReference.CreateFromStream(memStream));
                                }
                            }
                            catch(Exception ex)
                            {
                                // 忽略损坏的个别资源
                                GlobalVariable.Log.Exception(ex);
                            }
                        }
                    }
                }

                // 定义编译选项
                CSharpCompilationOptions compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

                // 创建编译对象并添加引用与语法树
                CSharpCompilation compilation = CSharpCompilation.Create(assemblyName)
                    .WithOptions(compilationOptions)
                    .AddReferences(references)
                    .AddSyntaxTrees(syntaxTree);

                // 执行内存编译
                using (MemoryStream ms = new MemoryStream())
                {
                    EmitResult result = compilation.Emit(ms);
                    if (!result.Success)
                    {
                        // 编译失败，拼接具体诊断错误信息
                        string errorMessage = string.Join(Environment.NewLine, result.Diagnostics.Select(diagnostic => diagnostic.GetMessage()));
                        throw new InvalidOperationException($"Unable to compile code: {Environment.NewLine}{errorMessage}");
                    }

                    // 加载已编译的内存程序集
                    ms.Seek(0, SeekOrigin.Begin);
                    return Assembly.Load(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                exception = ex;
                GlobalVariable.Log.Exception(ex);
                return null;
            }
        }

    }
}
