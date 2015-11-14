using System.Collections.Generic;

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

        private string _name;
        //名称
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != null)
                {
                    _name = value.ToLower();
                }
                else
                {
                    _name = null;
                }
            }
        }

        private string _scope;
        //域名称  标签所在实例名称  或 标签名称
        public virtual string scope 
        {
            get
            {
                return _scope;
            }
            set
            {
                if (string.IsNullOrEmpty(value)==false)
                {
                    char c = value[0];
                    if (c > 47 && c < 58)
                    {
                        _scope = "_" + value;
                    }
                    else _scope = value;
                }
            }
        }
        //在内容中的位置
        public int index { get; set; }
        //在内容中的长度
        public int length { get; set; }

        /// <summary>
        /// 值预处理行为
        /// </summary>
        public ValuePreAction action { get; set; }

        public byte actionCharCode { get; set; }

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
