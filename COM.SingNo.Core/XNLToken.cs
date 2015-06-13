using System;
using System.Collections.Generic;
using System.Text;

namespace COM.SingNo.XNLCore
{
    public enum XNLTokenType
    {
        Common,
        Attribute, //属性 @
        Express //= 2,  //表达式 $
        //Variable, //= 1变量 $
        //tag
    }
    public class XNLToken
    {
        public XNLTokenType type { get; set; }
        //名称
        public string name { get; set; }

        //域名称  标签所在实例名称  或 标签名称
        public string scope { get; set; }
        //在内容中的位置
        public int index { get; set; }
        //在内容中的长度
        public int length { get; set; }

        public ParseMode mode { get; set; }

        public string value { get; set; }
    }
}
