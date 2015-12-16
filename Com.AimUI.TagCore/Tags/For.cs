using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;

namespace Com.AimUI.TagCore.Tags
{
    public class For<T> : ITag<T> where T : TagContext
    {
        public int start { get; set; }

        public int end { get; set; }

        public int i { get; set; }

        public string split { get; set; }

        public string str { get; set; }

        private bool isChange = false;

        public int step = 1;

        private int pos = 0;

        private IList list;

        private object item;

        //private bool _break = false;
        public T tagContext
        {
            get;
            set;
        }

        public string instanceName { get; set; }
        public string curTag { get; set; }

        public For()
        {
            start = 0;
            end = -1;
            split = ",";
        }
        public virtual void OnInit()
        {

        }
        public virtual void OnStart()
        {
            i = 0;
            if (isChange)
            {
                isChange = false;
                if (str != null)
                {
                    list = str.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries);
                }
                if (list == null) return;
                if (end == -1)
                {
                    end = list.Count - 1;
                }
                else if (end >= list.Count) end = list.Count - 1;
            }
        }

        public virtual void OnEnd()
        {

        }

        public virtual void OnTag(OnTagDelegate tagDelegate = null)
        {
            if (step == 0) return;

            if (tagDelegate != null)
            {
                if (start > end)
                {
                    if (step > 0) step = -step;
                }
                else
                {
                    if (step < 0) step = -step;
                }
                pos = 1;
                for (i = start; i <= end; i += step)
                {
                    if (list != null)
                    {
                        item = list[i];
                    }
                    tagDelegate();
                    pos += 1;
                }
            }
        }

        public virtual void SetAttribute(string paramName, object value)
        {
            switch (paramName)
            {
                case "start":
                    start = Convert.ToInt32(value);
                    return;
                case "end":
                    end = Convert.ToInt32(value);
                    return;
                case "list":
                    if (value is string)
                    {
                        string v = Convert.ToString(value);
                        if (v != str)
                        {
                            str = v;
                            isChange = true;
                        }
                        list = null;
                    }
                    else
                    {
                        str = null;
                        if (value != list)
                        {
                            list = value as IList;
                            if (list == null)
                            {
                                ICollection coll = value as ICollection;
                                if (coll != null)
                                {
                                    object[] objs = new object[coll.Count];
                                    coll.CopyTo(objs, 0);
                                    list = objs;
                                }
                            }
                            isChange = true;
                        }
                    }
                    return;
                case "split":
                    string v2 = Convert.ToString(value);
                    if (v2 != split)
                    {
                        split = v2;
                        isChange = true;
                    }
                    return;
                case "step":
                    step = Convert.ToInt32(value);
                    return;
            }
        }

        public virtual object GetAttribute(string paramName, object[] userData = null) //, bool byRef = false
        {
            switch (paramName)
            {
                case "i":
                    return i;
                case "pos":
                    return pos;
                case "start":
                    return start;
                case "end":
                    return end;
                case "count":
                    if (list != null) return list.Count;
                    return 0;
                case "list":
                    return list;
                case "split":
                    return split;
                case "step":
                    return step;
                default:
                    return GetValue(paramName, userData);
            }
        }

        protected virtual object GetValue(string paramName, object[] userData, GetValueDelegate getValueFunc = null)
        {
            if (item != null)
            {
                if (paramName == "item" && (userData == null)) return item;
                try
                {
                    string prop = paramName;
                    int inx = 0;
                    if (paramName == "item")
                    {
                        prop = Convert.ToString(userData[0]);
                        if (string.IsNullOrEmpty(prop)) return null;
                        inx = 1;
                    }
                    if (getValueFunc == null) getValueFunc = Set<T>.GetValue;
                    object obj = getValueFunc(item, prop);
                    if (userData == null || inx >= userData.Length) return obj;
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
            return null;
        }

        //创建 
        public virtual ITag<T> Create()
        {
            return new For<T>();
        }
        public virtual bool ExistAttribute(string paramName)
        {
            return true;
        }

        public virtual string subTagNames
        {
            get { return null; }
        }

        public virtual TagEvents events
        {
            get { return TagEvents.Start | TagEvents.Tag; }
        }
    }
}