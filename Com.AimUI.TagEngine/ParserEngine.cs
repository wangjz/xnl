using System;
using System.Collections.Generic;
using System.Text;
using Com.AimUI.TagCore.Exceptions;
using System.Collections;
using Com.AimUI.TagCore;

namespace Com.AimUI.TagEngine
{
    public class ParserEngine<T> where T:TagContext
    {
        public static IParser<T> tagParser { get; private set; }

        public static void Initialize(IParser<T> parser, List<TagLib<T>> tagLibs = null)
        {
            tagParser = parser;
            TagLib<T>.Initialize(parser, tagLibs);
        }

        public static string ParseToString(string templateStr, T tagContext, ParseMode parseMode = ParseMode.Static)
        {
            Parse(templateStr,tagContext,parseMode);
            return tagContext.GetTagResponse().buffer.ToString();
        }

        public static void Parse(string templateStr, T tagContext, ParseMode parseMode = ParseMode.Static)
        {
            bool isNested = false;
            bool isTop = true;
            TagStruct tagStruct = null;

            if (templateStr == null)
            {
                object nestedTag = TagContext.GetItem(tagContext, "$__nestedTag");
                if (nestedTag != null)
                {
                    isTop = false;
                    isNested = true;
                    tagStruct = (TagStruct)nestedTag;
                    TagContext.RemoveItem(tagContext, "$__nestedTag"); //清除
                }
                else
                {
                    return;
                }
            }
            if (isNested == false && string.IsNullOrEmpty(templateStr))
            {
                return;
            }


            TagResponse response = tagContext.GetTagResponse();
            StringBuilder strBuilder = response.buffer;
            bool isDynamic = (parseMode == ParseMode.Dynamic);

            try
            {

                Dictionary<string, ITag<T>> tagsObj = null;
                Dictionary<string, ITag<T>> nameTags = null;  //命名标签对象

                int tagId = 0;
                int index = strBuilder.Length;
                if (isNested)
                {
                    tagsObj = (Dictionary<string, ITag<T>>)TagContext.GetItem(tagContext, "$__tags");
                    nameTags = (Dictionary<string, ITag<T>>)TagContext.GetItem(tagContext, "$__nameTags");
                    tagId = (int)TagContext.GetItem(tagContext, "$__tagid");

                }
                else
                {
                    object _tagsObj = TagContext.GetItem(tagContext, "$__tags");
                    if (_tagsObj != null)
                    {
                        isTop = false;
                        tagsObj = (Dictionary<string, ITag<T>>)_tagsObj;
                        nameTags = (Dictionary<string, ITag<T>>)TagContext.GetItem(tagContext, "$__nameTags");
                        tagId = (int)TagContext.GetItem(tagContext, "$__tagid");
                    }
                    else
                    {
                        tagsObj = new Dictionary<string, ITag<T>>();

                        nameTags = new Dictionary<string, ITag<T>>();

                        TagContext.SetItem(tagContext, "$__tags", tagsObj);

                        TagContext.SetItem(tagContext, "$__nameTags", nameTags);

                        TagContext.SetItem(tagContext, "$__tagid", tagId);
                    }

                    //清除注释标签
                    templateStr = tagParser.RemoveNoteTag(templateStr);
                    if (isTop) strBuilder.Capacity = Convert.ToInt32(templateStr.Length * 1.5);
                    //构建标签结构
                    tagStruct = tagParser.GetTagStruct(templateStr);

                    if (isDynamic && isTop)
                    {
                        if (tagStruct == null)
                        {
                            strBuilder.AppendLine("if(buffer==null) buffer = tagContext.response.buffer;");
                        }
                        else
                        {
                            strBuilder.AppendLine("if(buffer==null) buffer = tagContext.response.buffer;\ntry\n{");
                        }
                    }
                }
                if (tagStruct == null)
                {
                    OnNoTagAction(tagContext, templateStr, tagsObj, isDynamic);
                    return;
                }
                ITag<T> tagObj = null;
                TagStruct curStruct = tagStruct;
                int subTagCount = 0;
                bool isNoSubTag = false;
                bool isTagName = false;
                string instanceName = "";
                bool isEnd = false;
                string fullTagName = null;
                bool isTagNew = false;

                while (true)
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
                        if (string.IsNullOrEmpty(curStruct.bodyContent) == false) OnNoTagAction(tagContext, curStruct.bodyContent, tagsObj, isDynamic);
                    }
                    else if (isTagName)
                    {
                        if (curStruct.tagName == "inc" && curStruct.nameSpace == "at" && curStruct.tagParams!=null)
                        {
                            //include tag
                            string ref_ns = curStruct.nameSpace;
                            string ref_tagName = curStruct.tagName;
                            string include = tagContext.GetInclude(curStruct.tagParams["src"], ref ref_ns, ref ref_tagName);
                            if ("at" != ref_ns) curStruct.nameSpace = ref_ns;
                            if ("inc" != ref_tagName) curStruct.tagName = ref_tagName;

                            if (string.IsNullOrEmpty(include) == false)
                            {
                                curStruct.bodyContent = include;
                                curStruct.subTagStruct = null;
                            }

                            string isStatic = Convert.ToString(curStruct.tagParams["static"]);

                            if (isDynamic && isStatic != null && ("1" == isStatic || "true" == isStatic.Trim()))
                            {
                                curStruct.tagName = "inc2";
                                index = strBuilder.Length;
                                TagContext.SetItem(tagContext, "$__tagid", tagId);
                                TagContext.SetItem(tagContext, "$__nestedTag", curStruct);
                                Parse(null, tagContext);
                                int len = strBuilder.Length - index;
                                if (len > 0)
                                {
                                    include = strBuilder.ToString(index, len);
                                    strBuilder.Remove(index, len);

                                    strBuilder.Append("\nbuffer.Append(@\"");
                                    strBuilder.Append(include.Replace("\"", "\"\""));
                                    strBuilder.AppendLine("\");");
                                }
                                goto TagNext;
                            }
                        }
                        else if (isNested && isDynamic == false && curStruct.tagName == "inc2" && curStruct.nameSpace == "at")
                        {
                            curStruct.tagName = "inc";
                        }

                        if (curStruct.nameSpace == "at" && curStruct.tagName == "pre")
                        {
                            if (isDynamic)
                            {
                                strBuilder.Append("\nbuffer.Append(@\"");
                                strBuilder.Append(curStruct.bodyContent.Replace("\"", "\"\""));
                                strBuilder.AppendLine("\");");
                            }
                            else
                            {
                                strBuilder.Append(curStruct.bodyContent);
                            }
                            goto TagNext;
                        }

                        isTagNew = false;
                        isEnd = false;
                        index = strBuilder.Length;

                        try
                        {

                            fullTagName = curStruct.nameSpace + ":" + curStruct.tagName;

                            isTagNew = GetTagInstance(curStruct.nameSpace, curStruct.tagName, tagsObj, out tagObj);

                            instanceName = curStruct.instanceName;

                            if (isTagNew)
                            {
                                if (string.IsNullOrEmpty(instanceName))
                                {
                                    instanceName = "t__" + tagId;
                                    curStruct.instanceName = "";// instanceName;
                                    tagId += 1;
                                }
                                else
                                {
                                    //检测是否命名冲突
                                    if (nameTags.ContainsKey(instanceName))
                                    {
                                        throw new TagParseException("tag " + fullTagName + "#" + instanceName + ":命名冲突");
                                    }
                                    var fullInsName = fullTagName + "_" + instanceName;
                                    nameTags[fullInsName] = tagObj;
                                    nameTags[instanceName] = tagObj;
                                }
                                //tagId += 1;
                                tagObj.tagContext = tagContext;
                                tagObj.instanceName = instanceName;
                                if (isDynamic)
                                {
                                    strBuilder.Insert(0, "Com.AimUI.TagCore.ITag<T> " + instanceName + "= null;\n");
                                    strBuilder.AppendLine("\n" + instanceName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\").Create();");
                                    strBuilder.AppendLine(instanceName + ".tagContext = tagContext;");
                                    strBuilder.AppendLine(instanceName + ".instanceName = \"" + instanceName + "\";");
                                    strBuilder.AppendLine(instanceName + ".OnInit();");
                                }
                                else
                                {
                                    tagObj.OnInit();
                                }
                            }
                            else if (string.IsNullOrEmpty(instanceName) == false) //命名tag
                            {
                                ITag<T> newTagObj;
                                var fullInsName = fullTagName + "_" + instanceName;
                                bool isCreate = nameTags.TryGetValue(fullInsName, out newTagObj);
                                if (isCreate == false)
                                {
                                    //检测是否命名冲突
                                    if (nameTags.ContainsKey(instanceName))
                                    {
                                        throw new TagParseException("tag " + fullTagName + "#" + instanceName + ":命名冲突");
                                    }

                                    newTagObj = tagObj.Create();
                                    nameTags[fullInsName] = newTagObj;
                                    nameTags[instanceName] = newTagObj;
                                    if (isDynamic)
                                    {
                                        strBuilder.Insert(0, "Com.AimUI.TagCore.ITag<T> " + instanceName + "=null;\n");
                                        strBuilder.AppendLine("\n" + instanceName + " = " + tagObj.instanceName + ".Create();");
                                        strBuilder.AppendLine(instanceName + ".tagContext = tagContext;");
                                        strBuilder.AppendLine(instanceName + ".instanceName = \"" + instanceName + "\";");
                                        strBuilder.AppendLine(instanceName + ".OnInit();");
                                        tagObj = newTagObj;
                                        tagObj.tagContext = tagContext;
                                        tagObj.instanceName = instanceName;
                                    }
                                    else
                                    {
                                        tagObj = newTagObj;
                                        tagObj.tagContext = tagContext;
                                        tagObj.instanceName = instanceName;
                                        tagObj.OnInit();
                                    }

                                }
                                else
                                {
                                    if (isDynamic)
                                    {
                                        strBuilder.AppendLine("if(" + instanceName + "==null)\n{\t" + instanceName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\").Create();");
                                        strBuilder.AppendLine("\t" + instanceName + ".tagContext = tagContext;");
                                        strBuilder.AppendLine("\t" + instanceName + ".instanceName = \"" + instanceName + "\";");
                                        strBuilder.AppendLine("\t" + instanceName + ".OnInit();\n}");
                                    }
                                    tagObj = newTagObj;
                                }
                            }
                            else if (string.IsNullOrEmpty(instanceName))
                            {
                                instanceName = tagObj.instanceName;
                                if (isDynamic)
                                {
                                    strBuilder.AppendLine("if(" + instanceName + "==null)\n{\t" + instanceName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\").Create();");
                                    strBuilder.AppendLine("\t" + instanceName + ".tagContext = tagContext;");
                                    strBuilder.AppendLine("\t" + instanceName + ".instanceName = \"" + instanceName + "\";");
                                    strBuilder.AppendLine("\t" + instanceName + ".OnInit();\n}");
                                }
                            }
                            curStruct.tagObj = tagObj;
                            if (curStruct.tagParams != null)
                            {
                                ParseTagParams(tagContext, strBuilder, curStruct.tagParams, tagObj, instanceName, curStruct, tagsObj, isDynamic);
                            }

                            if (isDynamic)
                            {
                                strBuilder.AppendLine("try{");
                                strBuilder.AppendLine(instanceName + ".OnStart();");
                            }
                            else
                            {
                                tagObj.OnStart();
                            }


                            if (isNoSubTag)
                            {
                                tagObj.curTag = null;
                                if (isDynamic)
                                {
                                    strBuilder.AppendLine(instanceName + ".curTag = null;");
                                    if (string.IsNullOrEmpty(curStruct.bodyContent.Trim()) == false)
                                    {
                                        strBuilder.AppendLine("Com.AimUI.TagCore.OnTagDelegate " + instanceName + "_delegate=delegate (){");
                                        TagContext.SetItem(tagContext, "$__tagid", tagId);
                                        OnTagAction(tagObj, curStruct, tagsObj, isDynamic);
                                        strBuilder.AppendLine("};");
                                        strBuilder.AppendLine(instanceName + ".OnTag(" + instanceName + "_delegate);");
                                    }
                                    else
                                    {
                                        strBuilder.AppendLine(instanceName + ".OnTag(null);");
                                    }
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(curStruct.bodyContent.Trim()) == false)
                                    {
                                        tagObj.OnTag(delegate()
                                        {
                                            TagContext.SetItem(tagContext, "$__tagid", tagId);
                                            OnTagAction(tagObj, curStruct, tagsObj, isDynamic);
                                        });
                                    }
                                    else
                                    {
                                        tagObj.OnTag(null);
                                    }
                                }
                            }
                            else
                            {
                                TagStruct tmpSubTag = null;
                                for (int i = 0; i < subTagCount; i++)
                                {
                                    tmpSubTag = curStruct.subTagStruct[i];
                                    tmpSubTag.parent = curStruct;
                                    tmpSubTag.tagObj = tagObj;
                                    if (string.IsNullOrEmpty(tmpSubTag.tagName))
                                    {
                                        tagObj.curTag = "";
                                        if (string.IsNullOrEmpty(tmpSubTag.bodyContent.Trim()) == false)
                                        {
                                            TagContext.SetItem(tagContext, "$__tagid", tagId);
                                            OnTagAction(tagObj, tmpSubTag, tagsObj, isDynamic);
                                        }
                                    }
                                    else
                                    {
                                        tagObj.curTag = tmpSubTag.tagName;
                                        if (tmpSubTag.tagParams != null)
                                        {
                                            ParseTagParams(tagContext, strBuilder, tmpSubTag.tagParams, tagObj, instanceName, tmpSubTag, tagsObj, isDynamic);
                                        }
                                        if (isDynamic)
                                        {
                                            strBuilder.AppendLine(instanceName + ".curTag = @\"" + tmpSubTag.tagName + "\";");
                                            if (string.IsNullOrEmpty(tmpSubTag.bodyContent.Trim()) == false)
                                            {
                                                strBuilder.AppendLine("Com.AimUI.TagCore.OnTagDelegate " + instanceName + "_" + tagObj.curTag + "_delegate=delegate (){");
                                                TagContext.SetItem(tagContext, "$__tagid", tagId);
                                                OnTagAction(tagObj, tmpSubTag, tagsObj, isDynamic);
                                                strBuilder.AppendLine("};");
                                                strBuilder.AppendLine(instanceName + ".OnTag(" + instanceName + "_" + tagObj.curTag + "_delegate);");
                                            }
                                            else
                                            {
                                                strBuilder.AppendLine(instanceName + ".OnTag(null);");
                                            }
                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(tmpSubTag.bodyContent.Trim()) == false)
                                            {
                                                //tmpSubTag  保存到context中
                                                tagObj.OnTag(delegate()
                                                {
                                                    TagContext.SetItem(tagContext, "$__tagid", tagId);
                                                    OnTagAction(tagObj, tmpSubTag, tagsObj, isDynamic);
                                                });
                                            }
                                            else
                                            {
                                                tagObj.OnTag(null);
                                            }
                                        }
                                    }
                                }
                            }

                            isEnd = true;
                            if (isDynamic)
                            {
                                strBuilder.AppendLine(instanceName + ".OnEnd();");
                                strBuilder.AppendLine("}\ncatch (Com.AimUI.TagCore.Exceptions.TagStopException)\n{");
                                strBuilder.AppendLine(instanceName + ".OnEnd();\n}");
                            }
                            else
                            {
                                tagObj.OnEnd();
                            }
                        }
                        catch (TagStopException)
                        {
                            if (isEnd == false) tagObj.OnEnd();
                        }
                        catch (ResponseEndException)
                        {
                            break;
                        }
                        catch (TagParseException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("标签[" + curStruct.nameSpace + ":" + curStruct.tagName + (string.IsNullOrEmpty(curStruct.instanceName) ? "" : "#" + curStruct.instanceName) + "]:" + e.Message, e);
                        }
                    }
                TagNext:
                    if (curStruct == tagStruct)
                    {
                        tagStruct = null;
                    }
                    else if (tagStruct.subTagStruct != null && tagStruct.subTagStruct.Count > 0)
                    {
                        tagStruct.subTagStruct.Remove(curStruct);
                        if (tagStruct.subTagStruct == null || tagStruct.subTagStruct.Count == 0)
                        {
                            tagStruct = null;
                        }
                        else
                        {
                            curStruct = tagStruct;
                            continue;
                        }
                    }
                    if (tagStruct == null)
                    {
                        break;
                    }
                }
            }
            catch (NullReferenceException)
            {
                if (tagParser == null)
                {
                    throw new TagParseException("解析引擎未设置");
                }
                throw;
            }
            if (isTop && isDynamic)
            {
                strBuilder.AppendLine("\n}\ncatch (Com.AimUI.TagCore.Exceptions.ResponseEndException){}");
            }
        }

        static bool TestNestedTag(string content,out TagStruct outStruct)
        {

            if (string.IsNullOrEmpty(content))
            {
                outStruct = null;
                return false;
            }
            outStruct = tagParser.GetTagStruct(content);
            //判断是否有嵌套
            if (outStruct != null && (outStruct.tagName!=null || outStruct.subTagStruct!=null))
            {
                return true;
            }
            return false;
        }


        static void OnTagAction(ITag<T> tagObj,TagStruct curStruct, Dictionary<string, ITag<T>> tagsObj, bool isDynamic)
        {
            T tagContext = tagObj.tagContext;
            string body = curStruct.bodyContent;
            //检测是否有其它tag
            //如果有其它tag，递归解析
            TagStruct t_newTag;
            bool isHasNested = TestNestedTag(body, out t_newTag);
            if (isHasNested) //处理嵌套情况
            {
                t_newTag.parent = curStruct;
                if (t_newTag.tagName != null) //完全嵌套
                {
                    TagContext.SetItem(tagContext, "$__nestedTag", t_newTag);

                    Parse(null, tagContext,isDynamic?ParseMode.Dynamic:ParseMode.Static);

                }
                else
                {
                    //遍历
                    var t_count = t_newTag.subTagStruct.Count;
                    TagStruct t_subtag = null;
                    for (var i = 0; i < t_count; i++)
                    {
                        t_subtag = t_newTag.subTagStruct[i];
                        t_subtag.parent = t_newTag;
                        if (t_subtag.tagName != null) //完全嵌套
                        {
                            
                            TagContext.SetItem(tagContext, "$__nestedTag", t_subtag);
                            Parse(null, tagContext, isDynamic ? ParseMode.Dynamic : ParseMode.Static);
                        }
                        else
                        {

                            if (string.IsNullOrEmpty(t_subtag.bodyContent) == false) ParseAction(tagContext, tagObj, t_subtag.bodyContent, curStruct, tagsObj, isDynamic);
                        }
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(body)==false) ParseAction(tagContext, tagObj, body, curStruct,tagsObj,isDynamic);
            }
        }


        static void ParseAction(T tagContext, ITag<T> tagObj, string body, TagStruct tagStruct,Dictionary<string, ITag<T>> tagsObj, bool isDynamic)
        {
            if (string.IsNullOrEmpty(body)) return;
            StringBuilder strBuilder = tagContext.GetTagResponse().buffer;
            List<TagToken> tokens = tagParser.GetTagTokens(body);
            
            if (tokens != null)
            {
                ParseTokens(tagContext, tagObj, tokens, body, tagStruct, strBuilder, tagsObj, isDynamic);
            }
            else
            {
                if (isDynamic)
                {
                    strBuilder.Append("\nbuffer.Append(@\"");
                    strBuilder.Append(body.Replace("\"","\"\""));
                    strBuilder.AppendLine("\");");
                }
                else
                {
                    strBuilder.Append(body);
                }
            }
        }


        static void ParseTokens(T tagContext, ITag<T> tagObj, List<TagToken> tokens,string body, TagStruct tagStruct,StringBuilder strBuilder, Dictionary<string, ITag<T>> tagsObj, bool isDynamic)
        {
            if (tokens != null)
            {
                string nameScape = tagStruct.nameSpace;
                string tagName = tagStruct.tagName;
                TagStruct parentTag = tagStruct;
                while (true)
                {
                    if (string.IsNullOrEmpty(tagName) == false || parentTag == null) break;
                    nameScape = parentTag.nameSpace;
                    tagName = parentTag.tagName;
                    parentTag = parentTag.parent;
                }

                int index = 0;
                //ITag<T> parentObj = null;
                foreach (TagToken token in tokens)
                {
                    //表达式之间的 字符串
                    var len = token.index - index;
                    if (len > 0)
                    {
                        var str = body.Substring(index, len);
                        if (isDynamic)
                        {
                            strBuilder.Append("\nbuffer.Append(@\"");
                            strBuilder.Append(str.Replace("\"", "\"\""));
                            strBuilder.AppendLine("\");");
                        }
                        else
                        {
                            strBuilder.Append(str);
                        }
                    }
                    index = token.index + token.length;
                    if (token.type == TagTokenType.Express)
                    {
                        //表达式
                        object result = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic, tagStruct, tagObj);
                        if(isDynamic)
                        {
                            strBuilder.AppendLine("\nbuffer.Append(" + result + ");");
                        }
                        else
                        {
                            strBuilder.Append(result);
                        }
                        
                    }
                    else
                    {
                        object ret = null;
                        if(token.args==null)
                        {
                            ret = ParseAttribule(token, tagObj, tagStruct, isDynamic, tagContext, tagsObj);
                        }
                        else
                        {
                            ret = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic, tagStruct, tagObj);
                        }
                        
                        if (isDynamic)
                        {
                            strBuilder.AppendLine("\nbuffer.Append(" + ret + ");");
                        }
                        else
                        {
                            strBuilder.Append(ret);
                        }
                    }
                }

                if (index < body.Length)
                {
                    if (isDynamic)
                    {
                        strBuilder.Append("\nbuffer.Append(@\"");
                        strBuilder.Append(body.Substring(index).Replace("\"", "\"\""));
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
                    strBuilder.Append(body.Replace("\"", "\"\""));
                    strBuilder.AppendLine("\");");
                }
                else
                {
                    strBuilder.Append(body);
                }
            }
        }

        static void OnNoTagAction(T tagContext, string body, Dictionary<string, ITag<T>> tagsObj,bool isDynamic)
        {
            StringBuilder strBuilder = tagContext.GetTagResponse().buffer;
            List<TagToken> tokens = tagParser.GetTagTokens(body);
            if (tokens == null)
            {
                if(isDynamic)
                {
                    strBuilder.Append("\nbuffer.Append(@\"");
                    strBuilder.Append(body.Replace("\"", "\"\""));
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
                foreach (TagToken token in tokens)
                {
                    var len = token.index - inx;
                    if (len > 0)
                    {
                        if (isDynamic)
                        {
                            strBuilder.Append("\nbuffer.Append(@\"");
                            strBuilder.Append(body.Substring(inx, len).Replace("\"", "\"\""));
                            strBuilder.AppendLine("\");");
                        }
                        else
                        {
                            strBuilder.Append(body.Substring(inx, len));
                        }
                    }
                    inx = token.index + token.length;

                    switch(token.type)
                    {
                        case TagTokenType.Express:
                            object result = ParseTagToken(token, strBuilder,tagsObj, tagContext, isDynamic);
                            if(isDynamic)
                            {
                                strBuilder.Append("\nbuffer.Append(" + result + ");");
                            }
                            else
                            {
                                strBuilder.Append(result);
                            }
                            break;
                        case TagTokenType.Attribute:
                            if(token.args==null)
                            {
                                if (string.IsNullOrEmpty(token.scope))
                                {
                                    if (isDynamic)
                                    {
                                        strBuilder.Append("\nbuffer.Append(@\"");
                                        strBuilder.Append(token.value.Replace("\"", "\"\""));
                                        strBuilder.AppendLine("\");");
                                    }
                                    else
                                    {
                                        strBuilder.Append(token.value);
                                    }
                                }
                                else
                                {
                                    bool isOk;

                                    object s = ParseOutAttributeToken(token, tagContext, tagsObj, isDynamic, out isOk);

                                    if (isDynamic)
                                    {
                                        if (isOk)
                                        {
                                            strBuilder.Append("\nbuffer.Append(");
                                            strBuilder.Append(s);
                                            strBuilder.AppendLine(");");
                                        }
                                        else
                                        {
                                            strBuilder.Append("\nbuffer.Append(@\"");
                                            strBuilder.Append(s);
                                            strBuilder.AppendLine("\");");
                                        }
                                    }
                                    else
                                    {
                                        strBuilder.Append(s);
                                    }
                                }
                            }
                            else
                            {
                                object ret = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic);
                                if (isDynamic)
                                {
                                    strBuilder.Append("\nbuffer.Append(" + ret + ");");
                                }
                                else
                                {
                                    strBuilder.Append(ret);
                                }
                            }
                            break;
                    }
                }

                if (inx < body.Length)
                {
                    if (isDynamic)
                    {
                        strBuilder.Append("\nbuffer.Append(@\"");
                        strBuilder.Append(body.Substring(inx).Replace("\"", "\"\""));
                        strBuilder.AppendLine("\");");
                    }
                    else
                    {
                        strBuilder.Append(body.Substring(inx));
                    }
                }
            } 
        }

        static bool GetTagInstance(string nameSpace, string tagName, Dictionary<string, ITag<T>> tagsObj, out ITag<T> tagObj)
        {
            bool isTagNew = false;
            string fullTagName = nameSpace + ":" + tagName;
            if (tagsObj.TryGetValue(fullTagName, out tagObj) == false)
            {
                isTagNew = true;
                try
                {
                    tagObj = TagLib<T>.GetTagInstance(nameSpace, tagName).Create();
                }
                catch
                {
                    throw (new TagParseException("未找到标签" + fullTagName + "的实现"));
                }
                tagsObj[fullTagName] = tagObj;
            }
            return isTagNew;
        }

        static bool InitExpressTagObj(string nameSpace, string tagName, StringBuilder strBuilder, Dictionary<string, ITag<T>> tagsObj, T tagContext, bool isDynamic, out ITag<T> tagObj)
        {
            bool isNew = GetTagInstance(nameSpace, tagName, tagsObj, out tagObj);

            if (isNew)
            {
                tagObj.tagContext = tagContext;
                tagObj.instanceName = "exp_" + nameSpace + "_" + tagName;
                if (isDynamic)
                {
                    strBuilder.Insert(0, "Com.AimUI.TagCore.ITag<T> " + tagObj.instanceName + "= null;\n");
                    strBuilder.AppendLine("\n" + tagObj.instanceName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + nameSpace + "\",\"" + tagName + "\").Create();");
                    strBuilder.AppendLine(tagObj.instanceName + ".tagContext = tagContext;");
                    strBuilder.AppendLine(tagObj.instanceName + ".instanceName = \"" + tagObj.instanceName + "\";");
                    strBuilder.AppendLine(tagObj.instanceName + ".OnInit();");
                }
                else
                {
                    tagObj.OnInit();
                }
            }
            return isNew;
        }

        static object ParseAttribule(TagToken token, ITag<T> tagObj, TagStruct tagStruct, bool isDynamic, T tagContext, Dictionary<string, ITag<T>> tagsObj, object[] args = null)
        {
            bool isGetSelf = (token.name == "this");
            if (isGetSelf  && (string.IsNullOrEmpty(token.scope) || token.scope == tagStruct.tagName || token.scope == tagObj.instanceName))
            {
                return GetTagSelf(token, tagObj, isDynamic);
            }

            string nameScape = tagStruct.nameSpace;
            string tagName = tagStruct.tagName;
            TagStruct parentTag = tagStruct;
            while (true)
            {
                if (string.IsNullOrEmpty(tagName) == false || parentTag == null) break;
                nameScape = parentTag.nameSpace;
                tagName = parentTag.tagName;
                parentTag = parentTag.parent;
            }
            ITag<T> parentObj = null;

            bool isHas = false;
            if (isGetSelf == false)
            {
                if (string.IsNullOrEmpty(token.scope))
                {
                    isHas = tagObj.ExistAttribute(token.name);
                }
                else if (token.scope == tagName || token.scope == tagObj.instanceName)
                {
                    isHas = true;
                }
            }

            string args_str = null;
            if (isDynamic && args != null && args.Length > 0)
            {
                int _count = args.Length;
                args_str = "new object[] { ";
                for (var i = 0; i < _count; i++)
                {
                    string str = args[i].ToString();
                    args_str += (i > 0 ? "," : "") + (string.IsNullOrEmpty(str) ? "\"\"" : str);
                }
                args_str += "}";
            }
            

            if (isHas)
            {
                if (isDynamic)
                {
                    if (args_str != null)
                    {
                        if (token.action == ValuePreAction.NONE)
                        {
                            return tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + ")";
                        }

                        return "TagContext.OnValuePreAction(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + ")," + (byte)token.action + "," + token.actionCharCode + ")";
                    }
                    else
                    {
                        if (token.action == ValuePreAction.NONE)
                        {
                            return tagObj.instanceName + ".GetAttribute(\"" + token.name + "\")";
                        }

                        return "TagContext.OnValuePreAction(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\")," + (byte)token.action + "," + token.actionCharCode + ")";
                    }
                }
                else
                {
                    object obj = tagObj.GetAttribute(token.name, args);
                    if (token.action == ValuePreAction.NONE)
                    {
                        return obj;
                    }
                    return TagContext.OnValuePreAction(obj, (byte)token.action, token.actionCharCode);
                    
                }
            }
            else
            {
                //遍历父级
                parentTag = tagStruct.parent;

                while (true)
                {
                    if (parentTag == null) break;
                    if (string.IsNullOrEmpty(parentTag.tagName) == false && (parentTag.nameSpace != nameScape || parentTag.tagName != tagName) && parentTag.tagObj != null)
                    {
                        parentObj = (ITag<T>)parentTag.tagObj;
                        if (isGetSelf)
                        {
                            if (token.scope == parentTag.tagName || token.scope == parentObj.instanceName) isHas = true;
                        }
                        else if (string.IsNullOrEmpty(token.scope))
                        {
                            isHas = parentObj.ExistAttribute(token.name);
                        }
                        else if (token.scope == parentTag.tagName || token.scope == parentObj.instanceName)
                        {
                            isHas = true;
                        }
                        
                        if (isHas) break;
                    }
                    parentTag = parentTag.parent;
                }
                if (isHas)
                {
                    if(isGetSelf)
                    {
                        return GetTagSelf(token, parentObj, isDynamic);
                    }
                    if (isDynamic)
                    {
                        if (args_str != null)
                        {
                            if (token.action == ValuePreAction.NONE)
                            {
                                return parentObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + ")";
                            }
                            return "TagContext.OnValuePreAction(" + parentObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + ")," + (byte)token.action + "," + token.actionCharCode + ")";
                        }
                        else
                        {
                            if (token.action == ValuePreAction.NONE)
                            {
                                return parentObj.instanceName + ".GetAttribute(\"" + token.name + "\")";
                            }
                            return "TagContext.OnValuePreAction(" + parentObj.instanceName + ".GetAttribute(\"" + token.name + "\")," + (byte)token.action + "," + token.actionCharCode + ")";
                        }
                    }
                    else
                    {
                        if (token.action == ValuePreAction.NONE)
                        {
                            return parentObj.GetAttribute(token.name, args);
                        }
                        return TagContext.OnValuePreAction(parentObj.GetAttribute(token.name,args), (byte)token.action, token.actionCharCode);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(token.scope))
                    {
                        return null;
                    }
                    else
                    {
                        bool isOk;

                        object s = ParseOutAttributeToken(token, tagContext, tagsObj, isDynamic, out isOk, args);

                        if (isOk)
                        {
                            return s;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        static void ParseTagParams(T tagContext,StringBuilder strBuilder,TagParams tagParams, ITag<T> tagObj,string instanceName, TagStruct curStruct, Dictionary<string, ITag<T>> tagsObj, bool isDynamic)
        {
            string k;
            string v;
            TagToken token;
            List<TagToken> k_tokens;
            List<TagToken> v_tokens;
            int i = 0;
            foreach (KeyValuePair<string, string> kv in tagParams)
            {
                k = kv.Key;
                v = kv.Value;
                if (string.IsNullOrEmpty(k)) continue;
                k_tokens = tagParser.GetTagTokens(k);
                v_tokens = tagParser.GetTagTokens(v);

                object k_result = k;
                object v_result = v;
                if(k_tokens != null)
                {
                    if (k_tokens.Count == 1 && k_tokens[0].length == k.Length)
                    {
                        token = k_tokens[0];
                        if (token.type == TagTokenType.Attribute)
                        {
                            if(token.args==null)
                            {
                                k_result = ParseAttribule(token, tagObj, curStruct, isDynamic, tagContext, tagsObj);
                            }
                            else
                            {
                                k_result = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic, curStruct, tagObj);
                            }
                        }
                        else if (token.type == TagTokenType.Express)
                        {
                            k_result = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic, curStruct, tagObj);
                        }
                    }
                    else
                    {
                        int inx = strBuilder.Length;
                        ParseTokens(tagContext, tagObj, k_tokens, k, curStruct, strBuilder, tagsObj, isDynamic);
                        int len = strBuilder.Length - inx;
                        k_result = strBuilder.ToString(inx, len);
                        strBuilder.Remove(inx, len);
                    }
                }

                if (v_tokens != null)
                {
                    if (v_tokens.Count == 1 && v_tokens[0].length == v.Length)
                    {
                        token = v_tokens[0];
                        if (token.type == TagTokenType.Attribute)
                        {
                            if (token.args==null)
                            {
                                v_result = ParseAttribule(token, tagObj, curStruct, isDynamic, tagContext, tagsObj);
                            }
                            else
                            {
                                v_result = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic, curStruct, tagObj);
                            }
                        }
                        else if (token.type == TagTokenType.Express)
                        {
                            v_result = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic, curStruct, tagObj);
                        }
                    }
                    else
                    {
                        int inx = strBuilder.Length;
                        
                        ParseTokens(tagContext, tagObj, v_tokens, v, curStruct, strBuilder, tagsObj, isDynamic);
                        int len = strBuilder.Length-inx;
                        v_result = strBuilder.ToString(inx, len);
                        strBuilder.Remove(inx, len);
                    }
                }


                if ((k_tokens == null || (k_tokens.Count == 1 && k_tokens[0].length == k.Length)) && (v_tokens == null || (v_tokens.Count == 1 && v_tokens[0].length == v.Length)))
                {
                    if (isDynamic)
                    {
                        if(v_tokens==null)
                        {
                            strBuilder.AppendLine(instanceName + ".SetAttribute(\"" + k_result.ToString() + "\",@\"" + (v_result ?? "").ToString().Replace("\"", "\"\"") + "\");");
                        }
                        else
                        {
                            strBuilder.AppendLine(instanceName + ".SetAttribute(\"" + k_result.ToString() + "\"," + (v_result ?? "") + ");");
                        }
                    }
                    else
                    {
                        tagObj.SetAttribute(k_result.ToString(), v_result ?? "");
                    }
                    continue;
                }

                if (isDynamic)
                {
                    if(k_tokens == null || (k_tokens.Count == 1 && k_tokens[0].length == k.Length))
                    {
                        string _inx = instanceName + "_v_inx_" + i;
                        strBuilder.AppendLine("int " + _inx + " = buffer.Length;");
                        strBuilder.AppendLine(v_result.ToString());
                        strBuilder.AppendLine(instanceName + ".SetAttribute(\"" + k_result.ToString() + "\",buffer.ToString(" + _inx + ",buffer.Length-" + _inx + "));");
                        strBuilder.AppendLine("buffer.Remove(" + _inx + ",buffer.Length-"+_inx+");");
                    }
                    else if (v_tokens == null || (v_tokens.Count == 1 && v_tokens[0].length == v.Length))
                    {
                        string _inx = instanceName + "_k_inx_" + i;
                        strBuilder.AppendLine("int " + _inx + " = buffer.Length;");
                        strBuilder.AppendLine(k_result.ToString());
                        if(v_tokens==null)
                        {
                            strBuilder.AppendLine(instanceName + ".SetAttribute(buffer.ToString(" + _inx + ",buffer.Length-" + _inx + "),@\"" + v_result.ToString().Replace("\"", "\"\"") + "\");");
                        }
                        else
                        {
                            strBuilder.AppendLine(instanceName + ".SetAttribute(buffer.ToString(" + _inx + ",buffer.Length-" + _inx + ")," + v_result + ");");
                        }

                        strBuilder.AppendLine("buffer.Remove(" + _inx + ",buffer.Length-" + _inx + ");");
                    }
                    else
                    {
                        string _inx = instanceName + "_k_inx_" + i;
                        string k_name = instanceName + "_k_name_" + i;
                        strBuilder.AppendLine("int " + _inx + " = buffer.Length;");
                        strBuilder.AppendLine(k_result.ToString());
                        strBuilder.AppendLine("string  " + k_name + " = buffer.ToString(" + _inx + ",buffer.Length-"+_inx+");");
                        strBuilder.AppendLine("buffer.Remove(" + _inx + ",buffer.Length-" + _inx + ");");

                        _inx = instanceName + "_v_inx_" + i;
                        string v_name = instanceName + "_k_name_" + i;
                        strBuilder.AppendLine("int " + _inx + " = buffer.Length;");
                        strBuilder.AppendLine(v_result.ToString());
                        strBuilder.AppendLine("string  " + v_name + " = buffer.ToString(" + _inx + ",buffer.Length-" + _inx + ");");
                        strBuilder.AppendLine("buffer.Remove(" + _inx + ",buffer.Length-" + _inx + ");");

                        strBuilder.AppendLine(instanceName + ".SetAttribute(" + k_name + "," + v_name + ");");
                    }
                }
                else
                {
                    tagObj.SetAttribute(k_result.ToString(), v_result);
                }

                i++;
            }
        }


        static int GetArgsCount(IList<TagToken> tokens)
        {
            if (tokens == null || tokens.Count == 0) return 0;
            int _count = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                var local_args = tokens[i].args;
                if (local_args == null || local_args.Count == 0)
                {
                   _count += 1;
                }
                else if (tokens[i].type != TagTokenType.Common)
                {
                    _count += 1;
                }
                else
                {
                    for (int j = 0; j < local_args.Count; j++)
                    {
                        if (local_args[j].args == null || local_args[j].args.Count == 0)
                        {
                            _count += 1;
                        }
                        else if(local_args[j].type== TagTokenType.Common)
                        {
                            _count += GetArgsCount(local_args[j].args);
                        }
                        else
                        {
                            _count += 1;
                        }
                    }
                }
            }
            return _count;
        }
        /// <summary>
        /// 处理标 不明 属性访问
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        static object ParseOutAttributeToken(TagToken token, T tagContext, Dictionary<string, ITag<T>> tagsObj, bool isDynamic, out bool isOk, object[] args=null)
        {
            isOk = false;
            if(string.IsNullOrEmpty(token.scope))
            {
                if (isDynamic)
                {
                    return token.value.Replace("\"", "\"\"");
                }
                else
                {
                    return token.value;
                }
            }
            bool isGetSelf = (token.name == "this");
            Dictionary<string, ITag<T>> nameTags = (Dictionary<string, ITag<T>>)TagContext.GetItem(tagContext, "$__nameTags");
            int scopeInx = token.scope.IndexOf(':');
            string ns = null;
            string name = token.scope;

            string t_ns = null;
            string t_name = null;
            if (scopeInx != -1)
            {
                ns = token.scope.Substring(0, scopeInx);
                name = token.scope.Substring(scopeInx + 1);
            }
            string args_str = null;
            if (isDynamic && args != null && args.Length > 0)
            {
                int _count = args.Length;
                args_str = "new object[] { ";
                for (var i = 0; i < _count; i++)
                {
                    string str = args[i].ToString();
                    args_str += (i > 0 ? "," : "") + (string.IsNullOrEmpty(str) ? "\"\"" : str);
                }
                args_str += "}";
            }
            /*
            ITag<T> tagObj;
            isOk = nameTags.TryGetValue(name, out tagObj);
            if (isOk)
            {
                if (isDynamic)
                {
                    return tagObj.instanceName + ".GetAttribute(\"" + token.name + "\")";
                }
                else
                {
                    return tagObj.GetAttribute(token.name);
                }
            }
            */

            int _inx1 = -1;
            int _inx2 = -1;
            string i_name = null;
            foreach (KeyValuePair<string, ITag<T>> kv in nameTags)
            {
                _inx1 = kv.Key.IndexOf(':');
                t_ns = kv.Key.Substring(0, _inx1);
                _inx2 = kv.Key.IndexOf('_');
                //t_name = kv.Key.Substring(_inx1+1, _inx2-_inx1-1);
                i_name = kv.Key.Substring(_inx2 + 1);
                if (scopeInx != -1) //按名称查找
                {
                    if(ns == t_ns && (name == i_name))
                    {
                        isOk = true;
                    }
                }
                else if (name == i_name)
                {
                    isOk = true;
                }

                if (isOk)
                {
                    if (isGetSelf)
                    {
                        return GetTagSelf(token, kv.Value, isDynamic);
                    }
                    if (isDynamic)
                    {
                        if (args_str!=null)
                        {
                            if (token.action == ValuePreAction.NONE)
                            {
                                return i_name + ".GetAttribute(\"" + token.name + "\"," + args_str + ")";
                            }
                            return "TagContext.OnValuePreAction(" + i_name + ".GetAttribute(\"" + token.name + "\"," + args_str + ")," + (byte)token.action + "," + token.actionCharCode + ")";
                        }
                        if (token.action == ValuePreAction.NONE)
                        {
                            return i_name + ".GetAttribute(\"" + token.name + "\")";
                        }
                        return "TagContext.OnValuePreAction(" + i_name + ".GetAttribute(\"" + token.name + "\")," + (byte)token.action + "," + token.actionCharCode + ")";
                    }
                    else
                    {
                        if(token.action == ValuePreAction.NONE)
                        {
                            return kv.Value.GetAttribute(token.name, args);
                        }
                        return TagContext.OnValuePreAction(kv.Value.GetAttribute(token.name, args), (byte)token.action, token.actionCharCode);
                    }
                }
            }

            foreach (KeyValuePair<string, ITag<T>> kv in tagsObj)
            {
                _inx1 = kv.Key.IndexOf(':');
                t_ns = kv.Key.Substring(0, _inx1);
                t_name = kv.Key.Substring(_inx1+1);
                if (scopeInx != -1) //按名称查找
                {
                    if (ns == t_ns && (name == t_name))
                    {
                        isOk = true;
                    }
                }
                else if (name == t_name)
                {
                    isOk = true;
                }

                if (isOk)
                {
                    if (isGetSelf)
                    {
                        return GetTagSelf(token, kv.Value, isDynamic);
                    }

                    if (isDynamic)
                    {
                        if (args_str != null)
                        {
                            if (token.action == ValuePreAction.NONE)
                            {
                                return kv.Value.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + ")";
                            }
                            return "TagContext.OnValuePreAction(" + kv.Value.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + ")," + (byte)token.action + "," + token.actionCharCode + ")";
                        }
                        if (token.action == ValuePreAction.NONE)
                        {
                            return kv.Value.instanceName + ".GetAttribute(\"" + token.name + "\")";
                        }
                        return "TagContext.OnValuePreAction(" + kv.Value.instanceName + ".GetAttribute(\"" + token.name + "\")," + (byte)token.action + "," + token.actionCharCode + ")";
                    }
                    else
                    {
                        if (token.action == ValuePreAction.NONE)
                        {
                            return kv.Value.GetAttribute(token.name, args);
                        }
                        return TagContext.OnValuePreAction(kv.Value.GetAttribute(token.name, args), (byte)token.action, token.actionCharCode);
                    }
                }
            }

            if (isDynamic)
            {
                return token.value.Replace("\"", "\"\"");
            }
            else
            {
                return token.value;
            }
        }

        static object ParseTagToken(TagToken token, StringBuilder strBuilder, Dictionary<string, ITag<T>> tagsObj, T tagContext, bool isDynamic, TagStruct tagStruct = null, ITag<T> scopeTagObj = null)
        {
            string ns = token.scope;

            string tagName = null;

            ITag<T> tagObj = scopeTagObj;

            ITag<T> t_tagObj;

            if (token.type == TagTokenType.Express)
            {
                tagName = ((TagExpression)token).tagName;
                InitExpressTagObj(ns, tagName, strBuilder, tagsObj, tagContext, isDynamic, out tagObj);
            }

            if (token.args != null && token.args.Count > 0 && (token.args.Count == 1 && token.args[0].type == TagTokenType.Common && token.args[0].value==string.Empty && token.args[0].args==null) == false)
            {
                Stack<IEnumerator<TagToken>> tokens = new Stack<IEnumerator<TagToken>>();
                Stack<TagToken> prevTokens = new Stack<TagToken>();
                IEnumerator<TagToken> curTokens = token.args.GetEnumerator();
                TagToken prevToken = null;
                TagToken curToken = null;
                Stack args = new Stack();
                while (true)
                {
                    bool isHas = curTokens.MoveNext();
                    if (isHas)
                    {
                        curToken = curTokens.Current;
                        TagTokenType tokenType = curToken.type;

                        if (tokenType == TagTokenType.Common)
                        {
                            if (curToken.args == null)
                            {
                                if (isDynamic)
                                {
                                    args.Push("@\"" + curToken.value.Replace("\"", "\\\"") + "\"");
                                }
                                else
                                {
                                    args.Push(curToken.value);
                                }
                            }
                            else
                            {
                                //prevToken = curToken;
                                //prevTokens.Push(curToken);
                                tokens.Push(curTokens);
                                curTokens = curToken.args.GetEnumerator();
                                continue;
                            }
                        }
                        else if (tokenType == TagTokenType.Express)
                        {
                            if (curToken.args == null)
                            {
                                t_tagObj = scopeTagObj;

                                ns = curToken.scope;

                                tagName = ((TagExpression)curToken).tagName;

                                InitExpressTagObj(ns, tagName, strBuilder, tagsObj, tagContext, isDynamic, out t_tagObj);

                                if (curToken.name == "this")
                                {
                                    if(isDynamic)
                                    {
                                        if (curToken.action == ValuePreAction.NONE)
                                        {
                                            args.Push(t_tagObj.instanceName);
                                        }
                                        else
                                        {
                                            args.Push("TagContext.OnValuePreAction(" + t_tagObj.instanceName + "," + (byte)curToken.action + "," + curToken.actionCharCode + ")");
                                        }
                                    }
                                    else
                                    {
                                        if (curToken.action == ValuePreAction.NONE)
                                        {
                                            args.Push(t_tagObj);
                                        }
                                        else
                                        {
                                            args.Push(TagContext.OnValuePreAction(t_tagObj, (byte)curToken.action, curToken.actionCharCode));
                                        }
                                    }
                                }
                                else if (isDynamic)
                                {
                                    if (curToken.action == ValuePreAction.NONE)
                                    {
                                        args.Push(t_tagObj.instanceName + ".GetAttribute(\"" + curToken.name + "\")");
                                    }
                                    else
                                    {
                                        args.Push("TagContext.OnValuePreAction(" + t_tagObj.instanceName + ".GetAttribute(\"" + curToken.name + "\")," + (byte)curToken.action + "," + curToken.actionCharCode + ")");
                                    }
                                }
                                else
                                {
                                    if (curToken.action == ValuePreAction.NONE)
                                    {
                                        args.Push(t_tagObj.GetAttribute(curToken.name));
                                    }
                                    else
                                    {
                                        args.Push(TagContext.OnValuePreAction(t_tagObj.GetAttribute(curToken.name), (byte)curToken.action, curToken.actionCharCode));
                                    }
                                }
                                
                            }
                            else
                            {
                                //prevToken = curToken;
                                prevTokens.Push(curToken);
                                tokens.Push(curTokens);
                                curTokens = curToken.args.GetEnumerator();
                                continue;
                            }
                        }
                        else if (tokenType == TagTokenType.Attribute)
                        {
                            if (curToken.args == null)
                            {
                                //遍历查找
                                if (tagStruct != null)
                                {
                                    args.Push(ParseAttribule(curToken, scopeTagObj, tagStruct, isDynamic, tagContext, tagsObj));
                                }
                                else
                                {
                                    bool isOk;
                                    object s = ParseOutAttributeToken(curToken, tagContext, tagsObj, isDynamic, out isOk);
                                    args.Push(s);
                                }
                            }
                            else
                            {
                                //prevToken = curToken;
                                prevTokens.Push(curToken);
                                tokens.Push(curTokens);
                                curTokens = curToken.args.GetEnumerator();
                                continue;
                            }
                        }
                    }
                    else
                    {
                        if(prevTokens.Count>0) prevToken = prevTokens.Pop();
                        if (prevToken != null)
                        {
                            //出栈
                            if (prevToken.args != null)
                            {
                                
                                var count = prevToken.args.Count;
                                ArrayList t_args = new ArrayList(count);
                                for (var i = 0; i < count; i++)
                                {
                                    t_args.Insert(0, args.Pop());
                                }

                                t_tagObj = scopeTagObj;

                                if (prevToken.type == TagTokenType.Express)
                                {
                                    //执行
                                    ns = prevToken.scope;

                                    tagName = ((TagExpression)prevToken).tagName;

                                    InitExpressTagObj(ns, tagName, strBuilder, tagsObj, tagContext, isDynamic, out t_tagObj);

                                    if (isDynamic)
                                    {
                                        //设置参数
                                        string args_str = "null";
                                        if (count > 0)
                                        {
                                            args_str = "new object[] { ";
                                            for (var i = 0; i < count; i++)
                                            {
                                                string str = t_args[i].ToString();
                                                args_str += (i > 0 ? "," : "") + (string.IsNullOrEmpty(str) ? "\"\"" : str);
                                            }
                                            args_str += "}";
                                        }

                                        if (prevToken.action == ValuePreAction.NONE)
                                        {
                                            args.Push(t_tagObj.instanceName + ".GetAttribute(\"" + prevToken.name + "\"," + args_str + ")");
                                        }
                                        else
                                        {
                                            args.Push("TagContext.OnValuePreAction(" + t_tagObj.instanceName + ".GetAttribute(\"" + prevToken.name + "\"," + args_str + ")," + (byte)prevToken.action + "," + prevToken.actionCharCode + ")");
                                        }

                                    }
                                    else
                                    {
                                        if (prevToken.action == ValuePreAction.NONE)
                                        {
                                            args.Push(t_tagObj.GetAttribute(prevToken.name, t_args.ToArray()));
                                        }
                                        else
                                        {
                                            args.Push(TagContext.OnValuePreAction(t_tagObj.GetAttribute(prevToken.name, t_args.ToArray()), (byte)prevToken.action, prevToken.actionCharCode));
                                        }
                                    }

                                }
                                else if (prevToken.type == TagTokenType.Attribute)
                                {
                                    args.Push(ParseAttribule(prevToken, t_tagObj, tagStruct, isDynamic, tagContext, tagsObj, t_args.ToArray()));
                                }
                            }
                            prevToken = null;
                        }
                        if (tokens.Count > 0)
                        {
                            curTokens = tokens.Pop();
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                int _count = args.Count;
                ArrayList _args = new ArrayList(_count);
                for (var i = 0; i < _count; i++)
                {
                    _args.Insert(0, args.Pop());
                }

                if (_args.Count > token.args.Count)
                {
                    ArrayList tmp_args = new ArrayList(token.args.Count);

                    int tmp_count = 0;
                    TagToken local_token;
                    string local_str;
                    int inx = 0;
                    for (int i = 0; i < token.args.Count; i++)
                    {
                        local_token = token.args[i];
                        if (local_token.type == TagTokenType.Common && local_token.args != null && local_token.args.Count > 0)
                        {
                            var local_args = token.args[i].args;
                            if (local_args == null || local_args.Count == 0)
                            {
                                tmp_count = 1;
                            }
                            else
                            {
                                tmp_count = GetArgsCount(local_args);
                            }
                            
                            local_str = "";

                            for (int j = 0; j < tmp_count; j++)
                            {
                                string args_v = (_args[inx] == null ? "" : Convert.ToString(_args[inx]));
                                if (isDynamic)
                                {
                                    if (args_v.Length > 0 && args_v.IndexOf(".GetAttribute(") == -1)
                                    {
                                        local_str += (local_str.Length > 0 ? "+" : "") + args_v;
                                    }
                                    else
                                    {
                                        local_str += (local_str.Length > 0 ? "+" : "") + "Convert.ToString(" + args_v + ")";
                                    }
                                }
                                else
                                {
                                    local_str += args_v;
                                }
                                inx += 1;
                            }
                            tmp_args.Add(local_str);
                        }
                        else
                        {
                            tmp_args.Add(_args[inx]);
                            inx += 1;
                        }
                    }
                    _args = tmp_args;
                    _count = _args.Count;
                }

                if (token.type == TagTokenType.Attribute)
                {
                    return ParseAttribule(token, tagObj, tagStruct, isDynamic, tagContext, tagsObj, _args != null ? _args.ToArray() : null);
                }

                if (isDynamic)
                {
                    //设置参数
                    string args_str = "null";
                    if (_count > 0)
                    {
                        args_str = "new object[] { ";
                        for (var i = 0; i < _count; i++)
                        {
                            string str = _args[i].ToString();
                            args_str += (i > 0 ? "," : "") + (string.IsNullOrEmpty(str) ? "\"\"" : str);
                        }
                        args_str += "}";
                    }
                    if (token.action == ValuePreAction.NONE) return tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + ")";
                    return "TagContext.OnValuePreAction(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + ")," + (byte)token.action + "," + token.actionCharCode + ")";
                }
                else
                {
                    if (token.action == ValuePreAction.NONE) return tagObj.GetAttribute(token.name, _args.ToArray());
                    return TagContext.OnValuePreAction(tagObj.GetAttribute(token.name, _args.ToArray()), (byte)token.action, token.actionCharCode);
                }
            }
            else
            {
                if (token.type == TagTokenType.Attribute)
                {
                    return ParseAttribule(token, tagObj, tagStruct, isDynamic, tagContext, tagsObj);
                }
                
                if (token.name == "this")
                {
                    return GetTagSelf(token, tagObj, isDynamic);
                }
                else if (isDynamic)
                {
                    if (token.action == ValuePreAction.NONE) return tagObj.instanceName + ".GetAttribute(\"" + token.name + "\")";
                    return "TagContext.OnValuePreAction(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\")," + (byte)token.action + "," + token.actionCharCode + ")";
                }
                else
                {
                    if(token.action == ValuePreAction.NONE) return tagObj.GetAttribute(token.name);
                    return TagContext.OnValuePreAction(tagObj.GetAttribute(token.name), (byte)token.action, token.actionCharCode);
                }
            }
        }

        static ITag<T> GetTagObjByToken(TagToken token, T tagContext, Dictionary<string, ITag<T>> tagsObj, TagStruct tagStruct = null)
        {
            
            return null;
        }

        static object GetTagSelf(TagToken token,ITag<T> tagObj, bool isDynamic)
        {
            if (isDynamic)
            {
                if (token.action == ValuePreAction.NONE)
                {
                    return tagObj.instanceName;
                }
                return "TagContext.OnValuePreAction(" + tagObj.instanceName + "," + (byte)token.action + "," + token.actionCharCode + ")";
            }
            else
            {
                if (token.action == ValuePreAction.NONE)
                {
                    return tagObj;
                }
                return TagContext.OnValuePreAction(tagObj, (byte)token.action, token.actionCharCode);
            }
        }
    }
}