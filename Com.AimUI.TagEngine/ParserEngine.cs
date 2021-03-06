﻿using System;
using System.Collections.Generic;
using System.Text;
using Com.AimUI.TagCore.Exceptions;
using System.Collections;
using Com.AimUI.TagCore;
using Com.AimUI.TagCore.Tags;

namespace Com.AimUI.TagEngine
{
    public class ParserEngine<T> where T : TagContext
    {
        public static IParser<T> tagParser { get; private set; }

        public static void Initialize(IParser<T> parser, List<TagLib<T>> tagLibs = null)
        {
            tagParser = parser;
            TagLib<T>.Initialize(tagLibs, parser);
        }

        public static string ParseToString(string templateStr, T tagContext, ParseMode parseMode = ParseMode.Static)
        {
            Parse(templateStr, tagContext, parseMode);
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
                    TagContext.RemoveItem(tagContext, "$__nestedTag");
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
                Dictionary<string, ITag<T>> tagsObj = null;//首次声明标签对象 && 匿名
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
                        tagsObj = new Dictionary<string, ITag<T>>(StringComparer.OrdinalIgnoreCase);

                        nameTags = new Dictionary<string, ITag<T>>(StringComparer.OrdinalIgnoreCase);

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
                            strBuilder.AppendLine("if(buffer==null) buffer = tagContext.GetTagResponse().buffer;");
                        }
                        else
                        {
                            strBuilder.AppendLine("if(buffer==null) buffer = tagContext.GetTagResponse().buffer;\r\ntry\r\n{");
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
                        if (curStruct.nameSpace == "at" && curStruct.tagName == "pre")
                        {
                            if (isDynamic)
                            {
                                strBuilder.Append("buffer.Append(@\"");
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
                            bool isNestedInc = (isNested && curStruct.tagName.EndsWith("$inc$"));

                            if (isNestedInc)
                            {
                                curStruct.tagName = curStruct.tagName.Substring(0, curStruct.tagName.Length - 5);
                                isTagNew = curStruct.nameSpace.EndsWith("$new$");
                                if (isTagNew)
                                {
                                    curStruct.nameSpace = curStruct.nameSpace.Substring(0, curStruct.nameSpace.Length - 5);
                                }
                                fullTagName = curStruct.nameSpace + ":" + curStruct.tagName;
                                tagObj = curStruct.tagObj as ITag<T>;
                            }
                            else
                            {
                                fullTagName = curStruct.nameSpace + ":" + curStruct.tagName;

                                isTagNew = GetTagInstance(curStruct.nameSpace, curStruct.tagName, tagsObj, out tagObj);
                            }

                            instanceName = curStruct.instanceName;

                            if (isTagNew)
                            {
                                if (IsNullOrWhiteSpace(instanceName))
                                {
                                    if (isNestedInc == false)
                                    {
                                        instanceName = "__" + tagId;
                                        tagId += 1;
                                    }
                                    else
                                    {
                                        instanceName = "__" + (tagId - 1);
                                    }
                                }
                                else
                                {
                                    var fullInsName = fullTagName + "_" + instanceName;
                                    //检测是否命名冲突
                                    string name_prefix = curStruct.nameSpace + "_" + curStruct.tagName + "___";
                                    instanceName = name_prefix + instanceName;
                                    if (nameTags.ContainsKey(fullInsName))
                                    {
                                        throw new TagParseException("tag " + fullTagName + "#" + instanceName.Substring(name_prefix.Length) + ":命名冲突");
                                    }
                                    //tagObj.tagContext = tagContext;
                                    tagObj.instanceName = instanceName;
                                    tagObj = tagObj.Create();
                                    nameTags[fullInsName] = tagObj;
                                    //nameTags[instanceName.Substring(name_prefix.Length)] = tagObj;
                                    //nameTags[fullTagName] = tagObj;
                                }
                                tagObj.tagContext = tagContext;
                                tagObj.instanceName = instanceName;

                                if (isDynamic)
                                {
                                    strBuilder.Insert(0, "Com.AimUI.TagCore.ITag<T> " +  instanceName + "= null;\r\n");
                                    strBuilder.AppendLine(instanceName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\");");
                                    strBuilder.AppendLine(instanceName + ".tagContext = tagContext;");
                                    strBuilder.AppendLine(instanceName + ".instanceName = \"" + instanceName + "\";");
                                    if ((tagObj.events & TagEvents.Init) == TagEvents.Init) strBuilder.AppendLine(instanceName + ".OnInit();");
                                }
                                else
                                {
                                    if ((tagObj.events & TagEvents.Init) == TagEvents.Init) tagObj.OnInit();
                                }
                            }
                            else if (IsNullOrWhiteSpace(instanceName) == false) //命名tag
                            {
                                ITag<T> newTagObj;
                                var fullInsName = fullTagName + "_" + instanceName;
                                bool isCreate = nameTags.TryGetValue(fullInsName, out newTagObj);
                                string name_prefix = curStruct.nameSpace + "_" + curStruct.tagName + "___";
                                instanceName = name_prefix + instanceName;
                                if (isCreate == false)
                                {
                                    //检测是否命名冲突
                                    //if (nameTags.ContainsKey(instanceName))
                                    //{
                                    //    throw new TagParseException("tag " + fullTagName + "#" + instanceName + ":命名冲突");
                                    //}

                                    newTagObj = tagObj.Create();
                                    nameTags[fullInsName] = newTagObj;
                                    //nameTags[instanceName] = newTagObj;
                                    if (isDynamic)
                                    {
                                        strBuilder.Insert(0, "Com.AimUI.TagCore.ITag<T> " + instanceName + "=null;\r\n");
                                        strBuilder.AppendLine(instanceName + " = (" + tagObj.instanceName + "!=null?" + tagObj.instanceName + ".Create():Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\"));");
                                        strBuilder.AppendLine(instanceName + ".tagContext = tagContext;");
                                        strBuilder.AppendLine(instanceName + ".instanceName = \"" + instanceName + "\";");
                                        if ((newTagObj.events & TagEvents.Init) == TagEvents.Init) strBuilder.AppendLine(instanceName + ".OnInit();");
                                        tagObj = newTagObj;
                                        tagObj.tagContext = tagContext;
                                        tagObj.instanceName = instanceName;
                                    }
                                    else
                                    {
                                        tagObj = newTagObj;
                                        tagObj.tagContext = tagContext;
                                        tagObj.instanceName = instanceName;
                                        if ((tagObj.events & TagEvents.Init) == TagEvents.Init) tagObj.OnInit();
                                    }

                                }
                                else
                                {
                                    if (isDynamic)
                                    {
                                        strBuilder.AppendLine("if(" + instanceName + "==null)\r\n{\r\n\t" + instanceName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\");");
                                        strBuilder.AppendLine("\t" + instanceName + ".tagContext = tagContext;");
                                        strBuilder.AppendLine("\t" + instanceName + ".instanceName = \"" + instanceName + "\";");
                                        if ((newTagObj.events & TagEvents.Init) == TagEvents.Init)
                                        {
                                            strBuilder.AppendLine("\t" + instanceName + ".OnInit();\r\n}");
                                        }
                                        else
                                        {
                                            strBuilder.AppendLine("\t}");
                                        }
                                    }
                                    tagObj = newTagObj;
                                }
                            }
                            else if (IsNullOrWhiteSpace(instanceName))
                            {
                                string oldInsName = tagObj.instanceName;
                                if (tagObj.tagContext == null)
                                {
                                    tagObj.tagContext = tagContext;
                                    instanceName = "__" + tagId;
                                    tagId += 1;
                                    tagObj.instanceName = instanceName;
                                    if (isDynamic)
                                    {
                                        strBuilder.Insert(0, "Com.AimUI.TagCore.ITag<T> " + instanceName + "= null;\r\n");
                                    }
                                    else
                                    {
                                        if ((tagObj.events & TagEvents.Init) == TagEvents.Init) tagObj.OnInit();
                                    }
                                }
                                else
                                {
                                    instanceName = oldInsName;
                                }
                                if (isDynamic)
                                {
                                    if (oldInsName != instanceName)
                                    {
                                        strBuilder.AppendLine("if(" + instanceName + "==null)\r\n{\r\n\t" + instanceName + " = (" + oldInsName + "!=null?" + oldInsName + ".Create():Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\"));");
                                    }
                                    else
                                    {
                                        strBuilder.AppendLine("if(" + instanceName + "==null)\r\n{\r\n\t" + instanceName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + curStruct.nameSpace + "\",\"" + curStruct.tagName + "\");");
                                    }
                                    strBuilder.AppendLine("\t" + instanceName + ".tagContext = tagContext;");
                                    strBuilder.AppendLine("\t" + instanceName + ".instanceName = \"" + instanceName + "\";");
                                    if ((tagObj.events & TagEvents.Init) == TagEvents.Init)
                                    {
                                        strBuilder.AppendLine("\t" + instanceName + ".OnInit();\r\n}");
                                    }
                                    else
                                    {
                                        strBuilder.AppendLine("\t}");
                                    }
                                }
                            }
                            curStruct.tagObj = tagObj;
                            if (curStruct.tagParams != null)
                            {
                                ParseTagParams(tagContext, strBuilder, curStruct.tagParams, tagObj, instanceName, curStruct, tagsObj, isDynamic);
                            }

                            bool isIncludeTag = (isNestedInc == false && tagObj is IInclude);

                            if (isIncludeTag)
                            {
                                var incTag = (tagObj as IInclude);
                                string incSrc = (curStruct.tagParams == null ? null : Convert.ToString(curStruct.tagParams["src"]));

                                if (IsNullOrWhiteSpace(incSrc) == false)
                                {
                                    string include = incTag.GetTagBody(incSrc,curStruct.bodyContent);

                                    if (IsNullOrWhiteSpace(include) == false)
                                    {
                                        curStruct.bodyContent = include;
                                        if (IsNullOrWhiteSpace(tagObj.subTagNames) == false)
                                        {
                                            curStruct.subTagStruct = tagParser.GetTagStruct("<" + fullTagName + ">" + include + "</" + fullTagName + ">").subTagStruct;
                                            if (curStruct.subTagStruct != null)
                                            {
                                                subTagCount = curStruct.subTagStruct.Count;
                                                if (subTagCount > 0) isNoSubTag = false;
                                            }
                                        }
                                        else curStruct.subTagStruct = null;
                                    }
                                }

                                if (IsNullOrWhiteSpace(curStruct.bodyContent)) goto TagNext;

                                bool isStatic = false;
                                if (curStruct.tagParams != null)
                                {
                                    string static_s = Convert.ToString(curStruct.tagParams["static"]);
                                    isStatic = ("1" == static_s || string.Compare("true", static_s, true) == 0);
                                }

                                if (isDynamic && isStatic)
                                {
                                    curStruct.tagName = curStruct.tagName + "$inc$";
                                    curStruct.nameSpace = curStruct.nameSpace + (isTagNew ? "$new$" : "");
                                    curStruct.tagObj = tagObj;
                                    index = strBuilder.Length;
                                    TagContext.SetItem(tagContext, "$__tagid", tagId);
                                    TagContext.SetItem(tagContext, "$__nestedTag", curStruct);
                                    Parse(null, tagContext);
                                    int len = strBuilder.Length - index;
                                    if (len > 0)
                                    {
                                        incSrc = strBuilder.ToString(index, len);
                                        strBuilder.Remove(index, len);
                                        string incBody = incSrc.Replace("\"", "\"\"");
                                        strBuilder.Append("buffer.Append(@\"");
                                        strBuilder.Append(incBody);
                                        strBuilder.AppendLine("\");");
                                        strBuilder.AppendLine(instanceName + ".SetAttribute(\"#body\",@\"" + incBody + "\");");
                                    }
                                    goto TagNext;
                                }
                            }

                            if ((curStruct.subTagStruct == null || curStruct.subTagStruct.Count == 0) && IsNullOrWhiteSpace(curStruct.bodyContent))
                            {
                                goto TagNext;
                            }

                            if (isDynamic)
                            {
                                strBuilder.AppendLine("try{");
                                if ((tagObj.events & TagEvents.Start) == TagEvents.Start) strBuilder.AppendLine(instanceName + ".OnStart();");
                            }
                            else
                            {
                                if ((tagObj.events & TagEvents.Start) == TagEvents.Start) tagObj.OnStart();
                            }


                            if (isNoSubTag)
                            {
                                tagObj.curTag = null;
                                if (isDynamic)
                                {
                                    strBuilder.AppendLine(instanceName + ".curTag = null;");
                                    if ((tagObj.events & TagEvents.Tag) == TagEvents.Tag)
                                    {
                                        string delegateName = instanceName + "_" + tagId;
                                        strBuilder.AppendLine("Com.AimUI.TagCore.OnTagDelegate " + delegateName + "_delegate=delegate (){");
                                        tagId += 1;
                                        TagContext.SetItem(tagContext, "$__tagid", tagId);
                                        OnTagAction(tagObj, curStruct, tagsObj, isDynamic);
                                        strBuilder.AppendLine("};");
                                        strBuilder.AppendLine(instanceName + ".OnTag(" + delegateName + "_delegate);");
                                    }
                                }
                                else
                                {
                                    if ((tagObj.events & TagEvents.Tag) == TagEvents.Tag)
                                    {
                                        TagStruct tmpSubTag = curStruct;
                                        tagObj.OnTag(delegate()
                                        {
                                            TagContext.SetItem(tagContext, "$__tagid", tagId);
                                            OnTagAction(tagObj, tmpSubTag, tagsObj, isDynamic);
                                        });
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
                                        if (IsNullOrWhiteSpace(tmpSubTag.bodyContent) == false)
                                        {
                                            tagObj.curTag = null;
                                            if (isDynamic) strBuilder.AppendLine(instanceName + ".curTag = null;");
                                            TagContext.SetItem(tagContext, "$__tagid", tagId);
                                            OnTagAction(tagObj, tmpSubTag, tagsObj, isDynamic);
                                        }
                                    }
                                    else
                                    {
                                        tagObj.curTag = tmpSubTag.tagName;
                                        if (isDynamic) strBuilder.AppendLine(instanceName + ".curTag = @\"" + tmpSubTag.tagName + "\";");
                                        if (tmpSubTag.tagParams != null)
                                        {
                                            ParseTagParams(tagContext, strBuilder, tmpSubTag.tagParams, tagObj, instanceName, tmpSubTag, tagsObj, isDynamic);
                                        }
                                        if (isDynamic)
                                        {
                                            if ((tagObj.events & TagEvents.Tag) == TagEvents.Tag)
                                            {
                                                if (IsNullOrWhiteSpace(tmpSubTag.bodyContent) == false)
                                                {
                                                    string delegateName = instanceName + "_" + tagId;
                                                    strBuilder.AppendLine("Com.AimUI.TagCore.OnTagDelegate " + delegateName + "_" + tagObj.curTag + "_delegate=delegate (){");
                                                    tagId += 1;
                                                    TagContext.SetItem(tagContext, "$__tagid", tagId);
                                                    OnTagAction(tagObj, tmpSubTag, tagsObj, isDynamic);
                                                    strBuilder.AppendLine("};");
                                                    strBuilder.AppendLine(instanceName + ".OnTag(" + delegateName + "_" + tagObj.curTag + "_delegate);");
                                                }
                                                else
                                                {
                                                    strBuilder.AppendLine(instanceName + ".OnTag(null);");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if ((tagObj.events & TagEvents.Tag) == TagEvents.Tag)
                                            {
                                                if (IsNullOrWhiteSpace(tmpSubTag.bodyContent) == false)
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
                            }

                            isEnd = true;
                            if (isDynamic)
                            {
                                if ((tagObj.events & TagEvents.End) == TagEvents.End) strBuilder.AppendLine(instanceName + ".OnEnd();");
                                strBuilder.AppendLine("}\r\ncatch (Com.AimUI.TagCore.Exceptions.TagStopException)\r\n{");
                                if ((tagObj.events & TagEvents.End) == TagEvents.End) strBuilder.AppendLine(instanceName + ".OnEnd();");
                                strBuilder.AppendLine("}");
                            }
                            else
                            {
                                if ((tagObj.events & TagEvents.End) == TagEvents.End) tagObj.OnEnd();
                            }
                        }
                        catch (TagStopException)
                        {
                            if (isEnd == false)
                            {
                                if ((tagObj.events & TagEvents.End) == TagEvents.End) tagObj.OnEnd();
                            }
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
                strBuilder.AppendLine("}\ncatch (Com.AimUI.TagCore.Exceptions.ResponseEndException){}");
            }
        }

        static bool TestNestedTag(string content, out TagStruct outStruct)
        {

            if (IsNullOrWhiteSpace(content))
            {
                outStruct = null;
                return false;
            }
            outStruct = tagParser.GetTagStruct(content);
            //判断是否有嵌套
            if (outStruct != null && (outStruct.tagName != null || outStruct.subTagStruct != null))
            {
                return true;
            }
            return false;
        }

        static void OnTagAction(ITag<T> tagObj, TagStruct curStruct, Dictionary<string, ITag<T>> tagsObj, bool isDynamic)
        {
            T tagContext = tagObj.tagContext;
            string body = curStruct.bodyContent;
            //检测是否有其它tag
            //如果有其它tag，递归解析
            TagStruct t_newTag = null;
            bool isHasNested = false;
            if (curStruct.nested == 0)
            {
                isHasNested = TestNestedTag(body, out t_newTag);
                curStruct.nested = (sbyte)(isHasNested ? 1 : -1);
                if (isHasNested) curStruct.nestedTag = t_newTag;
            }
            else
            {
                isHasNested = (curStruct.nested > 0);
                if (isHasNested)
                {
                    t_newTag = curStruct.nestedTag;
                }
            }
            if (isHasNested) //处理嵌套情况
            {
                t_newTag.parent = curStruct;
                if (t_newTag.tagName != null) //完全嵌套
                {
                    TagContext.SetItem(tagContext, "$__nestedTag", t_newTag);

                    Parse(null, tagContext, isDynamic ? ParseMode.Dynamic : ParseMode.Static);

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

                            if (IsNullOrWhiteSpace(t_subtag.bodyContent) == false) ParseAction(tagContext, tagObj, t_subtag.bodyContent, curStruct, tagsObj, isDynamic);
                        }
                    }
                }
            }
            else
            {
                ParseAction(tagContext, tagObj, body, curStruct, tagsObj, isDynamic);
            }
        }

        static void ParseAction(T tagContext, ITag<T> tagObj, string body, TagStruct tagStruct, Dictionary<string, ITag<T>> tagsObj, bool isDynamic)
        {
            if (string.IsNullOrEmpty(body)) return;
            StringBuilder strBuilder = tagContext.GetTagResponse().buffer;
            List<TagToken> tokens = tagParser.GetTagTokens(body, tagStruct);

            if (tokens != null)
            {
                ParseTokens(tagContext, tagObj, tokens, body, tagStruct, strBuilder, tagsObj, isDynamic);
            }
            else
            {
                if (isDynamic)
                {
                    strBuilder.Append("buffer.Append(@\"");
                    strBuilder.Append(body.Replace("\"", "\"\""));
                    strBuilder.AppendLine("\");");
                }
                else
                {
                    strBuilder.Append(body);
                }
            }
        }

        static void ParseTokens(T tagContext, ITag<T> tagObj, List<TagToken> tokens, string body, TagStruct tagStruct, StringBuilder strBuilder, Dictionary<string, ITag<T>> tagsObj, bool isDynamic)
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
                            strBuilder.Append("buffer.Append(@\"");
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
                        if (isDynamic)
                        {
                            if (result != null) strBuilder.AppendLine("buffer.Append(" + result + ");");
                        }
                        else
                        {
                            if (result != null) strBuilder.Append(result);
                        }
                    }
                }

                if (index < body.Length)
                {
                    if (isDynamic)
                    {
                        strBuilder.Append("buffer.Append(@\"");
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
                    strBuilder.Append("buffer.Append(@\"");
                    strBuilder.Append(body.Replace("\"", "\"\""));
                    strBuilder.AppendLine("\");");
                }
                else
                {
                    strBuilder.Append(body);
                }
            }
        }

        static void OnNoTagAction(T tagContext, string body, Dictionary<string, ITag<T>> tagsObj, bool isDynamic)
        {
            StringBuilder strBuilder = tagContext.GetTagResponse().buffer;
            List<TagToken> tokens = tagParser.GetTagTokens(body, null);
            if (tokens == null)
            {
                if (isDynamic)
                {
                    strBuilder.Append("buffer.Append(@\"");
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
                int inx = 0;
                foreach (TagToken token in tokens)
                {
                    var len = token.index - inx;
                    if (len > 0)
                    {
                        if (isDynamic)
                        {
                            strBuilder.Append("buffer.Append(@\"");
                            strBuilder.Append(body.Substring(inx, len).Replace("\"", "\"\""));
                            strBuilder.AppendLine("\");");
                        }
                        else
                        {
                            strBuilder.Append(body.Substring(inx, len));
                        }
                    }
                    inx = token.index + token.length;
                    if (token.type == TagTokenType.Express)
                    {
                        object result = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic);
                        if (isDynamic)
                        {
                            if (result != null) strBuilder.Append("buffer.Append(" + result + ");");
                        }
                        else
                        {
                            if (result != null) strBuilder.Append(result);
                        }
                    }
                    /*
                    switch (token.type)
                    {
                        case TagTokenType.Express:
                            object result = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic);
                            if (isDynamic)
                            {
                                if (result != null) strBuilder.Append("buffer.Append(" + result + ");");
                            }
                            else
                            {
                                if (result != null) strBuilder.Append(result);
                            }
                            break;
                    }
                    */
                }

                if (inx < body.Length)
                {
                    if (isDynamic)
                    {
                        strBuilder.Append("buffer.Append(@\"");
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
                tagObj = TagLib<T>.GetTagInstance(nameSpace, tagName);
                tagsObj[fullTagName] = tagObj;
            }
            return isTagNew;
        }

        static bool InitExpressTagObj(string nameSpace, string tagName, string tokenInsName, StringBuilder strBuilder, Dictionary<string, ITag<T>> tagsObj, T tagContext, bool isDynamic, out ITag<T> tagObj, TagStruct tagStruct = null)
        {
            bool isNew = false;
            string expVarName = null;
            if (string.IsNullOrEmpty(tokenInsName) == false)
            {
                Dictionary<string, ITag<T>> nameTags = (Dictionary<string, ITag<T>>)TagContext.GetItem(tagContext, "$__nameTags");
                tagObj = FindTag(nameTags, tagsObj, nameSpace, tagName, tokenInsName);
                isNew = (tagObj == null);
                if (isNew)
                {
                    GetTagInstance(nameSpace, tagName, tagsObj, out tagObj);
                    tagObj = tagObj.Create();
                    nameTags[nameSpace + ":" + tagName + "_" + tokenInsName] = tagObj;
                }
                expVarName = nameSpace + "_" + tagName + "___" + tokenInsName;
            }
            else
            {
                isNew = GetTagInstance(nameSpace, tagName, tagsObj, out tagObj);
                expVarName = "__" + nameSpace + "_" + tagName;
            }
            if (isNew)
            {
                tagObj.tagContext = tagContext;
                tagObj.instanceName = expVarName;
                if (isDynamic)
                {
                    strBuilder.Insert(0, "Com.AimUI.TagCore.ITag<T> " + expVarName + "= null;\r\n");
                    strBuilder.AppendLine(expVarName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + nameSpace + "\",\"" + tagName + "\");");
                    strBuilder.AppendLine(expVarName + ".tagContext = tagContext;");
                    strBuilder.AppendLine(expVarName + ".instanceName = \"" + expVarName + "\";");
                    if ((tagObj.events & TagEvents.Init) == TagEvents.Init) strBuilder.AppendLine(expVarName + ".OnInit();");
                }
                else
                {
                    if ((tagObj.events & TagEvents.Init) == TagEvents.Init) tagObj.OnInit();
                }
            }
            else
            {
                isNew = (tagObj.tagContext == null);
                string firstName = tagObj.instanceName;
                if (isNew)
                {
                    tagObj.instanceName = expVarName;
                    tagObj.tagContext = tagContext;
                }
                if (isDynamic)
                {
                    string code = "";
                    if (isNew)
                    {
                        strBuilder.Insert(0, "Com.AimUI.TagCore.ITag<T> " + expVarName + "= null;\r\n");
                        if (string.IsNullOrEmpty(firstName) && firstName != tagObj.instanceName)
                        {
                            code += "if(" + tagObj.instanceName + " == null){\r\n" + tagObj.instanceName + " = (" + firstName + "!=null?" + firstName + ".Create():Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + nameSpace + "\",\"" + tagName + "\"));\r\n";
                        }
                        else
                        {
                            code += "if(" + tagObj.instanceName + " == null){\r\n" + tagObj.instanceName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + nameSpace + "\",\"" + tagName + "\");\r\n";
                        }
                    }
                    else
                    {
                        bool isInner = false;
                        if (tagStruct != null)
                        {
                            var curStruct = tagStruct;
                            while (curStruct != null)
                            {
                                if (tagObj == curStruct.tagObj)
                                {
                                    isInner = true;
                                    break;
                                }
                                else
                                {
                                    curStruct = curStruct.parent;
                                }
                            }
                        }
                        if (isInner == false) code += "if(" + tagObj.instanceName + " == null){\r\n" + tagObj.instanceName + " = Com.AimUI.TagCore.TagLib<T>.GetTagInstance(\"" + nameSpace + "\",\"" + tagName + "\");\r\n";
                    }
                    if (code.Length > 0)
                    {
                        code += tagObj.instanceName + ".tagContext = tagContext;\r\n";
                        code += tagObj.instanceName + ".instanceName = \"" + tagObj.instanceName + "\";\r\n";
                        if ((tagObj.events & TagEvents.Init) == TagEvents.Init)
                        {
                            code += tagObj.instanceName + ".OnInit();\r\n}\r\n";
                        }
                        else
                        {
                            code += "}\r\n";
                        }
                        if (strBuilder.Length >= code.Length)
                        {
                            if (strBuilder.ToString(strBuilder.Length - code.Length, code.Length) != code)
                            {
                                strBuilder.Append(code);
                            }
                        }
                        else
                        {
                            strBuilder.Append(code);
                        }
                    }
                }
                else if (isNew)
                {
                    if ((tagObj.events & TagEvents.Init) == TagEvents.Init) tagObj.OnInit();
                }
                isNew = false;
            }
            return isNew;
        }

        static bool MatchToken(TagToken token, string nameScape, string tagName, string instanceName)
        {
            if (nameScape == null) return false;
            bool isNoScope = string.IsNullOrEmpty(token.scope);
            bool isNoTagName = string.IsNullOrEmpty(token.tagName);
            if (isNoTagName == false && string.Compare(token.tagName, tagName, true) != 0)
            {
                return false;
            }

            if (isNoScope == false && string.Compare(token.scope, nameScape, true) != 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(token.instanceName) == false && string.Compare(token.instanceName, instanceName.Substring(nameScape.Length + tagName.Length + 4), true) != 0)
            {
                return false;
            }
            return true;
        }
        
        static void ParseTagParams(T tagContext, StringBuilder strBuilder, TagParams tagParams, ITag<T> tagObj, string instanceName, TagStruct curStruct, Dictionary<string, ITag<T>> tagsObj, bool isDynamic)
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
                k_tokens = tagParser.GetTagTokens(k, curStruct);
                v_tokens = tagParser.GetTagTokens(v, curStruct);

                object k_result = k;
                object v_result = v;
                if (k_tokens != null)
                {
                    if (k_tokens.Count == 1 && k_tokens[0].length == k.Length)
                    {
                        token = k_tokens[0];
                        if (token.type == TagTokenType.Express)
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
                        if (token.type == TagTokenType.Express)
                        {
                            v_result = ParseTagToken(token, strBuilder, tagsObj, tagContext, isDynamic, curStruct, tagObj);
                        }
                    }
                    else
                    {
                        int inx = strBuilder.Length;
                        ParseTokens(tagContext, tagObj, v_tokens, v, curStruct, strBuilder, tagsObj, isDynamic);
                        int len = strBuilder.Length - inx;
                        v_result = strBuilder.ToString(inx, len);
                        strBuilder.Remove(inx, len);
                    }
                }


                if ((k_tokens == null || (k_tokens.Count == 1 && k_tokens[0].length == k.Length)) && (v_tokens == null || (v_tokens.Count == 1 && v_tokens[0].length == v.Length)))
                {
                    if (isDynamic)
                    {
                        if (v_tokens == null)
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
                        if (k_result != null) tagObj.SetAttribute(k_result.ToString(), v_result ?? "");
                    }
                    continue;
                }

                if (isDynamic)
                {
                    if (k_tokens == null || (k_tokens.Count == 1 && k_tokens[0].length == k.Length))
                    {
                        string _inx = instanceName + "_v_inx_" + i;
                        strBuilder.AppendLine("int " + _inx + " = buffer.Length;");
                        strBuilder.AppendLine(v_result.ToString());
                        strBuilder.AppendLine(instanceName + ".SetAttribute(\"" + k_result.ToString() + "\",buffer.ToString(" + _inx + ",buffer.Length-" + _inx + "));");
                        strBuilder.AppendLine("buffer.Remove(" + _inx + ",buffer.Length-" + _inx + ");");
                    }
                    else if (v_tokens == null || (v_tokens.Count == 1 && v_tokens[0].length == v.Length))
                    {
                        string _inx = instanceName + "_k_inx_" + i;
                        strBuilder.AppendLine("int " + _inx + " = buffer.Length;");
                        strBuilder.AppendLine(k_result.ToString());
                        if (v_tokens == null)
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
                        strBuilder.AppendLine("string  " + k_name + " = buffer.ToString(" + _inx + ",buffer.Length-" + _inx + ");");
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
                        else if (local_args[j].type == TagTokenType.Common)
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

        static ITag<T> FindTag(Dictionary<string, ITag<T>> nameTags,Dictionary<string, ITag<T>> noNameTags,string nameScape,string tagName,string instanceName)
        {
            nameScape = (string.IsNullOrEmpty(nameScape) ? "at" : nameScape);
            if (string.IsNullOrEmpty(instanceName) == false)
            {
                if (string.IsNullOrEmpty(tagName))
                {
                    return FindTag(nameTags, instanceName);
                }
                else
                {
                    return FindTag(nameTags, nameScape + ":" + tagName + "_" + instanceName);
                }
            }
            else if (string.IsNullOrEmpty(tagName) == false)
            {
                return FindTag(noNameTags, nameScape + ":" + tagName);
            }
            return null;
        }

        static ITag<T> FindTag(Dictionary<string, ITag<T>> tags, string keyName)
        {
            if (tags == null) return null;
            ITag<T> tag;
            if (tags.TryGetValue(keyName, out tag)) return tag;
            return null;
        }

        static object ParseTagToken(TagToken token, StringBuilder strBuilder, Dictionary<string, ITag<T>> tagsObj, T tagContext, bool isDynamic, TagStruct tagStruct = null, ITag<T> scopeTagObj = null)
        {
            string ns = token.scope;

            ITag<T> tagObj = scopeTagObj;

            ITag<T> t_tagObj;
            string tagName = token.tagName;
            if (token.type == TagTokenType.Express)
            {
                if (token.args == null && string.Compare("at", ns, true) == 0 && string.Compare("exp", tagName, true) == 0)
                {
                    object baseValue = null;
                    bool isBaseType = false;
                    string baseValueS = "null";
                    if (token.name == "null")
                    {
                        isBaseType = true;
                    }
                    else if (token.name == "true")
                    {
                        isBaseType = true;
                        baseValue = true;
                        baseValueS = "true";
                    }
                    else if (token.name == "false")
                    {
                        isBaseType = true;
                        baseValue = false;
                        baseValueS = "false";
                    }
                    if (isBaseType)
                    {
                        if (isDynamic)
                        {
                            if (token.action == ValuePreAction.NONE) return baseValueS;
                            return "Com.AimUI.TagCore.TagContext.OnValuePreAction(" + baseValueS + ",Com.AimUI.TagCore.ValuePreAction." + token.action + ")";
                        }
                        else
                        {
                            if (token.action == ValuePreAction.NONE) return baseValue;
                            return TagContext.OnValuePreAction(baseValue, token.action);
                        }
                    }
                }
                InitExpressTagObj(ns, tagName, token.instanceName, strBuilder, tagsObj, tagContext, isDynamic, out tagObj, tagStruct);
            }

            if (token.args != null && token.args.Count > 0 && (token.args.Count == 1 && token.args[0].type == TagTokenType.Common && token.args[0].value == string.Empty && token.args[0].args == null) == false)
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
                                prevTokens.Push(curToken);
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

                                tagName = curToken.tagName;
                                if (string.Compare("at", ns, true) == 0 && string.Compare("exp", tagName, true) == 0)
                                {
                                    object baseValue = null;
                                    bool isBaseType = false;
                                    string baseValueS = "null";
                                    if (token.name == "null")
                                    {
                                        isBaseType = true;
                                    }
                                    else if (token.name == "true")
                                    {
                                        isBaseType = true;
                                        baseValue = true;
                                        baseValueS = "true";
                                    }
                                    else if (token.name == "false")
                                    {
                                        isBaseType = true;
                                        baseValue = false;
                                        baseValueS = "false";
                                    }
                                    if (isBaseType)
                                    {
                                        if (isDynamic)
                                        {
                                            if (curToken.action == ValuePreAction.NONE)
                                            {
                                                args.Push(baseValueS);
                                            }
                                            else
                                            {
                                                args.Push("Com.AimUI.TagCore.TagContext.OnValuePreAction(" + baseValueS + ",Com.AimUI.TagCore.ValuePreAction." + curToken.action + ")");
                                            }
                                        }
                                        else
                                        {
                                            if (curToken.action == ValuePreAction.NONE)
                                            {
                                                args.Push(baseValue);
                                            }
                                            else
                                            {
                                                args.Push(TagContext.OnValuePreAction(baseValue, curToken.action));
                                            }
                                        }
                                        continue;
                                    }
                                }
                                InitExpressTagObj(ns, tagName,curToken.instanceName, strBuilder, tagsObj, tagContext, isDynamic, out t_tagObj,tagStruct);

                                if (curToken.name == "this")
                                {
                                    if (isDynamic)
                                    {
                                        if (curToken.action == ValuePreAction.NONE)
                                        {
                                            args.Push(t_tagObj.instanceName);
                                        }
                                        else
                                        {
                                            args.Push("Com.AimUI.TagCore.TagContext.OnValuePreAction(" + t_tagObj.instanceName + ",Com.AimUI.TagCore.ValuePreAction." + curToken.action + ")");
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
                                            args.Push(TagContext.OnValuePreAction(t_tagObj, curToken.action));
                                        }
                                    }
                                }
                                else if (isDynamic)
                                {
                                    if (curToken.action == ValuePreAction.NONE)
                                    {
                                        if (curToken.IsChains())
                                        {
                                            args.Push("Com.AimUI.TagCore.TagHelper.ComplexQuery(" + t_tagObj.instanceName + ".GetAttribute(\"" + curToken.name + "\"),@\"" + curToken.chainsPath.Replace("\"", "\"\"") + "\")");
                                        }
                                        else
                                        {
                                            args.Push(t_tagObj.instanceName + ".GetAttribute(\"" + curToken.name + "\")");
                                        }
                                    }
                                    else
                                    {
                                        if (curToken.IsChains())
                                        {
                                            args.Push("Com.AimUI.TagCore.TagContext.OnValuePreAction(Com.AimUI.TagCore.TagHelper.ComplexQuery(" + t_tagObj.instanceName + ".GetAttribute(\"" + curToken.name + "\"),@\"" + curToken.chainsPath.Replace("\"", "\"\"") + "\"),Com.AimUI.TagCore.ValuePreAction." + curToken.action + ")");
                                        }
                                        else
                                        {
                                            args.Push("Com.AimUI.TagCore.TagContext.OnValuePreAction(" + t_tagObj.instanceName + ".GetAttribute(\"" + curToken.name + "\"),Com.AimUI.TagCore.ValuePreAction." + curToken.action + ")");
                                        }
                                    }
                                }
                                else
                                {
                                    if (curToken.action == ValuePreAction.NONE)
                                    {
                                        if (curToken.IsChains())
                                        {
                                            args.Push(TagHelper.ComplexQuery(t_tagObj.GetAttribute(curToken.name), curToken.chainsPath));
                                        }
                                        else
                                        {
                                            args.Push(t_tagObj.GetAttribute(curToken.name));
                                        }
                                    }
                                    else
                                    {
                                        if (curToken.IsChains())
                                        {
                                            args.Push(TagContext.OnValuePreAction(TagHelper.ComplexQuery(t_tagObj.GetAttribute(curToken.name), curToken.chainsPath), curToken.action));
                                        }
                                        else
                                        {
                                            args.Push(TagContext.OnValuePreAction(t_tagObj.GetAttribute(curToken.name), curToken.action));
                                        }
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
                    }
                    else
                    {
                        if (prevTokens.Count > 0) prevToken = prevTokens.Pop();
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

                                    tagName = prevToken.tagName;

                                    InitExpressTagObj(ns, tagName, prevToken.instanceName, strBuilder, tagsObj, tagContext, isDynamic, out t_tagObj, tagStruct);

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
                                            if (prevToken.IsChains())
                                            {
                                                args.Push("Com.AimUI.TagCore.TagHelper.ComplexQuery(" + t_tagObj.instanceName + ".GetAttribute(\"" + prevToken.name + "\"," + args_str + "),@\"" + prevToken.chainsPath.Replace("\"", "\"\"") + "\")");
                                            }
                                            else
                                            {
                                                args.Push(t_tagObj.instanceName + ".GetAttribute(\"" + prevToken.name + "\"," + args_str + ")");
                                            }
                                        }
                                        else
                                        {
                                            if (prevToken.IsChains())
                                            {
                                                args.Push("Com.AimUI.TagCore.TagContext.OnValuePreAction(Com.AimUI.TagCore.TagHelper.ComplexQuery(" + t_tagObj.instanceName + ".GetAttribute(\"" + prevToken.name + "\"," + args_str + "),@\"" + prevToken.chainsPath.Replace("\"", "\"\"") + "\"),Com.AimUI.TagCore.ValuePreAction." + prevToken.action + ")");
                                            }
                                            else
                                            {
                                                args.Push("Com.AimUI.TagCore.TagContext.OnValuePreAction(" + t_tagObj.instanceName + ".GetAttribute(\"" + prevToken.name + "\"," + args_str + "),Com.AimUI.TagCore.ValuePreAction." + prevToken.action + ")");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (prevToken.action == ValuePreAction.NONE)
                                        {
                                            if (prevToken.IsChains())
                                            {
                                                args.Push(TagHelper.ComplexQuery(t_tagObj.GetAttribute(prevToken.name, t_args.ToArray()), prevToken.chainsPath));
                                            }
                                            else
                                            {
                                                args.Push(t_tagObj.GetAttribute(prevToken.name, t_args.ToArray()));
                                            }
                                        }
                                        else
                                        {
                                            if (prevToken.IsChains())
                                            {
                                                args.Push(TagContext.OnValuePreAction(TagHelper.ComplexQuery(t_tagObj.GetAttribute(prevToken.name, t_args.ToArray()), prevToken.chainsPath), prevToken.action));
                                            }
                                            else
                                            {
                                                args.Push(TagContext.OnValuePreAction(t_tagObj.GetAttribute(prevToken.name, t_args.ToArray()), prevToken.action));
                                            }
                                        }
                                    }
                                }
                                else if (prevToken.type == TagTokenType.Common)
                                {
                                    string args_str = "";
                                    for (var i = 0; i < count; i++)
                                    {
                                        string str = (t_args[i] ?? "").ToString();
                                        if (isDynamic)
                                        {
                                            args_str += (i > 0 ? "+" : "") + (string.IsNullOrEmpty(str) ? "\"\"" : (str.IndexOf(".GetAttribute(") != -1 ? "Com.AimUI.TagCore.TagContext.ToString(" + str + ")" : str));
                                        }
                                        else
                                        {
                                            args_str += str ?? "";
                                        }
                                    }
                                    args.Push(args_str);
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
                                        local_str += (local_str.Length > 0 ? "+" : "") + "System.Convert.ToString(" + args_v + ")";
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

                if (isDynamic)
                {
                    //设置参数
                    string args_str = "null";
                    if (_count > 0)
                    {
                        args_str = "new object[] { ";
                        for (var i = 0; i < _count; i++)
                        {
                            string str = (_args[i] == null ? null : _args[i].ToString());
                            args_str += (i > 0 ? "," : "") + (string.IsNullOrEmpty(str) ? "\"\"" : str);
                        }
                        args_str += "}";
                    }
                    if (token.action == ValuePreAction.NONE)
                    {
                        if (token.IsChains())
                        {
                            return "Com.AimUI.TagCore.TagHelper.ComplexQuery(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + "),@\"" + token.chainsPath.Replace("\"", "\"\"") + "\")";
                        }
                        return tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + ")";
                    }
                    if (token.IsChains())
                    {
                        return "Com.AimUI.TagCore.TagContext.OnValuePreAction(Com.AimUI.TagCore.TagHelper.ComplexQuery(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + "),@\"" + token.chainsPath.Replace("\"", "\"\"") + "\"),Com.AimUI.TagCore.ValuePreAction." + token.action + ")";
                    }
                    return "Com.AimUI.TagCore.TagContext.OnValuePreAction(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"," + args_str + "),Com.AimUI.TagCore.ValuePreAction." + token.action + ")";
                }
                else
                {
                    if (token.action == ValuePreAction.NONE)
                    {
                        if (token.IsChains())
                        {
                            return TagHelper.ComplexQuery(tagObj.GetAttribute(token.name, _args.ToArray()), token.chainsPath);
                        }
                        return tagObj.GetAttribute(token.name, _args.ToArray());
                    }
                    if (token.IsChains())
                    {
                        return TagContext.OnValuePreAction(TagHelper.ComplexQuery(tagObj.GetAttribute(token.name, _args.ToArray()), token.chainsPath), token.action);
                    }
                    return TagContext.OnValuePreAction(tagObj.GetAttribute(token.name, _args.ToArray()), token.action);
                }
            }
            else
            {
                if (token.name == "this")
                {
                    return GetTagSelf(token, tagObj, isDynamic);
                }
                else if (isDynamic)
                {
                    if (token.action == ValuePreAction.NONE)
                    {
                        if (token.IsChains())
                        {
                            return "Com.AimUI.TagCore.TagHelper.ComplexQuery(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"),@\"" + token.chainsPath.Replace("\"", "\"\"") + "\")";
                        }
                        return tagObj.instanceName + ".GetAttribute(\"" + token.name + "\")";
                    }
                    if (token.IsChains())
                    {
                        return "Com.AimUI.TagCore.TagContext.OnValuePreAction(Com.AimUI.TagCore.TagHelper.ComplexQuery(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"),@\"" + token.chainsPath.Replace("\"", "\"\"") + "\"),Com.AimUI.TagCore.ValuePreAction." + token.action + ")";
                    }
                    return "Com.AimUI.TagCore.TagContext.OnValuePreAction(" + tagObj.instanceName + ".GetAttribute(\"" + token.name + "\"),Com.AimUI.TagCore.ValuePreAction." + token.action + ")";
                }
                else
                {
                    if (token.action == ValuePreAction.NONE)
                    {
                        if (token.IsChains())
                        {
                            return TagHelper.ComplexQuery(tagObj.GetAttribute(token.name), token.chainsPath);
                        }
                        return tagObj.GetAttribute(token.name);
                    }
                    if (token.IsChains())
                    {
                        return TagContext.OnValuePreAction(TagHelper.ComplexQuery(tagObj.GetAttribute(token.name), token.chainsPath), token.action);
                    }
                    return TagContext.OnValuePreAction(tagObj.GetAttribute(token.name), token.action);
                }
            }
        }

        static object GetTagSelf(TagToken token, ITag<T> tagObj, bool isDynamic)
        {
            if (isDynamic)
            {
                if (token.action == ValuePreAction.NONE)
                {
                    return tagObj.instanceName;
                }
                return "Com.AimUI.TagCore.TagContext.OnValuePreAction(" + tagObj.instanceName + ",Com.AimUI.TagCore.ValuePreAction." + token.action + ")";
            }
            else
            {
                if (token.action == ValuePreAction.NONE)
                {
                    return tagObj;
                }
                return TagContext.OnValuePreAction(tagObj, token.action);
            }
        }

        static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                if (value.Length == 0) return true;
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}