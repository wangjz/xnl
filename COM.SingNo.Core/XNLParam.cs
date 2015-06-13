using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore
{
    /// <summary>
    /// 定义xnl标签里的参数类型
    /// </summary>
    public class XNLParam
    {
        private object _val;
        public object value
        {
            get
            {
                if (_val == null) return string.Empty;
                return _val;
            }
            set
            {
                _val = value;
            }

        }

        public XNLParam()
        {
        }
        public XNLParam(object t_thevalue)
        {
            _val = t_thevalue;
        }
       
    }
}