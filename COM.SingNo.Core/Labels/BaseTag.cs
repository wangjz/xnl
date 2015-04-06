using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore.Labels
{
    public class XNLTag<T> : IXNLTag<T> where T : XNLContext
    {
       // private T _xnlContext;
        #region IXNLTag<T> 成员
        public T xnlContext
        {
            get;
            protected set;
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
            private set; 
        }

        public string curTag
        {
            get;
            set;
        }

        public virtual void onInit(T xnlContext, string instanceName)
        {
            this.xnlContext = xnlContext;
        }

        public virtual void onStart()
        {
            
        }

        public virtual void onEnd()
        {
           
        }

        public virtual void onTag(string tagName, OnTagDelegate<T> tagDelegate, string body)
        {
            tagDelegate(this, body);
        }

        public void setAttributeValue(string paramName, object value, string tagName=null)
        {
            throw new NotImplementedException();
        }


        public object getAttributeValue(string paramName, string tagName=null)
        {
            throw new NotImplementedException();
        }

        public IXNLTag<T> create()
        {
            throw new NotImplementedException();
        }

        public bool existAttribute(string paramName, string tagName=null)
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
