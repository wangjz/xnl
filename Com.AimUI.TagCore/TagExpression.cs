using System;
using System.Collections.Generic;
using System.Text;

namespace Com.AimUI.TagCore
{
    public class TagExpression : TagToken
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        private string _scope = "at";
        public override string scope
        {
            get
            {
                return _scope;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _scope = "at";
                }
                else
                {
                    _scope = value;
                }
            }
        }

        private string _tagName = "expression";
        public string tagName
        {
            get 
            {
                return _tagName;
            }
            set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    _tagName = "expression";
                }
                else
                {
                    _tagName = value;
                }
            }
        }

       
        /// <summary>
        /// 参数列表
        /// </summary>
        public IList<TagToken> args { get; set; }

        public TagExpression()
        {
            type = TagTokenType.Express;
        }
    }
}
