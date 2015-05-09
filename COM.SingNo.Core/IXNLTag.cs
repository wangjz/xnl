using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore
{
    public delegate void OnTagDelegate();
    public interface IXNLTag<T> where T:XNLContext //XNL标签接口
    {
        ParseMode parseMode { get; set; }
        string subTagNames { get; }
        /// 标签实例名
        /// </summary>
        string instanceName { get; set; }

        //别名
        string curTag { get; set; }

        T xnlContext { get; set; }
        //T xnlContext, string instanceName
        void OnInit();

        void OnStart(); //, string body

        void OnTag(OnTagDelegate tagDelegate=null);//子标签 //, string body

        void OnEnd();
        void SetAttribute(string paramName, object value, string tagName=null);

        object GetAttribute(string paramName, string tagName = null);

        bool TryGetAttribute(out object outValue,string paramName, string tagName=null);

        bool ExistAttribute(string paramName, string tagName = null);
        IXNLTag<T> Create();
    }
}
