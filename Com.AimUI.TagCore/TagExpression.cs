
namespace Com.AimUI.TagCore
{
    public class TagExpression : TagToken
    {

        protected string _scope;
        /// <summary>
        /// 域名称 
        /// </summary>
        public override string scope
        {
            get
            {
                return string.IsNullOrEmpty(_scope) ? "at" : _scope;
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

        /// <summary>
        /// 标签名称
        /// </summary>
        public override string tagName
        {
            get
            {
                return _tagName ?? "exp";
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
            : base()
        {
            type = TagTokenType.Express;
        }
    }
}
