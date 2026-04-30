using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ike.Standard
{
    /// <summary>
    /// CSV操作类
    /// </summary>
    public class Csv
    {

        /// <summary>
		/// 多个CSV文件加载到二维数组中
		/// </summary>
		/// <param name="files">文件路径数组</param>
		/// <param name="encoding">文件编码</param>
		/// <returns>CSV二维数组</returns>
		public string[,] MultipleCsvFilePathToArray(string[] files, Encoding encoding)
        {
            StringBuilder sb = new StringBuilder(files.Length * 10000);
            if (files.Length > 0)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    sb.Append(File.ReadAllText(files[i]).Trim() + "\r\n");
                }
                return CsvToArray(sb.ToString(), false, encoding);
            }
            return new string[,] { };
        }


        /// <summary>
        /// 加载CSV文件或者逗号分隔符文件/文本到二维数组
        /// </summary>
        /// <param name="filePathAndStr">内容</param>
        /// <param name="encoding">文件编码</param>
        /// <param name="stringIsPath">参数是否为路径</param>
        /// <returns></returns>
        public static string[,] CsvToArray(string filePathAndStr, bool stringIsPath, Encoding encoding)
        {
            return CsvToArray(filePathAndStr, 0, stringIsPath, encoding);
        }


        /// <summary>
        /// 加载逗号分隔符的CSV文件/文本到二维数组
        /// </summary>
        /// <param name="filePathAndStr">内容</param>
        /// <param name="maxLength">用于行长度不规格处使用,此处填写最长一行长度即可</param>
        /// <param name="stringIsPath">参数是否为路径</param>
        /// <param name="encoding">文件编码</param>
        /// <returns></returns>
        public static string[,] CsvToArray(string filePathAndStr, byte maxLength, bool stringIsPath, Encoding encoding)
        {
            string str = filePathAndStr;
            if (stringIsPath)
            {
                str = File.ReadAllText(filePathAndStr, encoding);
            }
            if (str.Contains(","))
            {
                char replace = Text.SpecialCharacters[4];
                string[] temp = str.Replace("\r\n", replace.ToString()).Replace("\n", replace.ToString()).Split(replace);
                if (temp.Length > 0)
                {
                    if (maxLength == 0)
                    {
                        string[] line = temp[0].Split(',');
                        maxLength = (byte)line.Length;
                    }
                    string[,] ret = new string[temp.Length, maxLength];
                    for (int i = 0; i < temp.Length; i++)
                    {
                        string[] line = temp[i].Split(',');
                        byte length = (byte)line.Length;
                        for (int j = 0; j < maxLength; j++)
                        {
                            if (j < length)
                            {
                                ret[i, j] = line[j];
                            }
                            else
                            {
                                ret[i, j] = string.Empty;
                            }
                        }
                    }
                    return ret;
                }
            }
            return new string[,] { };
        }



    }
}
