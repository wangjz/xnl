using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Com.AimUI.TagCore.Tags
{
    public class Set<T> : ITag<T> where T : TagContext
    {
        protected IDictionary<string, object> attrs;
        protected string body;
        protected StringBuilder buffer;
        protected OnTagDelegate _tagDelegate;
        protected object[] args;
        public T tagContext
        {
            get;
            set;
        }

        public string instanceName { get; set; }

        public string curTag { get; set; }

        public virtual void OnInit()
        {
            attrs = new Dictionary<string, object>();
            buffer = tagContext.GetTagResponse().buffer;
        }

        public virtual void OnStart()
        {

        }

        public virtual void OnEnd()
        {

        }


        public virtual void OnTag(OnTagDelegate tagDelegate = null)
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

        public virtual void SetAttribute(string paramName, object value)
        {
            switch (paramName)
            {
                case "body":
                    body = value as string;
                    return;
                case "call":
                    return;
                case "attrs":
                    attrs = value as IDictionary<string, object>;
                    return;
                default:
                    if (paramName.StartsWith("arg"))
                    {
                        if (paramName == "args" && value is Array)
                        {
                            args = value as object[];
                        }
                        else if (args != null)
                        {
                            int i = -1;
                            if (int.TryParse(paramName.Substring(3), out i) && i > 0 && (i - 1) < args.Length)
                            {
                                args[i] = value;
                                return;
                            }
                        }
                    }
                    attrs[paramName] = value;
                    return;
            }
        }

        public virtual object GetAttribute(string paramName, object[] userData = null)
        {
            switch (paramName)
            {
                case "body":
                    return GetBody();
                case "call":
                    args = userData;
                    body = null;
                    return GetBody();
                case "set":
                    if (userData != null && userData.Length == 2)
                    {
                        string key = Convert.ToString(userData[0]);
                        if (string.IsNullOrEmpty(key) == false) attrs[key.ToLower()] = userData[1];
                    }
                    return null;
                case "attrs":
                    return attrs;
            }
            if (paramName.StartsWith("arg"))
            {
                int i = -1;
                if (paramName == "args")
                {
                    if (userData == null)
                    {
                        return args;
                    }
                    else if (args != null && int.TryParse(userData[0].ToString(), out i) && i > 0 && (i - 1) < args.Length)
                    {
                        return args[i];
                    }
                    return null;
                }

                if (int.TryParse(paramName.Substring(3), out i) && args != null && i > 0)
                {
                    i = i - 1;
                    if (i < args.Length) return args[i];
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
                    IDictionary<string, object> colls = obj as IDictionary<string, object>;
                    if (colls != null) return colls[prop];
                    return obj.GetType().GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance).GetValue(obj, null);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return obj;
        }


        public virtual ITag<T> Create()
        {
            return new Set<T>();
        }

        public virtual bool ExistAttribute(string paramName)
        {
            return true;
        }

        public virtual string subTagNames
        {
            get
            {
                return null;
            }
        }

        public virtual TagEvents events
        {
            get { return TagEvents.Init | TagEvents.Tag; }
        }
    }
}
