#if NETFRAMEWORK
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Ike.Standard
{
	/// <summary>
	/// 进程相关
	/// </summary>
	public partial class Process
	{
		/// <summary>
		/// 获取指定进程运行数
		/// </summary>
		/// <param name="processName">进程名称</param>
		/// <returns></returns>
		public static System.Diagnostics.Process[] GetProcesses(string processName)
		{
			if (processName.ToLower().EndsWith(".exe"))
			{
				processName = processName.Remove(processName.Length - 4, 4);
			}
			List<System.Diagnostics.Process> process = new List<System.Diagnostics.Process>();
			foreach (var item in System.Diagnostics.Process.GetProcesses())
			{
				if (item.ProcessName == processName)
				{
					process.Add(item);
				}
			}
			return process.ToArray();
		}


		/// <summary>
		/// 结束所有指定名称的程序进程
		/// </summary>
		/// <param name="processName">进程名称</param>
		public static bool KillProcess(string processName)
		{
			System.Diagnostics.Process[] myproc = GetProcesses(processName);

			foreach (System.Diagnostics.Process item in myproc)
			{
				if (item.ProcessName == processName)
				{
					item.Kill();
				}
			}
			return true;
		}



		/// <summary>
		/// 获取指定进程运行状态
		/// </summary>
		/// <param name="processName">进程名称</param>
		/// <returns>返回进程知否在运行中</returns>
		public static bool GetProcessState(string processName)
		{
			if (GetProcessNumber(processName) > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}


		/// <summary>
		/// 获取指定进程运行个数
		/// </summary>
		/// <param name="processName">进程名称</param>
		/// <returns></returns>
		public static int GetProcessNumber(string processName)
		{
			return GetProcesses(processName).Length;
		}



		/// <summary>
		/// 打开可执行程序,不等待程序结束
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
			using (System.Diagnostics.Process process = new System.Diagnostics.Process())
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
			using (System.Diagnostics.Process process = new System.Diagnostics.Process())
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
			using (System.Diagnostics.Process process = new System.Diagnostics.Process())
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


#endif