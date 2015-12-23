using System;

namespace Com.AimUI.TagCore
{
    public delegate void OnTagDelegate();

    [Flags]
    public enum TagEvents : byte
    {
        None = 0,
        Init = 1,
        Start = 2,
        Tag = 4,
        End = 8
    }

    public interface ITag<T> where T : TagContext
    {
        /// <summary>
        /// 注册的事件
        /// </summary>
        TagEvents events { get; }
        string subTagNames { get; }
        /// 标签实例名
        /// </summary>
        string instanceName { get; set; }

        /// <summary>
        /// 当前执行的子标签名称
        /// </summary>
        string curTag { get; set; }

        T tagContext { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        void OnInit();
        /// <summary>
        /// 开始执行标签体
        /// </summary>
        void OnStart();
        /// <summary>
        /// 执行标签体
        /// </summary>
        /// <param name="tagDelegate"></param>
        void OnTag(OnTagDelegate tagDelegate = null);
        /// <summary>
        /// 结束
        /// </summary>
        void OnEnd();
        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        void SetAttribute(string paramName, object value);
        /// <summary>
        /// 获取属性
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        object GetAttribute(string paramName, object[] userData = null);
        /// <summary>
        /// 创建
        /// </summary>
        /// <returns></returns>
        ITag<T> Create();
    }
}
