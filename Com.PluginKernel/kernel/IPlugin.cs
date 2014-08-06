namespace Com.PluginKernel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IPlugin
    {
        PluginConnectionResult Connect(IPluginApp app);


        /// <summary>
        /// 安装
        /// </summary>
        bool Install();

        /// <summary>
        /// 卸载
        /// </summary>
        /// <returns></returns>
        bool Uninstall();

        /// <summary>
        /// 运行
        /// </summary>
        void Run();

        /// <summary>
        /// 暂停运行
        /// </summary>
        void Pause();

        /// <summary>
        /// 返回插件操作的消息
        /// </summary>
        /// <returns></returns>
        string GetMessage();
        object OpenCall(string method, params object[] parameters);
    }
}
