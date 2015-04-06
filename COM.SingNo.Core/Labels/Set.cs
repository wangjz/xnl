using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore.Labels
{
  public  class Set<T>:IXNLTag<T> where T:XNLContext
  {
      public T xnlContext
      {
          get;
          set;
      }
     
        //标签是否解析结束
        public bool isEnd
        {
            get;
            set;
        }

        public void end()
        {

        }
        public string instanceName { get; private set; }
        public string curTag { get; set; }
        // //标签开始,初始化参数等 抛异常初始化失败原因
        public void onInit(T xnlContext,string instanceName)
        {
            this.xnlContext = xnlContext;
        }

        public void onStart()
        {
        }

        public void onEnd()
        {

        }

        //子标签解析
        public void onTag(string tagName, OnTagDelegate<T> tagDelegate, string body)
        {
        }
        public void setAttributeValue(string paramName, object value, string tagName=null)
        {

        }
        public object getAttributeValue(string paramName, string tagName = null)
        {
            return "";
        }
        
        //创建 
        public IXNLTag<T> create()
        {
            return null;
        }

        public bool existAttribute(string paramName, string tagName=null)
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
