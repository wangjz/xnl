using System;
using System.Collections.Generic;
using COM.SingNo.XNLCore;
namespace COM.SingNo.XNLCore
{
    public class XNLTagCache : Dictionary<string, Dictionary<string, IXNLTag<XNLContext>>>
    {
        private XNLTagCache() {

            //this["xnl", "if"] = new If();
            //this["xnl", "for"] = new For();
            //this["xnl", "set"] = new Set();
            //this["xnl", "foreach"] = new Foreach();
        }
        private static class Nested
        {
            internal static XNLTagCache instance = new XNLTagCache();
        }
        public static XNLTagCache getInstance()
        {
            return Nested.instance;
        }
        public IXNLTag<XNLContext> this[string XNLKey, string Key]
        {
            get
            {
                Dictionary<string, IXNLTag<XNLContext>> ret;
                if (this.TryGetValue(XNLKey, out ret)) //查找标签缓存
                {
                    IXNLTag<XNLContext> ret2;
                    if (ret.TryGetValue(Key, out ret2)) //查找标签类缓存
                        return ret2;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Dictionary<string, IXNLTag<XNLContext>> ret;
                if (this.TryGetValue(XNLKey, out ret))
                {
                    IXNLTag<XNLContext> ret2;
                    if (ret.TryGetValue(Key, out ret2))
                    {
                        ret[Key] = value;
                        ret2 = null;
                    }
                    else
                    {
                        ret.Add(Key, value);
                    }
                }
                else
                {
                    Dictionary<string, IXNLTag<XNLContext>> Buffer = new Dictionary<string, IXNLTag<XNLContext>>();
                   Buffer.Add(Key, value);
                   this.Add(XNLKey, new Dictionary<string, IXNLTag<XNLContext>>(Buffer));
                }
            }
                
        }

        public  void RemoveXNL(string XNLKey)
        {
            this.Remove(XNLKey);
        }

        public  void RemoveXNLObj(string XNLKey, string key)
        {
            Dictionary<string, IXNLTag<XNLContext>> ret;
            if (this.TryGetValue(XNLKey, out ret))
            {
                ret.Remove(key);
            }
        }
    }
}
