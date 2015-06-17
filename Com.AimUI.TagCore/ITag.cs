using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Com.AimUI.TagCore
{
    public delegate void OnTagDelegate();
    public interface ITag<T> where T:TagContext
    {
        string subTagNames { get; }
        /// 标签实例名
        /// </summary>
        string instanceName { get; set; }

        //别名
        string curTag { get; set; }

        T tagContext { get; set; }
        void OnInit();

        void OnStart();

        void OnTag(OnTagDelegate tagDelegate=null);

        void OnEnd();
        void SetAttribute(string paramName, object value);

        object GetAttribute(string paramName, object userData = null);

        bool TryGetAttribute(out object outValue, string paramName, object userData = null);

        bool ExistAttribute(string paramName);
        ITag<T> Create();
    }
}
