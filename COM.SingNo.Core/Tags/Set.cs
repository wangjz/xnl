using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore.Tags
{
    public class Set<T> : IXNLTag<T> where T : XNLContext
    {
        Dictionary<string, object> attrs;
        string body;
        public T xnlContext
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
        }

        public void OnEnd()
        {

        }

        //子标签解析
        public void OnTag(OnTagDelegate tagDelegate = null)
        {
            if(tagDelegate!=null)
            {
                StringBuilder buffer = xnlContext.response.buffer;
                int inx = buffer.Length;
                tagDelegate();
                int len = buffer.Length - inx;
                body = buffer.ToString(inx, len);
                buffer.Remove(inx, len);
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
        public IXNLTag<T> Create()
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
