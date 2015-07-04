using System;
using System.Collections.Generic;
using System.Text;

namespace Com.AimUI.TagCore.Tags
{
    public class Inc<T> : ITag<T> where T : TagContext
    {
        Dictionary<string, object> attrs;
        public string src;
        public string subTagNames
        {
            get { return null; }
        }

        public string instanceName
        {
            get;
            set;
        }

        public string curTag
        {
            get;
            set;
        }

        public T tagContext
        {
            get;
            set;
        }

        public void OnInit()
        {
            
        }

        public void OnStart()
        {
            
        }

        public void OnTag(OnTagDelegate tagDelegate = null)
        {
            if (tagDelegate != null) tagDelegate();
        }

        public void OnEnd()
        {
        }

        public void SetAttribute(string paramName, object value)
        {
            if (attrs==null) attrs = new Dictionary<string, object>();
            attrs[paramName] = value;
        }

        public object GetAttribute(string paramName, object[] userData = null)
        {
            if (paramName == "src") return src;
            object obj;
            attrs.TryGetValue(paramName, out obj);
            return obj;
        }

        public bool ExistAttribute(string paramName)
        {
            return true;
        }

        public ITag<T> Create()
        {
            return new Inc<T>();
        }
    }
}
