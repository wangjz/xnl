using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace Com.AimUI.TagCore.Tags
{
    public class For<T> : ITag<T> where T : TagContext
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
        public T tagContext
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

        public void SetAttribute(string paramName, object value)
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

        public object GetAttribute(string paramName, object[] userData = null) //, bool byRef = false
        {
            if (paramName == "i")
            {
                return i;
            }
            else if (paramName == "pos")
            {
                return pos;
            }
            else if (paramName == "start")
            {
                return start;
            }
            else if (paramName == "end")
            {
                return end;
            }
            else if (paramName == "str")
            {
                if (strs != null && i < strs.Length)
                {
                    return strs[i];
                }
                return  "";
            }
            else if (paramName == "split")
            {
                return split;
            }
            else if (paramName == "step")
            {
                return step;
            }
            return null;
        }


        //创建 
        public ITag<T> Create()
        {
            return new For<T>();
        }
        public bool ExistAttribute(string paramName)
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