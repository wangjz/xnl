using Com.AimUI.TagCore;
using System;

namespace Com.AimUI.TagCore.Tags
{
    public class TagBase<T> : ITag<T> where T : TagContext
    {
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

        public virtual object GetAttribute(string paramName, object[] userData = null)
        {
            return null;
        }

        public virtual bool ExistAttribute(string paramName)
        {
            return true;
        }

        public virtual ITag<T> Create()
        {
            return Activator.CreateInstance(this.GetType()) as TagBase<T>;
        }

        public virtual TagEvents events
        {
            get { return TagEvents.Tag; }
        }
    }
}
