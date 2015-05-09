using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
namespace COM.SingNo.XNLCore.Labels
{
    public class For<T> : IXNLTag<T> where T : XNLContext
    {
        public int start { get; set; }

        public int end { get; set; }

        public int i { get; set; }

        public string split { get; set; }

        public string str { get; set; }

        private bool isChange = false;

        private string[] strs;

        public int step = 1;

        private int pos = 0;
        public T xnlContext
        {
            get;
            set;
        }

        public string instanceName { get; set; }
        public string curTag { get; set; }

        public void OnInit()  //T xnlContext, string instanceName
        {
            start = 0;
            end = 0;
            split = ",";
            strs = null;
            //this.xnlContext = xnlContext;
            //this.instanceName = instanceName;
        }
        public virtual void OnStart()
        {
            i = 0;
            if(isChange)
            {
                isChange = false;
                if(string.IsNullOrEmpty(str)==false)
                {
                    strs = str.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries);
                    if (end >= strs.Length) end = strs.Length-1;
                }
                else
                {
                    strs = null;
                }
            }
        }

        public void OnEnd()
        {
        }

        //子标签解析
        public void OnTag(OnTagDelegate tagDelegate = null)
        {
            if (step == 0) return;
            if (tagDelegate != null)
            {
                
                if (start > end)
                {
                    if (step > 0) step = -step;
                }
                else
                {
                    if (step < 0) step = -step;
                }
                pos = 1;
                for (i = start; i <= end; i += step)
                {
                    tagDelegate();
                    pos += 1;
                }   
            }
        }

        public void SetAttribute(string paramName, object value, string subTagName = null)
        {
            if (paramName == "start")
            {
                start = Convert.ToInt32(value);
            }
            else if (paramName == "end")
            {
                end = Convert.ToInt32(value);
            }
            else if (paramName == "str")
            {
                string v = Convert.ToString(value);
                if (v != str)
                {
                    str = v;
                    isChange = true;
                }
            }
            else if (paramName == "split")
            {
                string v = Convert.ToString(value);
                if (v != split)
                {
                    split = v;
                    isChange = true;
                }
            }
            else if(paramName == "step")
            {
                step = Convert.ToInt32(value);
            }
        }

        public object GetAttribute(string paramName, string tagName = null)
        {
            object v ;
            TryGetAttribute(out v, paramName, tagName);
            return v;
        }

        public bool TryGetAttribute(out object outValue, string paramName, string tagName = null)
        {
            if (paramName == "i")
            {
                outValue = i;
                return true;
            }
            else if (paramName == "pos")
            {
                outValue = pos;
                return true;
            }
            else if (paramName == "start")
            {
                outValue = start;
                return true;
            }
            else if (paramName == "end")
            {
                outValue = end;
                return true;
            }
            else if (paramName == "str")
            {
                if(strs!=null&&i<strs.Length)
                {
                    outValue = strs[i];
                    return true;
                }
                outValue = "";
                return true;
            }
            else if (paramName == "split")
            {
                outValue = split;
                return true;
            }
            else if (paramName == "step")
            {
                outValue = step;
                return true;
            }
            outValue = null;
            return false;
        }

        //创建 
        public IXNLTag<T> Create()
        {
            return new For<T>();
        }
        public bool ExistAttribute(string paramName, string tagName = null)
        {
            if (paramName == "i" || paramName == "str" || paramName == "pos" || paramName == "start" || paramName == "end" || paramName == "split" || paramName == "step") return true;
            return false;
        }

        public string subTagNames
        {
            get { return null; }
        }

        public ParseMode parseMode
        {
            get;
            set;
        }
    }
}


/*

  #region IXNLBase 成员

        public string main(XNLTagStruct tagStruct, T XNLPage)
        {
            return "";
            //int forStart = 0;
            //int forEnd = 0;
            //int step = 1;
            //XNLParam startParam;
            //XNLParam endParam;
            //XNLParam stepParam;
            //if (labelParams.TryGetValue("forstart", out startParam))
            //{
            //    forStart = Convert.ToInt32(startParam.value);
            //}
            //if (labelParams.TryGetValue("forend", out endParam))
            //{
            //    forEnd = Convert.ToInt32(endParam.value);
            //}
            //if (labelParams.TryGetValue("step", out stepParam))
            //{
            //    string tempStepStr = Convert.ToString(stepParam.value);
            //    if (UtilsCode.IsNumeric(tempStepStr))
            //    {
            //        step = Convert.ToInt32(tempStepStr);
            //    }
            //    else
            //    {
            //        step = 1;
            //    }
            //}

            //if (step == 0) step = 1;
            //if (!labelParams.ContainsKey("forid"))
            //{
            //    labelParams.Add("forid", new XNLParam(0));
            //}
            //int forid = 0;
            //string forLabelContent = labelContentStr;
            //MatchCollection forMatchsColl =XNLPage.xnlParser.matchsXNLTagByName(forLabelContent, "XNL", "for");
            //forLabelContent = XNLPage.xnlParser.disableNestedXNLTag(forLabelContent, forMatchsColl);
            //StringBuilder forReturnStr = new StringBuilder();
            //MatchCollection itemColls = XNLPage.xnlParser.matchsItemTagByName(forLabelContent, "forItem");
            //if (itemColls.Count == 0)
            //{
            //    return forItem(labelParams, forLabelContent, forMatchsColl,XNLPage);
            //}

            //labelParams.Add("forvar", new XNLParam(0));

            //Match itemMatch = itemColls[0];

            //string itemMatchStr = itemMatch.Groups[3].Value;

            //string forLabelReturnStr = forLabelContent.Replace(itemMatch.Groups[0].Value, "<forItemTmpReplaaceString></forItemTmpReplaaceString>");  //将匹配到的forItem替换为<forItemTmpReplaaceString></forItemTmpReplaaceString>

            //forLabelReturnStr = XNLPage.xnlParser.enabledNestedXNLTag(forLabelReturnStr, forMatchsColl);  //启用内容里的嵌套标签
            //forLabelReturnStr = XNLPage.xnlParser.replaceAttribleVariable(forLabelReturnStr,labelParams,XNLPage );

            //if(step>=0)
            //{
            //    for (var i = forStart; i <= forEnd; i=i+step)
            //    {
            //        forid++;
            //        labelParams["forid"].value = forid;
            //        string tmpItemStr = itemMatchStr;
            //        labelParams["forvar"].value = i;
            //        forReturnStr.Append(forItem(labelParams, tmpItemStr, forMatchsColl, XNLPage));

            //    }
            //}
            //else
            //{
            //    for (var i = forStart; i >= forEnd; i = i + step)
            //    {
            //        forid++;
            //        labelParams["forid"].value = forid;
            //        string tmpItemStr = itemMatchStr;
            //        labelParams["forvar"].value = i;
            //        forReturnStr.Append(forItem(labelParams, tmpItemStr, forMatchsColl, XNLPage));
            //    }
            //}
            //return forLabelReturnStr.Replace("<forItemTmpReplaaceString></forItemTmpReplaaceString>", forReturnStr.ToString());  //将<forItemTmpReplaaceString></forItemTmpReplaaceString>替换为相应的内容
        }

        #endregion
        //private string forItem(XNLParams forParams, string itemStr,MatchCollection matchs,XNLContext XNLPage)
        //{
        //    string returnStr = XNLPage.xnlParser.enabledNestedXNLTag(itemStr, matchs);
        //    returnStr = XNLPage.xnlParser.replaceAttribleVariable(returnStr, forParams, XNLPage);
        //    return returnStr;
        //}
*/