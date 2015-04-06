using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using COM.SingNo.XNLCore.Labels;
namespace COM.SingNo.XNLCore
{
    public class XNLLib<T> where T:XNLContext
    {
        private static Assembly libAssembly;
        public string nameSpace { get; private set; }
        public bool isCache { get; private set; }
        public string[] delayRunTags { get; private set; }
        public Dictionary<string, string> extExpTags { get; private set; }
        private XNLCache<T> tagCache;
        public XNLLib(string _nameSpace, bool _isCache, string[] _delayRunTags, Dictionary<string, string> _extExpTags)
        {
            nameSpace = _nameSpace;
            isCache = _isCache;
            delayRunTags = _delayRunTags;
            extExpTags = _extExpTags;
            if (_isCache)
            {
                tagCache = new XNLCache<T>();
                if (_nameSpace=="xnl")
                {
                    tagCache.Add("if", new If<T>());
                    tagCache.Add("set", new Set<T>());
                    tagCache.Add("for", new For<T>());
                    //tagCache.Add("forin", new Forin<T>());
                }
            }
        }
        private static IXNLTag<T> getTagInstanceFromAssembly(string nameSpace, string tagName)
        {
            if (libAssembly == null)
            {
                libAssembly = Assembly.Load("COM.SingNo.XNLLib." + nameSpace);
            }
            Type tagType = libAssembly.GetType("COM.SingNo.XNLLib." + nameSpace + "." + tagName, false, true);//得到类型
            if (tagType == null)
            {
                return null;
            }
            return  (IXNLTag<T>)Activator.CreateInstance(tagType);
        }
        public IXNLTag<T> getTagInstance(string tagName)
        {
            IXNLTag<T> obj;
            if (isCache)
            {
                obj = tagCache[tagName];
                if (obj == null)
                {
                    obj = getTagInstanceFromAssembly(nameSpace, tagName);
                    if (obj != null) tagCache[tagName] = obj;
                }
            }
            else
            {
                obj = getTagInstanceFromAssembly(nameSpace, tagName);
            }
            return obj;
        }
       
        private static List<string> delayRunTagList = new List<string>();
        private static string tagNameSpacesStr = "";
        private static Dictionary<string, XNLLib<T>> tagLibColls = new Dictionary<string, XNLLib<T>>();
        private static Dictionary<string, string[]> extExpColls = new Dictionary<string, string[]>();  //
        private static void addDelayRunTag(string tagFullName)
        {
            tagFullName = tagFullName.Trim().ToLower();
            if (delayRunTagList.Contains(tagFullName) == false) delayRunTagList.Add(tagFullName);
        }
        private static void removeDelayRunTag(string tagFullName)
        {
            if (delayRunTagList.Count == 0) return;
            tagFullName = tagFullName.Trim().ToLower();
            if (delayRunTagList.Contains(tagFullName)) delayRunTagList.Remove(tagFullName);
        }
        public static bool checkTagIsDelayRun(string tagFullName)
        {
            return delayRunTagList.Contains(tagFullName);
        }
        private static void updateTagNameSpacesStr()
        {
            string str = "";
            foreach (KeyValuePair<string, XNLLib<T>> kv in tagLibColls)
            {
                str += "|" + kv.Key;
            }
            tagNameSpacesStr = str;
        }
        /// <summary>
        /// 得到所有命名空间名称,以"|"分割
        /// </summary>
        /// <returns></returns>
        public static string getRegTagNameSpaces()
        {
            return tagNameSpacesStr;
        }
        /// <summary>
        /// 注册标签命名空间
        /// </summary>
        /// <param name="tagNameSpace">标签命名空间</param>
        /// <param name="isCache">是否缓存此空间下标签对象</param>
        public static void registerTagLib(XNLLib<T> tagLib)
        {
            string tagNameSpace = tagLib.nameSpace.ToLower();
            if (tagLibColls.ContainsKey(tagNameSpace))
            {
                tagLibColls[tagNameSpace] = tagLib;
            }
            else
            {
                tagLibColls.Add(tagNameSpace, tagLib);
            }
            if (tagLib.delayRunTags != null && tagLib.delayRunTags.Length > 0)
            {
                for(int i=0;i<tagLib.delayRunTags.Length;i++)
                {
                    addDelayRunTag(tagLib.nameSpace + ":" + tagLib.delayRunTags[i]);
                } 
            }
            if (tagLib.extExpTags != null)
            {
                foreach (KeyValuePair<string, string> kv in tagLib.extExpTags)
                {
                    addExtExp(kv.Key, kv.Value);
                }
            }
            updateTagNameSpacesStr();
        }
        /// <summary>
        /// 取消注册标签命名空间
        /// </summary>
        /// <param name="tagNameSpace"></param>
        public static void unRegisterTagNameSpace(string tagNameSpace)
        {
            tagNameSpace = tagNameSpace.ToLower();
            if (tagLibColls.ContainsKey(tagNameSpace))
            {
                XNLLib<T> tagLib = tagLibColls[tagNameSpace];
                if (tagLib.delayRunTags != null && tagLib.delayRunTags.Length > 0)
                {
                    for (int i = 0; i < tagLib.delayRunTags.Length; i++)
                    {
                        removeDelayRunTag(tagLib.nameSpace + ":" + tagLib.delayRunTags[i]);
                    }
                }
                if (tagLib.extExpTags != null)
                {
                    foreach (KeyValuePair<string, string> kv in tagLib.extExpTags)
                    {
                        removeExtExp(kv.Key);
                    }
                }
                tagLibColls.Remove(tagNameSpace);
            }
            updateTagNameSpacesStr();
        }

        private static void addExtExp(string extExpName, string executeTagFullName)
        {
            executeTagFullName = executeTagFullName.Trim().ToLower();
            extExpName = extExpName.Trim().ToLower().Replace(".", "\\.");
            if (extExpColls.ContainsKey(extExpName) == false)
            {
                string[] tag_arr = executeTagFullName.Split(':');
                extExpColls.Add(extExpName, new string[] { tag_arr[0], tag_arr[1], executeTagFullName });
            }
        }
        private static void removeExtExp(string extExpName)
        {
            if (extExpColls.Count == 0) return;
            extExpName = extExpName.Trim().ToLower();
            foreach (KeyValuePair<string, string[]> kv in extExpColls)
            {
                if (Regex.IsMatch(extExpName, kv.Key))
                {
                    string[] strs = kv.Key.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.Length <= 1)
                    {
                        extExpColls.Remove(extExpName);
                        return;
                    }
                    string str = "";
                    for (int i = 0; i < strs.Length; i++)
                    {
                        if (strs[i] != extExpName)
                        {
                            str += (string.IsNullOrEmpty(str) ? "" : "|") + strs[i];
                        }
                    }
                    extExpColls[str] = kv.Value;
                    break;
                }
            }
        }
        public static string[] getExtExpTagInfo(string extExpName)
        {
            if (extExpColls.Count == 0) return null;
            foreach (KeyValuePair<string, string[]> kv in extExpColls)
            {
                if (Regex.IsMatch(extExpName, "^(?:" + kv.Key + ")\\.\\w+"))
                {
                    return kv.Value;
                }
            }
            return null;
        }
        public static void initialize(List<XNLLib<T>> xnlLibs)
        {
            if (xnlLibs != null)
            {
                foreach (XNLLib<T> lib in xnlLibs)
                {
                    registerTagLib(lib);
                }
            }
        }
        public static XNLLib<T> getTagLib(string tagNameSpace)
        {
            XNLLib<T> tagLib;
            tagLibColls.TryGetValue(tagNameSpace, out tagLib);
            return tagLib;
        }

        public static IXNLTag<T> getTagInstance(string nameSpace, string tagName)
        {
            //nameSpace = nameSpace.ToLower(); //标签库名
            //tagName = tagName.ToLower();  //标签名
            if (string.Compare(nameSpace,"xnl.mytag")!=0)
            {
                XNLLib<T> tagLib = XNLLib<T>.getTagLib(nameSpace);
                if (tagLib == null) return null;
                IXNLTag<T> obj =tagLib.getTagInstance(tagName); //()
                if (obj == null) return null;
                return obj;
            }
            return null;
        }
    }
}
