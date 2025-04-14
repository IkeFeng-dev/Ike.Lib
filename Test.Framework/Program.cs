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


            Ike.Standard.WinMethod.EnableAnsiSupport();
            var log = new Ike.Standard.LOG(@"F:\Desktop\Logs\");

            string info = "This is test console  ->  ";
            for (int i = 0; i < 30; i++)
            {
                if (i < 4)
                {
                    log.Debug(info + i);
                }
                else if (i < 8)
                {
                    log.Info(info + i);
                }
                else if (i < 12)
                {
                    log.Warning(info + i);
                }
                else if (i < 16)
                {
                    log.Error(info + i);
                }
                else if (i < 20)
                {
                    log.Fatal(info + i);
                }
                else if (i < 25)
                {
                    log.Success(info + i);
                }
                else
                {
                    log.Verbose(info + i);
                }
            }

            Console.ReadKey();
        }


      





    }


}
