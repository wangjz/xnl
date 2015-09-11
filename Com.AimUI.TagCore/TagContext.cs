using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Com.AimUI.TagCore
{
    /// <summary>
    /// 解析模式  静态 动态
    /// </summary>
    public enum ParseMode
    {
        Static,
        Dynamic,
    }

    /// <summary>
    /// 值返回前处理方式
    /// </summary>
    public enum ValuePreAction
    {
        NONE,
        JSON_Serialize, // :
        JSON_Deserialize,
        XML_Serialize,
        USER_Defined
    }

    public delegate object OnValuePreActionDelegate(object value, byte actionCode, byte charCode);

    public class TagContext
    {
       
        public TagContext()
        {
            response = new TagResponse();
        }

        protected TagResponse response { get; set; }

        public virtual TagResponse GetTagResponse()
        {
            return response;
        }

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

        public static object OnValuePreAction( object value, byte actionCode, byte charCode = 0)
        {
            if (onValuePreActionDelegate != null) return onValuePreActionDelegate(value, actionCode, charCode);
            return value;
        }

        //工作目录
        protected string workDirPath { get; set; }

        protected static OnValuePreActionDelegate onValuePreActionDelegate {  get;  set; }

        public static void SetValuePreActionDelegate(OnValuePreActionDelegate actionDelegate)
        {
            onValuePreActionDelegate = actionDelegate;
        }
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
