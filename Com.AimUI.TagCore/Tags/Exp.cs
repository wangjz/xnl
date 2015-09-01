using Com.AimUI.TagCore.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Web;

namespace Com.AimUI.TagCore.Tags
{
    public class Exp<T> : ITag<T> where T : TagContext
    {
        DataTable dt;
        static readonly char[] chars = {'"','\''};
        public string subTagNames
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

        public void OnInit()
        {
            
        }

        public void OnStart()
        {
            
        }

        public void OnTag(OnTagDelegate tagDelegate = null)
        {

        }

        public void OnEnd()
        {
            
        }

        public void SetAttribute(string paramName, object value)
        {
            
        }

        public object GetAttribute(string paramName, object[] userData = null)
        {
            if (string.IsNullOrEmpty(paramName)) return null;
            switch (paramName[0])
            {
                case 'g':
                    switch (paramName)
                    {
                        case "get":
                            if (userData == null || userData.Length < 1 || userData[0] == null) return null;
                            return HttpContext.Current.Request.QueryString[userData[0].ToString()];
                        case "getvalue":
                            if (userData == null || userData.Length < 2 || userData[0] == null) return null;
                            string _name = Convert.ToString(userData[1]);
                            if (string.IsNullOrEmpty(_name)) return null;
                            try
                            {
                                object obj = userData[0];
                                
                                if(obj is Array)
                                {
                                    int inx;
                                    if (int.TryParse(_name, out inx))
                                    {
                                        object [] _arr = obj as object[];
                                        if(_arr==null)return null;
                                        return _arr[inx];
                                    }
                                }
                                return obj.GetType().GetProperty(_name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance).GetValue(obj, null);
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                            
                    }
                    break;
                case 'p':
                    switch (paramName)
                    {
                        case "post":
                            if (userData == null || userData.Length == 0 || userData[0] == null)
                            {
                                HttpRequest req = HttpContext.Current.Request;
                                int len = (int)req.InputStream.Length;
                                byte[] buffer = new byte[len];
                                req.InputStream.Read(buffer, 0, len);
                                return req.ContentEncoding.GetString(buffer);
                            }
                            return HttpContext.Current.Request.Form[userData[0].ToString()];
                    }
                    break;
                case 'l':
                    switch (paramName)
                    {
                        case "lower":
                            if (userData == null || userData.Length < 1 || userData[0] == null) return null;
                            return ToLower(userData[0].ToString());
                    }
                    break;
                
                case 'i':
                    switch(paramName)
                    {
                        case "indexof":
                            if (userData == null || userData.Length < 2 || userData[0] == null) return null;
                            if (userData[1] == null) return -1;
                            return userData[0].ToString().IndexOf(userData[1].ToString());
                        case "iif":
                            if (userData == null || userData.Length < 3) return null;
                            if (dt == null) dt = new DataTable();
                            if (userData.Length>3)
                            {
                                string str="";
                                for(int i=0;i<=userData.Length-3;i++)
                                {
                                    if(userData[i]==null)
                                    {
                                        str +=  "null";
                                    }
                                    else
                                    {
                                        int _v;
                                        string _s = Convert.ToString(userData[i]);
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
                                userData[0] = str;
                            }

                            try
                            {
                                string exp_s = Convert.ToString(userData[0]);
                                string left_s= Convert.ToString(userData[userData.Length-2]);
                                string right_s =Convert.ToString(userData[userData.Length-1]);

                                if (exp_s.IndexOfAny(chars)!=-1)
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
                            if (userData == null || userData.Length < 2) return null;
                            if (userData.Length>2)
                            {
                                string str="";
                                for(int i=0;i<=userData.Length-2;i++)
                                {
                                    if (userData[i] != null) str += userData[i];
                                }
                                userData[0] = str;
                            }
                            try
                            {
                                if ("isnull" == paramName) return (userData[0] == null ? userData[userData.Length-1] : userData[0]);
                                return string.IsNullOrEmpty(Convert.ToString(userData[0])) ? userData[userData.Length - 1] : userData[0];
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                    }
                    break;
                case 'r':
                    switch (paramName)
                    {
                        case "replace":
                            if (userData == null || userData.Length < 3 || userData[0] == null) return null;
                            if (userData[1] == null || userData[2] == null) return userData[0];
                            return userData[0].ToString().Replace(userData[1].ToString(), userData[2].ToString());
                        case "request":
                            if (userData == null || userData.Length < 1 || userData[0] == null) return null;
                            return HttpContext.Current.Request[userData[0].ToString()];
                        case "repeat":
                            if (userData == null || userData.Length < 2 || userData[0] == null) return null;
                            string src = Convert.ToString(userData[0]);
                            if (string.IsNullOrEmpty(src)) return src;
                            int repeat = 1;
                            int.TryParse(Convert.ToString(userData[1]), out repeat);
                            string to=src;
                            for (int i = 0; i < repeat; i++)
                            {
                                to += src;
                            }
                            return to;
                    }
                    break;
                case 'n':
                    switch(paramName)
                    {
                        case "now":
                            return DateTime.Now;
                    }
                    break;
                case 's':
                    switch (paramName)
                    {
                        case "session":
                            if (userData == null || userData.Length < 1 || userData[0] == null) return null;
                            return HttpContext.Current.Session[userData[0].ToString()];
                        case "stop":
                            tagContext.GetTagResponse().Stop();
                            return null;
                    }
                    break;
                case 'e':
                    switch(paramName)
                    {
                        case "end":
                            throw new ResponseEndException();
                    }
                    break;
            }
            if (paramName=="_")
            {
                try
                {
                    if (dt == null) dt = new DataTable();
                    if (userData.Length < 2)
                    {
                        return dt.Compute(userData[0].ToString(),"");
                    }
                    else
                    {
                        string _s = "";
                        for (int i = 0; i < userData.Length; i++)
                        {
                            _s += userData[i].ToString();
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

        public bool ExistAttribute(string paramName)
        {
            return false;
        }

        public ITag<T> Create()
        {
            return new Exp<T>();
        }

        public static string ToLower(string input)
        {
            return input.ToLower();
        }
    }
}
