using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
namespace COM.SingNo.XNLCore.Labels
{
    public  class Each<T>:IXNLTag<T> where T:XNLContext
    {
        public T xnlContext
        {
            get;
            set;
        }
        public string instanceName { get; set; }
        public string curTag { get; set;}
        // //标签开始,初始化参数等 抛异常初始化失败原因
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
        public object getAttributeValue(string paramName,string subTagName=null)
        {
            return "";
        }       
      
        //创建 
        public IXNLTag<T> create()
        {
            return null;
        }
        public bool existAttribute(string paramName, string tagName=null)
        {
            return false;
        }

        public string subTagNames
        {
            get { return "eachitem"; }
        }
        #region IXNLBase 成员

        //public string main(XNLTagStruct tagStruct, T XNLPage)
        //{
        //    return "";
            //XNLParams forParams = labelParams;

            //int step = 1;  //step默认为1
            //string []splitStr = {","};  //分隔字符默认为","
            //string foreachVarStr = "";
            //string SplitOptionsStr = "none";
            //XNLParam foreachVarParam;
            //XNLParam splitParam;
            //XNLParam stepParam;
            //XNLParam splitOptionParam;

            //if (forParams.TryGetValue("foreachvar", out foreachVarParam))
            //{
            //    foreachVarStr = Convert.ToString(foreachVarParam.value);
            //}

            //if (forParams.TryGetValue("split", out splitParam))
            //{
            //    splitStr[0] = Convert.ToString(splitParam.value);

            //}

            //if (forParams.TryGetValue("step", out stepParam))
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

            //if (forParams.TryGetValue("stringsplitoptions", out splitOptionParam))
            //{
            //    SplitOptionsStr = Convert.ToString(splitOptionParam.value).Trim().ToLower();               
            //}
            //if (step == 0) step = 1;
            //string forLabelContent = labelContentStr; //得到标签内容
            //MatchCollection forMatchsColl =XNLPage.xnlParser.matchsXNLTagByName(forLabelContent, "XNL", "foreach"); // 得到嵌套的 foreach标签
            //forLabelContent = XNLPage.xnlParser.disableNestedXNLTag(forLabelContent, forMatchsColl); //禁用嵌套的foreach 标签
            //StringBuilder forReturnStr = new StringBuilder();
            //MatchCollection itemColls = XNLPage.xnlParser.matchsItemTagByName(forLabelContent, "foreachItem"); //匹配foreachItem
            //if (itemColls.Count == 0)
            //{
            //    return foreachItem(forParams, forLabelContent, forMatchsColl,XNLPage);
            //}
            //int foreachId = 0;
            //forParams.Add("foreachid", new XNLParam("0"));
            //forParams.Add("foreachnum", new XNLParam("0"));
            //forParams.Add("foreachvalue", new XNLParam(""));

            //Match itemMatch = itemColls[0];

            //string itemMatchStr = itemMatch.Groups[3].Value;

            //string forLabelReturnStr = forLabelContent.Replace(itemMatch.Groups[0].Value, "<foreachItemTmpReplaaceString></foreachItemTmpReplaaceString>");  //将匹配到的forItem替换为<forItemTmpReplaaceString></forItemTmpReplaaceString>

            //forLabelReturnStr = XNLPage.xnlParser.enabledNestedXNLTag(forLabelReturnStr, forMatchsColl);  //启用内容里的嵌套标签

            //forLabelReturnStr = XNLPage.xnlParser.replaceAttribleVariable(forLabelReturnStr, forParams, XNLPage); //替换属性
            //string[] vars_arr;
           
            //if (SplitOptionsStr.Equals("none"))
            //{
            //    vars_arr = foreachVarStr.Split(splitStr, StringSplitOptions.None);
            //}
            //else
            //{
            //    vars_arr = foreachVarStr.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
            //}


            //if (step >= 0)
            //{
                
            //    for (var i = 0; i < vars_arr.Length; i = i + step)
            //    {
            //        foreachId++;
            //        string tmpItemStr = itemMatchStr;
            //        forParams["foreachnum"].value = (i + 1);
            //        forParams["foreachid"].value =foreachId;
            //        forParams["foreachvalue"].value = vars_arr[i];
            //        forReturnStr.Append(foreachItem(forParams, tmpItemStr, forMatchsColl,XNLPage));

            //    }
            //}
            //else
            //{
            //    for (var i = vars_arr.Length-1; i >= 0; i = i + step)
            //    {
            //        foreachId++;
            //        string tmpItemStr = itemMatchStr;
            //        forParams["foreachnum"].value = (i + 1);
            //        forParams["foreachid"].value = foreachId;
            //        forParams["foreachvalue"].value = vars_arr[i];
            //        forReturnStr.Append(foreachItem(forParams, tmpItemStr, forMatchsColl,XNLPage));
            //    }
            //}


            //return forLabelReturnStr.Replace("<foreachItemTmpReplaaceString></foreachItemTmpReplaaceString>", forReturnStr.ToString());  //将<forItemTmpReplaaceString></forItemTmpReplaaceString>替换为相应的内容
        //}

        #endregion

        //private string foreachItem(XNLParams forParams, string itemStr, MatchCollection matchs,XNLContext XNLPage)
        //{
        //    string returnStr = XNLPage.xnlParser.enabledNestedXNLTag(itemStr, matchs);

        //    returnStr = XNLPage.xnlParser.replaceAttribleVariable( returnStr,forParams,XNLPage);
        //    return returnStr;
        //}

        public ParseMode parseMode
        {
            get;
            set;
        }
    }
}
