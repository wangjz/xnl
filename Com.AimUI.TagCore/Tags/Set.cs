using System;
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

        public object GetAttribute(string paramName, object[] userData = null) //, bool byRef = false
        {
            if (paramName == "body") return body;
            object obj;
            attrs.TryGetValue(paramName, out obj);
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
            //if (paramName == "body") return true;
            //return attrs.ContainsKey(paramName);
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
