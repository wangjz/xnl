using System;

namespace Com.AimUI.TagCore.Tags
{
    public class Inc<T> : Set<T>, IInclude where T : TagContext
    {
        /// <summary>
        /// 是否输出
        /// </summary>
        protected string src;
        public override ITag<T> Create()
        {
            return new Inc<T>();
        }

        public virtual string GetTagBody(string _src, string innerBody)
        {
            if (string.IsNullOrEmpty(src) && string.IsNullOrEmpty(_src) == false) src = _src;
            if (string.IsNullOrEmpty(src)) return null;
            if (src.EndsWith(".ascx") == false) return null;
            return System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + src);
        }

        public override void SetAttribute(string paramName, object value)
        {
            if (paramName == "src")
            {
                src = Convert.ToString(value);
                return;
            }
            base.SetAttribute(paramName, value);
        }

        public override object GetAttribute(string paramName, object[] args = null)
        {
            if (paramName == "body")
            {
                object bodyObj;
                if (sets.TryGetValue("#body", out bodyObj)) return bodyObj;
            }
            else if (paramName == "src")
            {
                return src;
            }
            return base.GetAttribute(paramName, args);
        }
    }
}
