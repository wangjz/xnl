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

        public static string parse(string templateStr, T xnlContext) //, OnTagDelegate<T> tagDelegate = null
        {
            if (xnlParser == null)
            {
                return "解析引擎未设置";
            }
            if (string.IsNullOrEmpty(templateStr)) return string.Empty;

            //OnTagDelegate<T> _tagDelegate = onTagAction;

            //if (tagDelegate != null) _tagDelegate = tagDelegate;
            
            //清除xnl注释标签
            templateStr = xnlParser.RemoveXNLNotes(templateStr);

            StringBuilder strBuilder = xnlContext.response.buffer;

            XNLResponse response = xnlContext.response;

            strBuilder.Capacity = Convert.ToInt32(templateStr.Length*1.5);
            //try
            //{
                //构建标签结构
                XNLTagStruct tagStruct = xnlParser.GetTagStruct(templateStr);

                if (tagStruct == null) return string.Empty;
                IXNLTag<T> tagObj = null;

                XNLTagStruct curStruct = tagStruct;
                int subTagCount = 0;
                bool isNoSubTag = false;            
                bool isTagName = false;
                Dictionary<string,IXNLTag<T>> tagsObj = new Dictionary<string,IXNLTag<T>>();
                int tagId = 0;
                string instanceName = "";
                int index=0;
                string tmpStr = null;
                bool isEnd = false;
                string fullTagName = null;
                bool isTagNew = false;
                StringBuilder codeStrBuilder = null;
                if (xnlContext.parseMode == ParseMode.Dynamic)
                {
                    codeStrBuilder = new StringBuilder(Convert.ToInt32(templateStr.Length * 1.5));
                    XNLContext.setItem(xnlContext,"__codeBuffer",codeStrBuilder);
                    codeStrBuilder.AppendLine("System.Text.StringBuilder buffer = xnlContext.response.buffer" );
                }
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
                                    tagObj = XNLLib<T>.getTagInstance(curStruct.nameSpace, curStruct.tagName);
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

                                if (codeStrBuilder != null)
                                {
                                    codeStrBuilder.AppendLine("IXNLTag<T> " + instanceName + " = XNLLib<T>.getTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\");");
                                    codeStrBuilder.AppendLine(instanceName + ".onInit(xnlContext, \"" + instanceName + "\");");
                                }
                                tagObj.onInit(xnlContext, instanceName);
                            }
                            else if (string.IsNullOrEmpty(instanceName)==false)
                            {
                                if (codeStrBuilder != null)
                                {
                                    codeStrBuilder.AppendLine("IXNLTag<T> " + instanceName + " = "+tagObj.instanceName+".create();");
                                    codeStrBuilder.AppendLine(instanceName + ".onInit(xnlContext, \"" + instanceName + "\");");
                                }
                                tagObj = tagObj.create();
                                tagObj.onInit(xnlContext, instanceName);
                            }
                            else if (string.IsNullOrEmpty(instanceName))
                            {
                                instanceName = tagObj.instanceName;
                            }
                            
                            if (curStruct.tagParams != null)
                            {
                                foreach (KeyValuePair<string, XNLParam> kv in curStruct.tagParams)
                                {
                                    tagObj.setAttributeValue(kv.Key, kv.Value.value);
                                    if (codeStrBuilder != null)
                                    {
                                        //todo 处理特殊kv
                                        codeStrBuilder.AppendLine(instanceName + ".setAttributeValue(\""+kv.Key+"\",@\""+kv.Value+"\");");
                                    }
                                }
                            }
                            //
                            tagObj.onStart();
                            if (codeStrBuilder != null)
                            {
                                codeStrBuilder.AppendLine(instanceName + ".onStart();");
                            }
                            if (isNoSubTag)
                            {
                                //检测是否有其它tag
                                //如果有其它tag，保存当前标签解析状态，将新标签设为当前标签
                                //可先用递归实现
                                XNLTagStruct t_newTag;
                                bool isHasNested = testNestedTag(curStruct.bodyContent, out t_newTag);
                                if (isHasNested) //处理嵌套情况
                                {
                                    if(t_newTag.tagName!=null)
                                    {
                                        t_newTag.parent = curStruct;
                                        //保存当前标签解析状态，将新标签设为当前标签

                                    }
                                    else
                                    {
                                        //遍历
                                    }
                                }
                                else
                                {
                                    tagObj.curTag = "";
                                    if (codeStrBuilder != null)
                                    {
                                        //创建匿名方法
                                    }
                                    onTagAction(tagObj, curStruct.bodyContent);
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
                                        //检测是否有其它tag
                                        //如果有其它tag，保存当前标签解析状态，将新标签设为当前标签

                                        tagObj.curTag = "";
                                        onTagAction(tagObj, tmpSubTag.bodyContent);
                                    }
                                    else
                                    {
                                        tagObj.curTag = tmpSubTag.tagName;
                                        if (tmpSubTag.tagParams != null)
                                        {
                                            foreach (KeyValuePair<string, XNLParam> kv in tmpSubTag.tagParams)
                                            {
                                                tagObj.setAttributeValue(kv.Key, kv.Value.value, tmpSubTag.tagName);
                                                if (codeStrBuilder != null)
                                                {
                                                    //todo 处理特殊kv
                                                    codeStrBuilder.AppendLine(instanceName + ".setAttributeValue(\"" + kv.Key + "\",@\"" + kv.Value + "\");");
                                                }
                                            }
                                        }
                                        //检测是否有其它tag
                                        //如果有其它tag，保存当前标签解析状态，将新标签设为当前标签
                                        tagObj.onTag(tmpSubTag.tagName, onTagAction, tmpSubTag.bodyContent);
                                    }
                                }
                            }
                            isEnd = true;
                            tagObj.onEnd();
                            if (codeStrBuilder != null)
                            {
                                codeStrBuilder.AppendLine(instanceName + ".onEnd();");
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
                        //判断是否存在压栈列表，如有，还原

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
            outStruct = xnlParser.GetTagStruct(content);
            //判断是否有嵌套
            if (outStruct != null && (outStruct.tagName!=null || outStruct.subTagStruct!=null))
            {
                return true;
            }
            return false;
        }
        public static void onTagAction(IXNLTag<T> tagObj, string body)
        {
            int index = tagObj.xnlContext.response.buffer.Length;
            Object itemObj = XNLContext.getItem(tagObj.xnlContext, "__codeBuffer");
            StringBuilder codeStrBuilder = null;
            if (itemObj != null)
            {
                codeStrBuilder = (StringBuilder)itemObj;
            }
            tagObj.xnlContext.response.buffer.Append(body);

            List<XNLToken> tokens = xnlParser.GetTagTokens(body);
            if (tokens != null)
            {
                object obj = null;
                foreach (XNLToken token in tokens)
                {
                    if(string.IsNullOrEmpty(token.Scope)||token.Name == tagObj.instanceName)
                    {
                        obj = tagObj.getAttributeValue(token.Name, tagObj.curTag);
                        tagObj.xnlContext.response.buffer.Remove(index + token.Index, token.Length);
                        if(obj != null)
                        {
                            tagObj.xnlContext.response.buffer.Insert(index + token.Index, obj);
                        }
                       
                    }
                }
            }
        }

        public static void onNoTagAction(T xnlContext, string body)
        {
            int index = xnlContext.response.buffer.Length;
            Object obj = XNLContext.getItem(xnlContext, "__codeBuffer");
            StringBuilder codeStrBuilder = null;
            if(obj!=null)
            {
                codeStrBuilder = (StringBuilder)obj;
            }

            List<XNLToken> tokens = xnlParser.GetTagTokens(body);
            if (tokens == null)
            {
                xnlContext.response.buffer.Append(body);
                if (codeStrBuilder!=null)
                {
                    codeStrBuilder.Append("\nbuffer.Write(@\"");
                    codeStrBuilder.Append(body);
                    codeStrBuilder.AppendLine("\");");
                }
            }
            else
            {
                //xnlContext.response.buffer.Append(body);
                //{@:site.name}
                int inx = 0;
                foreach (XNLToken token in tokens)
                {
                    if(token.Index>inx)
                    {
                        xnlContext.response.buffer.Append(body.Substring(inx, token.Index));
                        if (codeStrBuilder != null)
                        {
                            codeStrBuilder.Append("\nbuffer.Write(@\"");
                            codeStrBuilder.Append(body.Substring(inx, token.Index));
                            codeStrBuilder.AppendLine("\");\n");
                        }
                        inx=token.Index+token.Length;
                    }
                    
                    if (xnlContext.parseMode == ParseMode.Static) token.mode = ParseMode.Static;
                    switch(token.Type)
                    {
                        case XNLTokenType.Variable:
                            switch(token.Scope.ToLower())
                            {
                                case "app":
                                    xnlContext.response.buffer.Append(HttpContext.Current.Application[token.Name]);
                                    
                                    if (codeStrBuilder != null)
                                    {
                                        if (token.mode == ParseMode.Static)
                                        {
                                            codeStrBuilder.Append(HttpContext.Current.Application[token.Name]);
                                        }
                                        else
                                        {
                                            codeStrBuilder.Append("\nbuffer.Write(HttpContext.Current.Application[\"" + token.Name + "\"]);\n");
                                        }
                                    }
                                    break;
                                case "session":
                                    xnlContext.response.buffer.Append(HttpContext.Current.Session[token.Name]);
                                    if (codeStrBuilder != null)
                                    {
                                        if (token.mode == ParseMode.Static)
                                        {
                                            codeStrBuilder.Append(HttpContext.Current.Session[token.Name]);
                                        }
                                        else
                                        {
                                            codeStrBuilder.Append("\nbuffer.Write(HttpContext.Current.Session[\"" + token.Name + "\"]);\n");
                                        }
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
            } 
        }
    }
}
