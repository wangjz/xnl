using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
namespace COM.SingNo.XNLCore
{
    //解析模式  静态 动态
    public enum ParseMode
    {
        Static=0,
        Dynamic=1,
    }
    public class XNLContext
    {
        public XNLContext()
        {
            //parseMode = ParseMode.Static;
            response = new XNLResponse();
        }

        public XNLResponse response { get; protected set; }
        //public ParseMode parseMode{get;set;}
        //工作目录
        public string workDirPath { get; set; }
        /// <summary>
        /// 自定义项集合
        /// </summary>
        protected Dictionary<string, object> items { get;set; }
        /// <summary>
        /// 自定义标签集合
        /// </summary>
        //public Dictionary<string, string> customTagColls{get;set;}
        /// <summary>
        /// 上下文全局属性集合
        /// </summary>
        public Dictionary<string, string> globalAttriableColls { get; set; }
        /*
        public static void setCustomTag(XNLContext xnlContext, string tagName, string tagStr)
        {
            if (xnlContext.customTagColls == null) xnlContext.customTagColls = new Dictionary<string, string>();
            xnlContext.customTagColls[tagName] = tagStr;
        }
        public static bool removeCustomTag(XNLContext xnlContext, string tagName)
        {
            if (xnlContext.customTagColls == null) return true;
            try
            {
                xnlContext.customTagColls.Remove(tagName);
                return true;
            }
            catch{ }
            return false;
        }
        public static string getCustomTag(XNLContext xnlContext, string tagName)
        {
            if (xnlContext.customTagColls == null) return null;
            string tagStr;
            xnlContext.customTagColls.TryGetValue(tagName, out tagStr);
            return tagStr;
        }
        */
        public static void setItem(XNLContext xnlContext, string itemName, object itemValue)
        {
            if (xnlContext.items == null) xnlContext.items = new Dictionary<string, object>();
            xnlContext.items[itemName] = itemValue;
        }
        public static bool removeItem(XNLContext xnlContext, string itemName)
        {
            if (xnlContext.items == null) return true;
            try
            {
                xnlContext.items.Remove(itemName);
                return true;
            }
            catch { }
            return false;
        }
        public static object getItem(XNLContext xnlContext, string itemName)
        {
            if (xnlContext.items == null) return null;
            object obj;
            xnlContext.items.TryGetValue(itemName, out obj);
            return obj;
        }
        /*
        /// <summary>
        /// 添加当前页属性
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void setGlobalAttriable(XNLContext xnlContext, string name, string value)
        {
            if (xnlContext.globalAttriableColls == null) xnlContext.globalAttriableColls = new Dictionary<string, string>();
            xnlContext.globalAttriableColls[name] = value;
        }
        /// <summary>
        /// 得到当前页属性
        /// </summary>
        /// <param name="xnlContext"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static string getGlobalAttriable(XNLContext xnlContext, string tagName)
        {
            if (xnlContext.globalAttriableColls == null) return null;
            string tagStr;
            xnlContext.globalAttriableColls.TryGetValue(tagName, out tagStr);
            return tagStr;
        }
       */ 
    }
}
