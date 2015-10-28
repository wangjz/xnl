using System;

namespace Com.AimUI.TagCore.Tags
{
    public class Inc<T> : Set<T>, IInclude where T : TagContext
    {
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
}
