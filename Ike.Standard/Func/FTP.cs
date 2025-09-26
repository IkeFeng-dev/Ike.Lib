using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Ike.Standard
{
	/// <summary>
	/// FTP 操作类
	/// </summary>
	public class Ftp
	{
		string ftpServerIP;
		string ftpUserID;
		string ftpPassword;
		string currentDirectory = "/";
		FtpWebRequest reqFTP;

		/// <summary>
		/// 资源信息结构
		/// </summary>
		public struct ResponseInfo
		{
			/// <summary>
			/// 是否文件
			/// </summary>
			public bool IsFile;
			/// <summary>
			/// 权限
			/// </summary>
			public string Limits;
			/// <summary>
			/// 大小
			/// </summary>
			public long Size;
			/// <summary>
			/// 修改时间
			/// </summary>
			public DateTime Modification;
			/// <summary>
			/// 名称
			/// </summary>
			public string Name;
		}

		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="ftpServerIP">IP[192.168.99.216:2121]</param>
		/// <param name="ftpUserID">用户名</param>
		/// <param name="ftpPassword">密码</param>
		public Ftp(string ftpServerIP, string ftpUserID, string ftpPassword)
		{
			UpdataUser(ftpServerIP, ftpUserID, ftpPassword);
		}

		/// <summary>
		/// 获取或设置当前FTP中的工作目录
		/// </summary>
		public string CurrentDirectory
		{
			get
			{
				string repl = currentDirectory.Replace("//", "/");
				if (repl.StartsWith("/"))
				{
					repl = repl.Substring(1);
				}
				while (repl.EndsWith("/"))
				{
					repl = repl.Substring(0, repl.Length - 1);
				}
				if (!string.IsNullOrWhiteSpace(repl))
				{
					repl += "/";
				}
				return repl;
			}
			set
			{
				string val = value.Replace("\\", "/").Replace("ftp://" + ftpServerIP + "/", string.Empty);
				currentDirectory = val;
			}
		}

		/// <summary>
		/// 进入FTP当前工作目录下的子目录
		/// </summary>
		/// <param name="directoryName"></param>
		public void CDIn(string directoryName)
		{
			directoryName = directoryName.Replace("\\", "/");
			if (!directoryName.StartsWith("/"))
			{
				directoryName = '/' + directoryName;
			}
			currentDirectory += directoryName;
		}

		/// <summary>
		/// 返回FTP当前工作目录的父目录
		/// </summary>
		/// <param name="level">返回上级目录的层次</param>
		public void CDOut(int level = 1)
		{
			if (level == 0)
			{
				return;
			}
			if (level < 0)
			{
				level = 1;
			}
			if (currentDirectory.EndsWith("/"))
			{
				level++;
			}
			string[] items = currentDirectory.Split('/');
			int max = items.Length;
			if (level > max)
			{
				currentDirectory = "/";
				return;
			}
			string add = string.Empty;
			for (int i = 0; i < items.Length - level; i++)
			{
				if (!string.IsNullOrWhiteSpace(items[i]))
				{
					add += items[i] + '/';
				}
			}
			currentDirectory = add;
		}


		/// <summary>
		/// 连接ftp,完整的Url
		/// </summary>
		/// <param name="url"></param>
		private void Connect(string url)
		{
			// 根据uri创建FtpWebRequest对象 
			reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(url));
			// 指定数据传输类型 
			reqFTP.UseBinary = true;
			//获取或设置客户端应用程序的数据传输过程的行为
			reqFTP.UsePassive = true;
			//是否关闭到 FTP 服务器的控制连接
			reqFTP.KeepAlive = false;
			// ftp用户名和密码 
			reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

		}


		/// <summary>
		/// 更新用户身份
		/// </summary>
		/// <param name="ftpServerIP">IP[192.168.99.216:2121]</param>
		/// <param name="ftpUserID">用户名</param>
		/// <param name="ftpPassword">密码</param>
		public void UpdataUser(string ftpServerIP, string ftpUserID, string ftpPassword)
		{
			ftpServerIP = ftpServerIP.ToLower();
			if (!ftpServerIP.StartsWith("ftp://"))
			{
				ftpServerIP = "ftp://" + ftpServerIP;
			}
			if (!ftpServerIP.EndsWith("/"))
			{
				ftpServerIP += "/";
			}
			this.ftpServerIP = ftpServerIP;
			this.ftpUserID = ftpUserID;
			this.ftpPassword = ftpPassword;
			Connect(this.ftpServerIP);
		}

		/// <summary>
		/// 解析资源信息
		/// </summary>
		/// <param name="line">资源文件或目录的详细信息</param>
		/// <returns></returns>
		private ResponseInfo? GetInfo(string line)
		{
			string[] items = line.Split(' ');
			string[] notEmpty = items.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
			if (items.Length < 10)
			{
				return null;
			}
			bool isFile = items[0].StartsWith("-");
			string[] mouths = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
			for (int i = 0; i < notEmpty.Length; i++)
			{
				if (i > notEmpty.Length - 4)
				{
					break;
				}
				if (!long.TryParse(notEmpty[i], out long size) && size >= 0)
				{
					continue;
				}
				if (!mouths.Contains(notEmpty[i + 1]))
				{
					continue;
				}
				if (!int.TryParse(notEmpty[i + 2], out int date))
				{
					continue;
				}
				if (notEmpty[i + 3].Contains(':') || int.TryParse(notEmpty[i + 3], out int year))
				{
					string strDate = $"{notEmpty[i + 1]} {notEmpty[i + 2]} {notEmpty[i + 3]}";
					string format = "MMM dd yyyy";
					if (notEmpty[i + 3].Contains(':'))
					{
						format = "MMM dd HH:mm";
					}
					DateTime dateTime = DateTime.ParseExact(strDate, format, CultureInfo.InvariantCulture);

					string name = string.Empty;
					for (int j = notEmpty.Length - 1; j > 0; j--)
					{
						if (notEmpty[j] == notEmpty[i + 3] && notEmpty[i + 2] == date.ToString("D2"))
						{
							name = name.Substring(1);
							break;
						}
						name = string.Format(" {0}{1}", notEmpty[j], name);
					}
					return new ResponseInfo()
					{
						Name = name,
						Limits = notEmpty[0],
						IsFile = isFile,
						Size = size,
						Modification = dateTime,
					};
				}
			}
			return null;
		}

		/// <summary>
		/// 获取指定目录下资源信息
		/// </summary>
		/// <param name="ftpPath">FTP目录</param>
		/// <returns></returns>
		public ResponseInfo[] GetResponseInfo(string ftpPath)
		{
			string[] lines = GetResponse(ftpPath, WebRequestMethods.Ftp.ListDirectoryDetails);
			List<ResponseInfo?> responseInfo = new List<ResponseInfo?>();
			foreach (var item in lines)
			{
				responseInfo.Add(GetInfo(item));
			}
			return responseInfo.Where(item => item != null).Cast<ResponseInfo>().ToArray();
		}




		/// <summary>
		/// 获得当前工作目录下文件列表
		/// </summary>
		/// <returns></returns>
		public string[] GetFileList()
		{
			List<string> list = new List<string>();
			foreach (var item in GetResponseInfo(CurrentDirectory))
			{
				if (item.IsFile)
				{
					list.Add(item.Name);
				}
			}
			return list.ToArray();
		}


		/// <summary>
		/// 获取FTP指定目录下文件列表
		/// </summary>
		/// <param name="ftpPath">FTP目录路径</param>
		/// <returns></returns>
		public string[] GetFileList(string ftpPath)
		{
			List<string> list = new List<string>();
			foreach (var item in GetResponseInfo(ftpPath))
			{
				if (item.IsFile)
				{
					list.Add(item.Name);
				}
			}
			return list.ToArray();
		}


		/// <summary>
		/// 获得当前工作目录下子目录列表
		/// </summary>
		/// <returns></returns>
		public string[] GetDirectoryList()
		{
			List<string> list = new List<string>();
			foreach (var item in GetResponseInfo(CurrentDirectory))
			{
				if (!item.IsFile)
				{
					list.Add(item.Name);
				}
			}
			return list.ToArray();
		}


		/// <summary>
		/// 获取FTP指定目录下目录列表
		/// </summary>
		/// <param name="ftpPath">FTP目录路径</param>
		/// <returns></returns>
		public string[] GetDirectoryList(string ftpPath)
		{
			List<string> list = new List<string>();
			foreach (var item in GetResponseInfo(ftpPath))
			{
				if (item.IsFile)
				{
					list.Add(item.Name);
				}
			}
			return list.ToArray();
		}


		/// <summary>
		/// 获取服务端指定路径的资源信息
		/// </summary>
		/// <param name="ftpPath">FTP目录完整路径</param>
		/// <param name="WRMethods">常量方法[LIST,NLST]</param>
		/// <returns></returns>
		public string[] GetResponse(string ftpPath, string WRMethods = "LIST")
		{
			StringBuilder result = new StringBuilder();
			char sp = '£';
			ftpPath = ftpPath.Replace(this.ftpServerIP, "");
			if (!ftpPath.EndsWith("/"))
			{
				ftpPath += "/";
			}
			if (ftpPath.StartsWith("/"))
			{
				ftpPath = ftpPath.Substring(1, ftpPath.Length - 1);
			}
			string url = this.ftpServerIP + ftpPath;
			Connect(url);
			reqFTP.Method = WRMethods;
			using (WebResponse response = reqFTP.GetResponse())
			{
				//UTF8可访问中文文件名 
				using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
				{
					string line = reader.ReadLine();
					while (line != null)
					{
						result.Append(line);
						result.Append(sp);
						line = reader.ReadLine();
					}
					//移除最后一行空行
					result.Remove(result.Length - 1, 1);
					reader.Close();
					response.Close();
					return result.ToString().Split(sp);
				}
			}
		}


		/// <summary>
		/// 判断FTP当前工作目录是否存在
		/// </summary>
		/// <returns></returns>
		public bool CheckDirectory()
		{
			return CheckDirectory(CurrentDirectory);
		}


		/// <summary>
		/// 判断FTP目录是否存在
		/// </summary>
		/// <param name="ftpPath"></param>
		/// <returns></returns>
		public bool CheckDirectory(string ftpPath)
		{
			try
			{
				if (!ftpPath.EndsWith("/"))
				{
					ftpPath += "/";
				}
				if (ftpPath.StartsWith("/"))
				{
					ftpPath = ftpPath.Substring(1, ftpPath.Length - 1);
				}
				string url = this.ftpServerIP + ftpPath;
				Connect(url);
				reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
				using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
				{
					response.Close();
					return true;
				}
			}
			catch (WebException e)
			{
				if (e.HResult == -2146233079)
				{
					return false;
				}
				else
				{
					throw;
				}
			}
		}


		/// <summary>
		/// 上传文件到当前工作目录
		/// </summary>
		/// <param name="filePath">本地文件所在路径</param>
		/// <param name="progress">上传进度</param>
		/// <returns></returns>
		public bool Upload(string filePath, IProgress<double> progress = null)
		{
			return Upload(filePath, CurrentDirectory, progress);
		}


		/// <summary>
		/// 上传文件
		/// </summary>
		/// <param name="filePath">本地文件所在路径</param>
		/// <param name="ftpPath">FTP目录路径</param>
		/// <param name="progress">上传进度</param>
		/// <returns></returns>
		public bool Upload(string filePath, string ftpPath, IProgress<double> progress = null)
		{
			return Upload(filePath, ftpPath, Path.GetFileName(filePath), progress);
		}


		/// <summary>
		/// 上传文件
		/// </summary>
		/// <param name="filePath">本地文件所在路径</param>
		/// <param name="ftpPath">FTP目录路径</param>
		/// <param name="ftpFileName">FTP文件名称</param>
		/// <param name="progress">上传进度</param>
		/// <returns></returns>
		public bool Upload(string filePath, string ftpPath, string ftpFileName, IProgress<double> progress = null)
		{
			ftpFileName = Uri.EscapeDataString(ftpFileName);
			if (!ftpPath.EndsWith("/"))
			{
				ftpPath += "/";
			}
			if (ftpPath.StartsWith("/"))
			{
				ftpPath = ftpPath.Substring(1, ftpPath.Length - 1);
			}
			//判断FTP目标目录是否存在                
			if (!CheckDirectory(ftpPath))
			{
				CreateDirectory(ftpPath);
			}
			FileInfo fileInf = new FileInfo(filePath);
			string uri = this.ftpServerIP + ftpPath + ftpFileName;
			Connect(uri);
			//是否销毁FTP连接
			reqFTP.KeepAlive = false;
			// 指定执行命令 
			reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
			// 上传文件时通知服务器文件的大小 
			reqFTP.ContentLength = fileInf.Length;
			int buffLength = 2048;
			byte[] buff = new byte[buffLength];
			int contentLen;
			long totalUploaded = 0;
			long lastTotalUploaded = 0;
			bool isCal = progress != null;
			using (FileStream fs = fileInf.OpenRead())
			{
				using (Stream strm = reqFTP.GetRequestStream())
				{
					contentLen = fs.Read(buff, 0, buffLength);
					while (contentLen != 0)
					{
						strm.Write(buff, 0, contentLen);
						if (isCal && totalUploaded >= lastTotalUploaded)
						{
							totalUploaded += contentLen;
							double percent = (double)totalUploaded / fileInf.Length;
							progress.Report(percent);
						}
						contentLen = fs.Read(buff, 0, buffLength);
					}
					strm.Close();
					fs.Close();
					return true;
				}
			}
		}


		/// <summary>
		/// 上传本地目录到FTP
		/// </summary>
		/// <param name="currDirectory">本地目录</param>
		/// <param name="ftpPath">FTP目录</param>
		/// <param name="search">上传文件的文件名通配符匹配,[示例:只上传png文件:(*.png)]</param>
		/// <returns></returns>
		public bool UploadDirectory(string currDirectory, string ftpPath, string search = "*")
		{
			if (!ftpPath.EndsWith("/"))
			{
				ftpPath += "/";
			}
			if (ftpPath.StartsWith("/"))
			{
				ftpPath = ftpPath.Substring(1, ftpPath.Length - 1);
			}
			string url = this.ftpServerIP + ftpPath;
			Connect(url);
			string[] folders = Directory.GetDirectories(currDirectory);
			int count = 0;
			for (int i = 0; i < folders.Length; i++)
			{
				if (UploadDirectory(folders[i], ftpPath + folders[i].Split('\\').Last()))
				{
					count++;
				}
			}
			string[] files = Directory.GetFiles(currDirectory, search);
			for (int i = 0; i < files.Length; i++)
			{
				if (Upload(files[i], ftpPath, Path.GetFileName(files[i])))
				{
					count++;
				}
			}
			return count > 0;
		}




		/// <summary>
		/// 下载文件(默认下载到程序根目录中)[例:("/One/Bi.pbix")]
		/// </summary>
		/// <param name="ftpFilePath">文件在FTP中位置,根目录开始计算</param>
		/// <returns></returns>
		public bool Download(string ftpFilePath)
		{
			string downPath = ftpFilePath.Replace("/", "\\");
			downPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, downPath);
			return Download(ftpFilePath, downPath.Replace("\\\\", "\\"));
		}


		/// <summary>
		/// 下载文件[例:("/One/Bi.pbix", @"C:\Users\Administrator\Desktop\Down.PBIX")]
		/// </summary>
		/// <param name="ftpFilePath">文件在FTP中位置,根目录开始计算</param>
		/// <param name="downFilePath">文件下载到目录</param>
		/// <param name="fileSize">文件大小,未知则默认</param>
		/// <returns></returns>
		public bool Download(string ftpFilePath, string downFilePath, int fileSize = 204800)
		{
			string fileName = Path.GetFileName(downFilePath);
			string ftpFileName = Path.GetFileName(ftpFilePath);
			if (fileName == string.Empty)
			{
				fileName = ftpFileName;
			}
			string ftpPath = ftpFilePath.Replace(ftpFileName, "");
			string pathDown = downFilePath.Replace(fileName, "");
			return Download(ftpPath, ftpFileName, pathDown, fileName, fileSize);
		}


		/// <summary>
		/// 下载文件
		/// </summary>
		/// <param name="ftpFilePath">文件在FTP中目录,初始目录留空即可</param>
		/// <param name="ftpFileName">文件在FTP中名称</param>
		/// <param name="downFilePath">文件下载到本地的目录</param>
		/// <param name="downFileName">文件下载到本地的名称</param>
		/// <param name="bufferSize">缓存区大小</param>
		/// <returns></returns>
		public bool Download(string ftpFilePath, string ftpFileName, string downFilePath, string downFileName, int bufferSize = 204800)
		{
			ftpFileName = Uri.EscapeDataString(ftpFileName);
			if (!downFilePath.EndsWith("\\"))
			{
				downFileName += "\\";
			}
			string newFileName = downFilePath + downFileName;
			if (!ftpFilePath.EndsWith("/"))
			{
				ftpFilePath += "/";
			}
			if (ftpFilePath.StartsWith("/"))
			{
				ftpFilePath = ftpFilePath.Substring(1, ftpFilePath.Length - 1);
			}
			if (ftpFilePath == "/")
			{
				ftpFilePath = string.Empty;
			}
			if (!Directory.Exists(downFilePath))
			{
				Directory.CreateDirectory(downFilePath);
			}
			string url = this.ftpServerIP + ftpFilePath + ftpFileName;
			Connect(url);
			using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
			{
				using (Stream Reader = response.GetResponseStream())
				{
					using (MemoryStream mem = new MemoryStream(bufferSize))
					{
						byte[] buffer = new byte[bufferSize];
						int bytesRead;
						int TotalByteRead = 0;
						while (true)
						{
							bytesRead = Reader.Read(buffer, 0, buffer.Length);
							TotalByteRead += bytesRead;
							if (bytesRead == 0)
								break;
							mem.Write(buffer, 0, bytesRead);
						}
						if (mem.Length > 0)
						{
							byte[] bt = mem.ToArray();
							using (FileStream stream = new FileStream(newFileName, FileMode.Create))
							{
								stream.Write(bt, 0, bt.Length);
								stream.Flush();
								stream.Close();
								return true;
							}
						}
						response.Close();
						return true;
					}
				}
			}
		}


		/// <summary>
		/// 下载FTP中文件夹
		/// </summary>
		/// <param name="ftpPath">FTP目录</param>
		/// <param name="downloadDirectory">下载到本地的目录</param>
		/// <returns></returns>
		public bool DownloadDirectory(string ftpPath, string downloadDirectory)
		{
			if (!ftpPath.EndsWith("/"))
			{
				ftpPath += "/";
			}
			if (ftpPath.StartsWith("/"))
			{
				ftpPath = ftpPath.Substring(1, ftpPath.Length - 1);
			}
			if (!downloadDirectory.EndsWith("\\"))
			{
				downloadDirectory += "\\";
			}
			if (!Directory.Exists(downloadDirectory))
			{
				Directory.CreateDirectory(downloadDirectory);
			}
			string url = this.ftpServerIP + ftpPath;
			Connect(url);
			string[] files = GetFileList(ftpPath);
			int count = 0;
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Contains("."))
				{
					if (Download(ftpPath + files[i], downloadDirectory + files[i]))
					{
						count++;
					}
				}
				else
				{
					if (DownloadDirectory(ftpPath + files[i], downloadDirectory + files[i]))
					{
						count++;
					}
				}
			}
			return count > 0;
		}


		/// <summary>
		/// 创建当前FTP目录
		/// </summary>
		/// <returns></returns>
		public bool CreateDirectory()
		{
			return CreateDirectory(currentDirectory);
		}



		/// <summary>
		/// 创建FTP目录
		/// </summary>
		/// <param name="ftpPath">目录名</param>
		/// <returns></returns>
		public bool CreateDirectory(string ftpPath)
		{
			if (!ftpPath.EndsWith("/"))
			{
				ftpPath += "/";
			}
			if (ftpPath.StartsWith("/"))
			{
				ftpPath = ftpPath.Substring(1, ftpPath.Length - 1);
			}
			string url = this.ftpServerIP + ftpPath;
			Connect(url);
			reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
			using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
			{
				response.Close();
				return true;
			}
		}


		/// <summary>
		/// 删除FTP指定文件
		/// </summary>
		/// <param name="ftpFilePath">FTP文件所在位置,如果<paramref name="isCurrenWorkDir"/>为<see langword="true"></see>,那么此参数就应该设为工作目录下的文件名,反之则是FTP完整的文件路径</param>
		/// <param name="isCurrenWorkDir">是否在当前工作目录下执行</param>
		/// <returns>执行结果</returns>
		public bool DeleteFile(string ftpFilePath, bool isCurrenWorkDir = true)
		{
			if (isCurrenWorkDir)
			{ 
			   ftpFilePath = CurrentDirectory + ftpFilePath;
			}
			if (ftpFilePath.StartsWith("/"))
			{
				ftpFilePath = ftpFilePath.Substring(1, ftpFilePath.Length - 1);
			}
			string url = this.ftpServerIP + ftpFilePath;
			Connect(url);
			Uri serverUri = new Uri(url);
			if (serverUri.Scheme != Uri.UriSchemeFtp)
			{
				return false;
			}
			reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
			using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
			{
				response.Close();
				return true;
			}
		}



		/// <summary>
		/// 删除FTP目录
		/// </summary>
		/// <param name="ftpPath">FTP目录</param>
		/// <returns></returns>
		public bool DeleteDirectory(string ftpPath)
		{
			if (!ftpPath.EndsWith("/"))
			{
				ftpPath += "/";
			}
			if (ftpPath.StartsWith("/"))
			{
				ftpPath = ftpPath.Substring(1);
			}
			ResponseInfo[] infos = GetResponseInfo(ftpPath);
			foreach (ResponseInfo info in infos) 
			{
				string name = ftpPath + info.Name;
				if (info.IsFile)
				{
					DeleteFile(name);
				}
				else
				{
					DeleteDirectory(name);
				}
			}
			string url = this.ftpServerIP + ftpPath;
			Connect(url);
			reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
			using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
			{
				response.Close();
				return true;
			}
		}


		/// <summary>
		/// 重命名 FTP 服务器上的文件
		/// </summary>
		/// <param name="ftpFilePath">FTP 服务器上的文件路径,如果<paramref name="isCurrenWorkDir"/>为<see langword="true"></see>,那么此参数就应该设为工作目录下的文件名,反之则是FTP完整的文件路径</param>
		/// <param name="newFileName">新的文件名</param>
		/// <param name="isCurrenWorkDir">是否在当前工作目录下执行</param>
		/// <returns>重命名是否成功</returns>
		public bool RenameFile(string ftpFilePath, string newFileName, bool isCurrenWorkDir = true)
		{
			string url = this.ftpServerIP + ftpFilePath;
			if (isCurrenWorkDir)
			{
				url = this.ftpServerIP + CurrentDirectory + ftpFilePath;
			}
			Connect(url);
			reqFTP.Method = WebRequestMethods.Ftp.Rename;
			reqFTP.RenameTo = newFileName;
			using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
			{
				response.Close();
				return true;
			}
		}



		//各方法的当前目录下操作逻辑

	}
}
