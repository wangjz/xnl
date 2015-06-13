using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Data;
using COM.SingNo.XNLCore;
using System.Security.Permissions;
using System.Reflection;
namespace COM.SingNo.XNLEngine
{
    public class RegxpEngineCommon<T> where T:XNLContext
    {
        internal static string XNLTagRegNames;
        internal const RegexOptions XNL_RegexOptions = (((RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline) | RegexOptions.Multiline) | RegexOptions.IgnoreCase);//|RegexOptions.Compiled
        
        internal const string RegexStr_XNLNotes = "<\\!--\\#.*?\\#-->";  //匹配XNL标签注释

        internal const string RegexStr_XNLInnerTagParams = "([^\\s]+?)=\"([.\\s\\S]*?)\"";
        internal const string RegexStr_XNLOutTagParams = @"<attrs>(?><attrs>(?<n>)|</attrs>(?<-n>)|(?! <attrs>|</attrs>)[.\\s\\S])*?(?(n)(?!))</attrs>";
        internal const string RegexStr_XNLOutTagParam = @"<attr\\s+([^><]*)>((?><attr\\s+[^><]*?>(?<n>)|</attr>(?<-n>)|(?! <attr\\s+([^><]*?)>|</attr>)[.\\s\\S])*?)(?(n)(?!))</attr>";
        internal const string RegexStr_XNLTagAttrName = "name=\"(.*?)\"";
        internal const string RegexStr_XNLTagAttrType = "type=\"(.*?)\"";

        internal static string RegexStr_XNLTagGroupAll;// = @"<(@XNL):([_a-zA-Z0-9]+)(::[a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>";
        internal static string RegexStr_XNLTagPart1Group0;// = @"<@XNL:[_a-zA-Z0-9]+(?:::[a-zA-Z0-9]+|)(?:[\s]*|[\s]+[^:>]+)>";
        internal static string RegexStr_XNLTagPart1Group3;// = @"<(@XNL):([_a-zA-Z0-9]+)(::[a-zA-Z0-9]+|)(?:[\s]*|[\s]+[^:>]+)>";
        internal static string RegexStr_XNLTagPart1Group4;// = @"<(@XNL):([_a-zA-Z0-9]+)(::[a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)>";
        

        internal const string RegexTemplate_XNLTagGroupAll = @"<(XNL):([_a-zA-Z0-9]+)(::[a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>";
        internal const string RegexTemplate_XNLTagPart1Group0 = @"<XNL:[_a-zA-Z0-9]+(?:::[a-zA-Z0-9]+|)(?:[\s]*|[\s]+[^:>]+)>";
        internal const string RegexTemplate_XNLTagPart1Group3 = @"<(XNL):([_a-zA-Z0-9]+)(::[a-zA-Z0-9]+|)(?:[\s]*|[\s]+[^:>]+)>";
        internal const string RegexTemplate_XNLTagPart1Group4 = @"<(XNL):([_a-zA-Z0-9]+)(::[a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)>";

       
        internal static string RegexStr_XNLSubTagName2GroupAll;
        internal const string RegexTemplate_XNLSubTagName2GroupAll = @"<(XNL)(::NAME|)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1\2>";

        //internal static string RegexStr_XNLTagWithName3GroupAll;
        //internal const string RegexTemplate_XNLTagWithName3GroupAll = @"<(XNL):(TAG)(::NAME)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>";

       
        internal static Regex RegexObj_XNLTagGroupAll;
        //internal static Regex RegexObj_XNLTagPart1Group0;
        internal static Regex RegexObj_XNLTagPart1Group3;
        //internal static Regex RegexObj_XNLTagPart1Group4;

        //internal static Regex RegexObj_XNLInnerTagParams=new Regex(RegexStr_XNLInnerTagParams,XNL_RegexOptions); //"([^\\s]+?)=\"([.\\s\\S]*?)\""
        //internal static Regex RegexObj_XNLOutTagParams = new Regex(RegexStr_XNLOutTagParams, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLOutTagParam = new Regex(RegexStr_XNLOutTagParam, XNL_RegexOptions);

        //internal static Regex RegexObj_XNLTagAttrName = new Regex(RegexStr_XNLTagAttrName, XNL_RegexOptions);// "name=\"(.*?)\"";
        //internal static Regex RegexObj_XNLTagAttrType = new Regex(RegexStr_XNLTagAttrType, XNL_RegexOptions);// "type=\"(.*?)\"";

        //internal static Regex RegexObj_XNLTagToken = new Regex(@"{([@$])([_a-zA-Z0-9\\.:]+)}");  //@#$%

        internal static Regex RegexObj_XNLTagToken = new Regex(@"{([@$])([_a-zA-Z0-9\.:]+.*?)}");  //{([@$])((?>{(?<n>)|}(?<-n>)|(?!{|}).)*?)(?(n)(?!))}
        /// <summary>
        /// 根据item项的内容，得到标签item项的所有属性
        /// </summary>
        /// <param name="attribStr"></param>
        /// <param name="itemContentStr"></param>
        /// <returns></returns>
        internal static XNLParams GetTagAllParams(string attribStr, string itemContentStr)
        {
            XNLParams labelParams = GetXNLParams(attribStr);
            XNLParams labelParams2 = GetXNLParams(itemContentStr);
            
            foreach (KeyValuePair<string, XNLParam> tmpParam in labelParams2)
            {
                labelParams[tmpParam.Key] = tmpParam.Value;
            }
            return labelParams;
        }
        
       
        /// <summary>
        /// 清除XNL注释标签
        /// </summary>
        /// <param name="contentStr"></param>
        /// <returns></returns>
        internal static string RemoveXNLNotes(string contentStr)
        {
            return Regex.Replace(contentStr, RegexStr_XNLNotes, "", XNL_RegexOptions);
        }


        internal static XNLParams GetXNLParams(string str) //, T xnlContext
        {
            if (string.IsNullOrEmpty(str)) return null;
            XNLParams xnlParams=null;
            XNLParam tmpxnlparam;
            string t_paramName=null;
            object t_paramValue;
            XNLType t_type;
            string trimStr = str.Trim();
            if (trimStr.IndexOf("<attrs>") == 0)
            {
                if (trimStr.EndsWith("</attrs>")==false)
                {
                    Match match = MatchXNLAttrsTag(str);
                    if (match.Success)
                    {
                        trimStr = match.Value;
                    }
                }
                xnlParams = new XNLParams();
                MatchCollection subColls = MatchXNLAttrTags(trimStr);
                foreach (Match subMatch in subColls)
                {
                    Match nameMatch = Regex.Match(subMatch.Groups[1].Value, RegexStr_XNLTagAttrName, XNL_RegexOptions);// RegexObj_XNLTagAttrName.Match(subMatch.Groups[1].Value);
                    Match typeMatch = Regex.Match(subMatch.Groups[1].Value, RegexStr_XNLTagAttrType, XNL_RegexOptions);// RegexObj_XNLTagAttrType.Match(subMatch.Groups[1].Value);
                    if (nameMatch.Success)
                    {
                        t_paramName = nameMatch.Groups[1].Value.ToLower();
                    }

                    if (typeMatch.Success)
                    {
                        t_type = XNLParam.getTypeFromTypeStr(typeMatch.Groups[1].Value);
                    }
                    //t_paramValue =parse(subMatch.Groups[2].Value, xnlContext, false,false);
                    t_paramValue = subMatch.Groups[2].Value;
                    tmpxnlparam = new XNLParam(t_paramValue);
                    xnlParams.Add(t_paramName, tmpxnlparam);
                }
            }
            else
            {
                MatchCollection paramColle = Regex.Matches(str, RegexStr_XNLInnerTagParams, XNL_RegexOptions);// RegxpEngineCommon<T>.RegexObj_XNLInnerTagParams.Matches(str);
                if (paramColle.Count > 0)
                {
                    xnlParams = new XNLParams();
                    foreach (Match paramMatch in paramColle)
                    {
                        t_paramName = paramMatch.Groups[1].Value.ToLower();
                        t_paramValue = paramMatch.Groups[2].Value;
                        tmpxnlparam = new XNLParam(t_paramValue);
                        xnlParams.Add(t_paramName, tmpxnlparam);
                    }
                }
            }
            return xnlParams;
        }
       
        internal static Match MatchXNLAttrsTag(string str)
        {
            Match match = Regex.Match(str, RegexStr_XNLOutTagParams, XNL_RegexOptions);// RegexObj_XNLOutTagParams.Match(str);
            return match;
        }
        internal static MatchCollection MatchXNLAttrTags(string str)
        {
            MatchCollection colls = Regex.Matches(str, RegexStr_XNLOutTagParam, XNL_RegexOptions);// RegxpEngineCommon<T>.RegexObj_XNLOutTagParam.Matches(str);
            return colls;
        }

        public static void UpdateXNLConfig()
        {
            Regex.CacheSize = 30;
            XNLTagRegNames = XNLLib<T>.GetRegTagNameSpaces().Replace(".", "\\.");
            RegexStr_XNLTagGroupAll = RegexTemplate_XNLTagGroupAll.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLTagPart1Group0 = RegexTemplate_XNLTagPart1Group0.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLTagPart1Group3 = RegexTemplate_XNLTagPart1Group3.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLTagPart1Group4 = RegexTemplate_XNLTagPart1Group4.Replace("XNL", XNLTagRegNames);

           // RegexStr_XNLTagWithName3GroupAll = RegexTemplate_XNLTagWithName3GroupAll.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLSubTagName2GroupAll = RegexTemplate_XNLSubTagName2GroupAll.Replace("XNL", "[_a-zA-Z0-9\\.]");

            RegxpEngineCommon<T>.RegexObj_XNLTagGroupAll = new Regex(RegexStr_XNLTagGroupAll, XNL_RegexOptions);
            //RegxpEngineCommon<T>.RegexObj_XNLTagPart1Group0 = new Regex(RegxpEngineCommon<T>.RegexStr_XNLTagPart1Group0.Replace("@XNL", XNLLib<T>.getRegTagNameSpaces()), XNL_RegexOptions);
            RegxpEngineCommon<T>.RegexObj_XNLTagPart1Group3 = new Regex(RegexStr_XNLTagPart1Group3, XNL_RegexOptions);
            //RegxpEngineCommon<T>.RegexObj_XNLTagPart1Group4 = new Regex(RegxpEngineCommon<T>.RegexStr_XNLTagPart1Group4.Replace("@XNL", XNLLib<T>.getRegTagNameSpaces()), XNL_RegexOptions);
        }

        

        internal static MatchCollection MatchSubTagsGroupAllByName(string contentStr,  string tagName, string tagObjName)
        {
            string regStr = GetSubTagRegStr(tagName, tagObjName);
            return Regex.Matches(contentStr, regStr, XNL_RegexOptions);
        }
        internal static string GetSubTagRegStr(string tagName, string tagObjName)
        {
            string regStr;
            if (string.IsNullOrEmpty(tagName))
            {
                regStr = RegexStr_XNLSubTagName2GroupAll;//.Replace("@XNL", "[_a-zA-Z0-9\\.]");
            }
            else
            {
                regStr = RegexTemplate_XNLSubTagName2GroupAll.Replace("XNL", tagName.Replace(".", "\\."));
            }
            if (string.IsNullOrEmpty(tagObjName)||tagObjName.StartsWith("_instance#"))
            {
                regStr = regStr.Replace("::NAME|", "");
            }
            else
            {
                regStr = regStr.Replace("NAME",  tagObjName.Replace(".", "\\."));
            }
            return regStr;
        }

        internal static XNLTagStruct CreateTagStruct(Match tagGroupAllMatch)
        {
            string _nameSpace = tagGroupAllMatch.Groups[1].Value;
            string _tagName = tagGroupAllMatch.Groups[2].Value;
            string _tagInstanceName = tagGroupAllMatch.Groups[3].Value.Trim();//.ToLower();
            if (_tagInstanceName.StartsWith("::")) _tagInstanceName = _tagInstanceName.Substring(2);
            string tagParamStr = tagGroupAllMatch.Groups[4].Value;  //标签属性
            string tagContentStr = tagGroupAllMatch.Groups[5].Value;  //标签内容
            XNLParams xnlParams = RegxpEngineCommon<T>.GetXNLParams(tagParamStr);
            Match attrsMatch = RegxpEngineCommon<T>.MatchXNLAttrsTag(tagContentStr);
            if (attrsMatch.Success)
            {
                tagContentStr = tagContentStr.Substring(attrsMatch.Index + attrsMatch.Length); //UtilsCode.onceReplace(tagContentStr, attrsMatch.Value, "");
                XNLParams params2 = RegxpEngineCommon<T>.GetXNLParams(attrsMatch.Value);
                if (params2 != null && xnlParams != null)
                {
                    foreach (KeyValuePair<string, XNLParam> kv in params2)
                    {
                        xnlParams[kv.Key] = kv.Value;
                    }
                }
            }
            XNLTagStruct tagStruct = new XNLTagStruct();
            tagStruct.nameSpace = _nameSpace;
            tagStruct.tagName = _tagName;

            tagStruct.instanceName = _tagInstanceName;
            tagStruct.tagParams = xnlParams;
            tagStruct.bodyContent = tagContentStr;
            tagStruct.allContent = tagGroupAllMatch.Value;
            IXNLTag<T> tagObj = XNLLib<T>.GetTagInstance(_nameSpace, _tagName);
            string subTagName = tagObj.subTagNames;
            //if (subTagName == null) subTagName = "";
            if (string.IsNullOrEmpty(subTagName)==false)
            {
                tagStruct.subTagStruct = GetSubTagStructs(tagContentStr, subTagName, _tagInstanceName);
            }
            return tagStruct;
        }

        internal static XNLTagStruct CreateSubTagStruct(Match subTagGroupAllMatch) //, T xnlcontext
        {
            string _tagName = subTagGroupAllMatch.Groups[1].Value;
            string _tagInstanceName = subTagGroupAllMatch.Groups[2].Value.Trim();//.ToLower();
            if (_tagInstanceName.StartsWith("::")) _tagInstanceName = _tagInstanceName.Substring(2);
            string tagParamStr = subTagGroupAllMatch.Groups[3].Value;  //标签属性
            string tagContentStr = subTagGroupAllMatch.Groups[4].Value;  //标签内容
            XNLParams xnlParams = RegxpEngineCommon<T>.GetXNLParams(tagParamStr);
            Match attrsMatch = RegxpEngineCommon<T>.MatchXNLAttrsTag(tagContentStr);
            if (attrsMatch.Success)
            {
                tagContentStr = tagContentStr.Substring(attrsMatch.Index + attrsMatch.Length);
                XNLParams params2 = RegxpEngineCommon<T>.GetXNLParams(attrsMatch.Value);
                if (params2 != null && xnlParams != null)
                {
                    foreach (KeyValuePair<string, XNLParam> kv in params2)
                    {
                        xnlParams[kv.Key] = kv.Value;
                    }
                }
            }
            XNLTagStruct tagStruct = new XNLTagStruct();
            tagStruct.tagName = _tagName;
            tagStruct.instanceName = _tagInstanceName;
            tagStruct.tagParams = xnlParams;
            tagStruct.bodyContent = tagContentStr;
            tagStruct.allContent = subTagGroupAllMatch.Value;
            return tagStruct;
        }

        internal static List<XNLTagStruct> GetSubTagStructs(string contentStr, string subTagName, string tagObjName) //, T xnlContext
        {
            if (string.IsNullOrEmpty(contentStr)) return null;
            if (string.IsNullOrEmpty(subTagName)) return null;
            MatchCollection matchs = MatchSubTagsGroupAllByName(contentStr, subTagName, tagObjName);
            int curIndex = 0;
            if (matchs.Count > 0)
            {
                List<XNLTagStruct> list = new List<XNLTagStruct>(matchs.Count*2+1); //
                XNLTagStruct tagStruct;
                foreach (Match match in matchs)
                {
                    if (match.Index > curIndex)
                    {
                        tagStruct = new XNLTagStruct();
                        tagStruct.allContent = contentStr.Substring(curIndex, match.Index - curIndex);
                        list.Add(tagStruct);
                    }
                    tagStruct = CreateSubTagStruct(match);
                    /*
                    string tmpSubTagName=tagObj.getSubTagNames(subTagName);
                    if (!string.IsNullOrEmpty(tmpSubTagName))
                    {
                        tagStruct.subTagStruct = getSubTagStructs(tagStruct.bodyContent, tagObj, tmpSubTagName, tagObjName,xnlContext);
                    }
                    */ 
                    list.Add(tagStruct);
                    curIndex = match.Index + match.Length;
                }
                if (curIndex < contentStr.Length)
                {
                    tagStruct = new XNLTagStruct();
                    tagStruct.allContent = contentStr.Substring(curIndex);
                    tagStruct.bodyContent = tagStruct.allContent;
                    list.Add(tagStruct);
                }
                return list;
            }
            return null;
        }
       
        internal static XNLTagStruct GetTagStruct(string contentStr)
        {
            if (string.IsNullOrEmpty(contentStr)) return null;
            XNLTagStruct tagStruct = null;
            //string namespaceName;
            //string tagName;
            int index = 0;
            //string tagGroup0Str;
            //string tagGroup5Str;
            MatchCollection matchs =RegxpEngineCommon<T>.RegexObj_XNLTagGroupAll.Matches(contentStr);
            int counts = matchs.Count;
            if (counts == 0)
            {
                tagStruct = new XNLTagStruct();
                tagStruct.allContent = contentStr;
            }
            else
            {
                Match tmpMatch = matchs[0];
                if (counts == 1 && tmpMatch.Index == 0 && tmpMatch.Length == contentStr.Length)
                {
                    tagStruct=CreateTagStruct(tmpMatch);
                }
                else
                {
                    tagStruct = new XNLTagStruct();
                    tagStruct.allContent = contentStr;
                    tagStruct.subTagStruct = new List<XNLTagStruct>();
                    for (int i = 0; i < counts; i++)
                    {
                        tmpMatch = matchs[i];
                        if (tmpMatch.Index > index)
                        {
                            var tmpStruct = new XNLTagStruct();
                            tmpStruct.allContent = contentStr.Substring(index, tmpMatch.Index - index);
                            tagStruct.subTagStruct.Add(tmpStruct);
                        }
                        tagStruct.subTagStruct.Add(CreateTagStruct(tmpMatch));
                        index = tmpMatch.Index + tmpMatch.Length;
                    }
                    if(index<contentStr.Length)
                    {
                        var tmpStruct = new XNLTagStruct();
                        tmpStruct.allContent = contentStr.Substring(index, contentStr.Length - index);
                        tagStruct.subTagStruct.Add(tmpStruct);
                    }
                }
            }
            
            return tagStruct;
        }

        internal static List<XNLToken> GetTagTokens(string contentStr)
        {
            if (string.IsNullOrEmpty(contentStr)) return null;
            MatchCollection matchs = RegexObj_XNLTagToken.Matches(contentStr);
            if (matchs.Count == 0) return null;
            List<XNLToken> tokens = new List<XNLToken>(matchs.Count);
            XNLToken token = null;
            string tokenValue = null;
            int dotInx = -1;
            ParseMode mode = ParseMode.Dynamic;
            foreach(Match match in matchs)
            {
                tokenValue = match.Groups[2].Value;
                mode = ParseMode.Dynamic;
                if (tokenValue.IndexOf(':') == 0)  //maybe use &
                {
                    mode = ParseMode.Static;
                    tokenValue = tokenValue.Remove(0, 1);
                }

                dotInx = tokenValue.LastIndexOf('.');
                if (dotInx == 0 || dotInx == tokenValue.Length - 1)
                {
                    continue;
                }

                switch (match.Groups[1].Value)
                {
                    case "@": //属性
                        token = new XNLToken();
                        token.type = XNLTokenType.Attribute;
                        if (dotInx == -1)
                        {
                            token.name = tokenValue;
                            token.scope = string.Empty;
                        }
                        else
                        {
                            token.scope = tokenValue.Substring(0, dotInx);
                            token.name = tokenValue.Substring(dotInx + 1);
                        }
                        break;
                    case "$": // //表达式  统一概念 无参数 变量表达式  有参数  方法表达式  {$site.url}  "{$isemail({$siteurl},abc)}"  {$indexof({@a},)}
                        token = GetExpression(tokenValue);
                        if (token == null) continue;
                        token.type = XNLTokenType.Express;
                        break;
                }

                token.value = match.Value;
                token.mode = mode;

                token.index = match.Index;
                token.length = match.Length;
                tokens.Add(token);
            }
            return tokens;
        }


        //获取表达式描述,  将表达式 转为 XNLExpression
        internal static XNLExpression GetExpression(string ExpressionStr)
        {
            
            Match match = Regex.Match(ExpressionStr, @"^([_a-zA-Z0-9\.:]+)(.*?)$", XNL_RegexOptions);
            if (match.Success)
            {
                XNLExpression express = new XNLExpression();

                string names = match.Groups[1].Value;
                var inx = names.LastIndexOf(':');
                if (inx == 0 || inx == names.Length - 1)
                {
                    return null;
                }
                express.name = names;
                if (inx != -1)
                {
                    express.scope = names.Substring(0, inx);
                    names = names.Substring(inx + 1);
                }

                //分离  tag name and exp
                inx = names.LastIndexOf('.');
                if (inx == 0 || inx == names.Length - 1)
                {
                    return null;
                }

                if (inx!=-1)
                {
                    express.tagName = names.Substring(0, inx);
                    express.name = names.Substring(inx + 1);
                }

                string args = match.Groups[2].Value.Trim();
                if (string.IsNullOrEmpty(args) == false)
                {
                    char[] trims = new char[] { '\'', '"' };
                    express.args = new List<XNLToken>();
                    string match_args = args.Trim(new char[] { '(', ')' });
                    //设置参数
                    MatchCollection expMatchs;
                    Dictionary<string, XNLExpression> nestedExp=null;
                    while(true)
                    {
                        //匹配嵌套表达式
                        //\$\s*([_a-zA-Z0-9\.:]+)\(([^\(\)]*?)\)
                        expMatchs = Regex.Matches(match_args, @"(?::|)\$\s*([_a-zA-Z0-9\.:]+)\(([^\(\)]*?)\)", XNL_RegexOptions);
                        if (expMatchs.Count > 0)
                        {
                            //嵌套表达式
                            string _args = null;
                            //解析 暂存
                            foreach (Match m in expMatchs)
                            {
                               
                                var _express = new XNLExpression();
                                names = m.Groups[1].Value;
                                _args = m.Groups[2].Value;

                                if(m.Value.StartsWith(":"))
                                {
                                    _express.mode = ParseMode.Static;
                                    _express.value = m.Value.Substring(2);
                                }
                                else
                                {
                                    _express.value = m.Value.Substring(1);
                                }
                                //_express.index = m.Index;
                                //_express.length = m.Value.Length;
                                
                                _express.name = names;
                                inx = names.LastIndexOf(':');

                                if (inx != -1)
                                {
                                    _express.scope = names.Substring(0, inx);
                                    names = names.Substring(inx + 1);
                                }

                                //分离  tag name and exp
                                inx = names.LastIndexOf('.');

                                if (inx != -1)
                                {
                                    _express.tagName = names.Substring(0, inx);
                                    _express.name = names.Substring(inx + 1);
                                }

                                //解析参数
                                if (string.IsNullOrEmpty(_args.Trim()) == false)
                                {
                                   
                                    string[] s_arr = _args.Split(',');

                                    _express.args = new List<XNLToken>(s_arr.Length);
                                    string _s;
                                    XNLToken _token;
                                    for (var i = 0; i < s_arr.Length; i++)
                                    {

                                        _s = s_arr[i].Trim();
                                        if(_s.StartsWith("@") || _s.StartsWith(":@"))
                                        {
                                            _token = new XNLToken() { type = XNLTokenType.Attribute };
                                        }
                                        else if (_s.StartsWith("$") || _s.StartsWith(":$"))
                                        {
                                            _token = new XNLExpression() { type = XNLTokenType.Express};
                                        }
                                        else
                                        {
                                            if ((_s.StartsWith("\"") && _s.EndsWith("\"")) || _s.StartsWith("'") && _s.EndsWith("'"))
                                            {
                                                _s = _s.Trim(trims);
                                            }
                                            else if (_s.StartsWith("~Exp~"))
                                            {
                                                _token = nestedExp[_s];
                                                _express.args.Add(_token);
                                                continue;
                                            }
                                            _token = new XNLToken() { type = XNLTokenType.Common, value = _s };
                                        }
                                        if (_token.type != XNLTokenType.Common)
                                        {
                                            if (_s[0] == ':')
                                            {
                                                _token.mode = ParseMode.Static;
                                                _token.value = _s.Substring(2);
                                            }
                                            else
                                            {
                                                _token.value = _s.Substring(1);
                                            }
                                            if (_token.type == XNLTokenType.Express)
                                            {
                                                inx = _token.value.LastIndexOf(':');
                                                names = _token.value;
                                                _token.name = names;
                                                if (inx != -1)
                                                {
                                                    _token.scope = _token.value.Substring(0, inx);
                                                    names = _token.value.Substring(inx + 1);
                                                }

                                                //分离  tag name and exp
                                                inx = names.LastIndexOf('.');

                                                if (inx != -1)
                                                {
                                                    ((XNLExpression)_token).tagName = names.Substring(0, inx);
                                                    _token.name = names.Substring(inx + 1);
                                                }
                                            }
                                            else
                                            {
                                                inx = _token.value.LastIndexOf('.');
                                                if (inx == -1)
                                                {
                                                    _token.name = _token.value;
                                                }
                                                else
                                                {
                                                    _token.scope = _token.value.Substring(0, inx);
                                                    _token.name = _token.value.Substring(inx + 1);
                                                }
                                            }
                                        }
                                        
                                        _express.args.Add(_token);
                                    }
                                }
                                //名称 参数
                                if (nestedExp == null) nestedExp = new Dictionary<string, XNLExpression>();
                                Random ra = new Random(unchecked((int)DateTime.Now.Ticks));//保证产生的数字的随机性 
                                string r = ra.Next().ToString();
                                string key = "~Exp~" + r;

                                nestedExp.Add(key, _express);
                                //替换表达式
                                match_args = match_args.Replace(m.Value, key);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    //split match_args
                    string[] e_arr = match_args.Split(',');
                    string e_s;
                    XNLToken e_token;
                    for (var i = 0; i < e_arr.Length; i++)
                    {
                        e_s = e_arr[i].Trim();
                        if (e_s.StartsWith("@") || e_s.StartsWith(":@"))
                        {
                            e_token = new XNLToken() { type = XNLTokenType.Attribute }; //, value = e_s.Remove(0, 1)
                        }
                        else if (e_s.StartsWith("$") || e_s.StartsWith(":@"))
                        {
                            e_token = new XNLExpression() { type = XNLTokenType.Express }; //, value = e_s.Remove(0, 1) 
                        }
                        else
                        {
                            if ((e_s.StartsWith("\"") && e_s.EndsWith("\"")) || e_s.StartsWith("'") && e_s.EndsWith("'"))
                            {
                                e_s = e_s.Trim(trims);
                            }
                            else if (e_s.StartsWith("~Exp~"))
                            {
                                e_token = nestedExp[e_s];
                                express.args.Add(e_token);
                                continue;
                            }
                            e_token = new XNLToken() { type = XNLTokenType.Common, value = e_s };
                        }
                        if (e_token.type != XNLTokenType.Common)
                        {
                            if (e_s[0] == ':')
                            {
                                e_token.mode = ParseMode.Static;
                                e_token.value = e_s.Substring(2);
                            }
                            else
                            {
                                e_token.value = e_s.Substring(1);
                            }
                            if (e_token.type == XNLTokenType.Express)
                            {
                                inx = e_token.value.LastIndexOf(':');
                                names = e_token.value; 
                                if (inx != -1)
                                {
                                    e_token.scope = e_token.value.Substring(0, inx);
                                    names = e_token.value.Substring(inx + 1);
                                }

                                //分离  tag name and exp
                                inx = names.LastIndexOf('.');

                                if (inx != -1)
                                {
                                    ((XNLExpression)e_token).tagName = names.Substring(0, inx);
                                    e_token.name = names.Substring(inx + 1);
                                }
                            }
                            else
                            {
                                inx = e_token.value.LastIndexOf('.');
                                if (inx == -1)
                                {
                                    e_token.name = e_token.value;
                                }
                                else
                                {
                                    e_token.scope = e_token.value.Substring(0, inx);
                                    e_token.name = e_token.value.Substring(inx + 1);
                                }
                            }
                        }
                        express.args.Add(e_token);
                    }
                }
                
                return express;
            }
            
            return null;
        }
    }
}


/*
 * //^([^\.]*)\((.*)?\)$
            //^(.*)\.([^()]*)$|^(.*)\.(.*)\((.*)\)$
            //string tmpExpressStr = ExpressionStr.Trim();
            //tmpExpressStr = XNLCommon.decodeHTMLEscapecharacter(tmpExpressStr);
            //tmpExpressStr = XNLCommon.decodeXNLEscapecharacter(tmpExpressStr);
            //尝试其它表达式
            try
            {
                #region 计算表达式
                Match excMatch = Regex.Match(ExpressionStr, "^([^\\.]*)\\((.*)?\\)$", XNL_RegexOptions);
                if (excMatch.Success) //内置表达式  xnl.expression
                {
                    string funcStr = excMatch.Groups[1].Value.Trim().ToLower();
                    string valueStrs = excMatch.Groups[2].Value;
                    string s = funcStr.Substring(0, 1);
                    MatchCollection valueMatchColl = Regex.Matches(valueStrs, "(\')([^\']*)?\'|(\")([^\"]*)?\"|(true|false)|([+-]?\\d*[.]?\\d*)", XNL_RegexOptions);
                    Dictionary<string, string> paramsDirectory = new Dictionary<string, string>();
                    int matchCount = 0;
                    foreach (Match match in valueMatchColl)
                    {
                        if (match.Groups[0].Value.Trim().Equals(string.Empty)) continue;
                        matchCount += 1;
                        if (match.Groups[1].Value == "'")
                        {
                            paramsDirectory.Add(Convert.ToString(matchCount), match.Groups[2].Value);
                        }
                        else if (match.Groups[3].Value == "\"")
                        {
                            paramsDirectory.Add(Convert.ToString(matchCount), match.Groups[4].Value);
                        }
                        else
                        {
                            if (match.Groups[5].Value.Trim().ToLower() == "true" || match.Groups[5].Value.Trim().ToLower() == "false")
                            {
                                paramsDirectory.Add(Convert.ToString(matchCount), match.Groups[5].Value);
                            }
                            else
                            {
                                paramsDirectory.Add(Convert.ToString(matchCount), match.Groups[6].Value);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
 Match excXnlMatch = Regex.Match(ExpressionStr, "^(.*)\\.([^()]*)$|^(.*)\\.(.*)\\((.*)\\)$", XNL_RegexOptions);
                    if (excXnlMatch.Success)
                    {
                        if (excXnlMatch.Groups[1].Value.Trim() != "") //xnl.label
                        {
                            string libNameStr = excXnlMatch.Groups[1].Value;
                            string labelNameStr = excXnlMatch.Groups[2].Value;
                            string returnStr = ParserEngine<T>.parse("<" + libNameStr + ":" + labelNameStr + "></" + libNameStr + ":" + labelNameStr + ">", XNLPage);
                            return returnStr;
                        }
                        else  //xnl.labelname('hjgj','jh')
                        {
                            string libNameStr = excXnlMatch.Groups[3].Value;
                            string labelNameStr = excXnlMatch.Groups[4].Value;
                            string valueStrs = excXnlMatch.Groups[5].Value;
                            MatchCollection valueMatchColl = Regex.Matches(valueStrs, "(\')([^\']*)?\'|(\")([^\"]*)?\"|(true|false)|([+-]?\\d*[.]?\\d*)", XNL_RegexOptions);
                            int matchCount = 0;
                            string paramStr = "<attrs>";
                            foreach (Match match in valueMatchColl)
                            {
                                if (match.Groups[0].Value.Trim().Equals(string.Empty)) continue;
                                matchCount += 1;
                                if (match.Groups[1].Value == "'")
                                {
                                    paramStr += "<attr name=\"param" + Convert.ToString(matchCount) + "\" >" + UtilsCode.encodeHtmlAndXnl(match.Groups[2].Value) + "</attr>";
                                }
                                else if (match.Groups[3].Value == "\"")
                                {
                                    paramStr += "<attr name=\"param" + Convert.ToString(matchCount) + "\" >" + UtilsCode.encodeHtmlAndXnl(match.Groups[4].Value) + "</attr>";
                                }
                                else
                                {
                                    if (match.Groups[5].Value.Trim().ToLower() == "true" || match.Groups[5].Value.Trim().ToLower() == "false")
                                    {
                                        paramStr += "<attr name=\"param" + Convert.ToString(matchCount) + "\" >" + UtilsCode.encodeHtmlAndXnl(match.Groups[5].Value) + "</attr>";
                                    }
                                    else
                                    {
                                        paramStr += "<attr name=\"param" + Convert.ToString(matchCount) + "\" >" + UtilsCode.encodeHtmlAndXnl(match.Groups[6].Value) + "</attr>";
                                    }
                                }
                            }
                            paramStr += "</attrs>";
                            string returnStr = ParserEngine<T>.parse("<" + libNameStr + ":" + labelNameStr + ">" + paramStr + "</" + libNameStr + ":" + labelNameStr + ">", XNLPage);
                            return returnStr;
                        }
            }
 * /

//internal static XNLTagStruct getTagStruct(string contentStr, T xnlContext, string nameSpaceName, string tagName, string tagObjName)
//{
//    return createTagStruct(matchTagGroupAllByName(contentStr, nameSpaceName, tagName, tagObjName), xnlContext);
//}

//internal static List<XNLTagStruct> getTagStructs(string contentStr, T xnlContext, string nameSpaceName, string tagName, string tagObjName)
//{
//    MatchCollection matchs = matchTagsGroupAllByName(contentStr, nameSpaceName, tagName, tagObjName);
//    if (matchs.Count > 0)
//    {
//        List<XNLTagStruct> list = new List<XNLTagStruct>(matchs.Count);
//        foreach (Match match in matchs)
//        {
//            list.Add(createTagStruct(match, xnlContext));
//        }
//        return list;
//    }
//    return null;
//}



/*
 internal static string DataTableCompute(string ExpressionStr)
        {
            DataTable dt = new DataTable();
            object returnObj;
            //string regex = "\'(?>[\\(,]\\s*\'(?<n>)|\'\\s*[,\\)](?<-n>)|(?!\'\\s*[,\\)]).)*(?(n)(?!))\'";
            //1.先判断字符串中是否有"字符
            if (ExpressionStr.IndexOf("\"") >= 2)
            {
                MatchCollection matchColl2 = Regex.Matches(ExpressionStr, "(?<=[,\\(])\"(?>[\\(,]\\s*\"(?<n>)|\"\\s*[,\\)](?<-n>)|(?!\"\\s*[,\\)]).)*(?(n)(?!))\"(?=[,\\)])", XNL_RegexOptions);
                //MatchCollection matchColl2 = Regex.Matches(ExpressionStr, "(?<=[,\\(])\"(?>[\\(,]\\s*\"(?<n>)|\"\\s*[,\\)](?<-n>)|(?!\"\\s*[,\\)]).)*(?(n)(?!))\"(?=[,\\)])", XNLCommon.MatchCompiledOptions);
                if (matchColl2.Count > 0)  //有" " 的匹配
                {
                    //判断是否有'字符
                    if (ExpressionStr.IndexOf("'") >= 2)
                    {
                        MatchCollection matchColl = Regex.Matches(ExpressionStr, "(?<=[,\\(])\'(?>[\\(,]\\s*\'(?<n>)|\'\\s*[,\\)](?<-n>)|(?!\'\\s*[,\\)]).)*(?(n)(?!))\'(?=[,\\)])", XNL_RegexOptions);
                        if (matchColl.Count > 0)
                        {
                            Dictionary<string, string> backColls = new Dictionary<string, string>();
                            for (int i = 0; i < matchColl.Count; i++)
                            {
                                Match match = matchColl[i];
                                Random ra = new Random(unchecked((int)DateTime.Now.Ticks));//保证产生的数字的随机性 
                                string r = ra.Next().ToString();
                                string reStr = "---tmp_express" + i.ToString() + r + "---";
                                string attrStr = match.Groups[0].Value;
                                backColls.Add(reStr, attrStr);
                                //2.备份并替换''内的字符
                                ExpressionStr = ExpressionStr.Replace(attrStr, reStr);
                            }
                            //3.替换""内的字符
                            foreach (Match j in matchColl2)
                            {
                                string matchstr = j.Groups[0].Value;
                                string rStr = "'" + (matchstr.Substring(1, matchstr.Length - 2).Replace("'", "''")) + "'";
                                ExpressionStr = ExpressionStr.Replace(matchstr, rStr);
                            }
                            //4还原''内的字符
                            foreach (KeyValuePair<string, string> k in backColls)
                            {
                                ExpressionStr = ExpressionStr.Replace(k.Key, k.Value);
                            }
                        }
                    }
                    else
                    {
                        //3.替换""内的字符
                        foreach (Match j in matchColl2)
                        {
                            string matchstr = j.Groups[0].Value;
                            string rStr = "'" + (matchstr.Substring(1, matchstr.Length - 2).Replace("'", "''")) + "'";
                            ExpressionStr = ExpressionStr.Replace(matchstr, rStr);
                        }
                    }
                }
            }
            returnObj = dt.Compute(ExpressionStr, "");
            return Convert.ToString(returnObj);

        }
        internal static string DataTableComputeUseMono(string ExpressionStr)
        {
            DataTable dt = new DataTable();
            //此内容用于mono
            DataColumn column;
            DataRow row;
            // Create new DataColumn, set DataType, ColumnName and add to DataTable.    
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Int32");
            column.ColumnName = "id";
            dt.Columns.Add(column);
            row = dt.NewRow();
            row["id"] = 1;
            dt.Rows.Add(row);
            //此内容用于mono
            object returnObj;
            returnObj = dt.Compute(ExpressionStr, "");
            return Convert.ToString(returnObj);
        }
        internal static string EvalExpression(string ExpressionStr, T XNLPage)
        {
            //^([^\.]*)\((.*)?\)$
            //^(.*)\.([^()]*)$|^(.*)\.(.*)\((.*)\)$
            //string tmpExpressStr = ExpressionStr.Trim();
            //tmpExpressStr = XNLCommon.decodeHTMLEscapecharacter(tmpExpressStr);
            //tmpExpressStr = XNLCommon.decodeXNLEscapecharacter(tmpExpressStr);
            //尝试其它表达式
            try
            {
                #region 计算表达式
                Match excMatch = Regex.Match(ExpressionStr, "^([^\\.]*)\\((.*)?\\)$", XNL_RegexOptions);
                if (excMatch.Success) //自定义方法表达式
                {
                    string funcStr = excMatch.Groups[1].Value.Trim().ToLower();
                    string valueStrs = excMatch.Groups[2].Value;
                    string s = funcStr.Substring(0, 1);
                    MatchCollection valueMatchColl = Regex.Matches(valueStrs, "(\')([^\']*)?\'|(\")([^\"]*)?\"|(true|false)|([+-]?\\d*[.]?\\d*)", XNL_RegexOptions);
                    Dictionary<string, string> paramsDirectory = new Dictionary<string, string>();
                    int matchCount = 0;
                    foreach (Match match in valueMatchColl)
                    {
                        if (match.Groups[0].Value.Trim().Equals(string.Empty)) continue;
                        matchCount += 1;
                        if (match.Groups[1].Value == "'")
                        {
                            paramsDirectory.Add(Convert.ToString(matchCount), match.Groups[2].Value);
                        }
                        else if (match.Groups[3].Value == "\"")
                        {
                            paramsDirectory.Add(Convert.ToString(matchCount), match.Groups[4].Value);
                        }
                        else
                        {
                            if (match.Groups[5].Value.Trim().ToLower() == "true" || match.Groups[5].Value.Trim().ToLower() == "false")
                            {
                                paramsDirectory.Add(Convert.ToString(matchCount), match.Groups[5].Value);
                            }
                            else
                            {
                                paramsDirectory.Add(Convert.ToString(matchCount), match.Groups[6].Value);
                            }
                        }
                    }
                    switch (s)
                    {
                        // (')([^']*)?'|(")([^"]*)?"
                        case "i":
                            switch (funcStr)
                            {
                                case "isnumeric":
                                    return Convert.ToString(UtilsCode.IsNumeric(paramsDirectory["1"]));
                                case "isint":
                                    return Convert.ToString(UtilsCode.IsInt(paramsDirectory["1"]));
                                case "isunsign":
                                    return Convert.ToString(UtilsCode.IsUnsign(paramsDirectory["1"]));
                                case "isdate":
                                    return Convert.ToString(UtilsCode.IsDate(paramsDirectory["1"]));
                                case "isemail":
                                    return Convert.ToString(UtilsCode.IsEMail(paramsDirectory["1"]));
                                case "isflash":
                                    return Convert.ToString(UtilsCode.IsFlash(paramsDirectory["1"]));
                                case "isidcard":
                                    return Convert.ToString(UtilsCode.IsIdCard(paramsDirectory["1"]));
                                case "isimage":
                                    return Convert.ToString(UtilsCode.IsImage(paramsDirectory["1"]));
                                case "isfiletype":
                                    return Convert.ToString(UtilsCode.IsFileType(paramsDirectory["1"], paramsDirectory["2"]));
                                case "isurl":
                                    return Convert.ToString(UtilsCode.IsUrl(paramsDirectory["1"]));
                                case "iszipno":
                                    return Convert.ToString(UtilsCode.IsZipNo(paramsDirectory["1"]));
                                case "Isgreatethanzero":
                                    return Convert.ToString(UtilsCode.IsGreateThanZero(paramsDirectory["1"]));
                                case "ishanzi":
                                    return Convert.ToString(UtilsCode.IsHanZi(paramsDirectory["1"]));
                                case "istelno":
                                    return Convert.ToString(UtilsCode.IsTelNo(paramsDirectory["1"]));
                                case "iscellphotono":
                                    return Convert.ToString(UtilsCode.IsCellPhotoNo(paramsDirectory["1"]));
                                case "ismatch":
                                    return Convert.ToString(UtilsCode.IsMatch(paramsDirectory["1"], paramsDirectory["2"]));
                                case "insert":
                                    return UtilsCode.Insert(paramsDirectory["1"], Convert.ToInt32(paramsDirectory["2"]), paramsDirectory["3"]);
                                case "indexof":
                                    return UtilsCode.IndexOf(paramsDirectory["1"], paramsDirectory["2"], Convert.ToInt32(paramsDirectory["3"]), Convert.ToInt32(paramsDirectory["4"]));
                                case "isnull":
                                    return DataTableCompute(ExpressionStr);
                                case "iif":
                                    return DataTableCompute(ExpressionStr);
                            }
                            break;
                        case "g":
                            switch (funcStr)
                            {
                                case "getmatchgroup":
                                    return UtilsCode.getMatchGroup(paramsDirectory["1"], paramsDirectory["2"], Convert.ToInt32(paramsDirectory["3"]));
                                case "getmatchcount":
                                    return UtilsCode.getMatchCount(paramsDirectory["1"], paramsDirectory["2"], Convert.ToInt32(paramsDirectory["3"]));
                                case "getmatchs":
                                    return UtilsCode.getMatchs(paramsDirectory["1"], paramsDirectory["2"], Convert.ToInt32(paramsDirectory["3"]));
                                case "getyear":
                                    return UtilsCode.GetYear(paramsDirectory["1"]);
                                case "getday":
                                    return UtilsCode.GetDay(paramsDirectory["1"]);
                                case "gettime":
                                    return UtilsCode.GetTime(paramsDirectory["1"]);
                                case "getweek":
                                    return UtilsCode.GetWeek(paramsDirectory["1"]);
                                case "getdate":
                                    return UtilsCode.GetDate(paramsDirectory["1"]);
                                case "getmonth":
                                    return UtilsCode.GetMonth(paramsDirectory["1"]);
                                case "gethour":
                                    return UtilsCode.GetHour(paramsDirectory["1"]);
                                case "getminute":
                                    return UtilsCode.GetMinute(paramsDirectory["1"]);
                                case "getsecond":
                                    return UtilsCode.GetSecond(paramsDirectory["1"]);
                                case "getmillisecond":
                                    return UtilsCode.GetMillisecond(paramsDirectory["1"]);
                            }
                            break;
                        case "d":
                            switch (funcStr)
                            {
                                case "decodehtml":
                                    return UtilsCode.decodeHtml(paramsDirectory["1"]);
                                case "decodexnl":
                                    return UtilsCode.decodeXNL(paramsDirectory["1"]);
                                case "decodehtmlandxnl":
                                    return UtilsCode.decodeHtmlAndXnl(paramsDirectory["1"]);
                                case "date":
                                    return UtilsCode.Date();
                                case "day":
                                    return UtilsCode.Day();
                            }
                            break;
                        case "e":
                            switch (funcStr)
                            {
                                case "encodehtml":
                                    return UtilsCode.encodeHtml(paramsDirectory["1"]);
                                case "encodexnl":
                                    return UtilsCode.encodeXNL(paramsDirectory["1"]);
                                case "encodehtmlandxnl":
                                    return UtilsCode.encodeHtmlAndXnl(paramsDirectory["1"]);
                            }
                            break;
                        case "r":
                            switch (funcStr)
                            {
                                case "replace":
                                    return UtilsCode.replace(paramsDirectory["1"], paramsDirectory["2"], paramsDirectory["3"]);
                                case "remove":
                                    return UtilsCode.Remove(paramsDirectory["1"], Convert.ToInt32(paramsDirectory["2"]), Convert.ToInt32(paramsDirectory["3"]));

                            }
                            break;
                        case "t":
                            switch (funcStr)
                            {
                                case "tolower":
                                    return UtilsCode.ToLower(paramsDirectory["1"]);
                                case "toupper":
                                    return UtilsCode.ToUpper(paramsDirectory["1"]);
                                case "trimstart":
                                    return UtilsCode.TrimStart(paramsDirectory["1"]);
                                case "trimend":
                                    return UtilsCode.TrimEnd(paramsDirectory["1"]);
                                case "trim":
                                    return DataTableCompute(ExpressionStr);
                                case "tolongdate":
                                    return UtilsCode.ToLongDate(paramsDirectory["1"]);
                                case "tolongtime":
                                    return UtilsCode.ToLongTime(paramsDirectory["1"]);
                                case "toshortdate":
                                    return UtilsCode.ToShortDate(paramsDirectory["1"]);
                                case "toshorttime":
                                    return UtilsCode.ToShortTime(paramsDirectory["1"]);
                                case "today":
                                    return UtilsCode.Today();
                                case "time":
                                    return UtilsCode.Time();
                            }
                            break;
                        case "f":
                            switch (funcStr)
                            {
                                case "formatdate":
                                    return UtilsCode.formatDate(paramsDirectory["1"], paramsDirectory["2"]);
                                case "formattime":
                                    return UtilsCode.formatTime(paramsDirectory["1"], paramsDirectory["2"]);
                                case "formatnumber":
                                    return UtilsCode.formatNumber(paramsDirectory["1"], paramsDirectory["2"]);
                            }
                            break;
                        case "c":
                            switch (funcStr)
                            {
                                case "convert":
                                    return DataTableCompute(ExpressionStr);
                                case "chs2pinyin":
                                    return UtilsCode.CHS2PinYin(paramsDirectory["1"], paramsDirectory["2"], Convert.ToBoolean(paramsDirectory["3"]));
                                case "chs2py":
                                    return UtilsCode.CHS2PY(paramsDirectory["1"], paramsDirectory["2"], Convert.ToBoolean(paramsDirectory["3"]));
                            }
                            break;
                        case "s":
                            switch (funcStr)
                            {
                                case "substring":
                                    return DataTableCompute(ExpressionStr);
                                case "second":
                                    return UtilsCode.Second();
                            }
                            break;
                        case "l":
                            switch (funcStr)
                            {
                                case "len":
                                    return DataTableCompute(ExpressionStr);
                            }
                            break;
                        case "n":
                            switch (funcStr)
                            {
                                case "now":
                                    return UtilsCode.now();
                            }
                            break;
                        case "m":
                            switch (funcStr)
                            {
                                case "month":
                                    return UtilsCode.Month();
                                case "minute":
                                    return UtilsCode.Minute();
                                case "millisecond":
                                    return UtilsCode.Millisecond();
                            }
                            break;
                        case "w":
                            switch (funcStr)
                            {
                                case "week":
                                    return UtilsCode.Week();
                            }
                            break;
                        case "y":
                            switch (funcStr)
                            {
                                case "year":
                                    return UtilsCode.Year();
                            }
                            break;
                        case "h":
                            switch (funcStr)
                            {
                                case "hour":
                                    return UtilsCode.Hour();
                            }
                            break;
                        default:
                            return RegxpEngineCommon<T>.DataTableCompute(ExpressionStr);
                    }
                }
                else  //扩展XNL标签表达式
                {
                    Match excXnlMatch = Regex.Match(ExpressionStr, "^(.*)\\.([^()]*)$|^(.*)\\.(.*)\\((.*)\\)$", XNL_RegexOptions);
                    if (excXnlMatch.Success)
                    {
                        if (excXnlMatch.Groups[1].Value.Trim() != "") //xnl.label
                        {
                            string libNameStr = excXnlMatch.Groups[1].Value;
                            string labelNameStr = excXnlMatch.Groups[2].Value;
                            string returnStr = ParserEngine<T>.parse("<" + libNameStr + ":" + labelNameStr + "></" + libNameStr + ":" + labelNameStr + ">", XNLPage);
                            return returnStr;
                        }
                        else  //xnl.labelname('hjgj','jh')
                        {
                            string libNameStr = excXnlMatch.Groups[3].Value;
                            string labelNameStr = excXnlMatch.Groups[4].Value;
                            string valueStrs = excXnlMatch.Groups[5].Value;
                            MatchCollection valueMatchColl = Regex.Matches(valueStrs, "(\')([^\']*)?\'|(\")([^\"]*)?\"|(true|false)|([+-]?\\d*[.]?\\d*)", XNL_RegexOptions);
                            int matchCount = 0;
                            string paramStr = "<attrs>";
                            foreach (Match match in valueMatchColl)
                            {
                                if (match.Groups[0].Value.Trim().Equals(string.Empty)) continue;
                                matchCount += 1;
                                if (match.Groups[1].Value == "'")
                                {
                                    paramStr += "<attr name=\"param" + Convert.ToString(matchCount) + "\" >" + UtilsCode.encodeHtmlAndXnl(match.Groups[2].Value) + "</attr>";
                                }
                                else if (match.Groups[3].Value == "\"")
                                {
                                    paramStr += "<attr name=\"param" + Convert.ToString(matchCount) + "\" >" + UtilsCode.encodeHtmlAndXnl(match.Groups[4].Value) + "</attr>";
                                }
                                else
                                {
                                    if (match.Groups[5].Value.Trim().ToLower() == "true" || match.Groups[5].Value.Trim().ToLower() == "false")
                                    {
                                        paramStr += "<attr name=\"param" + Convert.ToString(matchCount) + "\" >" + UtilsCode.encodeHtmlAndXnl(match.Groups[5].Value) + "</attr>";
                                    }
                                    else
                                    {
                                        paramStr += "<attr name=\"param" + Convert.ToString(matchCount) + "\" >" + UtilsCode.encodeHtmlAndXnl(match.Groups[6].Value) + "</attr>";
                                    }
                                }
                            }
                            paramStr += "</attrs>";
                            string returnStr =ParserEngine<T>.parse("<" + libNameStr + ":" + labelNameStr + ">" + paramStr + "</" + libNameStr + ":" + labelNameStr + ">", XNLPage);
                            return returnStr;
                        }
                    }
                    else
                    {
                        return RegxpEngineCommon<T>.DataTableCompute(ExpressionStr);
                    }
                }
                return RegxpEngineCommon<T>.DataTableCompute(ExpressionStr);
                #endregion 计算表达式
            }
            catch
            {
                return ExpressionStr;
            }
        }
 * 
 * 
 *  //internal static Regex RegexObj_XNLAttriableVariable = new Regex(RegexStr_XNLAttriableVariable, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLDataBaseVariable = new Regex(RegexStr_XNLDataBaseVariable, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLReqFormVariable = new Regex(RegexStr_XNLReqFormVariable, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLReqQueryVariable = new Regex(RegexStr_XNLReqQueryVariable, XNL_RegexOptions);

        //internal static Regex RegexObj_XNLReqVariable = new Regex(RegexStr_XNLReqVariable, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLSessionVariable = new Regex(RegexStr_XNLSessionVariable, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLApplicationVariable = new Regex(RegexStr_XNLApplicationVariable, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLAllPublicCariable = new Regex(RegexStr_XNLAllPublicCariable, XNL_RegexOptions);

        //internal static Regex RegexObj_XNLExpressionVariable = new Regex(RegexStr_XNLExpressionVariable, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLGlobalVariable = new Regex(RegexStr_XNLGlobalVariable, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLNotes = new Regex(RegexStr_XNLNotes, XNL_RegexOptions);
 * 
 * //internal const RegexOptions XNL_RegexOptionsNoCompiled = (((RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline) | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
        //@"<@XNL+:\w+[\s]*([^>]*)>([.\s\S]*)</\1:\2>"  //<(xnl|cms\.manage):([_a-zA-Z0-9]+)(::[_a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>  //捕捉所有
        //<xnl|cms\.manage:[_a-zA-Z0-9]+(?:::[_a-zA-Z0-9]+|)(?:[\s]*|[\s]+[^:>]+)> //不分组捕捉前半部分
        //<(xnl|cms\.manage):([_a-zA-Z0-9]+)(::[_a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)> 分组捕捉前半部分  1标签命名空间 2标签名 3标签实例名 4 标签参数

        //internal const string RegexStr_XNLAttriableVariable = @"{@([^\s{}><:]+?)}";
        //internal const string RegexStr_XNLDataBaseVariable = "{\\$([^\\s{><}:]+?)}";
        //internal const string RegexTemplate_XNLAttriableVariable = @"{@([^\s{}><:]+?)(::NAME|)}";
        //internal const string RegexTemplate_XNLDataBaseVariable = "{\\$([^\\s{><}:]+?)(::NAME|)}";


        //internal const string RegexStr_XNLExpressionVariable = @"{\[([^{}]+)\]}";//"{\\[((?>[^{][^\\[])+?(?>[^\\]][^}])*?)\\]}"; // //{\[([^{}]+|{\[(?<o>)|\]}(?<-o>)?)(?(o)(?!))\]}
        //internal const string RegexStr_XNLGlobalVariable = @"{!(\w+?)}";
 * 
 * //internal static string RegexStr_XNLTagWithName3GroupAll;// = @"<(@XNL):(@TAG)(@NAME)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>";
        //internal static string RegexStr_XNLSubTagName2GroupAll;// = @"<(@XNL)(@NAME)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1\2>";
*/
/*
         
        internal static MatchCollection MatcgXNLTagInnerAttrs(string str)
        {
            MatchCollection paramColle = Regex.Matches(str, RegexStr_XNLInnerTagParams, XNL_RegexOptions);// RegxpEngineCommon<T>.RegexObj_XNLInnerTagParams.Matches(str);
            return paramColle;
        }
      
        internal static Match MatchTagGroupAll(string contentStr)
        {
            return Regex.Match(contentStr, RegexStr_XNLTagGroupAll, XNL_RegexOptions);// RegexObj_XNLTagGroupAll.Match(contentStr);
        }
        internal static MatchCollection MatchTagsGroupAll(string contentStr)
        {
            return Regex.Matches(contentStr, RegexStr_XNLTagGroupAll, XNL_RegexOptions);
        }
        
        internal static Match MatchTagGroupAllByName(string contentStr, string nameSpace, string tagName,string tagObjName)
        {
            string regStr = GetTagRegStr(nameSpace, tagName, tagObjName);
            return Regex.Match(contentStr, regStr, XNL_RegexOptions);
        }

        internal static MatchCollection MatchTagsGroupAllByName(string contentStr, string nameSpace, string tagName,string tagObjName)
        {
            string regStr = GetTagRegStr(nameSpace, tagName, tagObjName);
            return Regex.Matches(contentStr, regStr, XNL_RegexOptions);
        }


        internal static Match MatchSubTagGroupAllByName(string contentStr, string tagName, string tagObjName)
        {
            string regStr = GetSubTagRegStr(tagName, tagObjName);
            return Regex.Match(contentStr, regStr, XNL_RegexOptions);
        }

        internal static string GetTagRegStr(string nameSpace, string tagName, string tagObjName)
        {
            string regStr;
            if (string.IsNullOrEmpty(nameSpace))
            {
                regStr = RegexStr_XNLTagWithName3GroupAll;
            }
            else
            {
                regStr = RegexTemplate_XNLTagWithName3GroupAll.Replace("XNL", nameSpace.Replace(".", "\\."));
            }
            if (string.IsNullOrEmpty(tagName))
            {
                regStr = regStr.Replace("TAG", "[_a-zA-Z0-9]+");
            }
            else
            {
                regStr = regStr.Replace("TAG", tagName.Replace(".", "\\."));
            }
            if (string.IsNullOrEmpty(tagObjName))
            {
                regStr = regStr.Replace("NAME", "[a-zA-Z0-9]+|");
            }
            else
            {
                regStr = regStr.Replace("NAME", tagObjName);
            }
            return regStr;
        }
        */