﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Com.AimUI.TagCore;
using Com.AimUI.TagCore.Exceptions;
namespace Com.AimUI.TagParser
{
    public class RegxpEngineCommon<T> where T : TagContext
    {
        internal static string TagRegNames;
        internal const RegexOptions Tag_RegexOptions = (((RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline) | RegexOptions.Multiline) | RegexOptions.IgnoreCase);

        internal const string RegexStr_TagNotes = @"<\!--\#.*?\#-->(?:\r?\n|)";  //匹配 aim 标签注释

        internal const string RegexStr_TagInnerTagParams = "([^\\s]+?)=\"([.\\s\\S]*?)\"";

        internal static string RegexStr_TagGroupAll;
        internal const string RegexTemplate_TagGroupAll = @"<(AT):([a-zA-Z0-9]+)(\#[a-zA-Z0-9][a-zA-Z0-9_]*|)(\s*|\s+.+?""\s*)>(?:\r?\n|)((?><\1:\2\3(?:\s*|\s+.+?""\s*)>(?<n>)|</\1:\2\3>(?<-n>)|(?!<\1:\2\3(?:\s*|\s+.+?""\s*)>|</\1:\2\3>)[.\s\S])*(?(n)(?!)))</\1:\2\3>(?:\r?\n|)";

        internal static string RegexStr_SubTagName2GroupAll;
        internal const string RegexTemplate_SubTagName2GroupAll = @"<(AT)(\#NAME|)(\s*|\s+.+?""\s*)>(?:\r?\n|)((?><\1\2(?:\s*|\s+.+?""\s*)>(?<n>)|</\1\2>(?<-n>)|(?!<\1\2(?:\s*|\s+.+?""\s*)>|</\1\2>)[.\s\S])*(?(n)(?!)))</\1\2>(?:\r?\n|)";

        private const string RegexStr_TagToken_Template = @"{([$])([:;]{0,1}[_a-zA-Z0-9]+.*?);{0,1}}(?:(?<=;})\r?\n|)";
        internal static string RegexStr_TagToken = RegexStr_TagToken_Template;

        private const string RegexStr_NestedToken_Template = @"[$]([:;]{0,1}[_a-zA-Z0-9][_a-zA-Z0-9\.:#\[\]]*?)\s*\(([^\(\)]*?)\)";
        internal static string RegexStr_NestedToken = RegexStr_NestedToken_Template;

        private const string RegexStr_TokenBody_Template = @"^[$][:;]{0,1}[_a-zA-Z0-9][_a-zA-Z0-9\.:#\[\]]*$";
        internal static string RegexStr_TokenBody = RegexStr_TokenBody_Template;

        private static string ValuePreActionChars = ":;";

        private static readonly char[] Char_Dot = new char[] { '.' };
        private static readonly char[] Chains_Char = new char[] { '.', '[' };
        /// <summary>
        /// 清除注释标签
        /// </summary>
        /// <param name="contentStr"></param>
        /// <returns></returns>
        internal static string RemoveTagNotes(string contentStr)
        {
            if (contentStr.Length < 13) return contentStr;

            return Regex.Replace(contentStr, RegexStr_TagNotes, "", Tag_RegexOptions);
        }

        public static void SetValuePreActionChar(char valuePreActionChar)
        {
            if (valuePreActionChar > 32 && valuePreActionChar < 48)
            {
                if (ValuePreActionChars.IndexOf(valuePreActionChar) == -1)
                {
                    ValuePreActionChars += valuePreActionChar;
                    RegexStr_TagToken = RegexStr_TagToken_Template.Replace("[:;]", "[" + ValuePreActionChars + "]");
                    RegexStr_NestedToken = RegexStr_NestedToken_Template.Replace("[:;]", "[" + ValuePreActionChars + "]");
                    RegexStr_TokenBody = RegexStr_TokenBody_Template.Replace("[:;]", "[" + ValuePreActionChars + "]");
                }
            }
            else
            {
                throw new Exception("字符的ascii码值需 >32 and <48");
            }
        }

        internal static TagParams GetTagParams(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (str.Length < 4) return null;
            TagParams tagParams = null;
            string t_paramName = null;
            string t_paramValue;
            MatchCollection paramColle = Regex.Matches(str, RegexStr_TagInnerTagParams, Tag_RegexOptions);
            if (paramColle.Count > 0)
            {
                tagParams = new TagParams();
                foreach (Match paramMatch in paramColle)
                {
                    t_paramName = paramMatch.Groups[1].Value.ToLower();
                    t_paramValue = paramMatch.Groups[2].Value;
                    tagParams.Add(t_paramName, t_paramValue);
                }
            }
            return tagParams;
        }

        internal static void UpdateTagConfig()
        {
            Regex.CacheSize = 30;
            TagRegNames = TagLib<T>.GetRegTagNameSpaces().Replace(".", "\\.");
            RegexStr_TagGroupAll = RegexTemplate_TagGroupAll.Replace("AT", TagRegNames);
            RegexStr_SubTagName2GroupAll = RegexTemplate_SubTagName2GroupAll.Replace("AT", "[_a-zA-Z0-9\\.]");
        }



        internal static MatchCollection MatchSubTagsGroupAllByName(string contentStr, string tagName, string tagObjName)
        {
            string regStr = GetSubTagRegStr(tagName, tagObjName);
            return Regex.Matches(contentStr, regStr, Tag_RegexOptions);
        }

        internal static string GetSubTagRegStr(string tagName, string tagObjName)
        {
            string regStr;
            if (string.IsNullOrEmpty(tagName))
            {
                regStr = RegexStr_SubTagName2GroupAll;
            }
            else
            {
                regStr = RegexTemplate_SubTagName2GroupAll.Replace("AT", tagName.Replace(".", "\\."));
            }

            if (string.IsNullOrEmpty(tagObjName) || tagObjName.StartsWith("__"))
            {
                regStr = regStr.Replace(@"\#NAME|", "");
            }
            else
            {
                regStr = regStr.Replace("NAME|", tagObjName.Replace(".", "\\."));
            }
            return regStr;
        }

        internal static TagStruct CreateTagStruct(Match tagGroupAllMatch)
        {
            string _nameSpace = tagGroupAllMatch.Groups[1].Value.ToLower();
            string _tagName = tagGroupAllMatch.Groups[2].Value.ToLower();
            string _tagInstanceName = tagGroupAllMatch.Groups[3].Value.Trim();//.ToLower();
            if (_tagInstanceName.StartsWith("#")) _tagInstanceName = _tagInstanceName.Substring(1);
            string tagParamStr = tagGroupAllMatch.Groups[4].Value;  //标签属性
            string tagContentStr = tagGroupAllMatch.Groups[5].Value;  //标签内容
            TagParams tagParams = RegxpEngineCommon<T>.GetTagParams(tagParamStr);

            TagStruct tagStruct = new TagStruct();
            tagStruct.nameSpace = _nameSpace;
            tagStruct.tagName = _tagName;

            tagStruct.instanceName = _tagInstanceName;
            tagStruct.tagParams = tagParams;
            tagStruct.bodyContent = tagContentStr;
            tagStruct.allContent = tagGroupAllMatch.Value;
            string subTagName = null;
            try
            {
                ITag<T> tagObj = TagLib<T>.GetTagInstance(_nameSpace, _tagName, false);
                subTagName = tagObj.subTagNames;
            }
            catch { }

            if (string.IsNullOrEmpty(subTagName) == false)
            {
                tagStruct.subTagStruct = GetSubTagStructs(tagContentStr, subTagName, _tagInstanceName);
            }
            return tagStruct;
        }

        internal static TagStruct CreateSubTagStruct(Match subTagGroupAllMatch)
        {
            string _tagName = subTagGroupAllMatch.Groups[1].Value;
            string _tagInstanceName = subTagGroupAllMatch.Groups[2].Value.Trim();//.ToLower();
            if (_tagInstanceName.StartsWith("#")) _tagInstanceName = _tagInstanceName.Substring(1);
            string tagParamStr = subTagGroupAllMatch.Groups[3].Value;  //标签属性
            string tagContentStr = subTagGroupAllMatch.Groups[4].Value;  //标签内容
            TagParams tagParams = RegxpEngineCommon<T>.GetTagParams(tagParamStr);

            TagStruct tagStruct = new TagStruct();
            tagStruct.tagName = _tagName;
            tagStruct.instanceName = _tagInstanceName;
            tagStruct.tagParams = tagParams;
            tagStruct.bodyContent = tagContentStr;
            tagStruct.allContent = subTagGroupAllMatch.Value;
            return tagStruct;
        }

        internal static List<TagStruct> GetSubTagStructs(string contentStr, string subTagName, string tagObjName)
        {
            if (string.IsNullOrEmpty(contentStr)) return null;
            if (string.IsNullOrEmpty(subTagName)) return null;
            if (contentStr.Length < 7) return null;
            MatchCollection matchs = MatchSubTagsGroupAllByName(contentStr, subTagName, tagObjName);
            int curIndex = 0;
            if (matchs.Count > 0)
            {
                List<TagStruct> list = new List<TagStruct>(matchs.Count * 2 + 1);
                TagStruct tagStruct;
                foreach (Match match in matchs)
                {
                    if (match.Index > curIndex)
                    {
                        tagStruct = new TagStruct();
                        tagStruct.allContent = contentStr.Substring(curIndex, match.Index - curIndex);
                        list.Add(tagStruct);
                    }
                    tagStruct = CreateSubTagStruct(match);
                    list.Add(tagStruct);
                    curIndex = match.Index + match.Length;
                }
                if (curIndex < contentStr.Length)
                {
                    tagStruct = new TagStruct();
                    tagStruct.allContent = contentStr.Substring(curIndex);
                    tagStruct.bodyContent = tagStruct.allContent;
                    list.Add(tagStruct);
                }
                return list;
            }
            return null;
        }

        internal static TagStruct GetTagStruct(string contentStr)
        {
            if (string.IsNullOrEmpty(contentStr)) return null;
            if (contentStr.Length < 11) return null;
            int index = 0;
            MatchCollection matchs = Regex.Matches(contentStr, RegexStr_TagGroupAll, Tag_RegexOptions);
            int counts = matchs.Count;
            if (counts > 0)
            {
                TagStruct tagStruct = null;
                Match tmpMatch = matchs[0];
                if (counts == 1 && tmpMatch.Index == 0 && tmpMatch.Length == contentStr.Length)
                {
                    tagStruct = CreateTagStruct(tmpMatch);
                }
                else
                {
                    tagStruct = new TagStruct();
                    tagStruct.allContent = contentStr;
                    tagStruct.subTagStruct = new List<TagStruct>();
                    for (int i = 0; i < counts; i++)
                    {
                        tmpMatch = matchs[i];
                        if (tmpMatch.Index > index)
                        {
                            var tmpStruct = new TagStruct();
                            tmpStruct.allContent = contentStr.Substring(index, tmpMatch.Index - index);
                            tagStruct.subTagStruct.Add(tmpStruct);
                        }
                        tagStruct.subTagStruct.Add(CreateTagStruct(tmpMatch));
                        index = tmpMatch.Index + tmpMatch.Length;
                    }
                    if (index < contentStr.Length)
                    {
                        var tmpStruct = new TagStruct();
                        tmpStruct.allContent = contentStr.Substring(index, contentStr.Length - index);
                        tagStruct.subTagStruct.Add(tmpStruct);
                    }
                }
                return tagStruct;
            }
            return null;
        }

        internal static List<TagToken> GetTagTokens(string contentStr, TagStruct tagStruct)
        {
            if (string.IsNullOrEmpty(contentStr)) return null;
            if (contentStr.Length < 4) return null;
            MatchCollection matchs = Regex.Matches(contentStr, RegexStr_TagToken);
            if (matchs.Count == 0) return null;
            List<TagToken> tokens = new List<TagToken>(matchs.Count);
            TagToken token = null;
            string tokenValue = null;
            ValuePreAction valuePreAction = ValuePreAction.NONE;
            foreach (Match match in matchs)
            {

                tokenValue = match.Groups[2].Value;
                //临时性修复包含 '}' 字符 解析错误
                int rInx = (match.Index + match.Value.Length);
                string trueValue = null;
                int? trueLen = null;
                if (tokenValue.IndexOf('(') != -1 && tokenValue.IndexOf(')') == -1 && contentStr.Length > rInx)
                {
                    int _inx1 = contentStr.IndexOf(')', rInx);
                    if (_inx1 != -1)
                    {
                        int _inx2 = contentStr.IndexOf('}', _inx1 + 1);
                        if (_inx2 != -1)
                        {
                            int subLen = _inx2 - _inx1;
                            if (subLen >= 1)
                            {
                                string sub = (subLen == 1 ? "" : null);
                                if (subLen > 1)
                                {
                                    sub = contentStr.Substring(_inx1 + 1, _inx2 - _inx1 - 1).Trim();
                                }
                                if (sub == "" || sub == ";")
                                {
                                    int addLen = _inx2 - rInx + 1;
                                    trueLen = match.Length + addLen;
                                    string added = contentStr.Substring(rInx, addLen);
                                    trueValue = match.Value + added;
                                    tokenValue = tokenValue + "}" + added.Substring(0, _inx1 - rInx + 1);
                                    if (sub == ";")
                                    {
                                        int newline = contentStr.Length - _inx2 - 1;
                                        if (newline >= 1 && contentStr[_inx2 + 1] == '\r')
                                        {
                                            trueLen += 1;
                                            trueValue += "\r";
                                            if (newline > 1 && contentStr[_inx2 + 2] == '\n')
                                            {
                                                trueLen += 1;
                                                trueValue += "\n";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                char act_char = tokenValue[0];
                if (act_char == (char)ValuePreAction.JSON_Serialize)
                {
                    valuePreAction = ValuePreAction.JSON_Serialize;
                    tokenValue = tokenValue.Remove(0, 1);
                }
                else if (act_char == (char)ValuePreAction.JSON_Deserialize)
                {
                    valuePreAction = ValuePreAction.JSON_Deserialize;
                    tokenValue = tokenValue.Remove(0, 1);
                }
                else if (act_char > 32 && act_char < 48)
                {
                    valuePreAction = ValuePreAction.USER_Defined;
                    tokenValue = tokenValue.Remove(0, 1);
                }
                else
                {
                    valuePreAction = ValuePreAction.NONE;
                }

                if ((tokenValue.Length == 1 && tokenValue[0] == '.') || tokenValue[tokenValue.Length - 1] == '.') continue;
                if (tokenValue.StartsWith("this[") || tokenValue.StartsWith("this."))
                {
                    if (tagStruct == null) throw new TagParseException("无效表达式:" + match.Value);
                    if (tokenValue.StartsWith("this["))
                    {
                        var inx = tokenValue.IndexOf(']');
                        if (inx == -1 || inx - 5 == 0) throw new TagParseException("无效表达式:" + tokenValue);
                        var inxName = tokenValue.Substring(5, inx - 5);
                        inxName = inxName.Trim('\'', '"');
                        tokenValue = inxName + (inx < tokenValue.Length - 1 ? tokenValue.Substring(inx + 1) : "");
                    }
                    else if (tokenValue.StartsWith("this."))
                    {
                        tokenValue = tokenValue.Substring(5);
                    }
                    if (tagStruct.nameSpace == null)
                    {
                        var rootTagStruct = tagStruct.parent;
                        while (rootTagStruct.tagName == null)
                        {
                            rootTagStruct = rootTagStruct.parent;
                        }
                        if (string.IsNullOrEmpty(rootTagStruct.instanceName))
                        {
                            tokenValue = rootTagStruct.nameSpace + ":" + rootTagStruct.tagName + "." + tokenValue;
                        }
                        else
                        {
                            tokenValue = rootTagStruct.nameSpace + ":" + rootTagStruct.tagName + "#" + rootTagStruct.instanceName + "." + tokenValue;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(tagStruct.instanceName))
                        {
                            tokenValue = tagStruct.nameSpace + ":" + tagStruct.tagName + "." + tokenValue;
                        }
                        else
                        {
                            tokenValue = tagStruct.nameSpace + ":" + tagStruct.tagName + "#" + tagStruct.instanceName + "." + tokenValue;
                        }
                    }
                }
                switch (match.Groups[1].Value)
                {
                    //case "@": //属性
                    //    token = GetAttrToken(tokenValue);
                    //    if (token == null) continue;
                    //    break;
                    case "$": // //表达式
                        token = GetExpression(tokenValue);
                        if (token == null) continue;
                        token.type = TagTokenType.Express;
                        break;
                }
                token.value = trueValue ?? match.Value;
                token.action = valuePreAction;
                token.index = match.Index;
                token.length = trueLen ?? match.Length;
                tokens.Add(token);
            }
            return tokens;
        }

        internal static void SetToken(TagToken token, string tokenValue, string args = null)
        {
            int inx = tokenValue.IndexOf('.');
            if (inx == 0 || inx == tokenValue.Length - 1)
            {
                return;
            }
            var oldValue = tokenValue;
            if (inx == -1)
            {
                token.name = tokenValue;
            }
            else
            {
                token.tagName = tokenValue.Substring(0, inx);
                oldValue = tokenValue.Substring(inx + 1);
                token.name = oldValue;
                inx = token.tagName.LastIndexOf(':');
                if (inx != -1)
                {
                    token.scope = token.tagName.Substring(0, inx);
                    token.tagName = token.tagName.Substring(inx + 1);
                }
                inx = token.tagName.IndexOf('#');
                if (inx != -1)
                {
                    token.instanceName = token.tagName.Substring(inx + 1);
                    token.tagName = token.tagName.Substring(0, inx);
                }
            }

            if (string.IsNullOrEmpty(args) == false)
            {
                token.args = GetTokenArgs(args);
            }

            if (token.args == null || token.args.Count == 0)
            {
                inx = oldValue.IndexOfAny(Chains_Char);
                if (inx != -1)
                {
                    token.chainsPath = GetChains(oldValue, inx);
                    token.name = oldValue.Substring(0, inx);
                }
            }
        }

        internal static string GetChains(string tokenString, int inx)
        {
            string args = tokenString.Substring(tokenString[inx] == '.' ? inx + 1 : inx);
            if (args != null && args.Length > 0)
            {
                return args;
            }
            return null;
        }

        internal static IList<TagToken> GetTokenArgs(string args)
        {
            if (string.IsNullOrEmpty(args) == false)
            {
                string names = null;
                List<TagToken> t_args = new List<TagToken>();
                string match_args = args;
                if (match_args.StartsWith("(") && match_args.EndsWith(")"))
                {
                    match_args = match_args.Substring(1, match_args.Length - 2);
                }

                //设置参数
                MatchCollection tokenMatchs;
                Dictionary<string, TagToken> nestedExp = null;
                Random ra = new Random();
                int random = ra.Next(10000, 90000);
                while (true)
                {
                    //匹配嵌套表达式
                    tokenMatchs = Regex.Matches(match_args, RegexStr_NestedToken, Tag_RegexOptions);
                    if (tokenMatchs.Count > 0)
                    {
                        //嵌套表达式
                        string _args = null;
                        //解析 暂存
                        foreach (Match m in tokenMatchs)
                        {
                            names = m.Groups[1].Value;
                            _args = m.Groups[2].Value;

                            TagToken tagToken = new TagExpression() { type = TagTokenType.Express };
                            //if (m.Value[0] == '@')
                            //{
                            //    tagToken = new TagToken() { type = TagTokenType.Attribute };
                            //}
                            //else
                            //{
                            //    tagToken = new TagExpression() { type = TagTokenType.Express };
                            //}

                            char act_char = m.Value[1];
                            if (act_char == (char)ValuePreAction.JSON_Serialize || act_char == (char)ValuePreAction.JSON_Deserialize || (act_char > 32 && act_char < 48))
                            {
                                if (act_char == (char)ValuePreAction.JSON_Serialize)
                                {
                                    tagToken.action = ValuePreAction.JSON_Serialize;
                                }
                                else if (act_char == (char)ValuePreAction.JSON_Deserialize)
                                {
                                    tagToken.action = ValuePreAction.JSON_Deserialize;
                                }
                                else
                                {
                                    tagToken.action = ValuePreAction.USER_Defined;
                                }
                                names = names.Substring(1);
                                tagToken.value = m.Value.Substring(2);
                            }
                            else
                            {
                                tagToken.value = m.Value.Substring(1);
                            }

                            tagToken.name = names;
                            //解析参数
                            if (string.IsNullOrEmpty(_args.Trim()) == false)
                            {
                                List<string> arg_list = GetArgList(_args);

                                tagToken.args = new List<TagToken>(arg_list.Count);
                                string _s;
                                TagToken _token;
                                for (var i = 0; i < arg_list.Count; i++)
                                {
                                    _s = arg_list[i].Trim();
                                    if (_s.Length > 1 && Regex.IsMatch(_s, RegexStr_TokenBody))
                                    {
                                        //if (_s.StartsWith("@"))
                                        //{
                                        //    _token = new TagToken() { type = TagTokenType.Attribute };
                                        //}
                                        //else 
                                        if (_s.StartsWith("$"))
                                        {
                                            _token = new TagExpression() { type = TagTokenType.Express };
                                        }
                                        else
                                        {
                                            _token = new TagToken() { type = TagTokenType.Common, value = _s };
                                        }
                                    }
                                    else
                                    {
                                        if ((_s.StartsWith("\"") && _s.EndsWith("\"")) || (_s.StartsWith("'") && _s.EndsWith("'")))
                                        {
                                            _s = _s.Substring(1, _s.Length - 2);
                                        }

                                        if (_s.IndexOf("~Exp~") != -1)
                                        {
                                            MatchCollection matchs = Regex.Matches(_s, "~Exp~[\\d]{5}");
                                            if (matchs.Count > 0)
                                            {
                                                if (_s.Length == 10)
                                                {
                                                    _token = nestedExp[_s];
                                                    tagToken.args.Add(_token);
                                                    continue;
                                                }

                                                _token = new TagToken() { type = TagTokenType.Common };
                                                _token.args = new List<TagToken>(matchs.Count);

                                                int _index = 0;
                                                foreach (Match match in matchs)
                                                {
                                                    if (match.Index > _index)
                                                    {
                                                        _token.args.Add(new TagToken() { type = TagTokenType.Common, value = _s.Substring(_index, match.Index - _index) });
                                                    }
                                                    _index = match.Index + match.Value.Length;
                                                    try
                                                    {
                                                        _token.args.Add(nestedExp[match.Value]);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                if (_s.Length > _index)
                                                {
                                                    _token.args.Add(new TagToken() { type = TagTokenType.Common, value = _s.Substring(_index) });
                                                }
                                                tagToken.args.Add(_token);
                                                continue;
                                            }
                                        }
                                        _token = new TagToken() { type = TagTokenType.Common, value = _s };
                                    }
                                    if (_token.type != TagTokenType.Common)
                                    {
                                        act_char = _s[1];
                                        if (act_char == (char)ValuePreAction.JSON_Serialize || act_char == (char)ValuePreAction.JSON_Deserialize || (act_char > 32 && act_char < 48))
                                        {
                                            if (act_char == (char)ValuePreAction.JSON_Serialize)
                                            {
                                                _token.action = ValuePreAction.JSON_Serialize;
                                            }
                                            else if (act_char == (char)ValuePreAction.JSON_Deserialize)
                                            {
                                                _token.action = ValuePreAction.JSON_Deserialize;
                                            }
                                            else
                                            {
                                                _token.action = ValuePreAction.USER_Defined;
                                            }
                                            _token.value = _s.Substring(2);
                                        }
                                        else
                                        {
                                            _token.value = _s.Substring(1);
                                        }
                                        SetToken(_token, _token.value);
                                    }
                                    tagToken.args.Add(_token);
                                }
                            }
                            SetToken(tagToken, tagToken.name);
                            //名称 参数
                            if (nestedExp == null) nestedExp = new Dictionary<string, TagToken>(StringComparer.OrdinalIgnoreCase);

                            string key = "~Exp~" + random.ToString();
                            random += 1;
                            nestedExp.Add(key, tagToken);
                            //替换表达式
                            match_args = match_args.Replace(m.Value, key);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                List<string> e_arr = GetArgList(match_args);
                string e_s;
                TagToken e_token;
                for (var i = 0; i < e_arr.Count; i++)
                {
                    e_s = e_arr[i].Trim();
                    if (e_s.Length > 1 && Regex.IsMatch(e_s, RegexStr_TokenBody))
                    {
                        //if (e_s.StartsWith("@"))
                        //{
                        //    e_token = new TagToken() { type = TagTokenType.Attribute };
                        //}
                        //else 
                        if (e_s.StartsWith("$"))
                        {
                            e_token = new TagExpression() { type = TagTokenType.Express };
                        }
                        else
                        {
                            e_token = new TagToken() { type = TagTokenType.Common, value = e_s };
                        }
                    }
                    else
                    {
                        if ((e_s.StartsWith("\"") && e_s.EndsWith("\"")) || (e_s.StartsWith("'") && e_s.EndsWith("'")))
                        {
                            e_s = e_s.Substring(1, e_s.Length - 2);
                        }

                        if (e_s.IndexOf("~Exp~") != -1)
                        {
                            MatchCollection matchs = Regex.Matches(e_s, "~Exp~[\\d]{5}");
                            if (matchs.Count > 0)
                            {
                                if (e_s.Length == 10)
                                {
                                    e_token = nestedExp[e_s];
                                    t_args.Add(e_token);
                                    continue;
                                }
                                e_token = new TagToken() { type = TagTokenType.Common };
                                e_token.args = new List<TagToken>(matchs.Count);
                                int _index = 0;
                                foreach (Match match in matchs)
                                {
                                    if (match.Index > _index)
                                    {
                                        e_token.args.Add(new TagToken() { type = TagTokenType.Common, value = e_s.Substring(_index, match.Index - _index) });
                                    }
                                    _index = match.Index + match.Value.Length;
                                    try
                                    {
                                        e_token.args.Add(nestedExp[match.Value]);
                                    }
                                    catch
                                    {
                                    }
                                }
                                if (e_s.Length > _index)
                                {
                                    e_token.args.Add(new TagToken() { type = TagTokenType.Common, value = e_s.Substring(_index) });
                                }
                                t_args.Add(e_token);
                                continue;
                            }
                        }
                        e_token = new TagToken() { type = TagTokenType.Common, value = e_s };
                    }
                    if (e_token.type != TagTokenType.Common)
                    {
                        char act_char = e_s[1];
                        if (act_char == (char)ValuePreAction.JSON_Serialize || act_char == (char)ValuePreAction.JSON_Deserialize || (act_char > 32 && act_char < 48))
                        {
                            if (act_char == (char)ValuePreAction.JSON_Serialize)
                            {
                                e_token.action = ValuePreAction.JSON_Serialize;
                            }
                            else if (act_char == (char)ValuePreAction.JSON_Deserialize)
                            {
                                e_token.action = ValuePreAction.JSON_Deserialize;
                            }
                            else
                            {
                                e_token.action = ValuePreAction.USER_Defined;
                            }
                            e_token.value = e_s.Substring(2);
                        }
                        else
                        {
                            e_token.value = e_s.Substring(1);
                        }
                        SetToken(e_token, e_token.value);
                    }
                    t_args.Add(e_token);
                }
                return t_args;
            }
            return null;
        }

        //获取表达式描述
        internal static TagExpression GetExpression(string expressionStr)
        {

            Match match = Regex.Match(expressionStr, @"^([_a-zA-Z0-9\.:#\[\]]+)(.*?)$", Tag_RegexOptions);
            if (match.Success)
            {
                TagExpression express = new TagExpression();
                SetToken(express, match.Groups[1].Value, match.Groups[2].Value.Trim());
                return express;
            }
            return null;
        }

        internal static List<string> GetArgList(string _args)
        {
            List<string> arg_list = new List<string>();
            if (_args.IndexOf(',') == -1)
            {
                arg_list.Add(_args);
            }
            else if (_args.IndexOf('\'') == -1 && _args.IndexOf('"') == -1)
            {
                arg_list.AddRange(_args.Split(','));
            }
            else
            {
                char prevChar = _args[0];
                int markPos = 0;
                int pairNum = 0;
                if (prevChar == ',')
                {
                    markPos = 1;
                    arg_list.Add("");
                }
                else if (prevChar == '\'' || prevChar == '"')
                {
                    pairNum = 1;

                }
                char curChar = prevChar;

                for (int c = 1; c < _args.Length; c++)
                {
                    curChar = _args[c];
                    if (curChar == '\'' || curChar == '"')
                    {
                        if (prevChar == curChar)
                        {
                            pairNum += 1;
                            if (c == _args.Length - 1)
                            {
                                arg_list.Add(_args.Substring(markPos, _args.Length - markPos));
                                markPos = _args.Length;
                            }
                        }
                        else if (prevChar == ',' || prevChar == ' ')
                        {
                            prevChar = curChar;
                            pairNum += 1;
                        }
                    }
                    else if (curChar == ',')
                    {
                        if (pairNum == 0 || pairNum == 2)
                        {
                            prevChar = curChar;
                            pairNum = 0;
                            arg_list.Add(_args.Substring(markPos, c - markPos));
                            markPos = c + 1;
                        }
                    }
                }
                if (markPos < _args.Length)
                {
                    arg_list.AddRange(_args.Substring(markPos, _args.Length - markPos).Split(','));
                }
            }
            return arg_list;
        }
    }
}