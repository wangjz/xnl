using System;
using System.Collections.Generic;
using System.Text;

namespace COM.SingNo.XNLCore
{
    public class XNLCache<T> : Dictionary<string, IXNLTag<T>> where T:XNLContext
    {
        public  XNLCache()
        {
        }
        public new IXNLTag<T> this[string Key]
        {
            get
            {
                IXNLTag<T> ret;
                if (this.TryGetValue(Key, out ret)) //查找标签缓存
                {
                    return ret;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                IXNLTag<T> ret;
                if (this.TryGetValue(Key, out ret))
                {
                    this[Key] = value;
                }
                else
                {
                    this.Add(Key, value);
                }
            }

        }

        public void RemoveXNLTag(string XNLKey)
        {
            this.Remove(XNLKey);
        }
    }
}
