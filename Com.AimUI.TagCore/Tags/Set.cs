using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Com.AimUI.TagCore.Tags
{
    public class Set<T> : ITag<T> where T : TagContext
    {
        Dictionary<string, object> attrs;
        string body;
        private StringBuilder buffer;
        OnTagDelegate _tagDelegate;
        object[] args;
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
            
        }

        public void OnEnd()
        {
            
        }


        public void OnTag(OnTagDelegate tagDelegate = null)
        {
            body = null;
            _tagDelegate = tagDelegate;
        }

        private string GetBody()
        {
            if (_tagDelegate == null) return null;
            if (body != null) return body;
            int inx = buffer.Length;
            _tagDelegate();
            int len = buffer.Length - inx;
            if (len > 0)
            {
                body = buffer.ToString(inx, len);
                buffer.Remove(inx, len);
            }
            else
            {
                body = string.Empty;
            }
            return body;
        }

        public void SetAttribute(string paramName, object value)
        {
            attrs[paramName] = value;
        }

        public object GetAttribute(string paramName, object[] userData = null)
        {
            if (paramName == "body")
            {
                return GetBody();
            }
            else if (paramName == "call")
            {
                args = userData;
                body = null;
                return GetBody();
            }
            else if (paramName.StartsWith("arg") && args!=null)
            {
                int i = -1;
                if (int.TryParse(paramName.Substring(3), out i))
                {
                    i = i - 1;
                    if(i < args.Length)return args[i];
                }
            }

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
                }
            }

            return obj;
        }


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

        public TagEvents events
        {
            get { return TagEvents.Init | TagEvents.Tag; }
        }
    }
}
