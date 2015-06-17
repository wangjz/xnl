using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using Com.AimUI.TagCore.Tags;

namespace Com.AimUI.TagCore
{
    public class TagLib<T> where T:TagContext
    {
        public static IParser<T> tagParser { get; private set; }

        private  Assembly libAssembly;
        private string low_nameSpace;

        public string nameSpace { get; private set; }
        
        public bool isCache { get; private set; }

        private static ITagCache<T> tagCache;

        public TagLib(string _nameSpace, bool _isCache) 
        {
            nameSpace = _nameSpace;
            low_nameSpace = _nameSpace.ToLower();
            isCache = _isCache;
        }

        private  ITag<T> GetTagInstanceFromAssembly(string nameSpace, string tagName)
        {
            if (libAssembly == null)
            {
                libAssembly = Assembly.Load("Com.AimUI.Tag." + nameSpace);
            }

            Type tagType = libAssembly.GetType("Com.AimUI.Tag." + nameSpace + "." + tagName, false, true);//得到类型

            if (tagType == null)
            {
                return null;
            }
            return  (ITag<T>)Activator.CreateInstance(tagType);
        }

        public ITag<T> GetTagInstance(string tagName)
        {
            ITag<T> obj;
            if (isCache)
            {
                string _n = low_nameSpace + ":" + tagName;
                obj = tagCache[_n];
                if (obj == null)
                {
                    obj = GetTagInstanceFromAssembly(nameSpace, tagName);
                    if (obj != null) tagCache[_n] = obj;
                }
            }
            else
            {
                obj = GetTagInstanceFromAssembly(nameSpace, tagName);
            }
            return obj;
        }
       
        private static string tagNameSpacesStr = "at";
        private static Dictionary<string, TagLib<T>> tagLibColls = new Dictionary<string, TagLib<T>>();
        
        private static void UpdateTagNameSpacesStr()
        {
            string str = "at";
            foreach (KeyValuePair<string, TagLib<T>> kv in tagLibColls)
            {
                if (("|" + str + "|").IndexOf("|" + kv.Key + "|") == -1)
                {
                    str += "|" + kv.Key;
                }
            }
            tagNameSpacesStr = str;
            if (tagParser != null) tagParser.Initialize();
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
        public static void RegisterTagLib(TagLib<T> tagLib)
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
                TagLib<T> tagLib = tagLibColls[tagNameSpace];
          
                tagLibColls.Remove(tagNameSpace);
            }
            UpdateTagNameSpacesStr();
        }

       
        public static void Initialize(IParser<T> parser,List<TagLib<T>> tagLibs = null)
        {
            tagParser = parser;
            if (tagCache == null) tagCache = new ITagCache<T>();
            tagCache.Add("at:if", new If<T>());
            tagCache.Add("at:set", new Set<T>());
            tagCache.Add("at:for", new For<T>());
            tagCache.Add("at:expression", new Expression<T>());
            if (tagLibs != null)
            {
                foreach (TagLib<T> lib in tagLibs)
                {
                    RegisterTagLib(lib);
                }
            }
            else if (tagParser != null)
            {
                tagParser.Initialize();
            }
        }
        public static TagLib<T> GetTagLib(string tagNameSpace)
        {
            TagLib<T> tagLib;
            tagLibColls.TryGetValue(tagNameSpace, out tagLib);
            return tagLib;
        }

        public static ITag<T> GetTagInstance(string nameSpace, string tagName)
        {
            ITag<T> obj = tagCache[nameSpace + ":" + tagName];
            if(obj==null)
            {
                TagLib<T> tagLib = TagLib<T>.GetTagLib(nameSpace);
                if (tagLib == null) return null;
                obj = tagLib.GetTagInstance(tagName);
            }
            return obj;
        }

        //扩展标签
        public bool SetTagExtend(string tagName, ITag<T> destTag)
        {
            if (destTag==null)
            {
                return false;
            }
            tagCache[low_nameSpace + ":" + tagName] = destTag;
            return true;
        }
        
    }
}
