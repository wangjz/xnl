using System;
using System.Collections.Generic;
using System.Text;
using COM.SingNo.XNLCore;
namespace COM.SingNo.XNLEngine
{
    public class RegexParser<T> : IXNLParser<T> where T : XNLContext
    {
        #region IXNLParser 成员
        private static class Nested
        {
            internal static RegexParser<T> instance = new RegexParser<T>();
        }
        public static RegexParser<T> GetInstance()
        {
            return Nested.instance;
        }

        public XNLTagStruct GetTagStruct(string contentStr)
        {
            return RegxpEngineCommon<T>.GetTagStruct(contentStr);
        }
        public string RemoveXNLNotes(string contentStr)
        {
            return RegxpEngineCommon<T>.RemoveXNLNotes(contentStr);
        }

        public List<XNLToken> GetTagTokens(string contentStr)
        {
            return RegxpEngineCommon<T>.GetTagTokens(contentStr);
        }
        #endregion
    }
}
