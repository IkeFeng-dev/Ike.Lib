#if NETFRAMEWORK
using System.Windows.Forms;
using System.Collections.Specialized;
using System.IO;

namespace Ike.Standard
{
	public static partial class FileDir
	{

		/// <summary>
		/// 加载本地文件到剪切板中
		/// </summary>
		/// <param name="targetFile">文件路径</param>
		/// <returns></returns>
		public static bool LoadFileToClipboard(string targetFile)
		{
			if (File.Exists(targetFile))
			{
				StringCollection paths = new StringCollection
					{
						targetFile
					};
				Clipboard.SetFileDropList(paths);
				return true;
			}
			return false;
		}




		/// <summary>
		/// 加载多个本地文件到剪切板中
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public static bool LoadFileToClipboard(string[] filePath)
		{
			Clipboard.Clear();
			StringCollection paths = new StringCollection();
			for (int i = 0; i < filePath.Length; i++)
			{
				if (File.Exists(filePath[i]))
				{
					paths.Add(filePath[i]);
				}
			}
			if (paths.Count > 0)
			{
				Clipboard.SetFileDropList(paths);
				return true;
			}
			else
			{
				return false;
			}
		}



		/// <summary>
		/// 选择文件夹
		/// </summary>
		/// <param name="description">标题</param>
		/// <returns></returns>
		public static string SelectFolder(string description = "请选择目录")
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog
			{
				Description = description
			};
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				return dialog.SelectedPath + "\\";
			}
			else
			{
				return string.Empty;
			}
		}



		/// <summary>
		/// 选择文件
		/// </summary>
		/// <param name="title">标题</param>
		/// <param name="filter">类型</param>
		/// <param name="multiselect">该值确定是否可以选择多个文件</param>
		/// <returns></returns>
		public static string SelectFile(string title = "请选择文件", string filter = "所有文件(*.*)|*.*", bool multiselect = true)
		{
			OpenFileDialog dialog = new OpenFileDialog
			{
				//该值确定是否可以选择多个文件
				Multiselect = multiselect,
				Title = title,
				Filter = filter
			};
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				return dialog.FileName;
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
#endif