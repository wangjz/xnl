using System.Collections.Generic;
using Com.AimUI.TagCore;
namespace Com.AimUI.TagParser
{
    public class RegexParser<T> : IParser<T> where T : TagContext
    {
        #region ITagParser 成员
        private static class Nested
        {
            internal static RegexParser<T> instance = new RegexParser<T>();
        }

        public static RegexParser<T> GetInstance()
        {
            return Nested.instance;
        }

        public void Initialize()
        {
            RegxpEngineCommon<T>.UpdateTagConfig();
        }

        public TagStruct GetTagStruct(string contentStr)
        {
            return RegxpEngineCommon<T>.GetTagStruct(contentStr);
        }
        public string RemoveNoteTag(string contentStr)
        {
            return RegxpEngineCommon<T>.RemoveTagNotes(contentStr);
        }

        public List<TagToken> GetTagTokens(string contentStr)
        {
            return RegxpEngineCommon<T>.GetTagTokens(contentStr);
        }
        public void SetValuePreActionChar(char valuePreActionChar)
        {
            RegxpEngineCommon<T>.SetValuePreActionChar(valuePreActionChar);
        }
        #endregion
    }
}