

using System;
using System.Web;

namespace com.mapfre.poi
{
    /// <summary>
    /// 验证码管理器
    /// </summary>
    internal static class VerifyCodeManager
    {
        /// <summary>
        /// 添加词语
        /// </summary>
        /// <param name="word"></param>
        public static void AddWord(string word)
        {
            if (word != null)
            {
                HttpContext.Current.Session["$manager.login.verifycode"] = word;
            }
        }

        /// <summary>
        /// 比较验证码
        /// </summary>
        /// <param name="inputWord"></param>
        /// <returns></returns>
        public static bool Compare(string inputWord)
        {
            var sess = HttpContext.Current.Session["$manager.login.verifycode"];
            if (sess != null)
            {
                return String.Compare(inputWord, sess.ToString(), true) == 0;
            }
            return true;
        }
    }
}
