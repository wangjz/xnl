using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Com.AimUI.TagCore.Tags
{
    public class BaseTag<T> : ITag<T> where T : TagContext
    {
        #region ITag<T> 成员
        public T tagContext
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

        public void SetAttribute(string paramName, object value)
        {
            throw new NotImplementedException();
        }

        public object GetAttribute(string paramName, object userData = null)
        {
            return null;
        }
        public bool TryGetAttribute(out object outValue, string paramName, object userData = null)
        {
            outValue = null;
            return false;
        }

        public ITag<T> Create()
        {
            throw new NotImplementedException();
        }

        public bool ExistAttribute(string paramName)
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
