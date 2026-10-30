using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using System.Net.Http;
using Ike.ActionEngine;
using System.Diagnostics;
using System.Configuration;
using System.Threading.Tasks;
using Ike.ActionEngine.Model;
using System.Collections.Generic;

namespace Ike.ActionEngine
{
    /// <summary>
    /// 动态编译执行的 Action 实现类
    /// </summary>
    public class DynamicTestScript : UpdateAction
    {
        /// <summary>
        /// 统一通过基类接口触发执行(程序入口点)
        /// </summary>
        /// <param name="context">上下文传递</param>
        public override void Execute(UpdateContext context)
        {
            try
            {
                GlobalVariable.Log.Debug("Start Dynamic C#");
                // 此处编写需要执行的代码

                GlobalVariable.Log.Success("Dynamic C# execution is completed");
            }
            catch (Exception ex)
            { 
              GlobalVariable.Log.Exception(ex);
            }
        }
    }
}
