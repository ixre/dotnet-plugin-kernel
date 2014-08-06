using Ops.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Com.PluginKernel
{
    public abstract class PluginAppBase : IPluginApp
    {
        internal static IDictionary<IPlugin, PluginPackAttribute> plugins;
        private static string pluginDirectory;
        protected static LogFile log;
        private PluginAppAttribute attr = null;


        static PluginAppBase()
        {
            plugins = new Dictionary<IPlugin, PluginPackAttribute>();
            var pluginFilePattern = "*.dll";

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            pluginDirectory = String.Concat(
                appDirectory,
                PluginConfig.PLUGIN_DIRECTORY);


            if (Directory.Exists(pluginDirectory))
            {
                var loadResult = true;

                var files = Directory.GetFiles(pluginDirectory, pluginFilePattern ?? "*.dll");
                log = new LogFile(appDirectory + PluginConfig.PLUGIN_TMP_DIRECTORY + "plugin.log");
                log.Truncate();

                if (log != null)
                {
                    log.Append(String.Format("\r\n\r\n{0:yyyy-MM-dd HH:mm:ss}   [+]Plugin Loading"
                            + "\r\n========================================\r\n"
                            + "Directory:{1} \t Total DLL:{2}",
                            DateTime.Now, pluginDirectory.Replace("\\", "/"),
                            files.Length.ToString()));
                }
                foreach (string file in files)
                {
                    if (!LoadPlugin(file))
                    {
                        loadResult = false;
                    }
                }

                if (log != null)
                {
                    log.Append(String.Format("\r\nload complete!result:{0}", loadResult ? "Ok" : "Error"));
                }
            }
            else
            {
                Directory.CreateDirectory(pluginDirectory).Create();
            }
        }


        public PluginAppBase()
        {
            var ppas = GetType().GetCustomAttributes(typeof(PluginAppAttribute), true);
            if (ppas.Length != 0)
            {
                attr = (PluginAppAttribute)ppas[0];
            }
        }

        /// <summary>
        /// 加载单个插件
        /// </summary>
        /// <param name="pluginFile"></param>
        public static bool LoadPlugin(string pluginFile)
        {
            PluginPackAttribute attribute = null;
            IPlugin obj = null;
            try
            {
                byte[] bytes = File.ReadAllBytes(pluginFile);
                var ass = Assembly.Load(bytes);
                var attbs = ass.GetCustomAttributes(typeof(PluginPackAttribute), true);

                if (attbs.Length != 0)
                {
                    attribute = (PluginPackAttribute)attbs[0];
                }
                else
                {
                    //StringBuilder sb = new StringBuilder();
                    //foreach (object a in ass.GetCustomAttributes(true))
                    //{
                    //    sb.Append("\r\n")
                    //        .Append(a.GetType() == typeof(PluginPackAttribute));
                    //}
                    throw new NotSupportedException("不可识别的插件！请为程序集标记PluginPack特性！");
                }

                var types = ass.GetTypes();
                foreach (Type type in types)
                {
                    if (type.IsClass)
                    {
                        foreach (Type t in type.GetInterfaces())
                        {
                            if (t == typeof(IPlugin))
                            {
                                obj = Activator.CreateInstance(type) as IPlugin;
                                if (obj == null)
                                {
                                    continue;
                                }

                                if (attribute != null)
                                {
                                    plugins.Add(obj, attribute);
                                }
                            }
                        }
                    }
                }
                if (log != null)
                {
                    log.Append(String.Format("\r\n{0}({1}) found plugin. version:{2}",
                        attribute.Name,
                        ass.GetName().Name,
                        attribute.Version));
                }
            }
            catch (Exception err)
            {
                if (log != null)
                {
                    log.Append(String.Format("\r\nAssembly {0} happend exception:{1}",
                        pluginFile.Substring(pluginFile.LastIndexOfAny(new char[] { '/', '\\' }) + 1),
                        err.Message));
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 连接插件
        /// </summary>
        /// <returns></returns>
        public virtual bool Connect()
        {
            Iterate(
                (p, a) =>
                {
                    if (a.State == PluginState.Normal)
                    {
                        p.Connect(this);
                    }
                }
            );
            return true;
        }

        /// <summary>
        /// 迭代插件
        /// </summary>
        /// <param name="handler"></param>
        public void Iterate(PluginHandler handler)
        {
            foreach (IPlugin p in plugins.Keys)
            {
                if (CanAdapter(p))
                {
                    handler(p, plugins[p]);
                }
            }
        }

        public virtual void Run()
        {
            Iterate((p, i) =>
            {
                if (i.State == PluginState.Normal)
                {
                    p.Run();
                }
            });
        }

        public virtual void Pause()
        {
            Iterate((p, i) =>
            {
                i.State = PluginState.Stop;
            });
        }

        public bool Run(string pluginID)
        {
            var runed = false;
            Iterate((p, i) =>
            {
                if (!runed && i.State == PluginState.Normal && String.Compare(pluginID, i.WorkIndent, true) == 0)
                {
                    p.Run();
                    runed = true;
                }
            });
            return runed;
        }

        public bool Pause(string pluginID)
        {
            var runed = false;
            Iterate((p, i) =>
            {
                if (!runed && i.State == PluginState.Normal && String.Compare(pluginID, i.WorkIndent, true) == 0)
                {
                    i.State = PluginState.Stop;
                    runed = true;
                }
            });
            return runed;
        }

        /// <summary>
        /// 检查是否能适配插件
        /// </summary>
        public bool CanAdapter(IPlugin plugin)
        {
            if (attr == null || String.IsNullOrEmpty(attr.TypePattern))
            {
                return true;
            }
            return Regex.IsMatch(plugin.GetType().Name, attr.TypePattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 附加日志
        /// </summary>
        /// <param name="content"></param>
        public void AppendLog(string content)
        {
            if (log != null)
            {
                log.Append(String.Format("\r\n\r\n{0:yyyy-MM-dd HH:mm:ss}\r\n========================================\r\n{1}\r\n", DateTime.Now, content));
            }
        }
    }
}
