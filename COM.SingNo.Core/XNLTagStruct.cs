using System;
using System.Collections.Generic;
using System.Text;

namespace COM.SingNo.XNLCore
{
    /// <summary>
    /// XNL标签结构
    /// </summary>
    public class XNLTagStruct
    {
        private string _bodyContent;
        /// <summary>
        /// 标签命名空间
        /// </summary>
        public string nameSpace { get; set; }
        /// <summary>
        /// 标签名
        /// </summary>
        public string tagName { get; set; }
        /// <summary>
        /// 标签实例名
        /// </summary>
        public string instanceName { get; set; }
        /// <summary>
        /// 标签内容
        /// </summary>
        public string bodyContent
        {
            get
            {
                if (tagName == null) //string.IsNullOrEmpty(_bodyContent) && 
                {
                    return allContent;
                }
                else if ( subTagStruct == null || subTagStruct.Count ==0 )
                {
                    return _bodyContent;
                }
                else
                {
                    return "";
                }
            }

            set
            {
                _bodyContent = value;
            }
         }

        public string allContent { get; set; }
        /// <summary>
        /// 标签参数
        /// </summary>
        public XNLParams tagParams { get; set; }
        public List<XNLTagStruct> subTagStruct { get; set; }

        /// <summary>
        /// 父标签
        /// </summary>
        public XNLTagStruct parent { get; set; }
    }
}
