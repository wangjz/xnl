using System;
using System.Collections.Generic;
using System.Text;

namespace COM.SingNo.XNLCore
{
    public class XNLParams:Dictionary<string,XNLParam>
    {
        public new XNLParam this[string key]
        {
            get
            {
                XNLParam xnlParam;
                this.TryGetValue(key, out xnlParam);
                return xnlParam;
            }
            set
            {
                this[key]  = value;
            }
        }
    }
}
