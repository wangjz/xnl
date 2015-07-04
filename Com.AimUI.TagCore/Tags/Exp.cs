using Com.AimUI.TagCore.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

        public object GetAttribute(string paramName, object[] userData = null) //, bool byRef = false
        {
            if (string.IsNullOrEmpty(paramName)) return null;
            object[] args = null;
            if (userData!=null)
            {
                args = userData;
            }
            switch (paramName[0])
            {
                case 'g':
                    switch (paramName)
                    {
                        case "get":
                            if (args == null || args.Length < 1 || args[0] == null) return null;
                            return HttpContext.Current.Request.QueryString[args[0].ToString()];
                    }
                    break;
                case 'p':
                    switch (paramName)
                    {
                        case "post":
                            if (args == null || args.Length < 1 || args[0] == null) return null;
                            return HttpContext.Current.Request.Form[args[0].ToString()];
                    }
                    break;
                case 'l':
                    switch (paramName)
                    {
                        case "lower":
                            if (args == null || args.Length < 1 || args[0] == null) return null;
                            return ToLower(args[0].ToString());
                    }
                    break;
                
                case 'i':
                    switch(paramName)
                    {
                        case "indexof":
                            if (args == null || args.Length < 2 || args[0] == null) return null;
                            if (args[1] == null) return -1;
                            return args[0].ToString().IndexOf(args[1].ToString());
                        case "iif":
                            if (args == null || args.Length < 3) return null;
                            if (dt == null) dt = new DataTable();
                            if (args.Length>3)
                            {
                                string str="";
                                for(int i=0;i<=args.Length-3;i++)
                                {
                                    if(args[i]==null)
                                    {
                                        str +=  "null";
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
                                string left_s= Convert.ToString(args[args.Length-2]);
                                string right_s =Convert.ToString(args[args.Length-1]);

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

                                return dt.Compute(string.Format(@"iif({0},{1},{2})", exp_s, left_s, right_s), "");
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        case "isnull":
                        case "isempty":
                            if (args == null || args.Length < 2) return null;
                            if (args.Length>2)
                            {
                                string str="";
                                for(int i=0;i<=args.Length-2;i++)
                                {
                                    if (args[i] != null) str += args[i];
                                }
                                args[0] = str;
                            }
                            try
                            {
                                if ("isnull" == paramName) return (args[0] == null ? args[args.Length-1] : args[0]);
                                return string.IsNullOrEmpty(Convert.ToString(args[0])) ? args[args.Length - 1] : args[0];
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
                            if (args == null || args.Length < 1 || args[0] == null) return null;
                            return HttpContext.Current.Session[args[0].ToString()];
                        case "stop":
                            tagContext.response.Stop();
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
                    if (args.Length < 2)
                    {
                        return dt.Compute(args[0].ToString(),"");
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
