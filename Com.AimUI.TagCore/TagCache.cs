using System;
using System.Collections.Generic;

namespace Com.AimUI.TagCore
{
    public class TagCache<T> where T : TagContext
    {
        private readonly object syncRoot = new object();
        private readonly Dictionary<string, ITag<T>> innerD = new Dictionary<string, ITag<T>>(StringComparer.OrdinalIgnoreCase);
        public TagCache()
        {
        }
        public ITag<T> this[string Key]
        {
            get
            {
                try
                {
                    return innerD[Key];
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    innerD[Key] = value;
                }
            }

        }
        public void Add(string key, ITag<T> value)
        {
            lock (syncRoot)
            {
                innerD.Add(key, value);
            }
        }

        public bool Remove(string key)
        {
            lock (syncRoot)
            {
                return innerD.Remove(key);
            }
        }

        public bool Remove(KeyValuePair<string, ITag<T>> item)
        {
            lock (syncRoot)
            {
                return ((ICollection<KeyValuePair<string, ITag<T>>>)innerD).Remove(item);
            }
        }
    }
}
