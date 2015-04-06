using System;
using System.Collections.Generic;
using System.Text;
namespace COM.SingNo.XNLCore
{
    /// <summary>
    /// 标签引擎配置
    /// </summary>
    public class XNLConfig
    {       
        public static string userTagSavePath="Template/UserTag/";// { get; set; }//自定义标签存放路径
        public static string templateSavePath= "Template/";// { get; set; } //模板存放路径
        public static string templateExtName = "ascx";// { get; set; }//模板文件扩展名
        public static string userTagExtName = "ascx";// { get; set; }//自定义标签文件扩展名
    }
}
