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
namespace COM.SingNo.XNLParser
{
    public class RegxpEngineCommon<T> where T:XNLContext
    {
        internal static string XNLTagRegNames;
        internal const RegexOptions XNL_RegexOptions = (((RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline) | RegexOptions.Multiline) | RegexOptions.IgnoreCase);//|RegexOptions.Compiled
        
        internal const string RegexStr_XNLNotes = "<\\!--\\#.*?\\#-->";  //匹配XNL标签注释

        internal const string RegexStr_XNLInnerTagParams = "([^\\s]+?)=\"([.\\s\\S]*?)\"";

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
       
        internal static Regex RegexObj_XNLTagGroupAll;
        internal static Regex RegexObj_XNLTagPart1Group3;
 

        internal static Regex RegexObj_XNLTagToken = new Regex(@"{([@$])([_a-zA-Z0-9\.:]+.*?)}");  //{([@$])((?>{(?<n>)|}(?<-n>)|(?!{|}).)*?)(?(n)(?!))}
       
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
            string t_paramName=null;
            string t_paramValue;
            MatchCollection paramColle = Regex.Matches(str, RegexStr_XNLInnerTagParams, XNL_RegexOptions);
            if (paramColle.Count > 0)
            {
                xnlParams = new XNLParams();
                foreach (Match paramMatch in paramColle)
                {
                    t_paramName = paramMatch.Groups[1].Value.ToLower();
                    t_paramValue = paramMatch.Groups[2].Value;
                    xnlParams.Add(t_paramName, t_paramValue);
                }
            }
            
            return xnlParams;
        }
        
        internal static void UpdateXNLConfig()
        {
            Regex.CacheSize = 30;
            XNLTagRegNames = XNLLib<T>.GetRegTagNameSpaces().Replace(".", "\\.");
            RegexStr_XNLTagGroupAll = RegexTemplate_XNLTagGroupAll.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLTagPart1Group0 = RegexTemplate_XNLTagPart1Group0.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLTagPart1Group3 = RegexTemplate_XNLTagPart1Group3.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLTagPart1Group4 = RegexTemplate_XNLTagPart1Group4.Replace("XNL", XNLTagRegNames);

            RegexStr_XNLSubTagName2GroupAll = RegexTemplate_XNLSubTagName2GroupAll.Replace("XNL", "[_a-zA-Z0-9\\.]");

            RegxpEngineCommon<T>.RegexObj_XNLTagGroupAll = new Regex(RegexStr_XNLTagGroupAll, XNL_RegexOptions);
            
            RegxpEngineCommon<T>.RegexObj_XNLTagPart1Group3 = new Regex(RegexStr_XNLTagPart1Group3, XNL_RegexOptions);
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
            
            XNLTagStruct tagStruct = new XNLTagStruct();
            tagStruct.nameSpace = _nameSpace;
            tagStruct.tagName = _tagName;

            tagStruct.instanceName = _tagInstanceName;
            tagStruct.tagParams = xnlParams;
            tagStruct.bodyContent = tagContentStr;
            tagStruct.allContent = tagGroupAllMatch.Value;
            IXNLTag<T> tagObj = XNLLib<T>.GetTagInstance(_nameSpace, _tagName);
            string subTagName = tagObj.subTagNames;
            if (string.IsNullOrEmpty(subTagName)==false)
            {
                tagStruct.subTagStruct = GetSubTagStructs(tagContentStr, subTagName, _tagInstanceName);
            }
            return tagStruct;
        }

        internal static XNLTagStruct CreateSubTagStruct(Match subTagGroupAllMatch)
        {
            string _tagName = subTagGroupAllMatch.Groups[1].Value;
            string _tagInstanceName = subTagGroupAllMatch.Groups[2].Value.Trim();//.ToLower();
            if (_tagInstanceName.StartsWith("::")) _tagInstanceName = _tagInstanceName.Substring(2);
            string tagParamStr = subTagGroupAllMatch.Groups[3].Value;  //标签属性
            string tagContentStr = subTagGroupAllMatch.Groups[4].Value;  //标签内容
            XNLParams xnlParams = RegxpEngineCommon<T>.GetXNLParams(tagParamStr);
            
            XNLTagStruct tagStruct = new XNLTagStruct();
            tagStruct.tagName = _tagName;
            tagStruct.instanceName = _tagInstanceName;
            tagStruct.tagParams = xnlParams;
            tagStruct.bodyContent = tagContentStr;
            tagStruct.allContent = subTagGroupAllMatch.Value;
            return tagStruct;
        }

        internal static List<XNLTagStruct> GetSubTagStructs(string contentStr, string subTagName, string tagObjName)
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
            int index = 0;
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
                if (tokenValue.IndexOf(':') == 0) //sample {:@a}
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
                    case "$": // //表达式  统一概念 无参数 变量表达式  有参数  方法表达式  {$site.url}  "{$isemail({$siteurl},abc)}"
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
                            e_token = new XNLToken() { type = XNLTokenType.Attribute };
                        }
                        else if (e_s.StartsWith("$") || e_s.StartsWith(":@"))
                        {
                            e_token = new XNLExpression() { type = XNLTokenType.Express };
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