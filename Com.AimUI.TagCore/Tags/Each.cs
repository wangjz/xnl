using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Com.AimUI.TagCore.Tags
{
    public class Each<T> : ITag<T> where T : TagContext
    {
        public IEnumerable list;
        public object item;
        public int step;
        public int pos;
        public int i;
        public int count;
        public virtual string subTagNames
        {
            get { return null; }
        }

        public string instanceName { get; set; }

        public string curTag { get; set; }

        public T tagContext { get; set; }

        public virtual void OnInit()
        {
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnTag(OnTagDelegate tagDelegate = null)
        {
            if (tagDelegate == null) return;
            if (list != null)
            {
                i = 0;
                pos = 1;
                if (step <= 0) step = 1;
                foreach (object _item in list)
                {
                    this.item = _item;
                    if (step != 1 && i % step == 0)
                    {
                        tagDelegate();
                    }
                    else
                    {
                        tagDelegate();
                    }
                    i++;
                    pos++;
                }
                count = pos;
            }
        }

        public virtual void OnEnd()
        {
        }

        public virtual void SetAttribute(string paramName, object value)
        {
            switch (paramName)
            {
                case "list":
                    IList _list = value as IList;
                    if (_list == null)
                    {
                        ICollection coll = value as ICollection;
                        if (coll == null)
                        {
                            this.list = value as IEnumerable;
                        }
                        else
                        {
                            count = coll.Count;
                            this.list = coll;
                        }
                    }
                    else
                    {
                        count = _list.Count;
                        this.list = _list;
                    }
                    return;
                case "step":
                    step = Convert.ToInt32(value);
                    return;
            }
        }

        public virtual object GetAttribute(string paramName, object[] userData = null)
        {
            switch (paramName)
            {
                case "list":
                    return list;
                case "i":
                    return i;
                case "pos":
                    return pos;
                case "step":
                    return step;  
                case "count":
                    if (count == 0 && list != null)
                    {
                        foreach (object o in list)
                        {
                            count++;
                        }
                    }
                    return count;
                default:
                    return GetValue(paramName, userData);
            }
        }

        protected virtual object GetValue(string paramName, object[] userData, ComplexQueryDelegate complexQueryFunc = null)
        {
            if (item != null)
            {
                if (paramName == "item" && (userData == null)) return item;
                try
                {
                    string prop = paramName;
                    int inx = 0;
                    if (paramName != "item")
                    {
                        inx = 1;
                    }
                    int len = (userData == null ? 0 : userData.Length)+inx;
                    string[] props = new string[len];
                    if (inx == 1) props[0] = paramName;
                    if (complexQueryFunc == null) complexQueryFunc = Set<T>.ComplexQueryValue;
                    if (userData == null) return complexQueryFunc(item, props);
                    for (int i = 0; i < userData.Length; i++)
                    {
                        paramName = Convert.ToString(userData[i]);
                        if (string.IsNullOrEmpty(paramName)) return null;
                        props[i + inx] = paramName;
                    }
                    return complexQueryFunc(item, props);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public virtual bool ExistAttribute(string paramName)
        {
            return true;
        }

        public virtual ITag<T> Create()
        {
            return new Each<T>();
        }

        public virtual TagEvents events
        {
            get { return TagEvents.Tag; }
        }
    }
}
