﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Com.AimUI.TagCore.Tags
{
    public class Each<T> : ITag<T> where T : TagContext
    {
        public IEnumerable<object> list;
        public object item;
        public int step;
        public int pos;
        public int i;
        public string subTagNames
        {
            get { return null; }
        }

        public string instanceName { get; set; }

        public string curTag { get; set; }

        public T tagContext { get; set; }

        public void OnInit()
        {
        }

        public void OnStart()
        {
        }

        public void OnTag(OnTagDelegate tagDelegate = null)
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
            }
        }

        public void OnEnd()
        {
        }

        public void SetAttribute(string paramName, object value)
        {
            switch (paramName)
            {
                case "list":
                    list = value as IEnumerable<object>;
                    return;
                case "step":
                    step = Convert.ToInt32(value);
                    return;
            }
        }

        public object GetAttribute(string paramName, object[] userData = null)
        {
            switch (paramName)
            {
                case "item":
                    if (userData == null) return item;
                    break;
                case "list":
                    return list;
                case "i":
                    return i;
                case "pos":
                    return pos;
                case "step":
                    return step;           
            }
            if (item != null)
            {
                if (paramName == "item" && (userData == null)) return null;
                try
                {
                    string prop = (paramName == "item" ? Convert.ToString(userData[0]) : paramName);
                    if (string.IsNullOrEmpty(prop)) return null;
                    IDictionary<string, object> colls = item as IDictionary<string, object>;
                    if (colls != null) return colls[prop];
                    return item.GetType().GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty).GetValue(item, null);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public bool ExistAttribute(string paramName)
        {
            return true;
        }

        public ITag<T> Create()
        {
            return new Each<T>();
        }

        public TagEvents events
        {
            get { return TagEvents.Tag; }
        }
    }
}
