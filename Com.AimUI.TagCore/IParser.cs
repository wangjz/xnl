using System.Collections.Generic;

namespace Com.AimUI.TagCore
{

    /// <summary>
    /// 标签出错处理方式索引
    /// </summary>
    //public enum XNLOnErrorAction { OutMsg, ThrowError, OutEmpty }
    /// <summary>
    /// 解析器接口
    /// </summary>
    public interface IParser<T> where T : TagContext
    {
        void Initialize();
        /// <summary>
        /// 清除注释标签
        /// </summary>
        /// <param name="contentStr"></param>
        /// <returns></returns>
        string RemoveNoteTag(string contentStr);

        TagStruct GetTagStruct(string contentStr);


        List<TagToken> GetTagTokens(string contentStr);

        //设置自定义返回值预处理符号
        void SetValuePreActionChar(char valuePreActionChar);
    }
}
