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

        private string _tagName = "exp";
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
                    _tagName = "exp";
                }
                else
                {
                    _tagName = value;
                }
            }
        }

        public TagExpression()
        {
            type = TagTokenType.Express;
        }
    }
}
