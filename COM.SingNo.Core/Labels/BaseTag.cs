using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore.Labels
{
    public class BaseTag<T> : IXNLTag<T> where T : XNLContext
    {
       // private T _xnlContext;
        #region IXNLTag<T> 成员
        public T xnlContext
        {
            get;
            set;
        }
        public string subTagNames
        {
            get
            {
                return null;
            }
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
        //T xnlContext, string instanceName
        // this.xnlContext = xnlContext;
        public virtual void OnInit()
        {
           
        }

        public virtual void OnStart()
        {
            
        }

        public virtual void OnEnd()
        {
           
        }

        public virtual void OnTag(OnTagDelegate tagDelegate=null)
        {
            tagDelegate();
        }

        public void SetAttribute(string paramName, object value, string tagName=null)
        {
            throw new NotImplementedException();
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

        public IXNLTag<T> Create()
        {
            throw new NotImplementedException();
        }

        public bool ExistAttribute(string paramName, string tagName=null)
        {
            return false;
        }
        #endregion

        public ParseMode parseMode
        {
            get;
            set;
        }
    }
}
