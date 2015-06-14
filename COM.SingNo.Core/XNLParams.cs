using System;
using System.Collections.Generic;
using System.Text;

namespace COM.SingNo.XNLCore
{
    public class XNLParams:Dictionary<string,string>
    {
        public new string this[string key]
        {
            get
            {
                string xnlParam;
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
