using System;
using System.Collections.Generic;
using System.Text;

namespace COM.SingNo.XNLCore
{
    public enum XNLTokenType
    {
        Attribute = 0, //属性 @
        Variable, //= 1变量 $
        Express //= 2,  //表达式
        //tag
    }
    public class XNLToken
    {
        public XNLTokenType Type { get; set; }
        //名称
        public string Name { get; set; }

        //域名称  标签所在实例名称  或 标签名称
        public string Scope { get; set; }
        //在内容中的位置
        public int Index { get; set; }
        //在内容中的长度
        public int Length { get; set; }

        public ParseMode mode { get; set; }

        public string value { get; set; }
    }
}
