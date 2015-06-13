using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using COM.SingNo.XNLCore.Tags;
namespace COM.SingNo.XNLCore
{
    public class XNLLib<T> where T:XNLContext
    {
        private static Assembly libAssembly;
        public string nameSpace { get; private set; }
        public bool isCache { get; private set; }
        //public Dictionary<string, string> extExpTags { get; private set; }
        private XNLCache<T> tagCache;
        public XNLLib(string _nameSpace, bool _isCache)  //, Dictionary<string, string> _extExpTags
        {
            nameSpace = _nameSpace;
            isCache = _isCache;
            //extExpTags = _extExpTags;
            if (_isCache)
            {
                tagCache = new XNLCache<T>();
                if (_nameSpace=="xnl")
                {
                    tagCache.Add("if", new If<T>());
                    tagCache.Add("set", new Set<T>());
                    tagCache.Add("for", new For<T>());
                    tagCache.Add("expression", new Expression<T>());
                }
            }
        }
        private static IXNLTag<T> GetTagInstanceFromAssembly(string nameSpace, string tagName)
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
        public IXNLTag<T> GetTagInstance(string tagName)
        {
            IXNLTag<T> obj;
            if (isCache)
            {
                obj = tagCache[tagName];
                if (obj == null)
                {
                    obj = GetTagInstanceFromAssembly(nameSpace, tagName);
                    if (obj != null) tagCache[tagName] = obj;
                }
            }
            else
            {
                obj = GetTagInstanceFromAssembly(nameSpace, tagName);
            }
            return obj;
        }
       
        //private static List<string> delayRunTagList = new List<string>();
        private static string tagNameSpacesStr = "";
        private static Dictionary<string, XNLLib<T>> tagLibColls = new Dictionary<string, XNLLib<T>>();
        private static Dictionary<string, string[]> extExpColls = new Dictionary<string, string[]>();  //
        /*
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
         */ 
        private static void UpdateTagNameSpacesStr()
        {
            string str = "";
            foreach (KeyValuePair<string, XNLLib<T>> kv in tagLibColls)
            {
                str += (str??"|") + kv.Key;
            }
            tagNameSpacesStr = str;
        }
        /// <summary>
        /// 得到所有命名空间名称,以"|"分割
        /// </summary>
        /// <returns></returns>
        public static string GetRegTagNameSpaces()
        {
            return tagNameSpacesStr;
        }
        /// <summary>
        /// 注册标签命名空间
        /// </summary>
        /// <param name="tagNameSpace">标签命名空间</param>
        /// <param name="isCache">是否缓存此空间下标签对象</param>
        public static void RegisterTagLib(XNLLib<T> tagLib)
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
            /*
            if (tagLib.extExpTags != null)
            {
                foreach (KeyValuePair<string, string> kv in tagLib.extExpTags)
                {
                    addExtExp(kv.Key, kv.Value);
                }
            }
            */ 
            UpdateTagNameSpacesStr();
        }
        /// <summary>
        /// 取消注册标签命名空间
        /// </summary>
        /// <param name="tagNameSpace"></param>
        public static void UnRegisterTagNameSpace(string tagNameSpace)
        {
            tagNameSpace = tagNameSpace.ToLower();
            if (tagLibColls.ContainsKey(tagNameSpace))
            {
                XNLLib<T> tagLib = tagLibColls[tagNameSpace];
                /*
                if (tagLib.extExpTags != null)
                {
                    foreach (KeyValuePair<string, string> kv in tagLib.extExpTags)
                    {
                        removeExtExp(kv.Key);
                    }
                }
                 */ 
                tagLibColls.Remove(tagNameSpace);
            }
            UpdateTagNameSpacesStr();
        }

        /*
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
        */
        public static void Initialize(List<XNLLib<T>> xnlLibs)
        {
            if (xnlLibs != null)
            {
                foreach (XNLLib<T> lib in xnlLibs)
                {
                    RegisterTagLib(lib);
                }
            }
        }
        public static XNLLib<T> GetTagLib(string tagNameSpace)
        {
            XNLLib<T> tagLib;
            tagLibColls.TryGetValue(tagNameSpace, out tagLib);
            return tagLib;
        }

        public static IXNLTag<T> GetTagInstance(string nameSpace, string tagName)
        {
            //nameSpace = nameSpace.ToLower(); //标签库名
            //tagName = tagName.ToLower();  //标签名
            if (string.Compare(nameSpace,"xnl.mytag")!=0)
            {
                XNLLib<T> tagLib = XNLLib<T>.GetTagLib(nameSpace);
                if (tagLib == null) return null;
                IXNLTag<T> obj =tagLib.GetTagInstance(tagName);
                if (obj == null) return null;
                return obj;
            }
            return null;
        }

        //重载标签  原标签全名  子类标签全名
        //SetOverride
    }
}
