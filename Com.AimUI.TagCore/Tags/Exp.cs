using Com.AimUI.TagCore.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Web;

namespace Com.AimUI.TagCore.Tags
{
    public class Exp<T> : ITag<T> where T : TagContext
    {
        DataTable dt;
        static readonly char[] chars = { '"', '\'' };
        public virtual string subTagNames
        {
            get { return null; }
        }

        public string instanceName
        {
            get;
            set;
        }

        public string curTag
        {
            get;
            set;
        }

        public T tagContext
        {
            get;
            set;
        }

        public virtual void OnInit()
        {

        }

        public virtual void OnStart()
        {

        }

        public virtual void OnTag(OnTagDelegate tagDelegate = null)
        {
            if (tagDelegate != null) tagDelegate();
        }

        public virtual void OnEnd()
        {

        }

        public virtual void SetAttribute(string paramName, object value)
        {

        }
        
        protected virtual object GetValue(object[] userData)
        {
            if (userData == null || userData.Length < 2 || userData[0] == null) return null;
            
            try
            {
                if(userData[1]==null)return null;
                object obj = userData[0];
                return TagHelper.ComplexQuery(obj, userData[1].ToString());
            }
            catch
            {
                return null;
            }
        }

        public virtual object GetAttribute(string paramName, object[] args = null)
        {
            if (string.IsNullOrEmpty(paramName)) return null;
            switch (paramName[0])
            {
                case 'g':
                    switch (paramName)
                    {
                        case "get":
                            if (args == null || args.Length < 1 || args[0] == null) return null;
                            return HttpContext.Current.Request.QueryString[args[0].ToString()];
                        case "getvalue":
                            return GetValue(args);
                    }
                    break;
                case 'p':
                    switch (paramName)
                    {
                        case "post":
                            if (args == null || args.Length == 0 || args[0] == null)
                            {
                                HttpRequest req = HttpContext.Current.Request;
                                int len = (int)req.InputStream.Length;
                                byte[] buffer = new byte[len];
                                req.InputStream.Read(buffer, 0, len);
                                return req.ContentEncoding.GetString(buffer);
                            }
                            return HttpContext.Current.Request.Form[args[0].ToString()];
                    }
                    break;
                case 'l':
                    switch (paramName)
                    {
                        case "lower":
                            if (args == null || args.Length < 1 || args[0] == null) return null;
                            return args[0].ToString().ToLower();
                        case "list":
                            if (args == null || args.Length < 2 || args[0] == null || args[1] == null) return null;
                            IEnumerable<object> list = args[0] as IEnumerable<object>;
                            if (list == null) return null;
                            string key = Convert.ToString(args[1]);
                            int intKey;
                            bool isIntKey = int.TryParse(key, out intKey);
                            if (isIntKey && intKey < 0) return null;
                            bool isDistinct = true;
                            if (args.Length > 2)
                            {
                                string v = Convert.ToString(args[2]);
                                if (v == "0" || string.Compare("false", v, true) == 0)
                                {
                                    isDistinct = false;
                                }
                            }
                            ArrayList newList = null;
                            foreach (var item in list)
                            {
                                if (isIntKey)
                                {
                                    IList<object> _list = item as IList<object>;
                                    if (_list != null)
                                    {
                                        if (intKey < _list.Count)
                                        {
                                            if (newList == null) newList = new ArrayList();
                                            if (isDistinct)
                                            {
                                                object o=_list[intKey];
                                                bool isHas = false;
                                                for (int i = 0; i < newList.Count; i++)
                                                {
                                                    if (object.Equals(newList[i], o))
                                                    {
                                                        isHas = true;
                                                        break;
                                                    }
                                                }
                                                if (isHas == false) newList.Add(o);
                                            }
                                            else
                                            {
                                                newList.Add(_list[intKey]);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    IDictionary<string, object> _colls = item as IDictionary<string, object>;
                                    if (_colls != null)
                                    {
                                        object o;
                                        if (_colls.TryGetValue(key, out o))
                                        {
                                            if (newList == null) newList = new ArrayList();
                                            if (isDistinct)
                                            {
                                                bool isHas = false;
                                                for (int i = 0; i < newList.Count; i++)
                                                {
                                                    if (object.Equals(newList[i], o))
                                                    {
                                                        isHas = true;
                                                        break;
                                                    }
                                                }
                                                if (isHas == false) newList.Add(o);
                                            }
                                            else
                                            {
                                                newList.Add(o);
                                            }
                                        }
                                    }
                                }
                            }
                            if (newList != null && newList.Count > 0) return newList;
                            return null;
                    }
                    break;
                case 'i':
                    switch (paramName)
                    {
                        case "indexof":
                            if (args == null || args.Length < 2 || args[0] == null) return -1;
                            if (args[1] == null) return -1;
                            return args[0].ToString().IndexOf(args[1].ToString());
                        case "iif":
                            if (args == null || args.Length < 3) return null;
                            if (dt == null) dt = new DataTable();
                            if (args.Length > 3)
                            {
                                string str = "";
                                for (int i = 0; i <= args.Length - 3; i++)
                                {
                                    if (args[i] == null)
                                    {
                                        str += "null";
                                    }
                                    else
                                    {
                                        int _v;
                                        string _s = Convert.ToString(args[i]);
                                        if (int.TryParse(_s, out _v) == false)
                                        {
                                            if (_s.IndexOfAny(chars) == -1)
                                            {
                                                if (!((_s.StartsWith("'") && _s.EndsWith("'")) || (_s.StartsWith("\"") && _s.EndsWith("\""))))
                                                {
                                                    _s = "'" + _s.Replace("'", "''") + "'";
                                                }
                                            }
                                        }
                                        str += _s;
                                    }

                                }
                                args[0] = str;
                            }

                            try
                            {
                                string exp_s = Convert.ToString(args[0]);
                                string left_s = Convert.ToString(args[args.Length - 2]);
                                string right_s = Convert.ToString(args[args.Length - 1]);

                                if (exp_s.IndexOfAny(chars) != -1)
                                {
                                    if (!((exp_s.StartsWith("'") && exp_s.EndsWith("'")) || (exp_s.StartsWith("\"") && exp_s.EndsWith("\""))))
                                    {
                                        exp_s = "'" + exp_s + "'";
                                    }
                                }

                                if (!((left_s.StartsWith("'") && left_s.EndsWith("'")) || (left_s.StartsWith("\"") && left_s.EndsWith("\""))))
                                {
                                    left_s = "'" + left_s.Replace("'", "''") + "'";
                                }

                                if (!((right_s.StartsWith("'") && right_s.EndsWith("'")) || (right_s.StartsWith("\"") && right_s.EndsWith("\""))))
                                {
                                    right_s = "'" + right_s.Replace("'", "''") + "'";
                                }
                                if (exp_s.StartsWith("=")) exp_s = "null" + exp_s;
                                if (exp_s.EndsWith("=")) exp_s = exp_s + "null";
                                return dt.Compute(string.Format(@"iif({0},{1},{2})", exp_s, left_s, right_s), "");
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        case "isnull":
                        case "isempty":
                            if (args == null || args.Length < 2) return null;
                            if (args.Length > 2)
                            {
                                string str = "";
                                for (int i = 0; i <= args.Length - 2; i++)
                                {
                                    if (args[i] != null) str += args[i];
                                }
                                args[0] = str;
                            }
                            try
                            {
                                if ("isnull" == paramName) return (args[0] == null ? args[args.Length - 1] : args[0]);
                                return string.IsNullOrEmpty(Convert.ToString(args[0])) ? args[args.Length - 1] : args[0];
                            }
                            catch
                            {
                                return null;
                            }
                        case "int":
                            if (args == null || args.Length == 0) return null;
                            try
                            {
                                return Convert.ToInt32(args[0]);
                            }
                            catch
                            {
                                return 0;
                            }
                    }
                    break;
                case 'r':
                    switch (paramName)
                    {
                        case "replace":
                            if (args == null || args.Length < 3 || args[0] == null) return null;
                            if (args[1] == null || args[2] == null) return args[0];
                            return args[0].ToString().Replace(args[1].ToString(), args[2].ToString());
                        case "request":
                            if (args == null || args.Length < 1 || args[0] == null) return null;
                            return HttpContext.Current.Request[args[0].ToString()];
                        case "repeat":
                            if (args == null || args.Length < 2 || args[0] == null) return null;
                            string src = Convert.ToString(args[0]);
                            if (string.IsNullOrEmpty(src)) return src;
                            int repeat = 1;
                            int.TryParse(Convert.ToString(args[1]), out repeat);
                            string to = src;
                            for (int i = 0; i < repeat; i++)
                            {
                                to += src;
                            }
                            return to;
                        case "redirect":
                            if (args == null) return null;
                            string url = Convert.ToString(args[0]);
                            bool end = true;
                            if (args.Length > 1)
                            {
                                if (args[1] is bool)
                                {
                                    end = Convert.ToBoolean(args[1]);
                                }
                                else
                                {
                                    string s = Convert.ToString(args[1]);
                                    end = (s == "1" || string.Compare(s, "true", true) == 0);
                                }
                            }
                            HttpContext.Current.Response.Redirect(url, end);
                            return null;
                    }
                    break;
                case 'n':
                    switch (paramName)
                    {
                        case "now":
                            return DateTime.Now;
                        case "null":
                            return null;
                    }
                    break;
                case 's':
                    switch (paramName)
                    {
                        case "session":
                            if (args == null || args.Length < 1 || args[0] == null) return null;
                            return HttpContext.Current.Session[args[0].ToString()];
                        case "stop":
                            tagContext.GetTagResponse().Stop();
                            return null;
                    }
                    break;
                case 't':
                    switch (paramName)
                    {
                        case "true":
                            return true;
                    }
                    return null;
                case 'f':
                    switch (paramName)
                    {
                        case "false":
                            return false;
                    }
                    return null;
                case 'e':
                    switch (paramName)
                    {
                        case "end":
                            throw new ResponseEndException();
                    }
                    break;
                case 'b':
                    switch (paramName)
                    {
                        case "bit":
                            if (args == null || args.Length < 2) return null;
                            string _s = Convert.ToString(args[1]);
                            char bitAction = (string.IsNullOrEmpty(_s) ? '&' : _s[0]);
                            int left = Convert.ToInt32(args[0]);
                            switch (bitAction)
                            {
                                case '&':
                                    return left & Convert.ToInt32(args[2]);
                                case '|':
                                    return left | Convert.ToInt32(args[2]);
                                case '^':
                                    return left ^ Convert.ToInt32(args[2]);
                                case '~':
                                    return ~left;
                                case '<':  //左移
                                    return left << Convert.ToInt32(args[2]);
                                case '>': //右移
                                    return left >> Convert.ToInt32(args[2]);
                            }
                            return null;
                    }
                    break;
                case 'j':
                    switch (paramName)
                    {
                        case "jsonencode":
                            return TagContext.OnValuePreAction(args[0], ValuePreAction.JSON_Serialize);
                        case "jsondecode":
                            return TagContext.OnValuePreAction(args[0], ValuePreAction.JSON_Deserialize);
                    }
                    break;
                case 'm':
                    switch (paramName)
                    {
                        case "mixsets":
                            if (args == null || args.Length < 4) return null;
                            var set1 = args[0] as IEnumerable<IDictionary<string, object>>;
                            if (set1 == null) return set1;
                            var set2 = args[1] as IEnumerable<IDictionary<string, object>>;
                            if (set2 == null) return set1;
                            string key = Convert.ToString(args[2]);
                            if (string.IsNullOrEmpty(key)) return set1;
                            string leftKey = Convert.ToString(args[3]);
                            if (string.IsNullOrEmpty(leftKey)) return set1;
                            string rightKey = (args.Length > 4 ? Convert.ToString(args[4]) : leftKey);
                            if (string.IsNullOrEmpty(rightKey)) return set1;
                            object obj1 = null;
                            object obj2 = null;
                            bool isCheckLeft = false;
                            bool isCheckRight = false;
                            foreach (var item1 in set1)
                            {
                                if (isCheckLeft == false)
                                {
                                    isCheckLeft = true;
                                    if (item1.ContainsKey(leftKey) == false) return set1;
                                }
                                item1.TryGetValue(leftKey, out obj1);
                                if (obj1 == null)
                                {
                                    item1[key] = null;
                                    continue;
                                }
                                foreach (var item2 in set2)
                                {
                                    if (isCheckRight == false)
                                    {
                                        isCheckRight = true;
                                        if (item2.ContainsKey(rightKey) == false) return set1;
                                    }
                                    item2.TryGetValue(rightKey, out obj2);
                                    if (object.Equals(obj1, obj2))
                                    {
                                        item1[key] = item2;
                                        break;
                                    }
                                }
                            }
                            return set1;
                    }
                    break;
            }
            if (paramName == "_")
            {
                try
                {
                    if (dt == null) dt = new DataTable();
                    if (args.Length < 2)
                    {
                        return dt.Compute(args[0].ToString(), "");
                    }
                    else
                    {
                        string _s = "";
                        for (int i = 0; i < args.Length; i++)
                        {
                            _s += args[i].ToString();
                        }
                        return dt.Compute(_s, "");
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public virtual ITag<T> Create()
        {
            return new Exp<T>();
        }

        public virtual TagEvents events
        {
            get { return TagEvents.Tag; }
        }
    }
}
