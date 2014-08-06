using Ops.Cms;
using Ops.Cms.Core.Plugins;
using Ops.Cms.Utility;
/*
 * 由SharpDevelop创建。
 * 用户： newmin
 * 日期: 2013/11/19
 * 时间: 21:35
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System.Web;

namespace com.mapfre.poi
{
    /// <summary>
    /// Description of HandleRequest.
    /// </summary>
    public class RequestProxry
    {
        private ExtendsPlugin app;
        private static RequestHandle handler = new RequestHandle();
        public RequestProxry(ExtendsPlugin app)
        {
            this.app = app;
        }

        public static bool VerifyLogin(HttpContext context)
        {
            bool result = UserState.Administrator.HasLogin;
            if (!result)
            {
                context.Response.Write("<script>window.parent.location.replace('/admin')</script>");
            }
            return result;
        }
        public void HandleGet(HttpContext context, ref bool handled)
        {
            if (Cms.Plugins.HanleRequestUse<RequestHandle>(handler, context, false))
            {
                handled = true;
            }
        }

        public void HandlePost(HttpContext context, ref bool handled)
        {
            if (Cms.Plugins.HanleRequestUse<RequestHandle>(handler, context, true))
            {
                handled = true;
            }
        }

    }
}
