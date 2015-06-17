using System;
using System.Collections.Generic;
using System.Text;

namespace Com.AimUI.TagCore
{
    /// <summary>
    /// 标签结构
    /// </summary>
    public class TagStruct
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
        public TagParams tagParams { get; set; }
        public List<TagStruct> subTagStruct { get; set; }

        /// <summary>
        /// 父标签
        /// </summary>
        public TagStruct parent { get; set; }
        /// <summary>
        /// 对应的标签实例
        /// </summary>
        public object tagObj { get; set; }
    }
}
