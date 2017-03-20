using System.Collections.Generic;

namespace Com.AimUI.TagCore
{
    public enum TagTokenType
    {
        Common = 0,
        /// <summary>
        /// 属性 @
        /// </summary>
        //Attribute = 1,
        /// <summary>
        /// 表达式 $
        /// </summary>
        Express = 1 << 1
    }

    //public enum TagScopeType
    //{
    //    /// <summary>
    //    /// 当前域
    //    /// </summary>
    //    Current,
    //    /// <summary>
    //    /// 命名空间域 匿名
    //    /// </summary>
    //    NameScape,
    //    /// <summary>
    //    /// 标签实例 命名
    //    /// </summary>
    //    Instance
    //}

    public class TagToken
    {
        /// <summary>
        /// 类型
        /// </summary>
        public TagTokenType type { get; set; }

        protected string _tagName;
        public virtual string tagName { get; set; }

        protected string _name;
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string name
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

        /// <summary>
        /// 域名称  标签所在命名空间
        /// </summary>
        public virtual string scope { get; set; }

        protected string _instance;
        /// <summary>
        /// 实例名称
        /// </summary>
        public virtual string instanceName 
        {
            get
            {
                return _instance;
            }
            set
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    char c = value[0];
                    if (c > 47 && c < 58)
                    {
                        _instance = "_" + value;
                    }
                    else _instance = value;
                }
                else _instance = value;
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
        /// <summary>
        /// 链操作路径
        /// </summary>
        public string chainsPath { get; set; }

        public TagToken()
        {
            action = ValuePreAction.NONE;
        }

        public bool IsChains()
        {
            return chainsPath != null && chainsPath.Length > 0;
        }
    }
}
