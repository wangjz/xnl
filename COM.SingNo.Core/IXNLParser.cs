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
        //XNLOnErrorAction onTagErrorAction { get;}
       
        /// <summary>
        /// 清除xnl注释标签
        /// </summary>
        /// <param name="contentStr"></param>
        /// <returns></returns>
        string RemoveXNLNotes(string contentStr);

        //string replaceAttribleVariable(IXNLTag<T> tagObj,string contentStr);
        /// <summary>
        /// 解析所有基本变量标签
        /// </summary>
        /// <param name="contentStr"></param>
        /// <param name="xnlContext"></param>
        /// <returns></returns>
        //string replaceAllBaseVariable(string contentStr, T xnlContext);

        XNLTagStruct GetTagStruct(string contentStr);

        //List<XNLAttribute> GetTagTokens(string contentStr,string tagInstance);

        //List<XNLExpression> GetTagTokens(string contentStr);

        //List<XNLVariable> GetTagTokens(string contentStr);

        List<XNLToken> GetTagTokens(string contentStr);
    }
}
