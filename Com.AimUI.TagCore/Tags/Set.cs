﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Com.AimUI.TagCore.Tags
{
    public class Set<T> : ITag<T> where T : TagContext
    {
        Dictionary<string, object> attrs;
        string body;
        int inx = -1;
        private StringBuilder buffer;
        public T tagContext
        {
            get;
            set;
        }

        public string instanceName { get; set; }

        public string curTag { get; set; }

        public void OnInit()
        {
            attrs = new Dictionary<string, object>();
            buffer = tagContext.GetTagResponse().buffer;
        }

        public void OnStart()
        {
            inx = buffer.Length;
        }

        public void OnEnd()
        {
            if(inx!=-1)
            {
                int len = buffer.Length - inx;
                if (len > 0)
                {
                    body = buffer.ToString(inx, len);
                    buffer.Remove(inx, len);
                    inx = -1;
                }
            }
        }

        //子标签解析
        public void OnTag(OnTagDelegate tagDelegate = null)
        {
            if(tagDelegate!=null)
            {
                tagDelegate();
            }
        }
        public void SetAttribute(string paramName, object value)
        {
            attrs[paramName] = value;
        }

        public object GetAttribute(string paramName, object[] userData = null) //, bool byRef = false
        {
            if (paramName == "body") return body;
            object obj;
            attrs.TryGetValue(paramName, out obj);
            if (obj != null && userData != null)
            {
                try
                {
                    string prop = Convert.ToString(userData[0]);
                    if (string.IsNullOrEmpty(prop)) return null;
                    return obj.GetType().GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance).GetValue(obj, null);
                }
                catch (Exception)
                {
                    return null;
                    //throw;
                }
            }
            
            return obj;
        }

        //创建 
        public ITag<T> Create()
        {
            return new Set<T>();
        }

        public bool ExistAttribute(string paramName)
        {
            return true;
        }

        public string subTagNames
        {
            get
            {
                return null;
            }
        }
    }
}
