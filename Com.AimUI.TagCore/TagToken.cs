using System;
using System.Collections.Generic;
using System.Text;

namespace Com.AimUI.TagCore
{
    public enum TagTokenType
    {
        Common,
        Attribute, //属性 @
        Express //= 2,  //表达式 $
    }
    public class TagToken
    {
        public TagTokenType type { get; set; }
        //名称
        public string name { get; set; }

        //域名称  标签所在实例名称  或 标签名称
        public virtual string scope { get; set; }
        //在内容中的位置
        public int index { get; set; }
        //在内容中的长度
        public int length { get; set; }

        /// <summary>
        /// 是否序列化输出
        /// </summary>
        public ValuePreAction action { get; set; }

        public char actionChar { get; set; }

        public string value { get; set; }

        /// <summary>
        /// 参数列表
        /// </summary>
        public IList<TagToken> args { get; set; }

        public TagToken()
        {
            action = ValuePreAction.NONE;
        }
    }
}
