using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Data;
using Com.AimUI.TagCore;
namespace Com.AimUI.TagParser
{
    public class RegxpEngineCommon<T> where T:TagContext
    {
        internal static string TagRegNames;
        internal const RegexOptions Tag_RegexOptions = (((RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline) | RegexOptions.Multiline) | RegexOptions.IgnoreCase);//|RegexOptions.Compiled
        
        internal const string RegexStr_TagNotes = "<\\!--\\#.*?\\#-->";  //匹配 aim 标签注释

        internal const string RegexStr_TagInnerTagParams = "([^\\s]+?)=\"([.\\s\\S]*?)\"";

        internal static string RegexStr_TagGroupAll;// = @"<(@AT):([_a-zA-Z0-9]+)(\#[a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>";
        internal const string RegexTemplate_TagGroupAll = @"<(AT):([_a-zA-Z0-9]+)(\#[a-zA-Z0-9]+|)(\s*|\s+.+?""\s*)>([.\s\S]*?)</\1:\2\3>";

       
        internal static string RegexStr_SubTagName2GroupAll;
        internal const string RegexTemplate_SubTagName2GroupAll = @"<(AT)(\#NAME|)(\s*|\s+.+?""\s*)>([.\s\S]*?)</\1\2>";
       
        internal const string RegexStr_TagToken = @"{([@$])([_a-zA-Z0-9\.:]+.*?)}";  //{([@$])((?>{(?<n>)|}(?<-n>)|(?!{|}).)*?)(?(n)(?!))}
       
        /// <summary>
        /// 清除注释标签
        /// </summary>
        /// <param name="contentStr"></param>
        /// <returns></returns>
        internal static string RemoveTagNotes(string contentStr)
        {
            return Regex.Replace(contentStr, RegexStr_TagNotes, "", Tag_RegexOptions);
        }


        internal static TagParams GetTagParams(string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            TagParams tagParams=null;
            string t_paramName=null;
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

        

        internal static MatchCollection MatchSubTagsGroupAllByName(string contentStr,  string tagName, string tagObjName)
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

            if (string.IsNullOrEmpty(tagObjName)||tagObjName.StartsWith("t__"))
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
            string _nameSpace = tagGroupAllMatch.Groups[1].Value;
            string _tagName = tagGroupAllMatch.Groups[2].Value;
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

            ITag<T> tagObj = TagLib<T>.GetTagInstance(_nameSpace, _tagName);
            string subTagName = tagObj.subTagNames;
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
            if (contentStr.Length < 7 || contentStr.Trim().Length < 7) return null;
            MatchCollection matchs = MatchSubTagsGroupAllByName(contentStr, subTagName, tagObjName);
            int curIndex = 0;
            if (matchs.Count > 0)
            {
                List<TagStruct> list = new List<TagStruct>(matchs.Count*2+1);
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
            if (contentStr.Length<11 || contentStr.Trim().Length < 11) return null;
            int index = 0;
            MatchCollection matchs = Regex.Matches(contentStr, RegexStr_TagGroupAll, Tag_RegexOptions); //RegexObj_TagGroupAll.Matches(contentStr);
            int counts = matchs.Count;
            //if (counts > 0)
            //{
            //    return null;
            //    //tagStruct = new TagStruct();
            //    //tagStruct.allContent = contentStr;
            //}
            if (counts > 0)
            {
                TagStruct tagStruct = null;
                Match tmpMatch = matchs[0];
                if (counts == 1 && tmpMatch.Index == 0 && tmpMatch.Length == contentStr.Length)
                {
                    tagStruct=CreateTagStruct(tmpMatch);
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
                    if(index<contentStr.Length)
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

        internal static List<TagToken> GetTagTokens(string contentStr)
        {
            if (string.IsNullOrEmpty(contentStr)) return null;
            if (contentStr.Length<4||contentStr.Trim().Length<4) return null;
            MatchCollection matchs = Regex.Matches(contentStr, RegexStr_TagToken);
            if (matchs.Count == 0) return null;
            List<TagToken> tokens = new List<TagToken>(matchs.Count);
            TagToken token = null;
            string tokenValue = null;
            //int dotInx = -1;
            //bool byRef = false;
            foreach(Match match in matchs)
            {
                
                tokenValue = match.Groups[2].Value;
                if (tokenValue[0] == ':') //sample {@:a}
                {
                    //byRef = true;
                    tokenValue = tokenValue.Remove(0, 1);
                }
                //else
                //{
                //    byRef = false;
                //}

                //dotInx = tokenValue.LastIndexOf('.');
                //if (dotInx == 0 || dotInx == tokenValue.Length - 1)
                //{
                //    continue;
                //}
                if ((tokenValue.Length == 1 && tokenValue[0] == '.') || tokenValue[tokenValue.Length - 1] == '.') continue;
                switch (match.Groups[1].Value)
                {
                    case "@": //属性
                        token = GetAttrToken(tokenValue);
                        if (token == null) continue;
                            /*
                            new TagToken();
                        token.type = TagTokenType.Attribute;
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
                             */ 
                        break;
                    case "$": // //表达式  统一概念 无参数 变量表达式  有参数  方法表达式  {$site.url}  "{$isemail({$siteurl},abc)}"
                        token = GetExpression(tokenValue);
                        if (token == null) continue;
                        token.type = TagTokenType.Express;
                        break;
                }

                token.value = match.Value;
                //token.byRef = byRef;
                token.index = match.Index;
                token.length = match.Length;
                tokens.Add(token);
            }
            return tokens;
        }

        internal static TagToken GetAttrToken(string tokenValue)
        {
            //int dotInx = tokenValue.LastIndexOf('.');
            //if (dotInx == 0 || dotInx == tokenValue.Length - 1)
            //{
            //    return null;
            //}
            Match match = Regex.Match(tokenValue, @"^([_a-zA-Z0-9\.:]+)(.*?)$", Tag_RegexOptions);
            if (match.Success)
            {
                TagToken token = new TagToken() { type = TagTokenType.Attribute };
                tokenValue = match.Groups[1].Value;
                int dotInx = tokenValue.LastIndexOf('.');
                if (dotInx == 0 || dotInx == tokenValue.Length - 1)
                {
                    return null;
                }
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
                string args = match.Groups[2].Value.Trim();
                token.args = GetTokenArgs(args);
                return token;
            }

            return null;
        }

        /*
        internal static void FixTokenArgs(Dictionary<string, TagToken> colls,TagToken token)
        {
            if (token != null && token.args != null && token.args.Count > 0)
            {
                TagToken _token;
                for (int i = 0; i < token.args.Count; i++)
                {
                    TagToken t = token.args[i];
                    if (t.args == null || t.args.Count == 0)
                    {
                        if (t.value != null && t.value.IndexOf("~Exp~") != -1)
                        {
                            MatchCollection matchs = Regex.Matches(t.value, "~Exp~[\\d]{5}");
                            if (matchs.Count > 0)
                            {
                                if (t.value.Length == 10)
                                {
                                    _token = colls[t.value];
                                    if (t.type == TagTokenType.Common)
                                    {
                                        token.args[i] = _token;
                                    }
                                    else
                                    {
                                        t.args = new List<TagToken>(1);
                                        t.args.Add(_token);
                                    }
                                    if (_token.args != null && _token.args.Count > 0) FixTokenArgs(colls, _token);
                                }
                                else
                                {
                                    _token = new TagToken() { type = TagTokenType.Common };
                                    _token.args = new List<TagToken>(matchs.Count);

                                    int _index = 0;
                                    foreach (Match match in matchs)
                                    {
                                        if (match.Index > _index)
                                        {
                                            _token.args.Add(new TagToken() { type = TagTokenType.Common, value = t.value.Substring(_index, match.Index - _index) });
                                            _index = match.Index + match.Value.Length;
                                            try
                                            {
                                                _token.args.Add(colls[match.Value]);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                    if (t.value.Length > _index)
                                    {
                                        _token.args.Add(new TagToken() { type = TagTokenType.Common, value = t.value.Substring(_index) });
                                    }

                                    if (t.type == TagTokenType.Common)
                                    {
                                        token.args[i] = _token;
                                    }
                                    else
                                    {
                                        t.args = new List<TagToken>(1);
                                        t.args.Add(_token);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        FixTokenArgs(colls, token);
                    }
                }
            }
        }
        */

        internal static IList<TagToken> GetTokenArgs(string args)
        {
            if (string.IsNullOrEmpty(args) == false)
            {
                string names = null;
                var inx = 0;
                List<TagToken> t_args = new List<TagToken>();
                string match_args = args;
                if (match_args.StartsWith("(") && match_args.EndsWith(")"))
                {
                    match_args = match_args.Substring(1, match_args.Length - 2);
                }

                //设置参数
                MatchCollection tokenMatchs;
                Dictionary<string, TagToken> nestedExp = null;
                Random ra = new Random();// unchecked((int)DateTime.Now.Ticks) 保证产生的数字的随机性 
                int random = ra.Next(10000,90000);
                while (true)
                {
                    //匹配嵌套表达式
                    //\$\s*([_a-zA-Z0-9\.:]+)\(([^\(\)]*?)\)
                    tokenMatchs = Regex.Matches(match_args, @"[@$]([_a-zA-Z0-9\.:]+)\(([^\(\)]*?)\)", Tag_RegexOptions);
                    if (tokenMatchs.Count > 0)
                    {
                        //嵌套表达式
                        string _args = null;
                        //解析 暂存
                        foreach (Match m in tokenMatchs)
                        {

                            names = m.Groups[1].Value;
                            _args = m.Groups[2].Value;

                            TagToken tagToken = null;
                            if (m.Value[0] == '@')
                            {
                                tagToken = new TagToken() { type = TagTokenType.Attribute };
                            }
                            else
                            {
                                tagToken = new TagExpression() { type = TagTokenType.Express };
                            }

                            if (m.Value[1] == ':')
                            {
                                //_express.byRef = true;
                                names = names.Substring(1);
                                tagToken.value = m.Value.Substring(2);
                            }
                            else
                            {
                                tagToken.value = m.Value.Substring(1);
                            }
                            
                            tagToken.name = names;
                            if (tagToken.type == TagTokenType.Attribute)
                            {
                                inx = names.LastIndexOf('.');
                                if (inx == -1)
                                {
                                    tagToken.name = names;
                                    tagToken.scope = string.Empty;
                                }
                                else
                                {
                                    tagToken.scope = names.Substring(0, inx);
                                    tagToken.name = names.Substring(inx + 1);
                                }
                            }
                            else
                            {
                                inx = names.LastIndexOf(':');

                                if (inx != -1)
                                {
                                    tagToken.scope = names.Substring(0, inx);
                                    names = names.Substring(inx + 1);
                                }

                                //分离  tag name and exp
                                inx = names.LastIndexOf('.');

                                if (inx != -1)
                                {
                                    ((TagExpression)tagToken).tagName = names.Substring(0, inx);
                                    tagToken.name = names.Substring(inx + 1);
                                }
                            }

                            //解析参数
                            if (string.IsNullOrEmpty(_args.Trim()) == false)
                            {

                                string[] s_arr = _args.Split(',');

                                tagToken.args = new List<TagToken>(s_arr.Length);
                                string _s;
                                TagToken _token;
                                for (var i = 0; i < s_arr.Length; i++)
                                {

                                    _s = s_arr[i].Trim();
                                    if (_s.StartsWith("@") || _s.StartsWith("@:"))
                                    {
                                        _token = new TagToken() { type = TagTokenType.Attribute };
                                    }
                                    else if (_s.StartsWith("$") || _s.StartsWith("$:"))
                                    {
                                        _token = new TagExpression() { type = TagTokenType.Express };
                                    }
                                    else
                                    {
                                        if ((_s.StartsWith("\"") && _s.EndsWith("\"")) || (_s.StartsWith("'") && _s.EndsWith("'")))
                                        {
                                            _s = _s.Substring(1, _s.Length - 2);//_s.Trim(trims);
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

                                                _token = new TagToken() { type = TagTokenType.Common }; //, value = _s
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
                                        if (_s[1] == ':')
                                        {
                                            //_token.byRef = true;
                                            _token.value = _s.Substring(2);
                                        }
                                        else
                                        {
                                            _token.value = _s.Substring(1);
                                        }
                                        if (_token.type == TagTokenType.Express)
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
                                                ((TagExpression)_token).tagName = names.Substring(0, inx);
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

                                    tagToken.args.Add(_token);
                                }
                            }
                            //名称 参数
                            if (nestedExp == null) nestedExp = new Dictionary<string, TagToken>();

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

                //split match_args
                string[] e_arr = match_args.Split(',');
                string e_s;
                TagToken e_token;
                for (var i = 0; i < e_arr.Length; i++)
                {
                    e_s = e_arr[i].Trim();
                    if (e_s.StartsWith("@") || e_s.StartsWith("@:"))
                    {
                        e_token = new TagToken() { type = TagTokenType.Attribute };
                    }
                    else if (e_s.StartsWith("$") || e_s.StartsWith("$:"))
                    {
                        e_token = new TagExpression() { type = TagTokenType.Express };
                    }
                    else
                    {
                        if ((e_s.StartsWith("\"") && e_s.EndsWith("\"")) || (e_s.StartsWith("'") && e_s.EndsWith("'")))
                        {
                            e_s = e_s.Substring(1, e_s.Length - 2);
                        }

                        if(e_s.IndexOf("~Exp~")!=-1)
                        {
                            MatchCollection matchs = Regex.Matches(e_s, "~Exp~[\\d]{5}");
                            if (matchs.Count>0)
                            {
                                if (e_s.Length == 10)
                                {
                                    e_token = nestedExp[e_s];
                                    t_args.Add(e_token);
                                    continue;
                                }
                                e_token = new TagToken() { type = TagTokenType.Common }; //, value = e_s
                                e_token.args = new List<TagToken>(matchs.Count);
                                int _index=0;
                                foreach(Match match in matchs)
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
                                if(e_s.Length>_index)
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
                        if (e_s[1] == ':')
                        {
                            //e_token.byRef = true;
                            e_token.value = e_s.Substring(2);
                        }
                        else
                        {
                            e_token.value = e_s.Substring(1);
                        }

                        if (e_token.type == TagTokenType.Express)
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
                                ((TagExpression)e_token).tagName = names.Substring(0, inx);
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
                    t_args.Add(e_token);
                }
                return t_args;
            }
            return null;
        }

        //获取表达式描述
        internal static TagExpression GetExpression(string ExpressionStr)
        {

            Match match = Regex.Match(ExpressionStr, @"^([_a-zA-Z0-9\.:]+)(.*?)$", Tag_RegexOptions);
            if (match.Success)
            {
                TagExpression express = new TagExpression();

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

                if (inx != -1)
                {
                    express.tagName = names.Substring(0, inx);
                    express.name = names.Substring(inx + 1);
                }

                string args = match.Groups[2].Value.Trim();
                express.args = GetTokenArgs(args);
                return express;
            }

            return null;
        }

        /*
        //获取表达式描述
        internal static TagExpression GetExpression(string ExpressionStr)
        {
            
            Match match = Regex.Match(ExpressionStr, @"^([_a-zA-Z0-9\.:]+)(.*?)$", Tag_RegexOptions);
            if (match.Success)
            {
                TagExpression express = new TagExpression();

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
                    express.args = new List<TagToken>();
                    string match_args = args;
                    if (match_args.StartsWith("(") && match_args.EndsWith(")"))
                    {
                        match_args = match_args.Substring(1, match_args.Length - 2);
                    }

                    //设置参数
                    MatchCollection expMatchs;
                    Dictionary<string, TagExpression> nestedExp=null;
                    Random ra = new Random(unchecked((int)DateTime.Now.Ticks));//保证产生的数字的随机性 
                    int random = ra.Next();
                    while(true)
                    {
                        //匹配嵌套表达式
                        //\$\s*([_a-zA-Z0-9\.:]+)\(([^\(\)]*?)\)
                        expMatchs = Regex.Matches(match_args, @"\$([_a-zA-Z0-9\.:]+)\(([^\(\)]*?)\)", Tag_RegexOptions);
                        if (expMatchs.Count > 0)
                        {
                            //嵌套表达式
                            string _args = null;
                            //解析 暂存
                            foreach (Match m in expMatchs)
                            {
                               
                                var _express = new TagExpression();
                                names = m.Groups[1].Value;
                                _args = m.Groups[2].Value;

                                if(m.Value[1]==':')
                                {
                                    //_express.byRef = true;
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

                                    _express.args = new List<TagToken>(s_arr.Length);
                                    string _s;
                                    TagToken _token;
                                    for (var i = 0; i < s_arr.Length; i++)
                                    {

                                        _s = s_arr[i].Trim();
                                        if(_s.StartsWith("@") || _s.StartsWith("@:"))
                                        {
                                            _token = new TagToken() { type = TagTokenType.Attribute };
                                        }
                                        else if (_s.StartsWith("$") || _s.StartsWith("$:"))
                                        {
                                            _token = new TagExpression() { type = TagTokenType.Express};
                                        }
                                        else
                                        {
                                            if ((_s.StartsWith("\"") && _s.EndsWith("\"")) || (_s.StartsWith("'") && _s.EndsWith("'")))
                                            {
                                                _s = _s.Substring(1,_s.Length-2);//_s.Trim(trims);
                                            }
                                            else if (_s.StartsWith("~Exp~"))
                                            {
                                                _token = nestedExp[_s];
                                                _express.args.Add(_token);
                                                continue;
                                            }
                                            _token = new TagToken() { type = TagTokenType.Common, value = _s };
                                        }
                                        if (_token.type != TagTokenType.Common)
                                        {
                                            if (_s[1] == ':')
                                            {
                                                //_token.byRef = true;
                                                _token.value = _s.Substring(2);
                                            }
                                            else
                                            {
                                                _token.value = _s.Substring(1);
                                            }
                                            if (_token.type == TagTokenType.Express)
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
                                                    ((TagExpression)_token).tagName = names.Substring(0, inx);
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
                                if (nestedExp == null) nestedExp = new Dictionary<string, TagExpression>();
                                string key = "~Exp~" + random;
                                random += 1;
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
                    TagToken e_token;
                    for (var i = 0; i < e_arr.Length; i++)
                    {
                        e_s = e_arr[i].Trim();
                        if (e_s.StartsWith("@") || e_s.StartsWith("@:"))
                        {
                            e_token = new TagToken() { type = TagTokenType.Attribute };
                        }
                        else if (e_s.StartsWith("$") || e_s.StartsWith("$:"))
                        {
                            e_token = new TagExpression() { type = TagTokenType.Express };
                        }
                        else
                        {
                            if ((e_s.StartsWith("\"") && e_s.EndsWith("\"")) || (e_s.StartsWith("'") && e_s.EndsWith("'")))
                            {
                                e_s = e_s.Substring(1,e_s.Length-2);
                            }
                            else if (e_s.StartsWith("~Exp~"))
                            {
                                e_token = nestedExp[e_s];
                                express.args.Add(e_token);
                                continue;
                            }
                            e_token = new TagToken() { type = TagTokenType.Common, value = e_s };
                        }
                        if (e_token.type != TagTokenType.Common)
                        {
                            if (e_s[1] == ':')
                            {
                                //e_token.byRef = true;
                                e_token.value = e_s.Substring(2);
                            }
                            else
                            {
                                e_token.value = e_s.Substring(1);
                            }
                            if (e_token.type == TagTokenType.Express)
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
                                    ((TagExpression)e_token).tagName = names.Substring(0, inx);
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
         */ 
    }
}