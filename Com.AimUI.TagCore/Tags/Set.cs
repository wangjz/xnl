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
        protected object[] tagArgs;
        protected byte execute = 3; //1 run  2 output
        protected bool innerTag = false;
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
            innerTag = true;
            body = null;
            _tagDelegate = tagDelegate;
            if (execute > 0)
            {
                GetBody();
            }
            innerTag = false;
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
                if ((execute & 2) != 2) buffer.Remove(inx, len);
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
                case "execute":
                    byte.TryParse(Convert.ToString(value), out execute);
                    return;
                case "body":
                    body = value as string;
                    return;
                case "call":
                    return;
                case "sets":
                    sets = value as IDictionary<string, object>;
                    return;
                default:
                    if (paramName.StartsWith("arg"))
                    {
                        if (paramName == "args" && value is Array)
                        {
                            tagArgs = value as object[];
                        }
                        else if (tagArgs != null)
                        {
                            int i = -1;
                            if (int.TryParse(paramName.Substring(3), out i) && i > 0 && (i - 1) < tagArgs.Length)
                            {
                                tagArgs[i] = value;
                                return;
                            }
                        }
                    }
                    sets[paramName] = value;
                    return;
            }
        }

        public virtual object GetAttribute(string paramName, object[] args = null)
        {
            switch (paramName)
            {
                case "execute":
                    return execute;
                case "sets":
                    if (args == null) return null;
                    SetValues(args);
                    return null;
                case "get":
                case "gets":
                    if (args == null) return sets;
                    if (sets == null) return null;
                    if (args.Length == 1 && paramName == "get")
                    {
                        string n = Convert.ToString(args[0]);
                        if (string.IsNullOrEmpty(n)) return n;
                        return GetValue(n);
                    }
                    else
                    {
                        return GetValues(args);
                    }
                case "set":
                    if (args != null && args.Length == 2)
                    {
                        string key = Convert.ToString(args[0]);
                        if (string.IsNullOrEmpty(key) == false) sets[key] = args[1];
                    }
                    return null;
                case "body":
                    if (innerTag) return null;
                    return GetBody();
                case "call":
                    tagArgs = args;
                    body = null;
                    return GetBody();
                case "clear":
                    sets.Clear();
                    return null;
                case "keys":
                    return sets.Keys;
                case "values":
                    return sets.Values;
                case "count":
                    return sets.Count;
                case "mix": //混合另一个集合
                    return null;
                case "remove":
                    if (args == null) return null;
                    if (args.Length == 1)
                    {
                        string s = Convert.ToString(args[0]);
                        if (string.IsNullOrEmpty(s) == false)
                        {
                            return sets.Remove(s);
                        }
                        return false;
                    }
                    else
                    {
                        string s = null;
                        foreach (object o in args)
                        {
                            s = Convert.ToString(o);
                            if (string.IsNullOrEmpty(s) == false)
                            {
                                sets.Remove(s);
                            }
                        }
                    }
                    return null;
                default:
                    return GetValue(paramName, args);
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

        protected virtual object GetValue(string paramName, object[] args)
        {
            object obj = null;
            if (paramName.StartsWith("arg"))
            {
                int i = -1;
                if (paramName == "args")
                {
                    if (args == null)
                    {
                        obj = tagArgs;
                    }
                    else if (tagArgs != null && int.TryParse(args[0].ToString(), out i) && i > 0 && (i - 1) < tagArgs.Length)
                    {
                        obj = tagArgs[i];
                    }
                }
                else if (int.TryParse(paramName.Substring(3), out i) && tagArgs != null && i > 0)
                {
                    i = i - 1;
                    if (i < tagArgs.Length) obj = tagArgs[i];
                }
            }
            else
            {
                sets.TryGetValue(paramName, out obj);
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
    }
}
