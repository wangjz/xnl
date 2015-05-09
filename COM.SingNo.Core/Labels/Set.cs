using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore.Labels
{
    public class Set<T> : IXNLTag<T> where T : XNLContext
    {
        public T xnlContext
        {
            get;
            set;
        }

        public string instanceName { get; set; }
        public string curTag { get; set; }

        public void OnInit()
        {
            
        }

        public void OnStart()
        {
        }

        public void OnEnd()
        {

        }

        //子标签解析
        public void OnTag(OnTagDelegate tagDelegate = null)
        {
        }
        public void SetAttribute(string paramName, object value, string tagName = null)
        {

        }

        public object GetAttribute(string paramName, string tagName = null)
        {
            return null;
        }
        public bool TryGetAttribute(out object outValue, string paramName, string tagName = null)
        {
            outValue = null;
            return false;
        }

        //创建 
        public IXNLTag<T> Create()
        {
            return null;
        }

        public bool ExistAttribute(string paramName, string tagName = null)
        {
            return false;
        }

        #region IXNLTagObj<T> 成员


        public string subTagNames
        {
            get
            {
                return null;
            }
        }
        #endregion

        public ParseMode parseMode
        {
            get;
            set;
        }
    }
}
