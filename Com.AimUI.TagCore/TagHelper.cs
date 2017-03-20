using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Com.AimUI.TagCore
{
    public static class TagHelper
    {
        public static object ComplexQuery(object obj, string queryPath)
        {
            if (queryPath == null || queryPath.Length == 0) return null;
            string prop;
            string[] props = queryPath.Split('.');
            if (props.Length == 1) return TryQueryValue(obj, props[0]);
            for (int i = 0; i < props.Length; i++)
            {
                prop = props[i];
                if (string.IsNullOrEmpty(prop)) return null;
                obj = TryQueryValue(obj, prop);
                if (obj == null) return null;
            }
            return obj;
        }

        public static object TryQueryValue(object obj, string propertyName)
        {
            if (obj == null) return null;
            if (string.IsNullOrEmpty(propertyName)) return null;
            try
            {
                if (propertyName.EndsWith("]"))
                {
                    while (true)
                    {
                        int inx = propertyName.IndexOf('[');
                        if (inx == -1) return null;
                        string nextProp = null;
                        if (inx == 0)
                        {
                            var oldPropName = propertyName;
                            inx = propertyName.IndexOf(']');
                            propertyName = propertyName.Substring(1, inx - 1);
                            if (inx < oldPropName.Length - 1)
                            {
                                //多个索引
                                nextProp = oldPropName.Substring(inx + 1);
                            }
                        }
                        else
                        {
                            //多个索引
                            nextProp = propertyName.Substring(inx);
                            propertyName = propertyName.Substring(0, inx);
                        }
                        if (propertyName.IndexOf(',') != -1)
                        {
                            string[] props = propertyName.Split(',');
                            object[] objs = new object[props.Length];
                            for (int i = 0; i < props.Length; i++)
                            {
                                objs[i] = GetValue(obj, props[i], true);
                            }
                            obj = objs;
                        }
                        else
                        {
                            obj = GetValue(obj, propertyName, true);
                        }
                        if (obj == null) return null;
                        if (nextProp != null)
                        {
                            propertyName = nextProp;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                }
                return GetValue(obj, propertyName, false);
            }
            catch
            {
                return null;
            }
        }

        public static object GetValue(object obj, string propertyName, bool isIndex)
        {
            if (obj == null) return null;
            if (propertyName == null || propertyName.Length == 0) return null;
            int i;
            bool isIntProp = false;
            if (int.TryParse(propertyName, out i))
            {
                isIntProp = true;
                string str = obj as string;
                if (str != null)
                {
                    if (i < 0) return null;
                    if (i < str.Length)
                    {
                        return str[i];
                    }
                    return null;
                }
                IEnumerable itor = obj as IEnumerable;
                if (itor != null)
                {
                    if (i < 0) return null;
                    IList arr = obj as IList;
                    if (arr != null)
                    {
                        if (i < arr.Count)
                        {
                            return arr[i];
                        }
                        return null;
                    }
                    int j = 0;
                    ICollection coll = obj as ICollection;
                    if (coll != null)
                    {
                        if (i < coll.Count)
                        {

                            foreach (object o in coll)
                            {
                                if (i == j) return o;
                                j++;
                            }
                            return null;
                        }
                        return null;
                    }
                    j = 0;
                    foreach (object o in itor)
                    {
                        if (i == j) return o;
                        j++;
                    }
                    return null;
                }
            }
            if (isIndex)
            {
                if (isIntProp)
                {
                    IDictionary<int, object> colls = obj as IDictionary<int, object>;
                    if (colls != null)
                    {
                        object outObj;
                        if (colls.TryGetValue(i, out outObj)) return outObj;
                    }
                }
                if (propertyName.StartsWith("\""))
                {
                    propertyName = propertyName.Trim('"');
                }
                else if (propertyName.StartsWith("'"))
                {
                    propertyName = propertyName.Trim('\'');
                }
                
                IDictionary<string, object> s_colls = obj as IDictionary<string, object>;
                if (s_colls != null)
                {
                    object outObj;
                    if (s_colls.TryGetValue(propertyName, out outObj)) return outObj;
                    foreach (KeyValuePair<string, object> kv in s_colls)
                    {
                        if (string.Compare(kv.Key, propertyName, true) == 0)
                        {
                            return kv.Value;
                        }
                    }
                    return null;
                }
            }
            IDictionary<string, object> _colls = obj as IDictionary<string, object>;
            if (_colls != null)
            {
                if (string.Compare("keys", propertyName, true) == 0)
                {
                    return _colls.Keys;
                }
                if (string.Compare("values", propertyName, true) == 0)
                {
                    return _colls.Values;
                }
                if (string.Compare("count", propertyName, true) == 0)
                {
                    return _colls.Count;
                }
                if (string.Compare("isreadonly", propertyName, true) == 0)
                {
                    return _colls.IsReadOnly;
                }
                object outObj;
                if (_colls.TryGetValue(propertyName, out outObj)) return outObj;
                foreach (KeyValuePair<string, object> kv in _colls)
                {
                    if (string.Compare(kv.Key, propertyName, true) == 0)
                    {
                        return kv.Value;
                    }
                }
                return null;
            }
            var _type = obj.GetType();
            PropertyInfo pro = _type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
            if (pro == null) return null;
            return pro.GetValue(obj,null);
        }
    }
}
