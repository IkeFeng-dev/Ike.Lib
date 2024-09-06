using NetFwTypeLib;
using System.Text;

namespace Test
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var i = Ike.Standard.FileDir.IniReadToInt("S", "K", 5, @"F:\Desktop\Files\Test.ini", Encoding.UTF8);

		}

	}
}
