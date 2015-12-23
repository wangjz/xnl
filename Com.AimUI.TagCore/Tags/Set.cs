using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Com.AimUI.TagCore.Tags
{
    public delegate object ComplexQueryDelegate(object obj, string[] props);
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

        protected string GetBody()
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
                case "get":
                case "gets":
                    if (userData == null) return sets;
                    if (sets == null) return null;
                    if (userData.Length == 1 && paramName=="get")
                    {
                        string n = Convert.ToString(userData[0]);
                        if (string.IsNullOrEmpty(n)) return n;
                        return GetValue(n);
                    }
                    else
                    {
                        return GetValues(userData);
                    }
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

        object GetValue(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return GetValue(name, null);
        }
        IDictionary<string, object> GetValues(object[] names)
        {
            if (names == null || names.Length == 0) return null;
            if (sets == null) return null;
            string name;
            IDictionary<string, object> colls = new Dictionary<string, object>(names.Length, StringComparer.OrdinalIgnoreCase);
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

        protected virtual object GetValue(string paramName, object[] userData,ComplexQueryDelegate complexQueryFunc=null)
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
                        string[] props = new string[userData.Length - inx];
                        for (int i = inx; i < userData.Length; i++)
                        {
                            paramName = Convert.ToString(userData[i]);
                            if (string.IsNullOrEmpty(paramName)) return null;
                            props[i - inx] = paramName;
                        }
                        if (complexQueryFunc == null) complexQueryFunc = ComplexQueryValue;
                        return complexQueryFunc(obj, props);
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

        public static object ComplexQueryValue(object obj, string[] props)
        {
            string prop;
            if (obj is string)
            {
                prop = obj as string;
                char firstChar = prop[0];
                if (firstChar == '{' || firstChar == '[')
                {
                    try
                    {
                        obj = TagContext.OnValuePreAction(obj, ValuePreAction.JSON_Deserialize);
                    }
                    catch
                    {
                        obj = prop;
                    }
                    if (obj == null) obj = prop;
                }
            }
            if (props.Length == 1) return GetValue(obj, props[0]);
            for (int i = 0; i < props.Length; i++)
            {
                prop = props[i];
                if (string.IsNullOrEmpty(prop)) return null;
                obj = GetValue(obj, prop);
                if (obj == null) return null;
            }
            return obj;
        }

        public static object GetValue(object obj, string prop)
        {
            if (obj == null) return null;
            if (string.IsNullOrEmpty(prop)) return null;
            try
            {
                int i;
                if (prop.EndsWith("]"))
                {
                    int inx = prop.IndexOf('[');
                    if (inx == -1) return null;
                    string nextProp=null;
                    if (inx == 0)
                    {
                        inx = prop.IndexOf(']');
                        prop = prop.Substring(1, inx-1);
                        if (inx < prop.Length - 1)
                        {
                            //多个索引
                            nextProp = prop.Substring(inx + 1);
                        }
                    }
                    else
                    {
                        //多个索引
                        nextProp = prop.Substring(inx);
                        prop = prop.Substring(0, inx);
                    }
                    if (prop.IndexOf(',') != -1)
                    {
                        string[] props = prop.Split(',');
                        object[] objs=new object[props.Length];
                        for (i = 0; i < props.Length; i++)
                        {
                            objs[i] = GetValue(obj, props[i]);
                        }
                        obj = objs;
                    }
                    if (nextProp != null)
                    {
                        obj = GetValue(obj, prop);
                        if (obj == null) return null;
                        obj = GetValue(obj, nextProp);
                        return obj;
                    }
                }
                if (int.TryParse(prop, out i))
                {
                    if (i < 0) return null;
                    string str = obj as string;
                    if (str != null)
                    {
                        if (i < str.Length)
                        {
                            return str[i];
                        }
                        return null;
                    }
                    IEnumerable itor = obj as IEnumerable;
                    if (itor != null)
                    {
                        IList arr = obj as IList;
                        if (arr != null)
                        {
                            if (i < arr.Count)
                            {
                                return arr[i];
                            }
                            return null;
                        }
                        int j = 0;
                        ICollection coll = obj as ICollection;
                        if (coll != null)
                        {
                            if (i < coll.Count)
                            {
                                
                                foreach (object o in coll)
                                {
                                    if (i == j) return o;
                                    j++;
                                }
                                return null;
                            }
                            return null;
                        }
                        j = 0;
                        foreach (object o in itor)
                        {
                            if (i == j) return o;
                            j++;
                        }
                        return null;
                    }
                }

                IDictionary<string, object> colls = obj as IDictionary<string, object>;
                if (colls != null)
                {
                    object outObj;
                    if (colls.TryGetValue(prop, out outObj))
                    {
                        return outObj;
                    }
                    foreach (KeyValuePair<string, object> kv in colls)
                    {
                        if (string.Compare(kv.Key, prop, true) == 0)
                        {
                            return kv.Value;
                        }
                    }
                    return null;
                }
                PropertyInfo propertyInfo=obj.GetType().GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.GetProperty|BindingFlags.GetField | BindingFlags.Instance);
                if (propertyInfo != null) return propertyInfo.GetValue(obj, null);
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
