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
        public static IXNLParser<T> xnlParser { get; private set; }

        private static Assembly libAssembly;
        public string nameSpace { get; private set; }
        public bool isCache { get; private set; }

        private XNLCache<T> tagCache;
        public XNLLib(string _nameSpace, bool _isCache) 
        {
            nameSpace = _nameSpace;
            isCache = _isCache;
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
       
        private static string tagNameSpacesStr = "";
        private static Dictionary<string, XNLLib<T>> tagLibColls = new Dictionary<string, XNLLib<T>>();
        private static Dictionary<string, string[]> extExpColls = new Dictionary<string, string[]>();  //
        
        private static void UpdateTagNameSpacesStr()
        {
            string str = "";
            foreach (KeyValuePair<string, XNLLib<T>> kv in tagLibColls)
            {
                str += (str??"|") + kv.Key;
            }
            tagNameSpacesStr = str;
            if (xnlParser != null) xnlParser.Initialize();
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
          
                tagLibColls.Remove(tagNameSpace);
            }
            UpdateTagNameSpacesStr();
        }

       
        public static void Initialize(IXNLParser<T> parser,List<XNLLib<T>> xnlLibs)
        {
            xnlParser = parser;
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
