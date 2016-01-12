using System;
using System.Collections.Generic;
using System.Text;

namespace Com.AimUI.TagCore.Tags
{
    public class Func<T> : TagBase<T> where T : TagContext
    {
        protected IDictionary<string, OnTagDelegate> funcs;
        public override ITag<T> Create()
        {
            return new Func<T>();
        }

        public override object GetAttribute(string paramName, object[] userData = null)
        {
            return null;
        }

        public override void OnTag(OnTagDelegate tagDelegate = null)
        {

        }

        public override string subTagNames
        {
            get
            {
                return "fn";
            }
        }
    }
}
