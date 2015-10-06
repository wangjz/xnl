using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
                    break;
                case "step":
                    step = Convert.ToInt32(value);
                    break;
            }
        }

        public object GetAttribute(string paramName, object[] userData = null)
        {
            if (item != null && userData != null && paramName == "item")
            {
                if (item == null) return null;
                try
                {
                    string prop = Convert.ToString(userData[0]);
                    if (string.IsNullOrEmpty(prop)) return null;
                    return item.GetType().GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance).GetValue(item, null);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            switch (paramName)
            {
                case "item":
                    return item;
                case "i":
                    return i;
                case "pos":
                    return pos;
                case "step":
                    return step;
            }
            return null;
        }

        public bool ExistAttribute(string paramName)
        {
            if (paramName == "i" || paramName == "item" || paramName == "pos" || paramName == "step") return true;
            return false;
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
