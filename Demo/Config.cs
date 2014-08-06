/*
 * 由SharpDevelop创建。
 * 用户： newmin
 * 日期: 2013/10/29
 * 时间: 22:35
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using Com.PluginKernel;
using Ops.Framework;

namespace com.mapfre.poi
{
	/// <summary>
	/// Description of Config.
	/// </summary>
	public class Config
	{
		/// <summary>
		/// 是否为开发模式
		/// </summary>
		public static bool DebugMode=!true;
		
        internal static PluginPackAttribute PluginAttrs;
		
		static Config()
		{
            PluginAttrs = PluginUtil.GetAttribute<Main>();
			bool isChanged=false;
			
			
            //if(!PackAttr.Settings.Contains("notify.workerindent"))
            //{
            //    PackAttr.Settings.Append("notify.workerindent","");
            //    isChanged=true;
            //}
			
            //if(!PackAttr.Settings.Contains("alipay.account"))
            //{
            //    PackAttr.Settings.Append("alipay.account","");
            //    isChanged=true;
            //}
			
            //if(!PackAttr.Settings.Contains("alipay.userkey"))
            //{
            //    PackAttr.Settings.Append("alipay.userkey","");
            //    isChanged=true;
            //}
			
            //if(!PackAttr.Settings.Contains("alipay.secret"))
            //{
            //    PackAttr.Settings.Append("alipay.secret","");
            //    isChanged=true;
            //}

            if (isChanged) PluginAttrs.Settings.Flush();
				
		}
	}
}
