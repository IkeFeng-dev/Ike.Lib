using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Ike.Standard
{
	/// <summary>
	/// 进程相关
	/// </summary>
	public class Progress
	{
		/// <summary>
		/// 打开可执行程序,不等待程序结束,无法获取程序返回值
		/// </summary>
		/// <param name="processPath">可执行程序路径</param>
		/// <param name="argument">传递参数,没有则为""或<see cref="string.Empty"/> </param>
		/// <exception cref="FileNotFoundException"></exception>
		public static void OpenProcess(string processPath, string argument)
		{
			if (!File.Exists(processPath))
			{
				throw new FileNotFoundException(processPath);
			}
			using (Process process = new Process())
			{
				process.StartInfo = new ProcessStartInfo()
				{
					FileName = processPath,
					Arguments = argument,
					WorkingDirectory = Path.GetDirectoryName(processPath),
					UseShellExecute = true,
				};
				process.Start();
			}
		}

		/// <summary>
		/// CMD命令执行类型
		/// </summary>
		public enum RunCmdType
		{
			/// <summary>
			/// 启用新窗口独立运行这个命令,返回<see cref="string.Empty"/>
			/// </summary>
			IndependentOperation,
			/// <summary>
			/// 启用新窗口独立运行这个命令,显示这个窗口,按任意键关闭它,返回<see cref="string.Empty"/>
			/// </summary>
			ShowWindow,
			/// <summary>
			/// 获取输出文本
			/// </summary>
			GetOutputText,
			/// <summary>
			/// 结果直接输出到调用程序控制台中,返回<see cref="string.Empty"/>
			/// </summary>
			Association,
			/// <summary>
			/// 后台执行,不输出结果,不显示窗体,返回<see cref="string.Empty"/>
			/// </summary>
			BackgroundExecution,
		}

		/// <summary>
		/// 运行CMD命令
		/// </summary>
		/// <param name="command">命令</param>
		/// <param name="cmdType">执行类型</param>
		/// <param name="waitForExit">是否等待进程结束,默认值是<see langword="false"/></param>
		/// <param name="workingDirectory">设置工作目录,默认是文件所在的目录</param>
		/// <returns>运行指令后输出的内容</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static string RunCmd(string command, RunCmdType cmdType, bool waitForExit = false, string workingDirectory = "")
		{
			if (string.IsNullOrWhiteSpace(command))
			{
				throw new ArgumentNullException(nameof(command));
			}
			string result = string.Empty;
			bool isRead = false;
			ProcessStartInfo info = new ProcessStartInfo()
			{
				FileName = "cmd.exe",
				Arguments = "/c " + command
			};
			if (!string.IsNullOrWhiteSpace(workingDirectory))
			{
				info.WorkingDirectory = workingDirectory;
			}
			switch (cmdType)
			{
				case RunCmdType.IndependentOperation:
					info.UseShellExecute = true;
					break;
				case RunCmdType.ShowWindow:
					info.UseShellExecute = true;
					info.Arguments = "/c " + command + " & pause>nul";
					break;
				case RunCmdType.Association:
					info.UseShellExecute = false;
					break;
				case RunCmdType.BackgroundExecution:
					info.UseShellExecute = false;
					info.CreateNoWindow = true;
					break;
				case RunCmdType.GetOutputText:
					info.UseShellExecute = false;
					info.RedirectStandardOutput = true;
					info.StandardOutputEncoding = Encoding.UTF8;
					waitForExit = true;
					isRead = true;
					break;
			}
			using (Process process = new Process())
			{
				process.StartInfo = info;
				process.Start();
				if (isRead)
				{
					result = process.StandardOutput.ReadToEnd();
				}
				if (waitForExit)
				{
					process.WaitForExit();
				}
			}
			return result;
		}

		/// <summary>
		/// 运行bat文件,将标准输出流输出到 <paramref name="outputHandler"/>委托对象
		/// </summary>
		/// <param name="batFilePath">bat文件路径</param>
		/// <param name="workingDirectory">工作目录</param>
		/// <param name="outputHandler">输出到委托对象</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		public static void RunBat(string batFilePath, string workingDirectory, Action<string> outputHandler)
		{
			if (string.IsNullOrWhiteSpace(batFilePath))
			{
				throw new ArgumentNullException(nameof(batFilePath));
			}
			if (!File.Exists(batFilePath))
			{
				throw new FileNotFoundException(batFilePath);
			}
			if (outputHandler is null)
			{ 
			    throw new ArgumentNullException(nameof(outputHandler), "回调函数不能为null");
			}
			using (Process process = new Process())
			{
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = batFilePath,
					UseShellExecute = false,
					RedirectStandardOutput = true
				};
				if (!string.IsNullOrWhiteSpace(workingDirectory))
				{
					process.StartInfo.WorkingDirectory = workingDirectory;
				}
				process.StartInfo = startInfo;
				process.OutputDataReceived += (sender, e) =>
				{
					if (e.Data != null)
					{
						outputHandler.Invoke(e.Data);
					}
				};
				process.Start();
				process.BeginOutputReadLine();
				process.WaitForExit();
			}
		}
	}
}
