using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Test.Framework
{
    internal class Program
    {
        static void Main(string[] args)
        {
			var i = Ike.Standard.FileDir.IniReadToInt("S", "K", 5, @"F:\Desktop\Files\Test.ini", Encoding.UTF8);

			Console.WriteLine("Port 12345 is now open for inbound traffic.");
           Console.ReadKey();
        }
    }
}
