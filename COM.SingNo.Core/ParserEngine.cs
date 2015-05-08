using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using COM.SingNo.XNLCore.Exceptions;
namespace COM.SingNo.XNLCore
{
    public class ParserEngine<T> where T:XNLContext
    {
        public static IXNLParser<T> xnlParser { get; set; }

        public static string parse(string templateStr, T xnlContext,ParseMode parseMode = ParseMode.Static)
        {
            if (xnlParser == null)
            {
                return "解析引擎未设置";
            }

            bool isNested = false;

            XNLTagStruct tagStruct=null;

            if (templateStr == null)
            {
                object nestedTag = XNLContext.getItem(xnlContext, "$__nestedTag");
                if (nestedTag != null)
                {
                    isNested = true;
                    tagStruct = (XNLTagStruct)nestedTag;
                    XNLContext.setItem(xnlContext, "$__nestedTag",null); //清除
                }
                else
                {
                    return string.Empty;
                }
            }
            if (isNested == false && string.IsNullOrEmpty(templateStr))
            {
                return string.Empty;
            }

            StringBuilder strBuilder = xnlContext.response.buffer;

            XNLResponse response = xnlContext.response;

            if (isNested == false)
            {
                //清除xnl注释标签
                templateStr = xnlParser.RemoveXNLNotes(templateStr);
                strBuilder.Capacity = Convert.ToInt32(templateStr.Length * 1.5);
                //构建标签结构
                tagStruct = xnlParser.GetTagStruct(templateStr);
            }
            
            //try
            //{
                
                if (tagStruct == null) return string.Empty;

                Dictionary<string, IXNLTag<T>> tagsObj = null;
                Dictionary<string, IXNLTag<T>> nameTags = null;  //命名标签对象
                //StringBuilder codeStrBuilder = null;
                
                int tagId = 0;
                int index = 0;
                if (isNested)
                {
                    parseMode = (ParseMode)XNLContext.getItem(xnlContext, "$__parseMode");
                    tagsObj = (Dictionary<string, IXNLTag<T>>)XNLContext.getItem(xnlContext, "$__tags");
                    nameTags = (Dictionary<string, IXNLTag<T>>)XNLContext.getItem(xnlContext, "$__nameTags");
                    //codeStrBuilder = (StringBuilder)XNLContext.getItem(xnlContext, "$__codeBuffer");
                    tagId = (int)XNLContext.getItem(xnlContext, "$__tagid");
                    index = strBuilder.Length;
                }
                else
                {
                    XNLContext.setItem(xnlContext, "$__parseMode", parseMode);

                    tagsObj = new Dictionary<string, IXNLTag<T>>();

                    nameTags = new Dictionary<string, IXNLTag<T>>();

                    XNLContext.setItem(xnlContext, "$__tags", tagsObj);

                    XNLContext.setItem(xnlContext, "$__nameTags", nameTags);

                    if (parseMode == ParseMode.Dynamic)
                    {
                        //codeStrBuilder = new StringBuilder(Convert.ToInt32(templateStr.Length * 1.5));
                       // XNLContext.setItem(xnlContext, "$__codeBuffer", codeStrBuilder);
                        strBuilder.AppendLine("System.Text.StringBuilder buffer = xnlContext.response.buffer;");
                    }
                }
                IXNLTag<T> tagObj = null;
                XNLTagStruct curStruct = tagStruct;
                int subTagCount = 0;
                bool isNoSubTag = false;            
                bool isTagName = false;
                string instanceName = "";
                string tmpStr = null;
                bool isEnd = false;
                string fullTagName = null;
                bool isTagNew = false;
                       
                while(true)
                {
                    subTagCount = 0;
                    isNoSubTag = (curStruct.subTagStruct == null || curStruct.subTagStruct.Count == 0);
                    
                    if (isNoSubTag == false) subTagCount = curStruct.subTagStruct.Count;

                    if (subTagCount > 0 && string.IsNullOrEmpty(curStruct.tagName))
                    {
                        curStruct = curStruct.subTagStruct[0];
                    }      

                    isTagName = !string.IsNullOrEmpty(curStruct.tagName);
                    isNoSubTag = (curStruct.subTagStruct == null || curStruct.subTagStruct.Count == 0);
                    if (isNoSubTag == false) subTagCount = curStruct.subTagStruct.Count;

                    if (isNoSubTag && isTagName == false)
                    {
                        index = strBuilder.Length;
                        onNoTagAction(xnlContext, curStruct.bodyContent);
                    }
                    else if (isTagName)
                    {
                        isTagNew = false;
                        isEnd = false;
                        index = strBuilder.Length;
                        //try
                        //{
                            fullTagName = curStruct.nameSpace + ":" + curStruct.tagName;
                            if (tagsObj.TryGetValue(fullTagName, out tagObj) == false)
                            {
                                isTagNew = true;
                                try
                                {
                                    tagObj = XNLLib<T>.getTagInstance(curStruct.nameSpace, curStruct.tagName).Create();
                                }
                                catch
                                {
                                    throw (new Exception("未找到标签" + fullTagName + "的实现"));
                                }
                                tagsObj[fullTagName] = tagObj;
                            }
                            
                            instanceName = curStruct.instanceName;
                           
                            if (isTagNew)
                            {
                                if (string.IsNullOrEmpty(instanceName))
                                {
                                    instanceName = "tag_" + tagId;
                                    curStruct.instanceName = "";// instanceName;
                                    tagId += 1;
                                }
                                tagObj.OnInit(xnlContext, instanceName);
                                if(parseMode==ParseMode.Dynamic)
                                {
                                    strBuilder.Insert(0, "IXNLTag<T> " + instanceName + "= null;\n"); //"IXNLTag<T> " +
                                    strBuilder.AppendLine("\n" + instanceName + " = XNLLib<T>.getTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\").Create();");
                                    strBuilder.AppendLine(instanceName + ".OnInit(xnlContext, \"" + instanceName + "\");");
                                }
                            }
                            else if (string.IsNullOrEmpty(instanceName)==false) //命名tag
                            {
                                IXNLTag<T> newTagObj ;
                                var fullInsName=fullTagName+"_"+instanceName;
                                bool isCreate = nameTags.TryGetValue(fullInsName, out newTagObj);
                                if (isCreate == false)
                                {
                                    newTagObj = tagObj.Create();
                                    nameTags[fullInsName] = newTagObj;
                                    if (parseMode == ParseMode.Dynamic)
                                    {
                                        strBuilder.Insert(0, "IXNLTag<T> " + instanceName + "=null;\n"); //"IXNLTag<T> " +
                                        strBuilder.AppendLine("\n"+instanceName + " = " + tagObj.instanceName + ".Create();");
                                        strBuilder.AppendLine(instanceName + ".OnInit(xnlContext, \"" + instanceName + "\");");
                                    }
                                    tagObj = newTagObj;
                                    tagObj.OnInit(xnlContext, instanceName);
                                }
                                else
                                {
                                    if (parseMode == ParseMode.Dynamic)
                                    {
                                        strBuilder.AppendLine("if(" + instanceName + "==null)\n{\tif(" + tagObj.instanceName + "==null){" + tagObj.instanceName + " = XNLLib<T>.getTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\").Create();" + tagObj.instanceName + ".OnInit(xnlContext, \"" + instanceName + "\");}\n\t" + instanceName + " = " + tagObj.instanceName + ".Create();");
                                        strBuilder.AppendLine("\t"+instanceName + ".OnInit(xnlContext, \"" + instanceName + "\");\n}");
                                    }
                                    tagObj = newTagObj;
                                }
                            }
                            else if (string.IsNullOrEmpty(instanceName))
                            {
                                instanceName = tagObj.instanceName;
                                if (parseMode == ParseMode.Dynamic)
                                {
                                    strBuilder.AppendLine("if(" + instanceName + "==null)\n{\t" + instanceName + " = XNLLib<T>.getTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\").Create();");
                                    strBuilder.AppendLine("\t"+instanceName + ".OnInit(xnlContext, \"" + instanceName + "\");\n}");
                                }
                            }
                            
                            if (curStruct.tagParams != null)
                            {
                                foreach (KeyValuePair<string, XNLParam> kv in curStruct.tagParams)
                                {
                                    //todo 处理特殊kv
                                    if (parseMode == ParseMode.Dynamic)
                                    {

                                        strBuilder.AppendLine(instanceName + ".SetAttribute(\"" + kv.Key + "\",@\"" + kv.Value.value + "\");");
                                    }
                                    else
                                    {
                                        tagObj.SetAttribute(kv.Key, kv.Value.value);
                                    }
                                }
                            }
                            //
                            if (parseMode == ParseMode.Dynamic)
                            {
                                strBuilder.AppendLine(instanceName + ".OnStart();");
                            }
                            else
                            {
                                tagObj.OnStart();
                            }
                            
                           
                            if (isNoSubTag)
                            {
                                tagObj.curTag = null;
                                if (parseMode == ParseMode.Dynamic)
                                {
                                    strBuilder.AppendLine("OnTagDelegate " + instanceName + "_delegate=delegate (){");
                                    XNLContext.setItem(xnlContext, "$__curStruct", curStruct);
                                    XNLContext.setItem(xnlContext, "$__tagid", tagId);
                                    OnTagAction(tagObj);
                                    strBuilder.AppendLine("};");
                                    strBuilder.AppendLine(instanceName + ".OnTag(" + instanceName + "_delegate);");
                                }
                                else
                                {
                                    tagObj.OnTag(delegate()
                                    {
                                        XNLContext.setItem(xnlContext, "$__curStruct", curStruct);
                                        XNLContext.setItem(xnlContext, "$__tagid", tagId);
                                        OnTagAction(tagObj);
                                    });
                                }
                            }
                            else
                            {
                                XNLTagStruct tmpSubTag = null;
                                for (int i = 0; i < subTagCount; i++)
                                {
                                    tmpSubTag = curStruct.subTagStruct[i];
                                    if (string.IsNullOrEmpty(tmpSubTag.tagName))
                                    {
                                        tagObj.curTag = "";
                                        //, tmpSubTag.bodyContent
                                        XNLContext.setItem(xnlContext, "$__curStruct", tmpSubTag);
                                        XNLContext.setItem(xnlContext, "$__tagid", tagId);
                                        OnTagAction(tagObj);
                                    }
                                    else
                                    {
                                        tagObj.curTag = tmpSubTag.tagName;
                                        if (tmpSubTag.tagParams != null)
                                        {
                                            foreach (KeyValuePair<string, XNLParam> kv in tmpSubTag.tagParams)
                                            {
                                                //todo 处理特殊kv
                                                if (parseMode == ParseMode.Dynamic)
                                                {
                                                    strBuilder.AppendLine(instanceName + ".SetAttribute(\"" + kv.Key + "\",@\"" + kv.Value + "\");");
                                                }
                                                else
                                                {
                                                    tagObj.SetAttribute(kv.Key, kv.Value.value, tmpSubTag.tagName);
                                                }
                                            }
                                        }
                                        if (parseMode == ParseMode.Dynamic)
                                        {
                                            strBuilder.AppendLine("OnTagDelegate " + instanceName + "_" + tagObj.curTag + "_delegate=delegate (){");
                                            XNLContext.setItem(xnlContext, "$__curStruct", tmpSubTag);
                                            XNLContext.setItem(xnlContext, "$__tagid", tagId);
                                            OnTagAction(tagObj);
                                            strBuilder.AppendLine("};");
                                            strBuilder.AppendLine(instanceName + ".OnTag(" + instanceName + "_" + tagObj.curTag + "_delegate);");
                                        }
                                        else
                                        {
                                            //tmpSubTag  保存到context中
                                            tagObj.OnTag(delegate()
                                            {
                                                XNLContext.setItem(xnlContext, "$__curStruct", tmpSubTag);
                                                XNLContext.setItem(xnlContext, "$__tagid", tagId);
                                                OnTagAction(tagObj);
                                            });
                                        }
                                    }
                                }
                            }

                            isEnd = true;
                            if (parseMode == ParseMode.Dynamic)
                            {
                                strBuilder.AppendLine(instanceName + ".OnEnd();");
                            }
                            else
                            {
                                tagObj.OnEnd();
                            }
                        //}
                        //catch (TagStopException)
                        //{
                        //    if(isEnd==false)tagObj.onEnd();
                        //}
                        //catch (ResponseEndException)
                        //{
                        //    break;
                        //}
                        //catch (Exception e)
                        //{
                        //    strBuilder.Append("标签[" + curStruct.nameSpace + ":" + curStruct.tagName + "]出错:" + e.Message);
                        //}
                    }

                    if (curStruct == tagStruct)
                    {
                        tagStruct = null;
                    }
                    else if (tagStruct.subTagStruct != null && tagStruct.subTagStruct.Count > 0)
                    {
                        tagStruct.subTagStruct.Remove(curStruct);
                        if (tagStruct.subTagStruct == null || tagStruct.subTagStruct.Count == 0) // && string.IsNullOrEmpty(tagStruct.tagName)) || string.IsNullOrEmpty(tagStruct.tagName)==false
                        {
                            tagStruct = null;
                        }
                        else
                        {
                            curStruct = tagStruct;
                        }
                    }

                    bool isTagEnd = (tagStruct == null);

                    if(isTagName)
                    {
                        tmpStr = strBuilder.ToString(index, strBuilder.Length - index);

                        curStruct = xnlParser.GetTagStruct(tmpStr);

                        isTagEnd = (curStruct == null || ((curStruct.subTagStruct == null || curStruct.subTagStruct.Count == 0) && string.IsNullOrEmpty(curStruct.tagName)));

                        if (isTagEnd == false)
                        {
                            //构造新的结构
                            if (tagStruct == null)
                            {
                                tagStruct = curStruct;
                            }
                            else
                            {
                                //两种结果
                                //1 整个内容是一个标签
                                if (string.IsNullOrEmpty(curStruct.tagName) == false)
                                {
                                    tagStruct.subTagStruct.Insert(0, curStruct);
                                }
                                else
                                {
                                    //2多个标签在一起 tagname 为空
                                    subTagCount = curStruct.subTagStruct.Count;
                                    for (int i = 0; i < subTagCount; i++)
                                    {
                                        tagStruct.subTagStruct.Insert(i, curStruct.subTagStruct[i]);
                                    }
                                }
                            }
                            curStruct = tagStruct;
                            strBuilder.Remove(index, strBuilder.Length-index);
                        }
                        else
                        {
                            curStruct = tagStruct;
                        }
                    }

                    if (tagStruct == null)
                    {
                        break;
                    }
                }
            //}
            //catch (Exception e)
            //{
            //    return e.Message;
            //}
            //if (codeStrBuilder != null) return codeStrBuilder.ToString();
            return strBuilder.ToString();
        }

        static bool testNestedTag(string content,out XNLTagStruct outStruct)
        {

            if (string.IsNullOrEmpty(content))
            {
                outStruct = null;
                return false;
            }
            outStruct = xnlParser.GetTagStruct(content);
            //判断是否有嵌套
            if (outStruct != null && (outStruct.tagName!=null || outStruct.subTagStruct!=null))
            {
                return true;
            }
            return false;
        }
     
        
        public static void OnTagAction(IXNLTag<T> tagObj)
        {
            XNLContext xnlContext = tagObj.xnlContext;
            XNLTagStruct curStruct = (XNLTagStruct)XNLContext.getItem(xnlContext, "$__curStruct");
            XNLContext.setItem(xnlContext, "$__curStruct", null);
            string body = curStruct.bodyContent;
            //检测是否有其它tag
            //如果有其它tag，递归解析
            XNLTagStruct t_newTag;
            bool isHasNested = testNestedTag(body, out t_newTag);
            if (isHasNested) //处理嵌套情况
            {
                t_newTag.parent = curStruct;
                if (t_newTag.tagName != null) //完全嵌套
                {
                    XNLContext.setItem(xnlContext, "$__nestedTag", t_newTag);

                    ParserEngine<XNLContext>.parse(null, xnlContext);

                }
                else
                {
                    //遍历
                    var t_count = t_newTag.subTagStruct.Count;
                    XNLTagStruct t_subtag = null;
                    for (var i = 0; i < t_count; i++)
                    {
                        t_subtag = t_newTag.subTagStruct[i];
                        if (t_subtag.tagName != null) //完全嵌套
                        {
                            t_subtag.parent = curStruct;
                            XNLContext.setItem(xnlContext, "$__nestedTag", t_subtag);
                            ParserEngine<XNLContext>.parse(null, xnlContext);
                        }
                        else
                        {
                            ParseAction(xnlContext, tagObj, t_subtag.bodyContent);
                        }
                    }
                }
            }
            else
            {
                ParseAction(xnlContext, tagObj, body);
            }
        }


        public static void ParseAction(XNLContext xnlContext,IXNLTag<T> tagObj,string body)
        {
            if (string.IsNullOrEmpty(body)) return;
            ParseMode parseMode =(ParseMode) XNLContext.getItem(xnlContext, "$__parseMode");
            bool isDynamic = (parseMode == ParseMode.Dynamic);
            StringBuilder strBuilder = xnlContext.response.buffer;
            List<XNLToken> tokens = xnlParser.GetTagTokens(body);
            if (tokens != null)
            {
                int index = 0;
                object obj = null;
                foreach (XNLToken token in tokens)
                {
                    var len = token.Index - index;
                    if (len > 0)
                    {
                        var str = body.Substring(index, len);
                        if (isDynamic)
                        {
                            strBuilder.Append("\nbuffer.Append(@\"");
                            strBuilder.Append(str);
                            strBuilder.AppendLine("\");");
                        }
                        else
                        {
                            strBuilder.Append(str);
                        }
                    }
                    index = token.Index + token.Length;
                    if (string.IsNullOrEmpty(token.Scope) || token.Name == tagObj.instanceName)
                    {
                        if (isDynamic)
                        {
                            bool isHas = tagObj.ExistAttribute(token.Name, tagObj.curTag);
                            if (isHas)
                            {
                                strBuilder.Append("\nbuffer.Append(");
                                strBuilder.Append(tagObj.instanceName + ".GetAttribute(\"" + token.Name + "\",\"" + tagObj.curTag + "\"));");
                                //strBuilder.AppendLine("\");");
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            bool isHas = tagObj.TryGetAttribute(out obj,token.Name, tagObj.curTag);
                            if (isHas && obj!=null)
                            {
                                strBuilder.Append(obj);
                            }
                        }
                    }
                }
                if(index<body.Length)
                {
                    if (isDynamic)
                    {
                        strBuilder.Append("\nbuffer.Append(@\"");
                        strBuilder.Append(body.Substring(index));
                        strBuilder.AppendLine("\");");
                    }
                    else
                    {
                        strBuilder.Append(body.Substring(index));
                    }
                }
            }
            else
            {
                if (isDynamic)
                {
                    strBuilder.Append("\nbuffer.Append(@\"");
                    strBuilder.Append(body);
                    strBuilder.AppendLine("\");");
                }
                else
                {
                    strBuilder.Append(body);
                }
            }
        }
        //todo 判断是否有嵌套 ??
        public static void onNoTagAction(T xnlContext, string body)
        {
            ParseMode parseMode = (ParseMode)XNLContext.getItem(xnlContext, "$__parseMode");
            bool isDynamic = (parseMode == ParseMode.Dynamic);
            StringBuilder strBuilder = xnlContext.response.buffer;
            //int index = strBuilder.Length;

            List<XNLToken> tokens = xnlParser.GetTagTokens(body);
            if (tokens == null)
            {
                if(isDynamic)
                {
                    strBuilder.Append("\nbuffer.Append(@\"");
                    strBuilder.Append(body);
                    strBuilder.AppendLine("\");");
                }
                else
                {
                    strBuilder.Append(body);
                }
            }
            else
            {
                //{@:site.name}
                int inx = 0;
                foreach (XNLToken token in tokens)
                {
                    var len = token.Index - inx;
                    if (len > 0)
                    {
                        if (isDynamic)
                        {
                            strBuilder.Append("\nbuffer.Append(@\"");
                            strBuilder.Append(body.Substring(inx, len));
                            strBuilder.AppendLine("\");\n");
                        }
                        else
                        {
                            strBuilder.Append(body.Substring(inx, token.Index));
                        }
                    }
                    inx = token.Index + token.Length;
                    if (isDynamic==false) token.mode = ParseMode.Static;
                    switch(token.Type)
                    {
                        case XNLTokenType.Variable:
                            switch(token.Scope.ToLower())
                            {
                                case "app":
                                    if (isDynamic)
                                    {
                                        if (token.mode == ParseMode.Static)
                                        {
                                            strBuilder.Append(HttpContext.Current.Application[token.Name]);
                                        }
                                        else
                                        {
                                            strBuilder.Append("\nbuffer.Append(HttpContext.Current.Application[\"" + token.Name + "\"]);\n");
                                        }
                                    }
                                    else
                                    {
                                        strBuilder.Append(HttpContext.Current.Application[token.Name]);
                                    }
                                    break;
                                case "session":
                                    if (isDynamic)
                                    {
                                        if (token.mode == ParseMode.Static)
                                        {
                                            strBuilder.Append(HttpContext.Current.Session[token.Name]);
                                        }
                                        else
                                        {
                                            strBuilder.Append("\nbuffer.Append(HttpContext.Current.Session[\"" + token.Name + "\"]);\n");
                                        }
                                    }
                                    else
                                    {
                                        strBuilder.Append(HttpContext.Current.Session[token.Name]);
                                    }
                                    break;
                                case "post":
                                    if (token.mode == ParseMode.Static)
                                    {

                                    }
                                    else
                                    {

                                    }
                                    break;
                                case "get":
                                    if (token.mode == ParseMode.Static)
                                    {

                                    }
                                    else
                                    {

                                    }
                                    break;
                                case "request":
                                    if (token.mode == ParseMode.Static)
                                    {

                                    }
                                    else
                                    {

                                    }
                                    break;
                            }
                            break;
                        case XNLTokenType.Express:

                            break;
                        case XNLTokenType.Attribute:

                            break;
                    }
                }
                if (inx < body.Length)
                {
                    if (isDynamic)
                    {
                        strBuilder.Append("\nbuffer.Append(@\"");
                        strBuilder.Append(body.Substring(inx));
                        strBuilder.AppendLine("\");");
                    }
                    else
                    {
                        strBuilder.Append(body.Substring(inx));
                    }
                }
            } 
        }
    }
}
