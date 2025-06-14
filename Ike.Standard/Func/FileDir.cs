﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;

namespace Ike.Standard
{
	/// <summary>
	/// 文件及目录相关
	/// </summary>
	public static class FileDir
	{


		/// <summary>
		/// 将程序内部镶嵌的资源文件保存包指定路径
		/// </summary>
		/// <param name="resPath">资源文件路径,格式为: [namespace.Res.source.png] 以'.'分割资源子级</param>
		/// <param name="outputPath">资源文件保存到的路径</param>
		/// <returns>写出后判断文件是否存在,存在则为<see langword="true"/>,反之为<see langword="false"/></returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static bool GetResourceToFile(string resPath, string outputPath)
		{
			using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resPath))
			{
				if (resourceStream is null)
				{
					throw new FileNotFoundException(resPath);
				}
				using (FileStream fileStream = File.Create(outputPath))
				{
					resourceStream.CopyTo(fileStream);
				}
				return File.Exists(outputPath);
			}
		}


		/// <summary>
		/// 获取指定文件SHA256哈希值
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>文件的哈希值</returns>
		/// <exception cref="FileNotFoundException">文件未找到</exception>
		public static string GetFileSHA256(string filePath)
		{
			// 检查文件是否存在
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException("file not found", filePath);
			}
			// 创建 SHA256 实例
			using (SHA256 sha256 = SHA256.Create())
			{
				// 打开文件流
				using (FileStream stream = File.OpenRead(filePath))
				{
					// 计算哈希值
					byte[] hashBytes = sha256.ComputeHash(stream);
					// 将字节数组转换为十六进制字符串
					StringBuilder sb = new StringBuilder();
					foreach (byte b in hashBytes)
					{
						// "x2" 表示两位小写十六进制
						sb.Append(b.ToString("x2"));
					}
					return sb.ToString();
				}
			}
		}


		/// <summary>
		/// 指针数据写入Bin文件
		/// </summary>
		/// <param name="intPtr">指针</param>
		/// <param name="length">数据长度</param>
		/// <param name="filePath">保存路径</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static bool IntPtrToFile(IntPtr intPtr, int length, string filePath)
		{
			if (intPtr == IntPtr.Zero)
			{
				throw new ArgumentException("指针为空", nameof(intPtr));
			}
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			byte[] buffer = new byte[length];
			Marshal.Copy(intPtr, buffer, 0, length);
			File.WriteAllBytes(filePath, buffer);
			return File.Exists(filePath);
		}


		/// <summary>
		/// Bin文件加载到指针
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns>指向新分配内存的指针,使用完成后必须使用<see cref="Marshal.FreeHGlobal"/>方法释放该内存</returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static IntPtr LoadBinFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(filePath);
			}
			byte[] fileBytes = File.ReadAllBytes(filePath);
			IntPtr intPtr = Marshal.AllocHGlobal(fileBytes.Length);
			Marshal.Copy(fileBytes, 0, intPtr, fileBytes.Length);
			return intPtr;
		}


		/// <summary>
		/// 追加文本到文件,如果文件不存在则会创建文件,使用<see cref="MemoryMappedFile"/>方式打开进行写入,适用于超大文件追加写入
		/// </summary>
		/// <param name="text">追加的文本</param>
		/// <param name="filePath">文件路径</param>
		/// <param name="encoding">编码格式</param>
		public static void AppendTextMMF(string text,string filePath,Encoding encoding)
		{
			if (!File.Exists(filePath))
			{
				File.Create(filePath).Close();
			}
			long offset = new FileInfo(filePath).Length;
			long newContentLength = Encoding.UTF8.GetByteCount(text);
			long capacity = offset + newContentLength; 
			using (var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.OpenOrCreate, null, capacity))
			{
				using (var accessor = mmf.CreateViewAccessor(offset, newContentLength))
				{
					byte[] buffer = encoding.GetBytes(text);
					accessor.WriteArray(0, buffer, 0, buffer.Length);
				}
			}
		}

		/// <summary>
		/// 准备文件目录,传递文件路径,如果不存在则创建这个文件路目录
		/// </summary>
		/// <param name="filePath">文件路径</param>
		public static void ReadinessFileDirectory(string filePath)
		{
			if (File.Exists(filePath))
			{
				return;
			}
			string path = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}


		/// <summary>
		/// 获取文件夹内文件,按照创建日期排序
		/// </summary>
		/// <param name="directoryPath">目标文件夹</param>
		/// <param name="search">通配符匹配文件</param>
		/// <param name="select">选择查询目录</param>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public static string[] GetFileCreationTimeOrder(string directoryPath, string search = "*", SearchOption select = SearchOption.TopDirectoryOnly)
		{
			if (!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException(directoryPath);
			}
			return Directory.GetFiles(directoryPath, search, select).OrderBy(File.GetCreationTime).ToArray();
		}

		/// <summary>
		/// 获取文件夹内文件,按照写入日期排序
		/// </summary>
		/// <param name="directoryPath">目标文件夹</param>
		/// <param name="search">通配符匹配文件</param>
		/// <param name="select">选择查询目录</param>
		/// <returns></returns>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public static string[] GetFileWriteTimeOrder(string directoryPath, string search = "*", SearchOption select = SearchOption.TopDirectoryOnly)
		{
			if (!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException(directoryPath);
			}
			return Directory.GetFiles(directoryPath, search, select).OrderBy(File.GetLastWriteTime).ToArray();
		}


		/// <summary>
		/// 获取文件夹内文件,按照最后修改文件的日期排序
		/// </summary>
		/// <param name="directoryPath">目标文件夹</param>
		/// <param name="search">通配符匹配文件</param>
		/// <param name="select">选择查询目录</param>
		/// <returns></returns>
		/// <exception cref="DirectoryNotFoundException"></exception>
		public static string[] GetFileLastWriteTimeOrder(string directoryPath, string search = "*", SearchOption select = SearchOption.TopDirectoryOnly)
		{
			if (!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException(directoryPath);
			}
			return Directory.GetFiles(directoryPath, search, select).OrderBy(File.GetLastWriteTime).ToArray();
		}


		/// <summary>
		/// 获取<see langword="ini"/>文件中所有的<see langword="Section"/>
		/// </summary>
		/// <param name="filePath"><see langword="ini"/>文件路径</param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static List<string> GetIniSectionNames(string filePath)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException(filePath);
			}
			List<string> sectionNames = new List<string>();
			using (StreamReader reader = new StreamReader(filePath))
			{
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("[") && line.EndsWith("]"))
					{
						string sectionName = line.Substring(1, line.Length - 2);
						sectionNames.Add(sectionName);
					}
				}
			}
			return sectionNames;
		}


		/// <summary>
		/// 获取<see langword="ini"/>文件指定<see langword="Section"/>下的所有<see langword="Key"/>和<see langword="Value"/>
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="sectionName">要获取的<see langword="Section"/></param>
		/// <returns></returns>
		/// <exception cref="FileNotFoundException"></exception>
		/// <exception cref="Exception"></exception>
		public static Dictionary<string, string> GetIniSectionKeyValues(string filePath, string sectionName)
		{
			if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);
			if (string.IsNullOrEmpty(sectionName)) throw new Exception("String cannot be empty.");
			Dictionary<string, string> keyValues = new Dictionary<string, string>();
			using (StreamReader reader = new StreamReader(filePath))
			{
				string line;
				string currentSection = null;
				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("#") || line.StartsWith(";"))
					{
						continue;
					}
					if (line.Trim().StartsWith("[") && line.Trim().EndsWith("]"))
					{
						currentSection = line.Substring(1, line.Length - 2);
					}
					else if (currentSection == sectionName && line.Contains('='))
					{
						int separatorIndex = line.IndexOf('=');
						var key = line.Substring(0, separatorIndex).Trim();
						var value = line.Substring(separatorIndex + 1).Trim();
						keyValues[key] = value;
					}
				}
			}
			return keyValues;
		}


		/// <summary>
		/// 获取<see langword="ini"/>文件中所有的<see langword="Key"/>和<see langword="Value"/>
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static Dictionary<string, Dictionary<string, string>> GetIniAllKeyValues(string filePath)
		{
			Dictionary<string, Dictionary<string, string>> keyValues = new Dictionary<string, Dictionary<string, string>>();
			foreach (string section in GetIniSectionNames(filePath))
			{
				keyValues.Add(section, GetIniSectionKeyValues(filePath, section));
			}
			return keyValues;
		}


		/// <summary>
		/// 使用<seealso cref="SHA256"/>哈希算法比较两个文件是否相同
		/// </summary>
		/// <param name="sourceFile">原文件路径</param>
		/// <param name="compareFile">比较文件路径</param>
		/// <returns>如果文件内容相同,则返回 <see langword="true"/>;反之则返回 <see langword="false"/></returns>
		public static bool CompareFileContent(string sourceFile, string compareFile)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				using (FileStream file1 = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					using (FileStream file2 = new FileStream(compareFile, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						return StructuralComparisons.StructuralEqualityComparer.Equals(sha256.ComputeHash(file1), sha256.ComputeHash(file2));
					}
				}
			}
		}


		/// <summary>
		/// 获取文件哈希值,使用<seealso cref="SHA256"/>算法
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static string GetFileHashValue(string filePath)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					return BitConverter.ToString(sha256.ComputeHash(file)).Replace("-", string.Empty);
				}
			}
		}


		/// <summary>
		/// 指定编码格式读取INI内容
		/// </summary>
		/// <param name="section">要检索的键所在的节名称</param>
		/// <param name="key">要检索的项的名称</param>
		/// <param name="def">如果在文件中找不到指定的键，则返回的默认值</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <param name="encoding">指定编码读取</param>
		/// <param name="bufferSize">缓冲区大小,用于保存返回的字符串,如果缓存区小于实际内容大小,则会返回不完整的内容</param>
		/// <remarks>
		///   <list type="bullet">
		///     <item>如果找不到指定的键,则返回默认值<paramref name="def"/></item>
		///     <item>如果找到指定的键,但其值为空字符串,则返回空字符串</item>
		///     <item>如果 INI 文件或指定的节和键不存在,或者发生其他错误,函数将返回空字符串</item>
		///   </list>
		/// </remarks>
		/// <returns>从 INI 文件中检索到的字符串值,如果找不到指定的键,则返回默认值<paramref name="def"/></returns>
		public static string IniRead(string section, string key, string def, string filePath, Encoding encoding, int bufferSize = 1024)
		{
			if (encoding is null)
			{
				encoding = Encoding.UTF8;
			}
			byte[] buffer = new byte[bufferSize];
			int count = WinAPI.GetPrivateProfileString(section.ToBytes(encoding), key.ToBytes(encoding), def.ToBytes(encoding), buffer, bufferSize, filePath);
			return encoding.GetString(buffer, 0, count);
		}


		/// <summary>
		/// 指定编码格式读取INI内容
		/// </summary>
		/// <param name="section">要检索的键所在的节名称</param>
		/// <param name="key">要检索的项的名称</param>
		/// <param name="def">如果在文件中找不到指定的键，则返回的默认值</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <param name="encoding">指定编码读取</param>
		/// <param name="bufferSize">缓冲区大小,用于保存返回的字符串,如果缓存区小于实际内容大小,则会返回不完整的内容</param>
		/// <param name="isWriteDefValue">如果读取数据为空是,是否将默认数据写入文件</param>
		/// <remarks>
		///   <list type="bullet">
		///     <item>如果找不到指定的键,则返回默认值<paramref name="def"/></item>
		///     <item>如果找到指定的键,但其值为空字符串,则返回空字符串</item>
		///     <item>如果 INI 文件或指定的节和键不存在,或者发生其他错误,函数将返回空字符串</item>
		///   </list>
		/// </remarks>
		/// <returns>从 INI 文件中检索到的字符串值,如果找不到指定的键,则返回默认值<paramref name="def"/></returns>
		public static string IniRead(string section, string key, string def, string filePath, Encoding encoding, int bufferSize = 1024, bool isWriteDefValue = false)
		{
			if (encoding is null)
			{
				encoding = Encoding.UTF8;
			}
			byte[] buffer = new byte[bufferSize];
			int count = WinAPI.GetPrivateProfileString(section.ToBytes(encoding), key.ToBytes(encoding), def.ToBytes(encoding), buffer, bufferSize, filePath);
			string read = encoding.GetString(buffer, 0, count);
			if (isWriteDefValue && string.IsNullOrEmpty(read))
			{
				WinAPI.WritePrivateProfileString(section.ToBytes(encoding), key.ToBytes(encoding), def.ToBytes(encoding), filePath);
			}
			return read;
		}


		/// <summary>
		/// 直接遍历文件读取INI指定键值
		/// </summary>
		/// <param name="section">数据部分</param>
		/// <param name="key">键</param>
		/// <param name="defAult">如果读取失败,则返回这个默认值</param>
		/// <param name="iniPath">INI文件路径</param>
		/// <returns>读取成功返回读取值,失败则返回默认值</returns>
        public static string ReadIniValue(string section, string key,string defAult, string iniPath)
        {
            if (!File.Exists(iniPath))
                return defAult;

            string[] lines = File.ReadAllLines(iniPath);
			// 如果未指定 section，则匹配所有键
            bool inCorrectSection = (section == null); 

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                // 检查是否是节标记（如 [Section]）
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    string currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    inCorrectSection = (section == null) || currentSection.Equals(section, StringComparison.OrdinalIgnoreCase);
                    continue;
                }
                // 如果不在正确的节，跳过
                if (!inCorrectSection)
                    continue;
                // 解析键值对
                int equalSignIndex = line.IndexOf('=');
                if (equalSignIndex > 0)
                {
                    string currentKey = line.Substring(0, equalSignIndex).Trim();
                    if (currentKey.Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        return line.Substring(equalSignIndex + 1).Trim();
                    }
                }
            }

            return defAult;
        }

        /// <summary>
        /// 指定编码格式读取INI中 <see cref="int"></see> 数据
        /// </summary>
        /// <param name="section">要检索的键所在的节名称</param>
        /// <param name="key">要检索的项的名称</param>
        /// <param name="def">如果在文件中找不到指定的键，则返回的默认值</param>
        /// <param name="filePath">INI 文件的完整路径</param>
        /// <param name="encoding">指定编码读取</param>
        /// <param name="bufferSize">缓冲区大小,用于保存返回的字符串,如果缓存区小于实际内容大小,则会返回不完整的内容</param>
        /// <returns>从 INI 文件中检索到的字符串值,如果找不到指定的键,则返回默认值<paramref name="def"/></returns>
        public static int IniReadToInt(string section, string key, int def, string filePath, Encoding encoding, int bufferSize = 1024)
		{
			if (encoding is null)
			{
				encoding = Encoding.UTF8;
			}
			byte[] buffer = new byte[bufferSize];
			int count = WinAPI.GetPrivateProfileString(section.ToBytes(encoding), key.ToBytes(encoding), def.ToString().ToBytes(encoding), buffer, bufferSize, filePath);
			int.TryParse(encoding.GetString(buffer, 0, count), out def);
			return def;
		}

		/// <summary>
		/// 指定编码格式读取INI中 <see cref="double"></see> 数据
		/// </summary>
		/// <param name="section">要检索的键所在的节名称</param>
		/// <param name="key">要检索的项的名称</param>
		/// <param name="def">如果在文件中找不到指定的键，则返回的默认值</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <param name="encoding">指定编码读取</param>
		/// <param name="bufferSize">缓冲区大小,用于保存返回的字符串,如果缓存区小于实际内容大小,则会返回不完整的内容</param>
		/// <returns>从 INI 文件中检索到的字符串值,如果找不到指定的键,则返回默认值<paramref name="def"/></returns>
		public static double IniReadToDouble(string section, string key, double def, string filePath, Encoding encoding, int bufferSize = 1024)
		{
			if (encoding is null)
			{
				encoding = Encoding.UTF8;
			}
			byte[] buffer = new byte[bufferSize];
			int count = WinAPI.GetPrivateProfileString(section.ToBytes(encoding), key.ToBytes(encoding), def.ToString().ToBytes(encoding), buffer, bufferSize, filePath);
			double.TryParse(encoding.GetString(buffer, 0, count), out def);
			return def;
		}



		/// <summary>
		/// 指定编码格式写入INI内容
		/// </summary>
		/// <param name="section">要写入的键所在的节名称</param>
		/// <param name="key">要写入的项的名称</param>
		/// <param name="value">要写入的项的新字符串,通过 <see cref="object.ToString()"/> 方法转为字符串</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <param name="encoding">指定编码写入</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>如果指定的 INI 文件不存在,此函数会创建文件</item>
		/// <item>如果指定的键已经存在,此函数会更新键的值</item>
		/// <item>如果 INI 文件中指定的节和键不存在,此函数会创建它们</item>
		/// </list>
		/// </remarks>
		/// <returns>如果写入成功,则返回 <seealso langword="true"/>;否则,返回 <seealso langword="false"/></returns>
		public static bool IniWrite(string section, string key, object value, string filePath, Encoding encoding)
		{
			if (encoding is null)
			{
				encoding = Encoding.UTF8;
			}
			return WinAPI.WritePrivateProfileString(section.ToBytes(encoding), key.ToBytes(encoding), value.ToString().ToBytes(encoding), filePath);
		}


		/// <summary>
		/// 目录复制
		/// </summary>
		/// <param name="sourceDirName">原目录</param>
		/// <param name="destDirName">目标目录</param>
		/// <param name="copySubDirs">是否复制子目录</param>
		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();
			// 如果目标目录不存在，创建它
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			// 获取源目录中的所有文件并复制到目标目录
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string tempPath = Path.Combine(destDirName, file.Name);
				file.CopyTo(tempPath, false);
			}
			// 如果复制子目录，递归调用
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string tempPath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
				}
			}
		}


	}
}
