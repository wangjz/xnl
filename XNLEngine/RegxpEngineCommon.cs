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
    [StrongNameIdentityPermissionAttribute(SecurityAction.LinkDemand, PublicKey =
"002400000480000094000000060200000024000052534131000400000100010015ce07c41d2a58" +
"1d81df04d0515fb1e3d5eeff895ef834010f59762599faf8c6866cf5f293002c2e30c43a84dba9" +
"44171fb1c049a275d53eff78cf79290f3112e8b37de7429a3ed1a8d9af3f678cd70cc51e704f43" +
"554aaf255d67a56847a3ace7cd5be1f622c50f74128df700319a5a8a6f8663ec625ea319fb78b4" +
"3eaeb8ba")]
    public class RegxpEngineCommon<T> where T:XNLContext
    {
        internal static string XNLTagRegNames;
        internal const RegexOptions XNL_RegexOptions = (((RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline) | RegexOptions.Multiline) | RegexOptions.IgnoreCase);//|RegexOptions.Compiled
        //internal const RegexOptions XNL_RegexOptionsNoCompiled = (((RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline) | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
        //@"<@XNL+:\w+[\s]*([^>]*)>([.\s\S]*)</\1:\2>"  //<(xnl|cms\.manage):([_a-zA-Z0-9]+)(::[_a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>  //捕捉所有
        //<xnl|cms\.manage:[_a-zA-Z0-9]+(?:::[_a-zA-Z0-9]+|)(?:[\s]*|[\s]+[^:>]+)> //不分组捕捉前半部分
        //<(xnl|cms\.manage):([_a-zA-Z0-9]+)(::[_a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)> 分组捕捉前半部分  1标签命名空间 2标签名 3标签实例名 4 标签参数

        internal const string RegexStr_XNLAttriableVariable = @"{@([^\s{}><:]+?)}";
        internal const string RegexStr_XNLDataBaseVariable = "{\\$([^\\s{><}:]+?)}";
        internal const string RegexTemplate_XNLAttriableVariable = @"{@([^\s{}><:]+?)(::NAME|)}";
        internal const string RegexTemplate_XNLDataBaseVariable = "{\\$([^\\s{><}:]+?)(::NAME|)}";

        internal const string RegexStr_XNLReqFormVariable = @"{%(\w+?)}";
        internal const string RegexStr_XNLReqQueryVariable = @"{&(\w+?)}";
        internal const string RegexStr_XNLReqVariable = @"{\\?(\w+?)}";
        internal const string RegexStr_XNLSessionVariable = @"{#(\w+?)}";
        internal const string RegexStr_XNLApplicationVariable = @"{^(\w+?)}";
        internal const string RegexStr_XNLAllPublicCariable = @"{([%&\\?#^])(\w+?)}";

        internal const string RegexStr_XNLExpressionVariable = @"{\[([^{}]+)\]}";//"{\\[((?>[^{][^\\[])+?(?>[^\\]][^}])*?)\\]}"; // //{\[([^{}]+|{\[(?<o>)|\]}(?<-o>)?)(?(o)(?!))\]}
        internal const string RegexStr_XNLGlobalVariable = @"{!(\w+?)}";
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
        //internal static string RegexStr_XNLTagWithName3GroupAll;// = @"<(@XNL):(@TAG)(@NAME)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>";
        //internal static string RegexStr_XNLSubTagName2GroupAll;// = @"<(@XNL)(@NAME)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1\2>";

        internal const string RegexTemplate_XNLTagGroupAll = @"<(XNL):([_a-zA-Z0-9]+)(::[a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>";
        internal const string RegexTemplate_XNLTagPart1Group0 = @"<XNL:[_a-zA-Z0-9]+(?:::[a-zA-Z0-9]+|)(?:[\s]*|[\s]+[^:>]+)>";
        internal const string RegexTemplate_XNLTagPart1Group3 = @"<(XNL):([_a-zA-Z0-9]+)(::[a-zA-Z0-9]+|)(?:[\s]*|[\s]+[^:>]+)>";
        internal const string RegexTemplate_XNLTagPart1Group4 = @"<(XNL):([_a-zA-Z0-9]+)(::[a-zA-Z0-9]+|)([\s]*|[\s]+[^:>]+)>";

        internal static string RegexStr_XNLTagWithName3GroupAll;
        internal static string RegexStr_XNLSubTagName2GroupAll;
        internal const string RegexTemplate_XNLTagWithName3GroupAll = @"<(XNL):(TAG)(::NAME)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>";
        internal const string RegexTemplate_XNLSubTagName2GroupAll = @"<(XNL)(::NAME|)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1\2>";

        

        //internal static Regex RegexObj_XNLAttriableVariable = new Regex(RegexStr_XNLAttriableVariable, XNL_RegexOptions);
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
        internal static Regex RegexObj_XNLTagGroupAll;
        //internal static Regex RegexObj_XNLTagPart1Group0;
        internal static Regex RegexObj_XNLTagPart1Group3;
        //internal static Regex RegexObj_XNLTagPart1Group4;

        //internal static Regex RegexObj_XNLInnerTagParams=new Regex(RegexStr_XNLInnerTagParams,XNL_RegexOptions); //"([^\\s]+?)=\"([.\\s\\S]*?)\""
        //internal static Regex RegexObj_XNLOutTagParams = new Regex(RegexStr_XNLOutTagParams, XNL_RegexOptions);
        //internal static Regex RegexObj_XNLOutTagParam = new Regex(RegexStr_XNLOutTagParam, XNL_RegexOptions);

        //internal static Regex RegexObj_XNLTagAttrName = new Regex(RegexStr_XNLTagAttrName, XNL_RegexOptions);// "name=\"(.*?)\"";
        //internal static Regex RegexObj_XNLTagAttrType = new Regex(RegexStr_XNLTagAttrType, XNL_RegexOptions);// "type=\"(.*?)\"";

        internal static Regex RegexObj_XNLTagToken = new Regex(@"{([@#$%])([_a-zA-Z0-9\\.:]+)}");
        /// <summary>
        /// 根据item项的内容，得到标签item项的所有属性
        /// </summary>
        /// <param name="attribStr"></param>
        /// <param name="itemContentStr"></param>
        /// <returns></returns>
        internal static XNLParams getTagAllParams(string attribStr, string itemContentStr)
        {
            XNLParams labelParams = getXNLParams(attribStr);
            XNLParams labelParams2 = getXNLParams(itemContentStr);
            //string key=null;
            foreach (KeyValuePair<string, XNLParam> tmpParam in labelParams2)
            {
                labelParams[tmpParam.Key] = tmpParam.Value;
                //key = tmpParam.Key;
                //if (!labelParams.ContainsKey(key))
                //{
                //    labelParams.Add(key, tmpParam.Value);
                //}
                //else
                //{
                //    labelParams[key] = tmpParam.Value;
                //}
            }
            return labelParams;
        }
        
       
        /// <summary>
        /// 清除XNL注释标签
        /// </summary>
        /// <param name="contentStr"></param>
        /// <returns></returns>
        internal static string removeXNLNotes(string contentStr)
        {
            return Regex.Replace(contentStr, RegexStr_XNLNotes, "", XNL_RegexOptions);
        }


        internal static XNLParams getXNLParams(string str) //, T xnlContext
        {
            if (string.IsNullOrEmpty(str)) return null;
           // str = RegxpEngineCommon<T>.replaceXNLExpressionVariable(str, xnlContext); //解析表达式
            //str = RegxpEngineCommon<T>.replaceGlobalAttribleVariable(str, xnlContext);
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
                    Match match = matchXNLAttrsTag(str);
                    if (match.Success)
                    {
                        trimStr = match.Value;
                    }
                }
                xnlParams = new XNLParams();
                MatchCollection subColls = matchXNLAttrTags(trimStr);
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
        internal static MatchCollection matcgXNLTagInnerAttrs(string str)
        {
            MatchCollection paramColle = Regex.Matches(str, RegexStr_XNLInnerTagParams, XNL_RegexOptions);// RegxpEngineCommon<T>.RegexObj_XNLInnerTagParams.Matches(str);
            return paramColle;
        }
        internal static Match matchXNLAttrsTag(string str)
        {
            Match match = Regex.Match(str, RegexStr_XNLOutTagParams, XNL_RegexOptions);// RegexObj_XNLOutTagParams.Match(str);
            return match;
        }
        internal static MatchCollection matchXNLAttrTags(string str)
        {
            MatchCollection colls = Regex.Matches(str, RegexStr_XNLOutTagParam, XNL_RegexOptions);// RegxpEngineCommon<T>.RegexObj_XNLOutTagParam.Matches(str);
            return colls;
        }

        public static void updateXNLConfig()
        {
            Regex.CacheSize = 30;
            XNLTagRegNames = XNLLib<T>.getRegTagNameSpaces().Replace(".", "\\.");
            RegexStr_XNLTagGroupAll = RegexTemplate_XNLTagGroupAll.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLTagPart1Group0 = RegexTemplate_XNLTagPart1Group0.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLTagPart1Group3 = RegexTemplate_XNLTagPart1Group3.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLTagPart1Group4 = RegexTemplate_XNLTagPart1Group4.Replace("XNL", XNLTagRegNames);

            RegexStr_XNLTagWithName3GroupAll = RegexTemplate_XNLTagWithName3GroupAll.Replace("XNL", XNLTagRegNames);
            RegexStr_XNLSubTagName2GroupAll = RegexTemplate_XNLSubTagName2GroupAll.Replace("XNL", "[_a-zA-Z0-9\\.]");

            RegxpEngineCommon<T>.RegexObj_XNLTagGroupAll = new Regex(RegexStr_XNLTagGroupAll, XNL_RegexOptions);
            //RegxpEngineCommon<T>.RegexObj_XNLTagPart1Group0 = new Regex(RegxpEngineCommon<T>.RegexStr_XNLTagPart1Group0.Replace("@XNL", XNLLib<T>.getRegTagNameSpaces()), XNL_RegexOptions);
            RegxpEngineCommon<T>.RegexObj_XNLTagPart1Group3 = new Regex(RegexStr_XNLTagPart1Group3, XNL_RegexOptions);
            //RegxpEngineCommon<T>.RegexObj_XNLTagPart1Group4 = new Regex(RegxpEngineCommon<T>.RegexStr_XNLTagPart1Group4.Replace("@XNL", XNLLib<T>.getRegTagNameSpaces()), XNL_RegexOptions);
        }
        
        internal static Match matchTagGroupAll(string contentStr)
        {
            return Regex.Match(contentStr, RegexStr_XNLTagGroupAll, XNL_RegexOptions);// RegexObj_XNLTagGroupAll.Match(contentStr);
        }
        internal static MatchCollection matchTagsGroupAll(string contentStr)
        {
            return Regex.Matches(contentStr, RegexStr_XNLTagGroupAll, XNL_RegexOptions);
        }

        internal static Match matchTagGroupAllByName(string contentStr, string nameSpace, string tagName,string tagObjName)
        {
            string regStr = getTagRegStr(nameSpace, tagName, tagObjName);
            return Regex.Match(contentStr, regStr, XNL_RegexOptions);
        }

        internal static MatchCollection matchTagsGroupAllByName(string contentStr, string nameSpace, string tagName,string tagObjName)
        {
            string regStr = getTagRegStr(nameSpace, tagName, tagObjName);
            return Regex.Matches(contentStr, regStr, XNL_RegexOptions);
        }
        internal static string getTagRegStr(string nameSpace, string tagName, string tagObjName)
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

        internal static Match matchSubTagGroupAllByName(string contentStr, string tagName, string tagObjName)
        {
            string regStr = getSubTagRegStr(tagName, tagObjName);
            return Regex.Match(contentStr, regStr, XNL_RegexOptions);
        }

        internal static MatchCollection matchSubTagsGroupAllByName(string contentStr,  string tagName, string tagObjName)
        {
            string regStr = getSubTagRegStr(tagName, tagObjName);
            return Regex.Matches(contentStr, regStr, XNL_RegexOptions);
        }
        internal static string getSubTagRegStr(string tagName, string tagObjName)
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

        internal static XNLTagStruct createTagStruct(Match tagGroupAllMatch)
        {
            string _nameSpace = tagGroupAllMatch.Groups[1].Value;
            string _tagName = tagGroupAllMatch.Groups[2].Value;
            string _tagInstanceName = tagGroupAllMatch.Groups[3].Value.Trim();//.ToLower();
            if (_tagInstanceName.StartsWith("::")) _tagInstanceName = _tagInstanceName.Substring(2);
            string tagParamStr = tagGroupAllMatch.Groups[4].Value;  //标签属性
            string tagContentStr = tagGroupAllMatch.Groups[5].Value;  //标签内容
            XNLParams xnlParams = RegxpEngineCommon<T>.getXNLParams(tagParamStr);
            Match attrsMatch = RegxpEngineCommon<T>.matchXNLAttrsTag(tagContentStr);
            if (attrsMatch.Success)
            {
                tagContentStr = tagContentStr.Substring(attrsMatch.Index + attrsMatch.Length); //UtilsCode.onceReplace(tagContentStr, attrsMatch.Value, "");
                XNLParams params2 = RegxpEngineCommon<T>.getXNLParams(attrsMatch.Value);
                if (params2 != null && xnlParams != null)
                {
                    foreach (KeyValuePair<string, XNLParam> kv in params2)
                    {
                        xnlParams[kv.Key] = kv.Value;
                        //if (xnlParams.ContainsKey(kv.Key))
                        //{
                        //    xnlParams[kv.Key] = kv.Value;
                        //}
                        //else
                        //{
                        //    xnlParams.Add(kv.Key, kv.Value);
                        //}
                    }
                }
            }
            XNLTagStruct tagStruct = new XNLTagStruct();
            tagStruct.nameSpace = _nameSpace;
            tagStruct.tagName = _tagName;
            //if (string.IsNullOrEmpty(_tagInstanceName))
            //{
            //    _tagInstanceName = "_tag_" + _tagName + xnlcontext.z__TagFlagId.ToString();
            //    xnlcontext.z__TagFlagId += 1;
            //}
            tagStruct.instanceName = _tagInstanceName;
            tagStruct.tagParams = xnlParams;
            tagStruct.bodyContent = tagContentStr;
            tagStruct.allContent = tagGroupAllMatch.Value;
            IXNLTag<T> tagObj = XNLLib<T>.getTagInstance(_nameSpace, _tagName);
            string subTagName = tagObj.subTagNames;
            //if (subTagName == null) subTagName = "";
            if (string.IsNullOrEmpty(subTagName)==false)
            {
                //string _subTagNames = "";
                //for (int t = 0; t < subTagName.Length; t++)
                //{
                //    _subTagNames += (t > 0 ? "|" : "") + subTagName[t];
                //}
                tagStruct.subTagStruct = getSubTagStructs(tagContentStr, subTagName, _tagInstanceName);
            }
            return tagStruct;
        }

        internal static XNLTagStruct createSubTagStruct(Match subTagGroupAllMatch) //, T xnlcontext
        {
            string _tagName = subTagGroupAllMatch.Groups[1].Value;
            string _tagInstanceName = subTagGroupAllMatch.Groups[2].Value.Trim();//.ToLower();
            if (_tagInstanceName.StartsWith("::")) _tagInstanceName = _tagInstanceName.Substring(2);
            string tagParamStr = subTagGroupAllMatch.Groups[3].Value;  //标签属性
            string tagContentStr = subTagGroupAllMatch.Groups[4].Value;  //标签内容
            XNLParams xnlParams = RegxpEngineCommon<T>.getXNLParams(tagParamStr);
            Match attrsMatch = RegxpEngineCommon<T>.matchXNLAttrsTag(tagContentStr);
            if (attrsMatch.Success)
            {
                tagContentStr = tagContentStr.Substring(attrsMatch.Index + attrsMatch.Length);
                XNLParams params2 = RegxpEngineCommon<T>.getXNLParams(attrsMatch.Value);
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

        internal static List<XNLTagStruct> getSubTagStructs(string contentStr, string subTagName, string tagObjName) //, T xnlContext
        {
            if (string.IsNullOrEmpty(contentStr)) return null;
            if (string.IsNullOrEmpty(subTagName)) return null;
            MatchCollection matchs = matchSubTagsGroupAllByName(contentStr, subTagName, tagObjName);
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
                    tagStruct = createSubTagStruct(match);//, xnlContext
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
       
        internal static XNLTagStruct getTagStruct(string contentStr)
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
                    tagStruct=createTagStruct(tmpMatch);
                    //tagStruct.allContent = contentStr;
                    //tagStruct.bodyContent = tmpMatch.Groups[5].Value;
                    //tagStruct.instanceName = tmpMatch.Groups[3].Value;
                    //tagStruct.nameSpace = tmpMatch.Groups[1].Value;
                    //tagStruct.tagName = tmpMatch.Groups[2].Value;
                    //tagStruct.tagParams = getTagAllParams(tmpMatch.Groups[4].Value, tmpMatch.Groups[5].Value);
                    //tagStruct.subTagStruct = getSubTagStructs(tmpMatch.Groups[5].Value,)
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
                        tagStruct.subTagStruct.Add(createTagStruct(tmpMatch));
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
                if (tokenValue.IndexOf(':') == 0)
                {
                    mode = ParseMode.Static;
                    tokenValue = tokenValue.Remove(0, 1);
                }

                dotInx = tokenValue.LastIndexOf('.');
                if (dotInx == 0 || dotInx == tokenValue.Length - 1)
                {
                    continue;
                }
                token = new XNLToken();
                token.value = match.Value;
                token.mode = mode;
               
                switch(match.Groups[1].Value)
                {
                    case "@": //属性
                        token.Type = XNLTokenType.Attribute;
                        break;
                    case "#": 
                        break;
                    case "$": //变量
                        token.Type = XNLTokenType.Variable;
                        break;
                    case "%": //表达式
                        token.Type = XNLTokenType.Express;
                        break;
                }
                if (dotInx==-1)
                {
                    token.Name = tokenValue;
                    token.Scope = string.Empty;
                }
                else
                {
                    token.Scope = tokenValue.Substring(0, dotInx);
                    token.Name = tokenValue.Substring(dotInx+1);
                }
                token.Index = match.Index;
                token.Length = match.Length;
                tokens.Add(token);
            }
            return tokens;
        }
    }
}




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
internal static XNLTagStruct getSubTagStruct(string contentStr, T xnlContext, string tagName, string tagObjName)
{
    return createSubTagStruct(matchSubTagGroupAllByName(contentStr,  tagName, tagObjName), xnlContext);
}
*/

/*
/// <summary>
        /// 根据属性名替换内容相应属性值
        /// </summary>
        /// <param name="contentStr"></param>
        /// <param name="attribNameStr"></param>
        /// <param name="varValueStr"></param>
        /// <returns></returns>
        internal static string replaceAttribleVariableByName(string contentStr, string attribNameStr, string varValueStr, string tagInstanceName)
        {
            Regex regex = new Regex("{@(@*)" + attribNameStr + "}", XNL_RegexOptions);
            MatchCollection matchColls = regex.Matches(contentStr);
            if(matchColls.Count>0)
            {
                Dictionary<int, List<string>> match1Colls = new Dictionary<int, List<string>>();
                List<string> match2Colls = new List<string>(); //可以替换的
                foreach (Match match in matchColls)
                {
                    string allMatch = match.Groups[0].Value;
                    string attribStr = match.Groups[1].Value;
                    if (attribStr.Length > 0 && attribStr.Substring(0, 1) == "@")
                    {
                        int len = attribStr.Length;
                        List<string> attrList;
                        if (match1Colls.TryGetValue(len, out attrList))
                        {
                            if (!attrList.Contains(allMatch)) attrList.Add(allMatch);
                        }
                        else //添加
                        {
                            attrList = new List<string>();
                            attrList.Add(allMatch);
                            match1Colls.Add(len, attrList);
                        }
                        //contentStr = contentStr.Replace(match.Groups[0].Value, "{" + attribStr + attribNameStr + "}");
                    }
                    else  //可以替换
                    {
                        if (!match2Colls.Contains(allMatch))
                        {
                            contentStr = contentStr.Replace(allMatch, varValueStr);
                            match2Colls.Add(allMatch);
                        }
                    }
                }
                //替换其它层变量
                for (int i = 1; i <= match1Colls.Count; i++)
                {
                    foreach (string str in match1Colls[i])
                    {
                        contentStr = contentStr.Replace(str, "{" + str.Substring(2));
                    }
                }
            }
            return contentStr;
        }
 * 
 * 
 * 
 *  /// <summary>
        /// 替换内容中的XNL标签属性变量 @
        /// </summary>
        /// <returns></returns>
        internal static string replaceAttribleVariable(XNLParams attParams, string contentStr,string tagInstanceName)
        {
            MatchCollection matchColls = Regex.Matches(contentStr, RegexStr_XNLAttriableVariable);
            if (matchColls.Count > 0)
            {
                Dictionary<int, List<string>> match1Colls = new Dictionary<int, List<string>>();
                List<string> match2Colls = new List<string>(); //可以替换的
                foreach (Match match in matchColls)
                {
                    string trueAttr=match.Groups[1].Value;
                    string attribStr = trueAttr.ToLower();
                    if (attribStr.Substring(0, 1) == "@")
                    {
                        string trueAttrStr = trueAttr.Replace("@", "");
                        string lowTrueAttr = trueAttrStr.ToLower();
                        int len = attribStr.Length - trueAttrStr.Length;
                        List<string> attrList;
                        string _attrStr = trueAttr.Substring(1);
                        if (match1Colls.TryGetValue(len, out attrList))
                        {
                            if (attParams.ContainsKey(lowTrueAttr) && !attrList.Contains(_attrStr)) attrList.Add(_attrStr);
                        }
                        else //添加
                        {
                            attrList = new List<string>();
                            attrList.Add(_attrStr);
                            match1Colls.Add(len, attrList);
                        }
                        //if (attParams.ContainsKey(attribStr.Replace("@", ""))) contentStr = contentStr.Replace(match.Groups[0].Value, "{" + attribStr + "}");
                    }
                    else  //可以替换
                    {
                        XNLParam param;
                        if (!match2Colls.Contains(trueAttr) && attParams.TryGetValue(attribStr, out param))
                        {
                            contentStr = contentStr.Replace(match.Groups[0].Value, Convert.ToString(param.value));
                            match2Colls.Add(trueAttr);
                        }
                    }

                }
                //替换其它层变量
                for (int i = 1; i <= match1Colls.Count; i++)
                {
                    foreach (string str in match1Colls[i])
                    {
                        contentStr = contentStr.Replace("{@" + str + "}", "{" + str + "}");
                    }
                }
            }
            return contentStr;
        }
 * 
 * 
 * //MatchCollection matchColls = Regex.Matches(contentStr, RegexStr_XNLReqFormVariable);
            //if (matchColls.Count>0)
            //{
            //    List<string> list = new List<string>();
            //    foreach (Match match in matchColls)
            //    {
            //        string attribStr = match.Groups[1].Value;
            //        if (list.Contains(attribStr))
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            list.Add(attribStr);
            //        }
            //        if (HttpContext.Current == null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //            return contentStr;
            //        }
            //        string reqStr = HttpContext.Current.Request.Form[attribStr];
            //        if (reqStr != null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, UtilsCode.encodeHtmlAndXnl(reqStr));
            //        }
            //        else
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //        }
            //    }
            //}
            
            //return contentStr;
 * 
 * 
 *  //MatchCollection matchColls = Regex.Matches(contentStr, RegexStr_XNLReqQueryVariable);
            //if (matchColls.Count>0)
            //{
            //    List<string> list = new List<string>();
            //    foreach (Match match in matchColls)
            //    {
            //        string attribStr = match.Groups[1].Value;
            //        if (list.Contains(attribStr))
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            list.Add(attribStr);
            //        }
            //        if (HttpContext.Current == null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //            return contentStr;
            //        }
            //        string reqStr = HttpContext.Current.Request.QueryString[attribStr];
            //        if (reqStr != null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, UtilsCode.encodeHtmlAndXnl(reqStr));
            //        }
            //        else
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //        }
            //    }
            //}
            //return contentStr;
 * 
 * 
 *    //MatchCollection matchColls = Regex.Matches(contentStr, RegexStr_XNLReqVariable);
            //if (matchColls.Count > 0)
            //{
            //    List<string> list = new List<string>();
            //    foreach (Match match in matchColls)
            //    {
            //        string attribStr = match.Groups[1].Value;
            //        if (list.Contains(attribStr))
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            list.Add(attribStr);
            //        }
            //        if (HttpContext.Current == null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //            return contentStr;
            //        }
            //        string reqStr = HttpContext.Current.Request[attribStr];
            //        if (reqStr != null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, UtilsCode.encodeHtmlAndXnl(reqStr));
            //        }
            //        else
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //        }
            //    }
            //}
            //return contentStr;
 * 
 * 
 * //MatchCollection matchColls = Regex.Matches(contentStr, RegexStr_XNLSessionVariable);
            //if (matchColls.Count > 0)
            //{
            //    List<string> list = new List<string>();
            //    foreach (Match match in matchColls)
            //    {
            //        string attribStr = match.Groups[1].Value;
            //        if (list.Contains(attribStr))
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            list.Add(attribStr);
            //        }
            //        if (HttpContext.Current == null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //            return contentStr;
            //        }
            //        string reqStr = HttpContext.Current.Session[attribStr].ToString();
            //        if (reqStr != null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, reqStr);
            //        }
            //        else
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //        }
            //    }
            //}
            //return contentStr;
 * 
 * 
 * 
 * //MatchCollection matchColls = Regex.Matches(contentStr, RegexStr_XNLApplicationVariable);
            //if (matchColls.Count>0)
            //{
            //    List<string> list = new List<string>();
            //    foreach (Match match in matchColls)
            //    {
            //        string attribStr = match.Groups[1].Value;
            //        if (list.Contains(attribStr))
            //        {
            //            continue;
            //        }
            //        else
            //        {
            //            list.Add(attribStr);
            //        }
            //        if (HttpContext.Current == null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //            return contentStr;
            //        }
            //        string reqStr = HttpContext.Current.Application[attribStr].ToString();
            //        if (reqStr != null)
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, reqStr);
            //        }
            //        else
            //        {
            //            contentStr = contentStr.Replace(match.Groups[0].Value, "");
            //        }
            //    }
            //}
            //return contentStr;
 * 
 * 
 * //return RegexObj_XNLAllPublicCariable.Replace(contentStr, allPublicVariableEvaluator);
            /*
            MatchCollection matchColls = RegexObj_XNLAllPublicCariable.Matches(contentStr);
            if (matchColls.Count > 0)
            {
                List<string> list = new List<string>();
                foreach (Match match in matchColls)
                {
                    if (list.Contains(match.Value))
                    {
                        continue;
                    }
                    else
                    {
                        list.Add(match.Value);
                    }
                    if (HttpContext.Current == null)
                    {
                        contentStr = contentStr.Replace(match.Groups[0].Value, "");
                        continue;
                        //return contentStr;
                    }
                    string attribStr = match.Groups[2].Value;
                    string attribType = match.Groups[1].Value;
                    string reqStr=null;
                    switch (attribType)
                    {
                        case "#":
                            reqStr = HttpContext.Current.Session[attribStr].ToString();
                            break;
                        case "%":
                            reqStr = HttpContext.Current.Request.Form[attribStr].ToString();
                            break;
                        case "?":
                            reqStr = HttpContext.Current.Request[attribStr].ToString();
                            break;
                        case "^":
                            reqStr = HttpContext.Current.Application[attribStr].ToString();
                            break;
                        case "&":
                            reqStr = HttpContext.Current.Request.QueryString[attribStr].ToString();
                            break;
                    }
                    if (reqStr != null)
                    {
                        contentStr = contentStr.Replace(match.Groups[0].Value, reqStr);
                    }
                    else
                    {
                        contentStr = contentStr.Replace(match.Groups[0].Value, "");
                    }
                }
            }
            return contentStr;
             */ 

/*

/// <summary>
        /// 替换内容中的上下文全局属性变量 !
        /// </summary>
        /// <returns></returns>
        internal static string replaceGlobalAttribleVariable(string contentStr, T xnlContext)
        {
            //{!([^\s>]*[\s]*[^\s>]*)}  //匹配属性变量标签
            MatchCollection matchColls = Regex.Matches(contentStr, RegexStr_XNLGlobalVariable);//, XNL_RegexOptions // RegexObj_XNLGlobalVariable.Matches(contentStr);// Regex.Matches(contentStr, RegexStr_XNLGlobalVariable); 
            if(matchColls.Count>0)
            {
                StringBuilder sb = new StringBuilder();
                int curInx = 0;
                foreach (Match match in matchColls)
                {
                    if (match.Index > curInx)
                    {
                        sb.Append(contentStr.Substring(curInx, match.Index));
                    }
                    else
                    {
                        if (xnlContext.globalAttriableColls != null)
                        {
                            string attribStr = match.Groups[1].Value.Trim().ToLower();
                            string param = null;// XNLContext.getGlobalAttriable(xnlContext, attribStr);
                            if (string.IsNullOrEmpty(param)==false)
                            {
                                sb.Append(param);
                            }
                        }
                    }
                    curInx = match.Index + match.Length;
                }
                if (curInx < contentStr.Length)
                {
                    sb.Append(contentStr.Substring(curInx,contentStr.Length));
                }
                return sb.ToString();
            }
            return contentStr;
        }
*/

/*
       private static string runTagWithErrorHandle(IXNLTag<T> xnlTagObj, XNLTagStruct tagStruct, T xnlContext, XNLOnErrorAction onErrorAction)
       {
           try
           {
               string s = "";
               xnlTagObj.onInit(null, xnlContext);
               if (xnlTagObj.isTagEnd==false)
               {
                   for (int i = 0; i < tagStruct.subTagStruct.Count; i++)
                   {
                       XNLTagStruct subTag=tagStruct.subTagStruct[i];
                       if (string.IsNullOrEmpty(subTag.tagName))
                       {
                           s += subTag.allContent;
                       }
                       else
                       {
                       }
                   }
               }
               //return xnlTagObj.main(tagStruct, xnlContext);
           }
           catch (System.Exception ex)
           {
               switch (onErrorAction)
               {
                   case XNLOnErrorAction.ThrowError:
                       throw (ex);
                   case XNLOnErrorAction.OutEmpty:
                       return string.Empty;
                   case XNLOnErrorAction.OutMsg:
                       return tagStruct.nameSpace + ":" + tagStruct.tagName + " " + ex.Message;
               }
           }
           return string.Empty;
       }
       /// <summary>
       ///  解析模板内容
       /// </summary>
       /// <param name="templeteStr">模板内容</param>
       /// <returns>页面html内容</returns>
       internal static string parse(string templateStr, T xnlContext, bool isDecodeXNLEscape, bool isDebug) // IBasePage XNLPage
       {
           //标签是否闭合
           //    //嵌套标签中是否有未命名同名标签
           //    //标签调用是否出错
           //    //是否声明标签实例
           if (string.IsNullOrEmpty(templateStr)) return string.Empty;
           string s = RegxpEngineCommon<T>.removeXNLNotes(templateStr);
           s = RegxpEngineCommon<T>.replaceAllPublicVariable(s, xnlContext);

           string namespaceName;// = MatchObj.match.Groups[1].Value.ToLower(); //标签命名空间名
           string tagName;// = MatchObj.match.Groups[2].Value.ToLower();  //标签名
           int index = 0;
           int nextIndex = 0;
           int delayTagInx = 0;
           string tagGroup0Str;// = MatchObj.match.Groups[0].Value;  //所有标签内容
           string tagGroup5Str;
           string genStr;
           XNLTagStruct tagStruct;
           IXNLTag<T> tagObj = null;
           int parseStep = 0; //解析步骤 ==2表示是解析延迟标签阶段
           //IXNLTagObj<T> ifnew = new If<T>();
           //return ifnew.ToString();
           #region 调试解析
         PARSELOOP:
           for (var j = 0; j < 1000000; j++)
           {
               if (nextIndex > index) index = nextIndex;
               Match tmpMatch = null;
               if (isDebug)
               {
                   tmpMatch = RegxpEngineCommon<T>.RegexObj_XNLTagPart1Group3.Match(s, index);
               }
               else
               {
                   tmpMatch = RegxpEngineCommon<T>.RegexObj_XNLTagGroupAll.Match(s, index);
               }
               if (!tmpMatch.Success)
               {
                   if (parseStep == 0 && index == 0)
                   {
                       goto PARSEEND;
                   }
                   else
                   {
                       break;
                   }
               }
               else
               {
                   namespaceName = tmpMatch.Groups[1].Value.ToLower();
                   tagName = tmpMatch.Groups[2].Value.ToLower();
                   string tagFullName = namespaceName + ":" + tagName;
                   nextIndex = tmpMatch.Index;
                   if (nextIndex > index) index = nextIndex;
                   Match allGroupMatch;
                   if (isDebug)
                   {
                       allGroupMatch = RegxpEngineCommon<T>.RegexObj_XNLTagGroupAll.Match(s, index);
                       if (!allGroupMatch.Success)
                       {
                           throw (new Exception(tagFullName + "标签没有闭合"));
                       }
                   }
                   else
                   {
                       allGroupMatch = tmpMatch;
                   }
                    
                   tagGroup0Str = allGroupMatch.Groups[0].Value;
                   tagGroup5Str = allGroupMatch.Groups[5].Value;
                   if (tagFullName == "xnl.mytag")
                   {
                       string userTagStr;
                       userTagStr = XNLContext.getCustomTag(xnlContext, tagName);
                       if (userTagStr == null)
                       {
                           //加载此标签内容
                           userTagStr = XNLBaseCommon.loadUserTagByName(tagName, xnlContext.workDirPath + XNLConfig.userTagSavePath + tagName + XNLConfig.userTagExtName);
                           XNLContext.setCustomTag(xnlContext, tagName, userTagStr);
                       }
                       s = s.Replace(tagGroup0Str, userTagStr);
                       continue;
                   }
                   else
                   {
                       if (parseStep == 0 && XNLLib<T>.checkTagIsDelayRun(tagFullName))
                       {
                           if (delayTagInx == 0) delayTagInx = index;
                           nextIndex = index + tmpMatch.Length;
                           continue;
                       }
                       else
                       {
                           try
                           {
                               tagObj = XNLLib<T>.getTagInstance(namespaceName, tagName);
                           }
                           catch //(ReflectionTypeLoadException e)
                           {
                               throw (new Exception("未找到标签" + tagFullName + "的实现"));
                           }
                           tagStruct = RegxpEngineCommon<T>.createTagStruct(allGroupMatch,tagObj,xnlContext);
                                                 
                           genStr = runTagWithErrorHandle(tagObj, tagStruct, xnlContext, XNLOnErrorAction.ThrowError);// tagObj.main(tagStruct.content, tagStruct.tagParams, xnlContext);
                           s = s.Replace(tagGroup0Str, genStr);
                       }
                   }
               }
           }
           #endregion
           if (parseStep == 0)
           {
               index = delayTagInx;
               nextIndex = delayTagInx;
               parseStep += 1;
               goto PARSELOOP;
           }
           else
           {
               goto PARSEEND;
           }
       PARSEEND:
           s = RegxpEngineCommon<T>.replaceGlobalAttribleVariable(s, xnlContext);
           s = RegxpEngineCommon<T>.replaceXNLExpressionVariable(s, xnlContext);
           if (isDecodeXNLEscape) s = XNLBaseCommon.decodeXNL(s);
           return s;
       }
       */

/*
      /// <summary>
      /// 替换XNL表达式
      /// </summary>
      /// <param name="labelStr"></param>
      /// <returns>表达式计算结果</returns>
      internal static string replaceXNLExpressionVariable(string labelStr, T XNLPage)
      {
          return "";
          
          MatchCollection expressMatchs = Regex.Matches(labelStr, RegexStr_XNLExpressionVariable, XNL_RegexOptions);// RegexObj_XNLExpressionVariable.Matches(labelStr);// getMatchCollsByRegex(labelStr, RegexStr_XNLExpressionVariable);
          if (expressMatchs.Count>0)
          {
              List<string> list = new List<string>();
              foreach (Match i in expressMatchs)
              {
                   
                  if (list.Contains(i.Value))
                  {
                      continue;
                  }
                  else
                  {
                      list.Add(i.Value);
                  }
                  string matchStr = i.Groups[1].Value;
                  string[] extExpTag =XNLLib<T>.getExtExpTagInfo(matchStr);
                  if (extExpTag != null)
                  {
                      XNLTagStruct tagStruct=new XNLTagStruct();
                      tagStruct.bodyContent=matchStr;
                      labelStr = labelStr.Replace(i.Value, XNLLib<T>.getTagInstance(extExpTag[0], extExpTag[1]).main(tagStruct, XNLPage));
                  }
                  else
                  {
                      string  expStr = EvalExpression(matchStr, XNLPage);
                      //labelStr = labelStr.Replace(i.Groups[0].Value, XNLBaseCommon.decodeXNL(expStr));
                      labelStr = labelStr.Replace(i.Groups[0].Value, expStr);
                  }
              }
              Match expressMatch = Regex.Match(labelStr, RegexStr_XNLExpressionVariable, XNL_RegexOptions);// RegexObj_XNLExpressionVariable.Match(labelStr);
              if (expressMatch.Success)
              {
                  labelStr = replaceXNLExpressionVariable(labelStr, XNLPage);
              }
              else
              {
                  return labelStr;
              }
          }
          return labelStr;
            
      }
*/

/*

        internal static string RequestFormVariableEvaluator(Match match)
        {
            if (HttpContext.Current == null)
            {
                return "";
            }
            string attribStr = match.Groups[1].Value;
            string reqStr = HttpContext.Current.Request.Form[attribStr].ToString();
            if (reqStr != null)
            {
                return UtilsCode.encodeHtmlAndXnl(reqStr);
            }
            else
            {
                return "";
            }
        }
        internal static MatchEvaluator requestFormVariableEvaluator = new MatchEvaluator(RequestFormVariableEvaluator);
        /// <summary>
        /// 替换request.form变量 %
        /// </summary>
        /// <returns></returns>
        internal static string replaceReqFormVariable(string contentStr, T xnlContext)
        {
            return Regex.Replace(contentStr, RegexStr_XNLReqFormVariable, RequestFormVariableEvaluator); //, XNL_RegexOptions
        }


        internal static string RequestQueryVariableEvaluator(Match match)
        {
            if (HttpContext.Current == null)
            {
                return "";
            }
            string attribStr = match.Groups[1].Value;
            string reqStr = HttpContext.Current.Request.QueryString[attribStr].ToString();
            if (reqStr != null)
            {
                return UtilsCode.encodeHtmlAndXnl(reqStr);
            }
            else
            {
                return "";
            }
        }
        internal static MatchEvaluator requestQueryVariableEvaluator = new MatchEvaluator(RequestQueryVariableEvaluator);
        /// <summary>
        /// 替换request.queryString变量  &
        /// </summary>
        /// <returns></returns>
        internal static string replaceReqQueryVariable(string contentStr, T xnlContext)
        {
            return Regex.Replace(contentStr, RegexStr_XNLReqQueryVariable, requestQueryVariableEvaluator, XNL_RegexOptions);
        }


        internal static string RequestVariableEvaluator(Match match)
        {
            if (HttpContext.Current == null)
            {
                return "";
            }
            string attribStr = match.Groups[1].Value;
            string reqStr = HttpContext.Current.Request[attribStr].ToString();
            if (reqStr != null)
            {
                return UtilsCode.encodeHtmlAndXnl(reqStr);
            }
            else
            {
                return "";
            }
        }
        internal static MatchEvaluator requestVariableEvaluator = new MatchEvaluator(RequestVariableEvaluator);
        /// 替换request.queryString变量  &
        /// </summary>
        /// <returns></returns>
        internal static string replaceRequestVariable(string contentStr, T xnlContext)
        {
            return Regex.Replace(contentStr, RegexStr_XNLReqVariable, requestVariableEvaluator); //, XNL_RegexOptions
        }

        internal static string SessionVariableEvaluator(Match match)
        {
            if (HttpContext.Current == null)
            {
                return "";
            }
            string attribStr = match.Groups[1].Value;
            string reqStr = HttpContext.Current.Session[attribStr].ToString();
            if (reqStr != null)
            {
                return UtilsCode.encodeHtmlAndXnl(reqStr);
            }
            else
            {
                return "";
            }
        }
        internal static MatchEvaluator sessionVariableEvaluator = new MatchEvaluator(SessionVariableEvaluator);
        /// <summary>
        /// 替换session变量  #
        /// </summary>
        /// <returns></returns>
        internal static string replaceSessionVariable(string contentStr, T xnlContext)
        {
            return Regex.Replace(contentStr, RegexStr_XNLSessionVariable, sessionVariableEvaluator); //, XNL_RegexOptions
        }

        internal static string ApplicationVariableEvaluator(Match match)
        {
            if (HttpContext.Current == null)
            {
                return "";
            }
            string attribStr = match.Groups[1].Value;
            string reqStr = HttpContext.Current.Application[attribStr].ToString();
            if (reqStr != null)
            {
                return UtilsCode.encodeHtmlAndXnl(reqStr);
            }
            else
            {
                return "";
            }
        }
        internal static MatchEvaluator applicationVariableEvaluator = new MatchEvaluator(ApplicationVariableEvaluator);
        /// <summary>
        /// 替换application变量 ^
        /// </summary>
        /// <returns></returns>
        internal static string replaceApplicationVariable(string contentStr, T xnlContext)
        {
            return Regex.Replace(contentStr, RegexStr_XNLApplicationVariable, applicationVariableEvaluator);  //, XNL_RegexOptions
        }

        internal static string AllPublicVariableEvaluator(Match match)
        {
            if (HttpContext.Current == null)
            {
                return "";
            }
            string attribStr = match.Groups[2].Value;
            string attribType = match.Groups[1].Value;
            string reqStr = null;
            switch (attribType)
            {
                case "#":
                    reqStr = HttpContext.Current.Session[attribStr].ToString();
                    break;
                case "%":
                    reqStr = HttpContext.Current.Request.Form[attribStr].ToString();
                    break;
                case "?":
                    reqStr = HttpContext.Current.Request[attribStr].ToString();
                    break;
                case "^":
                    reqStr = HttpContext.Current.Application[attribStr].ToString();
                    break;
                case "&":
                    reqStr = HttpContext.Current.Request.QueryString[attribStr].ToString();
                    break;
            }
            if (reqStr != null)
            {
                return UtilsCode.encodeHtmlAndXnl(reqStr);
            }
            else
            {
                return "";
            }
        }
        internal static MatchEvaluator allPublicVariableEvaluator = new MatchEvaluator(AllPublicVariableEvaluator);

        internal static string replaceAllPublicVariable(string contentStr, T xnlContext)
        {
            return Regex.Replace(contentStr, RegexStr_XNLAllPublicCariable, allPublicVariableEvaluator);  //, XNL_RegexOptions
        }

        /// 替换内容中的XNL标签属性变量 @
        /// </summary>
        /// <returns></returns>
        internal static string replaceAttribleVariable(IXNLTag<T> tagObj, string contentStr)
        {
            MatchCollection matchColls;
            if (string.IsNullOrEmpty(tagObj.instanceName)) //|| tagObj.instanceName.StartsWith("_instance#")
            {
                matchColls = Regex.Matches(contentStr, RegexStr_XNLAttriableVariable);
            }
            else
            {
                matchColls = Regex.Matches(contentStr, RegexTemplate_XNLAttriableVariable.Replace("NAME", tagObj.instanceName));
            }
            if (matchColls!=null&&matchColls.Count > 0)
            {
                int curIndex = 0;
                StringBuilder sb = new StringBuilder();

                foreach (Match match in matchColls)
                {
                    if (match.Index > curIndex)
                    {
                        sb.Append(contentStr.Substring(curIndex, match.Index - curIndex));
                    }

                    string attrName = match.Groups[1].Value.ToLower();

                    if (tagObj.ExistAttribute(attrName))
                    {
                        sb.Append(tagObj.GetAttribute(attrName).ToString());
                    }
                    else if (string.Compare(match.Groups[2].Value, tagObj.instanceName,true) != 0)
                    {
                        sb.Append(match.Value);
                    }
                    curIndex = match.Index + match.Length;
                }
                if (curIndex < contentStr.Length)
                {
                    sb.Append(contentStr.Substring(curIndex,contentStr.Length));
                }
                return sb.ToString();
            }
            return contentStr;
        }

        /// <summary>
        /// 替换数据库变量 $
        /// </summary>
        /// <returns></returns>
        internal static string replaceDataBaseVariable(DataRow dataRow, string contentStr, T xnlContext, string tagInstanceName)
        {
            MatchCollection matchColls = Regex.Matches(contentStr, RegexStr_XNLDataBaseVariable);
            if (matchColls.Count>0)
            {
                Dictionary<int, List<string>> match1Colls = new Dictionary<int, List<string>>();
                List<string> match2Colls = new List<string>(); //可以替换的
                foreach (Match match in matchColls)
                {
                    string attribStr = match.Groups[1].Value;
                    if (attribStr.Substring(0, 1) == "$")
                    {
                        string trueAttrStr = attribStr.Replace("$", "");
                        List <string> attrList;
                        int len = attribStr.Length-trueAttrStr.Length;
                        if (match1Colls.TryGetValue(len, out attrList))
                        {
                            if(dataRow.Table.Columns.Contains(trueAttrStr)&&!attrList.Contains(attribStr))attrList.Add(attribStr);
                        }
                        else //添加
                        {
                            attrList=new List<string>();
                            attrList.Add(attribStr);
                            match1Colls.Add(len, attrList);
                        }
                    }
                    else  //可以替换
                    {
                        try
                        {
                            if (!match2Colls.Contains(attribStr) && dataRow.Table.Columns.Contains(attribStr))
                            {
                                contentStr = contentStr.Replace(match.Groups[0].Value, dataRow[attribStr].ToString());
                                match2Colls.Add(attribStr);
                            }
                        }
                        catch //(Exception e)
                        {

                        }
                    }
                }
                //替换其它层变量
                for (int i = 1; i <=match1Colls.Count;i++ )
                {
                    foreach (string str in match1Colls[i])
                    {
                        contentStr = contentStr.Replace("{$" + str + "}", "{"+str+"}"); 
                    }
                }
            }
            return contentStr;
        }
*/

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
*/