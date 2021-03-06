﻿using System.Collections.Generic;

namespace Com.AimUI.TagCore
{
    /// <summary>
    /// 解析模式  静态 动态
    /// </summary>
    public enum ParseMode : byte
    {
        Static,
        Dynamic,
    }

    /// <summary>
    /// 值返回前处理方式
    /// </summary>
    public enum ValuePreAction : byte
    {
        NONE = 0,
        JSON_Serialize = 58, // :
        JSON_Deserialize = 59, // ;
        USER_Defined = 1
    }

    public delegate object OnValuePreActionDelegate(object value, ValuePreAction actionCode);

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

        public static object OnValuePreAction(object value, ValuePreAction actionCode)
        {
            if (onValuePreActionDelegate != null) return onValuePreActionDelegate(value, actionCode);
            return value;
        }

        protected static OnValuePreActionDelegate onValuePreActionDelegate { get; set; }

        public static void SetValuePreActionDelegate(OnValuePreActionDelegate actionDelegate)
        {
            onValuePreActionDelegate = actionDelegate;
        }
        /// <summary>
        /// 自定义项集合
        /// </summary>
        protected Dictionary<string, object> items { get; set; }

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

        public static string ToString(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }
    }
}
