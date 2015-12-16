using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Com.AimUI.TagCore.Tags
{
    public delegate object GetValueDelegate(object obj, string prop);
    public class Set<T> : ITag<T> where T : TagContext
    {
        protected IDictionary<string, object> sets;
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
            sets = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
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
                case "colls":
                case "attrs":
                case "sets":
                    sets = value as IDictionary<string, object>;
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
                    sets[paramName] = value;
                    return;
            }
        }

        public virtual object GetAttribute(string paramName, object[] userData = null)
        {
            switch (paramName)
            {
                case "sets":
                    if (userData == null) return null;
                    SetValues(userData);
                    return null;
                case "gets":
                    if (userData == null) return sets;
                    if (sets == null) return null;
                    return GetValues(userData);
                case "set":
                    if (userData != null && userData.Length == 2)
                    {
                        string key = Convert.ToString(userData[0]);
                        if (string.IsNullOrEmpty(key) == false) sets[key] = userData[1];
                    }
                    return null;
                case "body":
                    return GetBody();
                case "call":
                    args = userData;
                    body = null;
                    return GetBody();
                case "colls":
                case "attrs":
                    return sets;
                default:
                    return GetValue(paramName, userData);
            }
        }

        void SetValues(object[] objs)
        {
            if (objs == null) return;
            foreach (object obj in objs)
            {
                SetValue(obj);
            }
        }

        void SetValue(object obj)
        {
            IDictionary<string, object> colls = obj as IDictionary<string, object>;
            if (colls == null) return;
            if (sets == null)
            {
                sets = colls;
            }
            else if (colls.Count > 0)
            {
                foreach (var pair in colls) sets.Add(pair);
            }
        }

        IDictionary<string,object> GetValues(params object[] names)
        {
            if (names == null || names.Length == 0) return null;
            if (sets == null) return null;
            IDictionary<string, object> colls = new Dictionary<string, object>(names.Length, StringComparer.OrdinalIgnoreCase);
            string name;
            foreach (object o in names)
            {
                name = Convert.ToString(o);
                if (string.IsNullOrEmpty(name) == false)
                {
                    colls.Add(name, GetValue(name, null));
                }
            }
            return colls;
        }

        protected virtual object GetValue(string paramName, object[] userData, GetValueDelegate getValueFunc = null)
        {
            object obj = null;
            int inx = 0;
            if (paramName.StartsWith("arg"))
            {
                int i = -1;
                if (paramName == "args")
                {
                    if (userData == null)
                    {
                        obj = args;
                    }
                    else if (args != null && int.TryParse(userData[0].ToString(), out i) && i > 0 && (i - 1) < args.Length)
                    {
                        obj = args[i];
                        inx = 1;
                    }
                }
                else if (int.TryParse(paramName.Substring(3), out i) && args != null && i > 0)
                {
                    i = i - 1;
                    if (i < args.Length) obj = args[i];
                }
            }
            else
            {
                sets.TryGetValue(paramName, out obj);
            }
            if (userData == null) return obj;
            if (obj != null)
            {
                if (userData.Length > inx)
                {
                    try
                    {
                        if (getValueFunc == null) getValueFunc = GetValue;
                        for (; inx < userData.Length; inx++)
                        {
                            paramName = Convert.ToString(userData[inx]);
                            if (string.IsNullOrEmpty(paramName)) return null;
                            obj = getValueFunc(obj, paramName);
                            if (obj == null) return null;
                        }
                        return obj;
                    }
                    catch
                    {
                        return null;
                    }
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

        public static object GetValue(object obj, string prop)
        {
            if (obj == null) return null;
            if (string.IsNullOrEmpty(prop)) return null;
            try
            {
                IDictionary<string, object> colls = obj as IDictionary<string, object>;
                if (colls != null)
                {
                    if (colls.TryGetValue(prop, out obj))
                    {
                        return obj;
                    }
                    foreach (KeyValuePair<string, object> kv in colls)
                    {
                        if (string.Compare(kv.Key, prop, true) == 0)
                        {
                            return kv.Value;
                        }
                    }
                }
                int i;
                if (int.TryParse(prop, out i))
                {
                    if (i < 0) return null;
                    IList arr = obj as IList;
                    if (arr != null)
                    {
                        if (i < arr.Count)
                        {
                            return arr[i];
                        }
                        return null;
                    }
                    ICollection coll = obj as ICollection;
                    if (coll != null)
                    {
                        if (i < coll.Count)
                        {
                            int j = 0;
                            foreach (object o in coll)
                            {
                                if (i == j) return o;
                                j++;
                            }
                            return null;
                        }
                        return null;
                    }
                    IEnumerable itor = obj as IEnumerable;
                    if (itor != null)
                    {
                        int j = 0;
                        foreach (object o in itor)
                        {
                            if (i == j) return o;
                            j++;
                        }
                        return null;
                    }
                    return null;
                }
                return obj.GetType().GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance).GetValue(obj, null);
            }
            catch
            {
                return null;
            }
        }
    }
}
