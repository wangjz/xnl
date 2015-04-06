using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
namespace COM.SingNo.XNLCore.Labels
{
  public  class For<T>:IXNLTag<T> where T:XNLContext
    {
      public T xnlContext
      {
          get;
          private set;
      }

      public string instanceName { get; set; }
      public string curTag { get; set; }
      //标签开始,初始化参数等 抛异常初始化失败原因
      public void onInit(T xnlContext, string instanceName)
        {
        }
        public void onStart()
        {
        }

        public void onEnd()
        {
        }

        //子标签解析
        public void onTag(string tagName, OnTagDelegate<T> tagDelegate, string body)
        {
        }

        public void setAttributeValue(string paramName, object value, string subTagName=null)
        {

        }
        public object getAttributeValue(string paramName, string subTagName = null)
        {
            return "";
        }

        //创建 
        public IXNLTag<T> create()
        {
            return new For<T>();
        }
        public bool existAttribute(string paramName, string tagName=null)
        {
            return false;
        }

        public string subTagNames
        {
            get { return "foritem"; }
        }
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

      public ParseMode parseMode
      {
          get;
          set;
      }
    }
}
