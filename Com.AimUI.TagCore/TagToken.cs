using System.Collections.Generic;

namespace Com.AimUI.TagCore
{
    public enum TagTokenType
    {
        Common,
        /// <summary>
        /// 属性 @
        /// </summary>
        Attribute,
        /// <summary>
        /// 表达式 $
        /// </summary>
        Express, 
    }

    public enum TagScopeType
    {
        /// <summary>
        /// 当前域
        /// </summary>
        Current,
        /// <summary>
        /// 命名空间域 匿名
        /// </summary>
        NameScape,
        /// <summary>
        /// 标签实例 命名
        /// </summary>
        Instance
    }

    public class TagToken
    {
        /// <summary>
        /// 类型
        /// </summary>
        public TagTokenType type { get; set; }

        private string _name;
        /// <summary>
        /// 名称
        /// </summary>
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
        /// <summary>
        /// 域名称  标签所在实例名称  或 标签名称
        /// </summary>
        public virtual string scope 
        {
            get
            {
                return _scope;
            }
            set
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    char c = value[0];
                    if (c > 47 && c < 58)
                    {
                        _scope = "_" + value;
                    }
                    else _scope = value;
                }
                else _scope = value;
            }
        }

        /// <summary>
        /// 在内容中的位置
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// 在内容中的长度
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// 值预处理行为
        /// </summary>
        public ValuePreAction action { get; set; }

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
