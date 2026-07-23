#if NETFRAMEWORK

using Microsoft.Win32;
using System;
using System.IO;

namespace Ike.Standard
{
	/// <summary>
	/// 注册表操作类
	/// </summary>
	public partial class Regedit
	{
		/// <summary>
		/// 获取注册表路径的根键
		/// </summary>
		/// <param name="fullPath">注册表完整路径</param>
		/// <param name="subKeyPath">返回注册表根键下的子项路径</param>
		/// <returns></returns>
		public RegistryKey GetRootKey(string fullPath,out string subKeyPath)
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
		public RegistryKey OpenRegistryKey(string fullPath)
		{
			RegistryKey rootKey = GetRootKey(fullPath,out string subKeyPath);
			return rootKey.OpenSubKey(subKeyPath);
		}


		/// <summary>
		/// 指定注册表项是否存在
		/// </summary>
		/// <param name="fullPath"><inheritdoc cref="GetRootKey" path="/param[@name='fullPath']"/></param>
		/// <returns></returns>
		public bool Exist(string fullPath)
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
		public RegistryKey CreateRegistryKey(string fullPath)
		{
			RegistryKey rootKey = GetRootKey(fullPath,out string subKeyPath);
			return rootKey.CreateSubKey(subKeyPath); 
		}


		/// <summary>
		/// 获取指定项的所有子项名字
		/// </summary>
		/// <param name="fullPath"><inheritdoc cref="GetRootKey" path="/param[@name='fullPath']"/></param>
		/// <returns>获取到的子项的字符串集合</returns>
		public string[] GetSubitemNames(string fullPath)
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
		public string[] GetGetValueNames(string fullPath)
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
		public object GetValue(string fullPath, string name, string defaultValue)
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
		public void SetValue(string fullPath,string name,object value, RegistryValueKind valueKind)
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
		public void CreateFileRightMenu(string name, string iconPath, string command)
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
		public void DeleteFileRightMenu(string name)
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
		public void CreateEmptyRightMenu(string name, string iconPath, string command)
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
		public void DeleteEmptyRightMenu(string name)
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
		public void CreateDirectoryRightMenu(string name, string iconPath, string command)
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
		public void DeleteDirectoryRightMenu(string name)
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
		public void BindFileOpenMethod(string suffixName, string keyName, string operationSoftPath)
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
		public void UnbindFileOpenMethod(string suffixName, string keyName)
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



		
	}
}
#endif
