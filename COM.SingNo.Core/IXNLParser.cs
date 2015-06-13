using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace COM.SingNo.XNLCore
{
    /// <summary>
    /// 标签出错处理方式索引
    /// </summary>
    //public enum XNLOnErrorAction { OutMsg, ThrowError, OutEmpty }
    /// <summary>
    /// 解析器接口
    /// </summary>
    public interface IXNLParser<T> where T : XNLContext
    {
        void Initialize();
        /// <summary>
        /// 清除xnl注释标签
        /// </summary>
        /// <param name="contentStr"></param>
        /// <returns></returns>
        string RemoveXNLNotes(string contentStr);

        XNLTagStruct GetTagStruct(string contentStr);


        List<XNLToken> GetTagTokens(string contentStr);
    }
}
