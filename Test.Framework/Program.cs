using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Test.Framework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string a = "-rwxr-xr-x 1 ftp ftp       69659434 Sep 19 10:30 SignalrClient.exe";
            string b = "drwxr-xr-x 1 ftp ftp              0 Aug 08  2024 ATS Team";
            string c = "-rw-r--r-- 1 ftp ftp       10261525 Mar 26  2024 CD Audio -V6.2.9-1.zip";
            var data1 = GetInfo(a);
            var data2 = GetInfo(b);
            var data3 = GetInfo(c);

        }


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


        private static ResponseInfo? GetInfo(string line)
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






    }
}
