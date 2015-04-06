using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
namespace COM.SingNo.XNLCore
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
        public static string loadTemplete(string pathStr, string charset)
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
        public static string loadUserTagByName(string tagName, string siteRootPath)
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
        public static Dictionary<string, string> getTemplateUserTag(string template, string sitePath)
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
                        myTagColls.Add(tagName, loadUserTagByName(tagName, sitePath));
                    }
                }
                return myTagColls;
            }
            return null;
        }
        public static string encodeXNL(string scrStr)
        {
            scrStr = scrStr.Replace("{", "&xnlal;");
            scrStr = scrStr.Replace("}", "&xnlar;");
            return scrStr;
        }
        public static string decodeXNL(string scrStr)
        {
            scrStr = scrStr.Replace("&xnlal;", "{");
            scrStr = scrStr.Replace("&xnlar;", "}");
            return scrStr;
        }
        public static string encodeHTML(string scrStr)
        {
            return UtilsCode.encodeHtml(scrStr);
        }
        public static string decodeHTML(string scrStr)
        {
            scrStr = scrStr.Replace("&gt;", ">");
            scrStr = scrStr.Replace("&lt;", "<");
            scrStr = scrStr.Replace("&quot;", "\"");
            return scrStr;
        }
        
    }
}
