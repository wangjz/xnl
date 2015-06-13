using System;
using System.Collections.Generic;
using System.Text;

namespace COM.SingNo.XNLCore
{
    public class XNLExpression : XNLToken
    {
        /// <summary>
        /// 标签名称
        /// </summary>

        public string tagName { get; set; }

       
        /// <summary>
        /// 参数列表
        /// </summary>
        public IList<XNLToken> args { get; set; }

        public XNLExpression()
        {
            type = XNLTokenType.Express;
        }
    }
}
