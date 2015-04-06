using System;
using System.Collections.Generic;
using System.Text;
using COM.SingNo.XNLCore.Exceptions;
namespace COM.SingNo.XNLCore
{
    public class XNLResponse 
    {
        public StringBuilder buffer { get; protected set; }
        public XNLResponse()
        {
            buffer = new StringBuilder();
        }

        public void Write(string s)
        {
            buffer.Append(s);
        }

        public void Clear()
        {
            buffer.Remove(0, buffer.Length);
        }
        public void End()
        {
            throw new ResponseEndException();
        }
        /// <summary>
        /// 停止当前标签的解析
        /// </summary>
        public void Stop()
        {
            throw new TagStopException();
        }
    }
}
