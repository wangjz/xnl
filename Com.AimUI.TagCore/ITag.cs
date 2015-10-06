using System;

namespace Com.AimUI.TagCore
{
    public delegate void OnTagDelegate();

    [Flags]
    public enum TagEvents:byte
    {
        None=0,
        Init=1,
        Start=2,
        Tag=4,
        End=8
    }

    public interface ITag<T> where T:TagContext
    {
        /// <summary>
        /// 注册的事件
        /// </summary>
        TagEvents events { get; }
        string subTagNames { get; }
        /// 标签实例名
        /// </summary>
        string instanceName { get; set; }

        //别名
        string curTag { get; set; }

        T tagContext { get; set; }
        void OnInit();

        void OnStart();

        void OnTag(OnTagDelegate tagDelegate=null);

        void OnEnd();

        void SetAttribute(string paramName, object value);

        object GetAttribute(string paramName, object[] userData = null);

        bool ExistAttribute(string paramName);
        ITag<T> Create();
    }
}
