﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
namespace COM.SingNo.XNLCore.Tags
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

        public void OnInit()
        {
            start = 0;
            end = 0;
            split = ",";
            strs = null;
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

        public object GetAttribute(string paramName, string tagName = null, object userData = null)
        {
            object v ;
            TryGetAttribute(out v, paramName, tagName);
            return v;
        }

        public bool TryGetAttribute(out object outValue, string paramName, string tagName = null, object userData = null)
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