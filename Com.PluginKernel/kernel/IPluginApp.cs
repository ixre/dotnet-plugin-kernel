namespace Com.PluginKernel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 插件应用接口
    /// </summary>
    public interface IPluginApp
    {
        /// <summary>
        /// 连接所有插件
        /// </summary>
        /// <returns></returns>
        bool Connect();

        /// <summary>
        /// 迭代插件集合
        /// </summary>
        /// <param name="handler"></param>
        void Iterate(PluginHandler handler);

        /// <summary>
        /// 运行所有插件
        /// </summary>
        void Run();

        /// <summary>
        /// 停用所有插件
        /// </summary>
        void Pause();

        /// <summary>
        /// 运行指定的插件
        /// </summary>
        /// <param name="pluginID"></param>
        /// <returns></returns>
        bool Run(string pluginID);

        /// <summary>
        /// 停用指定的插件
        /// </summary>
        /// <param name="pluginID"></param>
        /// <returns></returns>
        bool Pause(string pluginID);
    }
}
