﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Com.AimUI.TagCore.Tags
{
    public class Set<T> : ITag<T> where T : TagContext
    {
        Dictionary<string, object> attrs;
        string body;
        int inx = -1;
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
        }

        public void OnStart()
        {
            inx = tagContext.response.buffer.Length;
        }

        public void OnEnd()
        {
            if(inx!=-1)
            {
                StringBuilder buffer = tagContext.response.buffer;
                int len = buffer.Length - inx;
                body = buffer.ToString(inx, len);
                buffer.Remove(inx, len);
                inx = -1;
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

        public object GetAttribute(string paramName, object userData = null)
        {
            if (paramName == "body") return body;
            object obj;
            TryGetAttribute(out obj, paramName);
            return obj;
        }
        public bool TryGetAttribute(out object outValue, string paramName, object userData = null)
        {
            if (paramName == "body")
            {
                outValue = body;
                return true;
            }
            return attrs.TryGetValue(paramName, out outValue);
        }

        //创建 
        public ITag<T> Create()
        {
            return new Set<T>();
        }

        public bool ExistAttribute(string paramName)
        {
            if (paramName == "body") return true;
            return attrs.ContainsKey(paramName);
        }

        public string subTagNames
        {
            get
            {
                return null;
            }
        }

        public ParseMode parseMode
        {
            get;
            set;
        }
    }
}
