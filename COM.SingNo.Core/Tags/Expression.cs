using COM.SingNo.XNLCore.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
namespace COM.SingNo.XNLCore.Tags
{
    public class Expression<T> : IXNLTag<T> where T : XNLContext
    {
        DataTable dt;
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

        public T xnlContext
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
            throw new NotImplementedException();
        }

        public void SetAttribute(string paramName, object value, string tagName = null)
        {
            
        }

        public object GetAttribute(string paramName, string tagName = null, object userData = null)
        {
            if (string.IsNullOrEmpty(paramName)) return "";
            ArrayList args = null;
            if (userData!=null)
            {
                args = userData as ArrayList;
            }
            switch (paramName[0])
            {
                case 'g':
                    switch (paramName)
                    {
                        case "get":
                            if (args == null || args.Count < 1 || args[0] == null) return "";
                            return HttpContext.Current.Request.QueryString[args[0].ToString()];
                    }
                    break;
                case 'p':
                    switch (paramName)
                    {
                        case "post":
                            if (args == null || args.Count < 1 || args[0] == null) return "";
                            return HttpContext.Current.Request.Form[args[0].ToString()];
                    }
                    break;
                case 'l':
                    switch (paramName)
                    {
                        case "lower":
                            if (args == null || args.Count < 1 || args[0] == null) return "";
                            return ToLower(args[0].ToString());
                    }
                    break;
                
                case 'i':
                    switch(paramName)
                    {
                        case "indexof":
                            if (args == null || args.Count < 2 || args[0] == null) return "";
                            if (args[1] == null) return -1;
                            return args[0].ToString().IndexOf(args[1].ToString());
                        case "iif":
                            if (args == null || args.Count < 3 || args[0] == null) return "";
                            if (dt == null) dt = new DataTable();
                            return dt.Compute(string.Format(@"iif({0},{1},{2})",args[0],args[1],args[2]),args.Count > 3 ? args[3].ToString():"");
                    }
                    break;
                case 'r':
                    switch (paramName)
                    {
                        case "replace":
                            if (args == null || args.Count < 3 || args[0] == null) return "";
                            if (args[1] == null || args[2] == null) return args[0];
                            return args[0].ToString().Replace(args[1].ToString(), args[2].ToString());
                        case "request":
                            if (args == null || args.Count < 1 || args[0] == null) return "";
                            return HttpContext.Current.Request[args[0].ToString()];
                    }
                    break;
                case 'n':
                    switch(paramName)
                    {
                        case "now":
                            return DateTime.Now.ToString();
                    }
                    break;
                case 's':
                    switch (paramName)
                    {
                        case "session":
                            if (args == null || args.Count < 1 || args[0] == null) return "";
                            return HttpContext.Current.Session[args[0].ToString()];
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
                    return dt.Compute( args[0].ToString(), args.Count > 1 ? args[1].ToString() : "");
                }
                catch
                {
                    return "";
                }
            }
            return "";
        }

        public bool TryGetAttribute(out object outValue, string paramName, string tagName = null, object userData = null)
        {
            outValue = null;
            return true;
        }

        public bool ExistAttribute(string paramName, string tagName = null)
        {
            return true;
        }

        public IXNLTag<T> Create()
        {
            return new Expression<T>();
        }

        public static string ToLower(string input)
        {
            return input.ToLower();
        }
    }
}
