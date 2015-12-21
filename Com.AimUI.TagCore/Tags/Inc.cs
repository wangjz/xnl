using System;

namespace Com.AimUI.TagCore.Tags
{
    public class Inc<T> : Set<T>, IInclude where T : TagContext
    {
        /// <summary>
        /// 是否输出
        /// </summary>
        bool output = true;
        public override ITag<T> Create()
        {
            return new Inc<T>();
        }

        public virtual string GetTagBody(string src)
        {
            if (string.IsNullOrEmpty(src)) return null;
            if (src.EndsWith(".ascx") == false) return null;
            return System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + src);
        }

        public override void OnTag(OnTagDelegate tagDelegate = null)
        {
            base.OnTag(tagDelegate);
            if (output)
            {
                if (tagDelegate != null)
                {
                    int inx = buffer.Length;
                    tagDelegate();
                    int len = buffer.Length - inx;
                    if (len > 0)
                    {
                        body = buffer.ToString(inx, len);
                    }
                    else
                    {
                        body = string.Empty;
                    }
                }
            }
        }
        public override void SetAttribute(string paramName, object value)
        {
            if (paramName == "output")
            {
                string v=Convert.ToString(value);
                output = (v == "1" || string.Compare(v, "true", true) == 0);
                return;
            }
            base.SetAttribute(paramName, value);
        }

        public override object GetAttribute(string paramName, object[] userData = null)
        {
            if (paramName == "body")
            {
                object bodyObj;
                if (sets.TryGetValue("#body", out bodyObj)) return bodyObj;
            }
            else if (paramName == "output")
            {
                return output;
            }
            return base.GetAttribute(paramName, userData);
        }
    }
}
