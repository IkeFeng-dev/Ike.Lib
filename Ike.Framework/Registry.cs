using Microsoft.Win32;
using System;
using System.IO;

namespace Ike.Framework
{
	/// <summary>
	/// 注册表操作类
	/// </summary>
	public class Regedit
	{
		/// <summary>
		/// 获取注册表路径的根键
		/// </summary>
		/// <param name="fullPath">注册表完整路径</param>
		/// <param name="subKeyPath">返回注册表根键下的子项路径</param>
		/// <returns></returns>
		public static RegistryKey GetRootKey(string fullPath,out string subKeyPath)
		{
			if (fullPath.Contains("/"))
			{
				fullPath = fullPath.Replace("/", "\\");
			}
			if (fullPath.StartsWith("\\"))
			{
				fullPath = fullPath.Substring(1);
			}
			int firstBackslash = fullPath.IndexOf('\\');
			if (firstBackslash == -1)
			{
				throw new ArgumentException("路径格式不正确", nameof(fullPath));
			}
			string rootKeyName = fullPath.Substring(0, firstBackslash);
			 subKeyPath = fullPath.Substring(firstBackslash + 1);
			RegistryKey rootKey;
			switch (rootKeyName.ToUpper())
			{
				case "HKEY_CLASSES_ROOT":
					rootKey = Registry.ClassesRoot;
					break;
				case "HKEY_CURRENT_USER":
					rootKey = Registry.CurrentUser;
					break;
				case "HKEY_LOCAL_MACHINE":
					rootKey = Registry.LocalMachine;
					break;
				case "HKEY_USERS":
					rootKey = Registry.Users;
					break;
				case "HKEY_CURRENT_CONFIG":
					rootKey = Registry.CurrentConfig;
					break;
				default:
					throw new ArgumentException("路径根键格式不正确", nameof(fullPath));
			}
			return rootKey;
		}


		/// <summary>
		/// 输入注册表完整路径,打开注册表子项
		/// </summary>
		/// <param name="fullPath"><inheritdoc cref="GetRootKey" path="/param[@name='fullPath']"/></param>
		/// <returns>指定路径下的的注册表对象,如果操作失败,则为<see langword="null"></see></returns>
		/// <exception cref="ArgumentException">参数异常</exception>
		public static RegistryKey OpenRegistryKey(string fullPath)
		{
			RegistryKey rootKey = GetRootKey(fullPath,out string subKeyPath);
			return rootKey.OpenSubKey(subKeyPath);
		}


		/// <summary>
		/// 指定注册表项是否存在
		/// </summary>
		/// <param name="fullPath"><inheritdoc cref="GetRootKey" path="/param[@name='fullPath']"/></param>
		/// <returns></returns>
		public static bool Exist(string fullPath)
		{
			RegistryKey registryKey = OpenRegistryKey(fullPath);
			bool result = registryKey != null;
			if (result)
			{
				registryKey.Close();
			}
			return result;
		}


		/// <summary>
		/// 创建注册表项
		/// </summary>
		/// <param name="fullPath">注册表完整路径</param>
		/// <returns></returns>
		public static RegistryKey CreateRegistryKey(string fullPath)
		{
			RegistryKey rootKey = GetRootKey(fullPath,out string subKeyPath);
			return rootKey.CreateSubKey(subKeyPath); 
		}


		/// <summary>
		/// 获取指定项的所有子项名字
		/// </summary>
		/// <param name="fullPath"><inheritdoc cref="GetRootKey" path="/param[@name='fullPath']"/></param>
		/// <returns>获取到的子项的字符串集合</returns>
		public static string[] GetSubitemNames(string fullPath)
		{
			using (RegistryKey registryKey = OpenRegistryKey(fullPath))
			{
				return registryKey.GetSubKeyNames();
			}
		}

		/// <summary>
		/// 获取指定项的所有值名称
		/// </summary>
		/// <param name="fullPath"><inheritdoc cref="GetRootKey" path="/param[@name='fullPath']"/></param>
		/// <returns>获取到的值名称的字符串集合</returns>
		public static string[] GetGetValueNames(string fullPath)
		{
			using (RegistryKey registryKey = OpenRegistryKey(fullPath))
			{
				return registryKey.GetValueNames();
			}
		}


		/// <summary>
		/// 获取注册表指定值
		/// </summary>
		/// <param name="fullPath"><inheritdoc cref="GetRootKey" path="/param[@name='fullPath']"/></param>
		/// <param name="name">值名称</param>
		/// <param name="defaultValue">如果未找到指定内容,则返回这个默认值</param>
		/// <returns></returns>
		public static object GetValue(string fullPath, string name, string defaultValue)
		{
			using (RegistryKey key = OpenRegistryKey(fullPath))
			{
				return key.GetValue(name, defaultValue);
			}
		}



		/// <summary>
		/// 写入值到对应的注册表项中
		/// </summary>
		/// <param name="fullPath"><inheritdoc cref="GetRootKey" path="/param[@name='fullPath']"/></param>
		/// <param name="name">键名称</param>
		/// <param name="value">写入值</param>
		/// <param name="valueKind">写入的数据类型</param>
		public static void SetValue(string fullPath,string name,object value, RegistryValueKind valueKind)
		{
			using (RegistryKey rootKey = GetRootKey(fullPath, out string subKey))
			{
				using (RegistryKey subK = rootKey.CreateSubKey(subKey))
				{
					subK.SetValue(name,value, valueKind);
				}
			}
		}


		/// <summary>
		/// 创建文件右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		/// <param name="iconPath">图标路径,ico或可执行程序路径</param>
		/// <param name="command">右键打开文件菜单时打开的程序文件路径,可执行文件需要完整路径</param>
		public static void CreateFileRightMenu(string name, string iconPath, string command)
		{
			if (iconPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
			{
				iconPath = "\"" + iconPath + "\",0";
			}
			if (command.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(command))
			{
				command = "\"" + command + "\"" + " %L";
			}
			using (RegistryKey registryCreate = OpenRegistryKey(Paths.FileRightKeyShell))
			{
				registryCreate.CreateSubKey(name, true);
				registryCreate.Close();
				using (RegistryKey registry = OpenRegistryKey(Paths.FileRightKeyShell + "\\" + name))
				{
					registry.SetValue("icon", iconPath);
					registry.CreateSubKey("command", true).SetValue("", command);
					registry.Close();
				}
			}
		}

		/// <summary>
		/// 删除文件右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		public static void DeleteFileRightMenu(string name)
		{
			using (RegistryKey registry = OpenRegistryKey(Paths.FileRightKeyShell + "\\" + name))
			{
				registry.DeleteSubKey("command", false);
				registry.Close();
				using (RegistryKey key = OpenRegistryKey(Paths.FileRightKeyShell))
				{
					key.DeleteSubKey(name, false);
					key.Close();
				}
			}
		}

		/// <summary>
		/// 创建空白处右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		/// <param name="iconPath">图标路径,ico或可执行程序路径</param>
		/// <param name="command">启动菜单时打开的程序传递右键时目录路径,可执行文件需要完整路径</param>
		public static void CreateEmptyRightMenu(string name, string iconPath, string command)
		{
			if (iconPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
			{
				iconPath = "\"" + iconPath + "\",0";
			}
			if (command.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) && File.Exists(command))
			{
				command = "\"" + command + "\"" + " -path " + "%V";
			}
			using (RegistryKey registry = OpenRegistryKey(Paths.EmptyRightKeyShell))
			{
				registry.CreateSubKey(name, true);
				registry.Close();
				using (RegistryKey key = OpenRegistryKey(Paths.EmptyRightKeyShell + "\\" + name))
				{
					registry.SetValue("icon", iconPath);
					registry.CreateSubKey("command", true).SetValue("", command);
					registry.Close();
				}
			}
		}



		/// <summary>
		/// 删除空白处右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		public static void DeleteEmptyRightMenu(string name)
		{
			using (RegistryKey registry = OpenRegistryKey(Paths.EmptyRightKeyShell + "\\" + name))
			{
				if (registry != null)
				{
					registry.DeleteSubKey("command", false);
					registry.Close();
					using (RegistryKey key = OpenRegistryKey(Paths.EmptyRightKeyShell))
					{
						registry.DeleteSubKey(name, false);
						registry.Close();
					}
				}
			}
		}


		/// <summary>
		/// 创建文件夹右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		/// <param name="iconPath">图标路径,ico或可执行程序路径</param>
		/// <param name="command">启动菜单时打开的程序传递右键时目录路径,可执行文件需要完整路径</param>
		public static void CreateDirectoryRightMenu(string name, string iconPath, string command)
		{
			if (iconPath.ToLower().EndsWith(".exe")) iconPath = "\"" + iconPath + "\",0";
			if (command.ToLower().EndsWith(".exe") && File.Exists(command)) command = "\"" + command + "\"" + " %L";
			using (RegistryKey registry = OpenRegistryKey(Paths.DirectoryRightKeyShell))
			{
				registry.CreateSubKey(name, true);
			}
			using (RegistryKey registry = OpenRegistryKey(Paths.DirectoryRightKeyShell + "\\" + name))
			{
				registry.SetValue("icon", iconPath);
				registry.CreateSubKey("command", true).SetValue("", command);
			}
		}



		/// <summary>
		/// 删除文件夹右键菜单
		/// </summary>
		/// <param name="name">菜单名称</param>
		public static void DeleteDirectoryRightMenu(string name)
		{
			using (RegistryKey registry = OpenRegistryKey(Paths.DirectoryRightKeyShell + "\\" + name))
			{
				registry.DeleteSubKey("command", false);
			}
			using (RegistryKey registry = OpenRegistryKey(Paths.DirectoryRightKeyShell))
			{
				registry.DeleteSubKey(name, false);
			}
		}


		/// <summary>
		/// 绑定文件打开方式(使用指定程序打开指定后缀文件
		/// </summary>
		/// <param name="suffixName">文件后缀</param>
		/// <param name="keyName">注册表项名称</param>
		/// <param name="operationSoftPath">打开此后缀文件的程序(完整路径)</param>
		public static void BindFileOpenMethod(string suffixName, string keyName, string operationSoftPath)
		{
			// 确保后缀名以"."开始
			if (!suffixName.StartsWith("."))
			{
				suffixName = "." + suffixName;
			}
			// 使用using语句自动关闭注册表键
			using (var rootKey = Registry.ClassesRoot)
			{
				// 创建或打开后缀名对应的键，并设置其默认值为keyName
				using (var suffixKey = rootKey.CreateSubKey(suffixName))
				{
					suffixKey.SetValue("", keyName);
				}
				// 为文件类型创建一个键
				using (var fileTypeKey = rootKey.CreateSubKey(keyName))
				{
					// 创建“DefaultIcon”子键
					using (var defaultIconKey = fileTypeKey.CreateSubKey("DefaultIcon"))
					{
						// 设置程序图标路径
						defaultIconKey.SetValue("", $"{operationSoftPath},0");
					}
					// 创建“shell”子键和其下的“open”子键
					using (var shellKey = fileTypeKey.CreateSubKey("shell"))
					using (var openKey = shellKey.CreateSubKey("open"))
					{
						// 创建“command”子键并绑定打开文件的程序路径
						using (var commandKey = openKey.CreateSubKey("command"))
						{
							commandKey.SetValue("", $"\"{operationSoftPath}\" \"%1\"");
						}
					}
				}
			}
		}

		/// <summary>
		/// 取消绑指定后缀文件的打开方式
		/// </summary>
		/// <param name="suffixName">文件后缀</param>
		/// <param name="keyName">注册表项名称</param>
		public static void UnbindFileOpenMethod(string suffixName, string keyName)
		{
			// 确保后缀名以"."开始
			if (!suffixName.StartsWith("."))
			{
				suffixName = "." + suffixName;
			}
			// 使用using语句确保RegistryKey对象正确关闭
			using (var rootKey = Registry.ClassesRoot)
			{
				// 删除文件类型关联的键（如果存在）
				rootKey.DeleteSubKeyTree(keyName, false);
				// 尝试删除后缀名对应的键（如果存在）
				rootKey.DeleteSubKeyTree(suffixName, false);
			}
		}



		/// <summary>
		/// 注册表常用地址
		/// </summary>
		public class Paths
		{
			/// <summary>
			/// 控制USB存储设备驱动程序的加载行为
			/// <list type="table">
			/// <item>
			/// <term>Start</term>
			/// <description>控制USB存储设备驱动程序的加载行为 值包括：
			///     <list type="bullet">
			///     <item>0 (SERVICE_BOOT_START): 启动时加载</item>
			///     <item>1 (SERVICE_SYSTEM_START): 系统初始化时加载</item>
			///     <item>2 (SERVICE_AUTO_START): 自动加载</item>
			///     <item>3 (SERVICE_DEMAND_START): 按需加载</item>
			///     <item>4 (SERVICE_DISABLED): 禁用</item>
			///     </list>
			/// </description>
			/// </item>
			/// <item>
			/// <term>Type</term>
			/// <description>指示服务类型。值包括：
			///     <list type="bullet">
			///     <item>1 (SERVICE_KERNEL_DRIVER): 内核驱动程序。</item>
			///     <item>2 (SERVICE_FILE_SYSTEM_DRIVER): 文件系统驱动程序，对于USBSTOR通常不适用</item>
			///     </list>
			/// </description>
			/// </item>
			/// <item>
			/// <term>ErrorControl</term>
			/// <description>指定如果无法加载驱动程序，系统启动过程中的错误控制级别。值包括：
			///     <list type="bullet">
			///     <item>0 (SERVICE_ERROR_IGNORE): 忽略错误继续启动</item>
			///     <item>1 (SERVICE_ERROR_NORMAL): 正常错误处理，显示消息框请求用户输入</item>
			///     <item>2 (SERVICE_ERROR_SEVERE): 严重错误，系统尝试重新启动，使用最后一次成功的配置</item>
			///     <item>3 (SERVICE_ERROR_CRITICAL): 关键错误，系统尝试重新启动，如果失败则停止</item>
			///     </list>
			/// </description>
			/// </item>
			/// <item>
			/// <term>ImagePath</term>
			/// <description>指定服务的可执行文件的路径。例如：“\SystemRoot\System32\drivers\USBSTOR.SYS”</description>
			/// </item>
			/// <item>
			/// <term>DisplayName</term>
			/// <description>提供服务的友好名称。例如：“USB Mass Storage Driver”表示这是USB大容量存储设备的驱动程序</description>
			/// </item>
			/// <item>
			/// <term>Group</term>
			/// <description>指定驱动程序加载的顺序组。例如：“SCSI miniport”，表示该驱动程序在SCSI小型端口驱动程序组中加载</description>
			/// </item>
			/// </list>
			/// </summary>

			public const string USBDriveLoad = @"SYSTEM\CurrentControlSet\Services\USBSTOR";
			/// <summary>
			/// 定义在系统启动时自动执行的程序列表
			/// </summary>
			public const string AutoRun = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run";
			/// <summary>
			/// 文件右键
			/// </summary>
			public const string FileRightKey = @"HKEY_CLASSES_ROOT\*";
			/// <summary>
			/// 文件右键Shell项
			/// </summary>
			public const string FileRightKeyShell = @"HKEY_CLASSES_ROOT\*\shell";
			/// <summary>
			/// 文件夹右键
			/// </summary>
			public const string DirectoryRightKey = @"HKEY_CLASSES_ROOT\Directory";
			/// <summary>
			/// 文件夹右键Shell项
			/// </summary>
			public const string DirectoryRightKeyShell = @"HKEY_CLASSES_ROOT\Directory\shell";
			/// <summary>
			/// 空白处右键
			/// </summary>
			public const string EmptyRightKey = @"HKEY_CLASSES_ROOT\Directory\Background";
			/// <summary>
			/// 空白处右键Shell项
			/// </summary>
			public const string EmptyRightKeyShell = @"HKEY_CLASSES_ROOT\Directory\Background\shell";
			/// <summary>
			/// 使用的USB设备
			/// </summary>
			public const string USBRecord = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceClasses\{a5dcbf10-6530-11d2-901f-00c04fb951ed}";
			/// <summary>
			/// 近期打开的文件记录
			/// </summary>
			public const string RecentlyOpenedFile = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU\*";
			/// <summary>
			/// 网卡信息
			/// </summary>
			public const string NetworkCard = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Class\{4d36e972-e325-11ce-bfc1-08002be10318}";

		}
		
	}
}
