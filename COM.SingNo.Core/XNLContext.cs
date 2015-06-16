using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
namespace COM.SingNo.XNLCore
{
    //解析模式  静态 动态
    public enum ParseMode
    {
        Static,
        Dynamic,
        //Code  //代码模式
    }
    public class XNLContext
    {
       
        public XNLContext(ParseMode parseMode = ParseMode.Static)
        {
            this.parseMode = parseMode;
            response = new XNLResponse();
        }

        public XNLResponse response { get; protected set; }

        public ParseMode parseMode { get; protected set; }

        //工作目录
        public string workDirPath { get; set; }
        /// <summary>
        /// 自定义项集合
        /// </summary>
        protected Dictionary<string, object> items { get;set; }
        
        public static void SetItem(XNLContext xnlContext, string itemName, object itemValue)
        {
            if (xnlContext.items == null) xnlContext.items = new Dictionary<string, object>();
            xnlContext.items[itemName] = itemValue;
        }
        public static bool RemoveItem(XNLContext xnlContext, string itemName)
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
        public static object GetItem(XNLContext xnlContext, string itemName)
        {
            if (xnlContext.items == null) return null;
            object obj;
            xnlContext.items.TryGetValue(itemName, out obj);
            return obj;
        }
        
    }
}
