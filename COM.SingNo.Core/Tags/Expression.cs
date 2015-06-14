using COM.SingNo.XNLCore.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

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
                args = (ArrayList)userData;
            }
            switch (paramName[0])
            {
                case 'l':
                    switch (paramName)
                    {
                        case "lower":
                            return ToLower(args[0].ToString());
                    }
                    break;
                
                case 'i':
                    switch(paramName)
                    {
                        case "indexof":
                            return args[0].ToString().IndexOf(args[1].ToString());
                        case "iif":
                            if (dt == null) dt = new DataTable();
                            return dt.Compute(string.Format(@"iif({0},{1},{2})",args[0],args[1],args[2]),args.Count > 3 ? args[3].ToString():"");
                    }
                    break;
                case 'r':
                    switch (paramName)
                    {
                        case "replace":
                            return args[0].ToString().Replace(args[1].ToString(), args[2].ToString());
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
