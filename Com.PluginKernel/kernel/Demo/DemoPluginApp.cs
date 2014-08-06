using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.PluginKernel
{
    public class DemoPluginApp : PluginAppBase
    {
        public event PluginHandler<string> OnPrinting;

        public override bool Connect()
        {
            Iterate((p, i) =>
            {
                Console.WriteLine("连接插件：" + i.Name + " 作者：" + i.Author + " 版本:" + i.Version);
                p.Connect(this);
            });

            return true;
        }

        public void Print(string message)
        {
            var result = true;
            if (OnPrinting != null)
            {
                OnPrinting(message, ref result);
            }
        }
    }
}
