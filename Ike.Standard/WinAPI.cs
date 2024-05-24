using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Ike.Standard
{
	/// <summary>
	/// Windows API方法,非托管动态链接库方法调用
	/// </summary>
	public class WinAPI
	{
		/// <summary>
		/// 从 INI 文件中检索指定的键的值
		/// </summary>
		/// <param name="section">要检索的键所在的节名称</param>
		/// <param name="key">要检索的项的名称</param>
		/// <param name="def">如果在文件中找不到指定的键，则返回的默认值</param>
		/// <param name="retVal">用于保存返回的字符串值的缓冲区</param>
		/// <param name="size">缓冲区大小,用于保存返回的字符串</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <remarks>
		///   <list type="bullet">
		///     <item>如果找不到指定的键,则返回默认值<paramref name="def"/></item>
		///     <item>如果找到指定的键,但其值为空字符串,则返回空字符串</item>
		///     <item>如果 INI 文件或指定的节和键不存在,或者发生其他错误,函数将返回空字符串</item>
		///   </list>
		/// </remarks>
		/// <returns>从 INI 文件中检索到的字符串的字节长度</returns>
		[DllImport("kernel32")]
		public static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);


		/// <summary>
		/// 向 INI 文件中写入指定的键和值,如果文件不存在,会创建文件;如果键已经存在,会更新键的值
		/// </summary>
		/// <param name="section">要写入的键所在的节名称</param>
		/// <param name="key">要写入的项的名称</param>
		/// <param name="val">要写入的项的新字符串</param>
		/// <param name="filePath">INI 文件的完整路径</param>
		/// <remarks>
		/// <list type="bullet">
		/// <item>如果指定的 INI 文件不存在,此函数会创建文件</item>
		/// <item>如果指定的键已经存在,此函数会更新键的值</item>
		/// <item>如果 INI 文件中指定的节和键不存在,此函数会创建它们</item>
		/// </list>
		/// </remarks>
		/// <returns>如果函数成功,则返回 <seealso langword="true"/>;否则,返回 <seealso langword="false"/></returns>
		[DllImport("kernel32")]
		public static extern bool WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);

	}
}
