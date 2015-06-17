using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
namespace Com.AimUI.TagCore
{
    public class XNLBaseCommon
    {
        internal const RegexOptions XNL_RegexOptions = (((RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline) | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
       
        /// <summary>
        /// 加载模板内容
        /// </summary>
        /// <param name="pathStr">路径</param>
        /// <param name="encoder">编码</param>
        /// <returns></returns>
        public static string LoadTemplete(string pathStr, string charset)
        {
            Encoding encoder = Encoding.GetEncoding(charset);
            StreamReader TxtReader = new StreamReader(pathStr, encoder);
            string FileContent;
            FileContent = TxtReader.ReadToEnd();
            TxtReader.Close();
            return FileContent;
        }

        /// <summary>
        /// 得到用户标签内容
        /// </summary>
        /// <param name="tagName">用户标签名称</param>
        /// <param name="siteRootPath">用户标签所属站点根目录</param>
        /// <returns></returns>
        public static string LoadUserTagByName(string tagName, string siteRootPath)
        {

            //string pathStr = (SystemConfig.systemDir + (siteRootPath.Replace("/", "\\"))).Replace("\\\\", "\\") + "Template\\UserTag\\UT_" + tagName + ".ascx";
            string pathStr = siteRootPath.Replace("/", "\\").Replace("\\\\", "\\") + "Template\\UserTag\\UT_" + tagName + ".ascx"; //先去掉systemConfig引用
            StreamReader TxtReader = new StreamReader(pathStr, System.Text.Encoding.UTF8);
            try
            {
                string FileContent = TxtReader.ReadToEnd();
                return FileContent;
            }
            catch
            {
                return "";
            }
            finally
            {
                TxtReader.Close();
            }
        }

        /// <summary>
        /// 得到模板中所有用户标签内容
        /// </summary>
        /// <param name="template"></param>
        /// <param name="sitePath"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetTemplateUserTag(string template, string sitePath)
        {
            MatchCollection matchColl = Regex.Matches(template, "<xnl\\.mytag:(\\w+)>", XNLBaseCommon.XNL_RegexOptions);
            if (matchColl.Count > 0)
            {
                Dictionary<string, string> myTagColls = new Dictionary<string, string>(matchColl.Count);
                foreach (Match m in matchColl)
                {
                    string tagName = m.Groups[1].Value.ToLower();
                    if (!myTagColls.ContainsKey(tagName))
                    {
                        myTagColls.Add(tagName, LoadUserTagByName(tagName, sitePath));
                    }
                }
                return myTagColls;
            }
            return null;
        }
        public static string EncodeXNL(string scrStr)
        {
            scrStr = scrStr.Replace("{", "&xnlal;");
            scrStr = scrStr.Replace("}", "&xnlar;");
            return scrStr;
        }
        public static string DecodeXNL(string scrStr)
        {
            scrStr = scrStr.Replace("&xnlal;", "{");
            scrStr = scrStr.Replace("&xnlar;", "}");
            return scrStr;
        }
        public static string EncodeHTML(string scrStr)
        {
            return UtilsCode.encodeHtml(scrStr);
        }
        public static string DecodeHTML(string scrStr)
        {
            scrStr = scrStr.Replace("&gt;", ">");
            scrStr = scrStr.Replace("&lt;", "<");
            scrStr = scrStr.Replace("&quot;", "\"");
            return scrStr;
        }
        
    }
}

//internal static string RegexStr_XNLTagWithName3GroupAll;
//internal const string RegexTemplate_XNLTagWithName3GroupAll = @"<(XNL):(TAG)(::NAME)([\s]*|[\s]+[^:>]+)>([.\s\S]*?)</\1:\2\3>";
//internal static Regex RegexObj_XNLTagPart1Group0;
//internal static Regex RegexObj_XNLTagPart1Group4;
//internal const string RegexStr_XNLOutTagParams = @"<attrs>(?><attrs>(?<n>)|</attrs>(?<-n>)|(?! <attrs>|</attrs>)[.\\s\\S])*?(?(n)(?!))</attrs>";
//internal const string RegexStr_XNLOutTagParam = @"<attr\\s+([^><]*)>((?><attr\\s+[^><]*?>(?<n>)|</attr>(?<-n>)|(?! <attr\\s+([^><]*?)>|</attr>)[.\\s\\S])*?)(?(n)(?!))</attr>";
//internal const string RegexStr_XNLTagAttrName = "name=\"(.*?)\"";
//internal const string RegexStr_XNLTagAttrType = "type=\"(.*?)\"";

//internal static Regex RegexObj_XNLInnerTagParams=new Regex(RegexStr_XNLInnerTagParams,XNL_RegexOptions); //"([^\\s]+?)=\"([.\\s\\S]*?)\""
//internal static Regex RegexObj_XNLOutTagParams = new Regex(RegexStr_XNLOutTagParams, XNL_RegexOptions);
//internal static Regex RegexObj_XNLOutTagParam = new Regex(RegexStr_XNLOutTagParam, XNL_RegexOptions);

//internal static Regex RegexObj_XNLTagAttrName = new Regex(RegexStr_XNLTagAttrName, XNL_RegexOptions);// "name=\"(.*?)\"";
//internal static Regex RegexObj_XNLTagAttrType = new Regex(RegexStr_XNLTagAttrType, XNL_RegexOptions);// "type=\"(.*?)\"";

//internal static Regex RegexObj_XNLTagToken = new Regex(@"{([@$])([_a-zA-Z0-9\\.:]+)}");  //@#$%

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
            //XNLType t_type;
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
                    Match nameMatch = Regex.Match(subMatch.Groups[1].Value, RegexStr_XNLTagAttrName, XNL_RegexOptions);
                    
                    if (nameMatch.Success)
                    {
                        t_paramName = nameMatch.Groups[1].Value.ToLower();
                    }

                    t_paramValue = subMatch.Groups[2].Value;
                    xnlParams.Add(t_paramName, t_paramValue);
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
                        xnlParams.Add(t_paramName, t_paramValue);
                    }
                }
            }
             */
/*
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
        */

/*
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
            
            foreach (KeyValuePair<string, object> tmpParam in labelParams2)
            {
                labelParams[tmpParam.Key] = tmpParam.Value;
            }
            return labelParams;
        }
 
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
/*
Match attrsMatch = RegxpEngineCommon<T>.MatchXNLAttrsTag(tagContentStr);
            if (attrsMatch.Success)
            {
                tagContentStr = tagContentStr.Substring(attrsMatch.Index + attrsMatch.Length); //UtilsCode.onceReplace(tagContentStr, attrsMatch.Value, "");
                XNLParams params2 = RegxpEngineCommon<T>.GetXNLParams(attrsMatch.Value);
                if (params2 != null && xnlParams != null)
                {
                    foreach (KeyValuePair<string, object> kv in params2)
                    {
                        xnlParams[kv.Key] = kv.Value;
                    }
                }
            }
*/