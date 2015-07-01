using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Com.AimUI.TagCore
{
    //解析模式  静态 动态
    public enum ParseMode
    {
        Static,
        Dynamic,
        //Code  //代码模式
    }
    public class TagContext
    {
       
        public TagContext(/*ParseMode parseMode = ParseMode.Static*/)
        {
            //this.parseMode = parseMode;
            response = new TagResponse();
        }

        public TagResponse response { get; protected set; }

        public virtual string GetInclude(string src, ref string tagNamespace,ref string tagName, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(src)) return null;
            if(src.IndexOf(":\\")!=-1)
            {
                if (encoding == null) encoding = Encoding.UTF8;
                return System.IO.File.ReadAllText(src, encoding);
            }
            if(workDirPath!=null)
            {
                if (encoding == null) encoding = Encoding.UTF8;
                return System.IO.File.ReadAllText(workDirPath+src, encoding);
            }
            return null;
        }
        //public ParseMode parseMode { get; protected set; }

        //工作目录
        public string workDirPath { get; set; }
        /// <summary>
        /// 自定义项集合
        /// </summary>
        protected Dictionary<string, object> items { get;set; }
        
        public static void SetItem(TagContext tagContext, string itemName, object itemValue)
        {
            if (tagContext.items == null) tagContext.items = new Dictionary<string, object>();
            tagContext.items[itemName] = itemValue;
        }
        public static bool RemoveItem(TagContext tagContext, string itemName)
        {
            if (tagContext.items == null) return true;
            try
            {
                tagContext.items.Remove(itemName);
                return true;
            }
            catch { }
            return false;
        }
        public static object GetItem(TagContext tagContext, string itemName)
        {
            if (tagContext.items == null) return null;
            object obj;
            tagContext.items.TryGetValue(itemName, out obj);
            return obj;
        }
        
    }
}
