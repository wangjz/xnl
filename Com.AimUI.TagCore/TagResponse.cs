using System.Text;
using Com.AimUI.TagCore.Exceptions;

namespace Com.AimUI.TagCore
{
    public class TagResponse
    {
        public StringBuilder buffer { get; protected set; }
        public TagResponse()
        {
            buffer = new StringBuilder();
        }

        public void Write(string s)
        {
            buffer.Append(s);
        }

        public void Clear()
        {
            buffer.Remove(0, buffer.Length);
        }
        public void End()
        {
            throw new ResponseEndException();
        }
        /// <summary>
        /// 停止当前标签的解析
        /// </summary>
        public void Stop()
        {
            throw new TagStopException();
        }
    }
}
