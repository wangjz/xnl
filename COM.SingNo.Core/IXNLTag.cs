using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore
{
    public delegate void OnTagDelegate<T>(IXNLTag<T> tagObj ,string body) where T : XNLContext;
    public interface IXNLTag<T> where T:XNLContext //XNL标签接口
    {
        ParseMode parseMode { get; set; }
        string subTagNames { get; }
        /// 标签实例名
        /// </summary>
        string instanceName { get; }

        //别名
        string curTag { get; set; }

        T xnlContext { get; }
      
        void onInit(T xnlContext, string instanceName);

        void onStart();

        void onTag(string tagName, OnTagDelegate<T> tagDelegate, string body);//子标签

        void onEnd();
        void setAttributeValue(string paramName, object value, string tagName=null);
        object getAttributeValue(string paramName, string tagName=null);

        bool existAttribute(string paramName, string tagName = null);
        IXNLTag<T> create();
    }
}
