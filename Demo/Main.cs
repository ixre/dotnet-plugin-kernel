using Com.PluginKernel;
using Ops.Cms;
using Ops.Cms.Core.Plugins;
/*
 * 由SharpDevelop创建。
 * 用户： newmin
 * 日期: 2013/11/19
 * 时间: 21:31
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace com.mapfre.poi
{
	/// <summary>
	/// Description of Main.
	/// </summary>
	public class Main:IPlugin
	{
		
		public PluginConnectionResult Connect(IPluginApp app)
		{
			ExtendsPlugin _app=app as ExtendsPlugin;
			if(_app!=null)
			{
				RequestProxry req=new RequestProxry(_app);
				_app.Register(this,req.HandleGet,req.HandlePost);
                Cms.Plugins.MapExtendPluginRoute(this);
			}
			
			return PluginConnectionResult.Success;
		}
		
		public bool Install()
		{
			return true;
		}
		
		public bool Uninstall()
		{
			return true;
		}
		
		public void Run()
		{
			
		}
		
		public void Pause()
		{
		}
		
		public string GetMessage()
		{
			return "";
		}
		
		public object OpenCall(string method, params object[] parameters)
		{
			throw new NotImplementedException();
		}
	}
}
