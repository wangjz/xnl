using System;
using System.Collections.Generic;
using System.Text;

namespace COM.SingNo.XNLCore
{
    public class XNLParams:Dictionary<string,object>
    {
        public new object this[string key]
        {
            get
            {
                object xnlParam;
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
