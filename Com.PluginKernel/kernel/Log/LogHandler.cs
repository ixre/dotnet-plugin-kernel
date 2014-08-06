using Ops.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Com.PluginKernel.Log
{
    internal static class LogHandler
    {
       // private static string logDirectory = "";

        
        /// <summary>
        /// 记录异常
        /// </summary>
        /// <param name="addr">引起异常的地址，比如网址</param>
        /// <param name="except"></param>
        public static void RecordException(string addr,Exception except)
        {
            if (PluginConfig.PLUGIN_LOG_OPENED)
            {
                Exception exc = except;
                DateTime dt = DateTime.Now;
                string dtStr = String.Format("{0:yyyyMMdd}", dt);

                string logDir = AppDomain.CurrentDomain.BaseDirectory + PluginConfig.PLUGIN_TMP_DIRECTORY + "logs";
                //创建日志目录
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir).Create();
                }

                LogFile logFile = new LogFile(logDir + "/" + dtStr + ".log");

                if (exc.InnerException != null)
                {
                    exc = except.InnerException;
                }

                Hashtable hash = new Hashtable();
                hash.Add("addr", addr ?? "application");
                hash.Add("message", exc.Message);
                hash.Add("stack", exc.StackTrace);
                hash.Add("time", String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt));
                hash.Add("source", exc.Source);

                //附加记录
                logFile.Append(PluginConfig.PLUGIN_LOG_EXCEPT_FORMAT.Template(hash));
            }
            throw except;        //继续抛出异常
        }
    }
}
