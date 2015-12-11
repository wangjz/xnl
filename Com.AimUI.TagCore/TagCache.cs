using System;
using System.Collections.Generic;

namespace Com.AimUI.TagCore
{
    public class TagCache<T> : Dictionary<string, ITag<T>> where T : TagContext
    {
        public TagCache()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }
        public new ITag<T> this[string Key]
        {
            get
            {
                ITag<T> ret;
                if (base.TryGetValue(Key, out ret)) //查找标签缓存
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
                ITag<T> ret;
                if (base.TryGetValue(Key, out ret))
                {
                    base[Key] = value;
                }
                else
                {
                    base.Add(Key, value);
                }
            }

        }

        public void RemoveXNLTag(string XNLKey)
        {
            this.Remove(XNLKey);
        }
    }
}
