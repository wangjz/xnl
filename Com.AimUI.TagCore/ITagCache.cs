using System.Collections.Generic;

namespace Com.AimUI.TagCore
{
    public class ITagCache<T> : Dictionary<string, ITag<T>> where T : TagContext
    {
        public ITagCache()
        {
        }
        public new ITag<T> this[string Key]
        {
            get
            {
                ITag<T> ret;
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
                ITag<T> ret;
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
