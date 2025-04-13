using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Ike.Standard.Ini;

namespace Test.Framework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;

            // 初始化配置管理器
            IniConfigManager<IniData> configManager = new IniConfigManager<IniData>("F:\\Desktop\\test.ini");
            // 加载配置
            configManager.Load();
            // 修改配置档数据
            configManager.Data.Port = 5555;
            // 保存配置
            configManager.Save();



        }


        public class IniData
        {
            [IniSection("Test")]
            public string IP { get; set; } = "127.0.0.1";

            [IniSection("Test")]
            public int Port { get; set; } = 5200;

            [IniSection("Socket")]
            public bool Open { get; set; } = true;

            [IniSection("Test")]
            public int[] TestIntArr { get; set; } = new int[] { 2, 4, 6, 8, 12, 34, 45, 0, -1 };

            [IniSection("Test")]
            [IniKey("KeyName")]
            public string Temp { get; set; } = "The Key in the file is KeyName";
        }







    }


}
