﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Com.AimUI.TagCore.Tags
{
    public class ListTag<T> : TagBase<T> where T : TagContext
    {
        public IList<object> list;
        public override ITag<T> Create()
        {
            return new ListTag<T>();
        }

        void init()
        {
            list = new List<object>();
        }

        void init(int capacity)
        {
            list = new List<object>(capacity);
        }

        void set(object[] userData)
        {
            if (list == null) init();
        }

        bool has(object obj)
        {
            if (list == null) return false;
            return list.Contains(obj); 
        }

        int IndexOf(object obj)
        {
            if (list == null) return -1;
            return list.IndexOf(obj);
        }

        static string join(IList<object> list, string split = ",", int start = -1, int count = -1)
        {
            if (list == null) return null;
            if (fixNumber(ref start, ref count, list.Count) == false) return null;
            string s = "";
            for (int i = start; i < count; i++)
            {
                s += (s.Length == 0 ? "" : split) + (Convert.ToString(list[i]) ?? "");
            }
            return s;
        }

        static bool fixNumber(ref int start, ref int count, int listCount)
        {
            if (start >= listCount) return false;
            if (start < 0) start = 0;

            if (count < 0 || count > listCount)
            {
                count = listCount;
            }
            if (start + count > listCount) count = listCount - start;
            if (count <= 0) return false;
            return true;
        }

        public override void SetAttribute(string paramName, object value)
        {
            switch (paramName)
            {
                case "init":
                    if (value is int)
                    {
                        init((int)value);
                    }
                    else if (value is string)
                    {
                        int i = 0;
                        if (int.TryParse(value.ToString(), out i) && i > 0)
                        {
                            init(i);
                        }
                    }
                    else
                    {
                        IEnumerable<object> _list = value as IEnumerable<object>;
                        if (_list != null) list = new List<object>(_list);
                    }
                    return;
            }
        }

        void add(object[] obj,int start=-1,int count=-1)
        {
            if (obj == null || obj.Length == 0) return;
            if (list == null) init(obj.Length);
            foreach (object o in obj)
            {
                list.Add(o);
            }
        }
        void remove(object[] obj,int start=-1,int count=-1)
        {
            if (list == null) return;
            if (obj == null || obj.Length == 0) return;
            foreach (object o in obj)
            {
                list.Remove(o);
            }
        }

        void addAt(int inx,object[] obj, int start = -1, int count = -1)
        {
            if (obj == null || obj.Length == 0) return;
            if (list == null) init();
            if (inx < 0) inx = 0;
            if (inx >= list.Count) return;
            if (start < 0) start = 0;
            if (start >= obj.Length) return;
            if (count < 0) count = obj.Length;
            if (start + count > obj.Length) count = obj.Length - start;
            if (count < 0) return;
            for (int i = start; i < start + count; i++)
            {
                list.Insert(inx + (i - start), obj[i]);
            }
        }
        void removeAt(int inx, int count = -1)
        {
            if (list == null) return;
            if (list.Count == 0) return;
            if (inx < 0) inx = 0;
            if (inx >= list.Count) return;
            if (count < 0) count = list.Count;

            if (inx == 0 && count == list.Count)
            {
                list.Clear();
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    list.RemoveAt(inx);
                    if (list.Count == 0) break;
                    if (inx > list.Count) break;
                }
            }
        }

        public override object GetAttribute(string paramName, object[] args = null)
        {
            switch (paramName)
            {
                case "add":
                    add(args);
                    return null;
                case "remove":
                    remove(args);
                    return null;
                case "addat":
                    //addAt()
                    return null;
                case "removeat":
                    //removeAt
                    return null;
                case "get":
                case "gets":
                    if (args == null || args.Length == 0) return list;
                    if (list == null) return null;
                    int start = 0;
                    int.TryParse(Convert.ToString(args[0]), out start);
                    if (start >= list.Count) start = list.Count - 1;
                    if (start < 0) start = 0;
                    int count = list.Count;
                    if (args.Length > 1) int.TryParse(Convert.ToString(args[1]), out count);
                    if (start + count > list.Count) count = list.Count - start;
                    if (count <= 0) return null;
                    object[] objs=new object[count];
                    for (int i = 0; i < count; i++)
                    {
                        objs[i] = list[start + i];
                    }
                    return objs;
                case "clear":
                    if (list != null) list.Clear();
                    return null;
                case "join":
                    if (list != null)
                    {
                        if (args == null)
                        {
                            return join(list);
                        }
                        else
                        {
                            start = -1;
                            count = -1;
                            if (args.Length > 1)
                            {
                                if (args[1] is int)
                                {
                                    start = (int)args[1];
                                }
                                else int.TryParse(Convert.ToString(args[1]), out start);
                            }
                            if (args.Length > 2)
                            {
                                if (args[2] is int)
                                {
                                    start = (int)args[2];
                                }
                                else int.TryParse(Convert.ToString(args[2]), out start);
                            }
                            return join(list, args.Length > 0 ? Convert.ToString(args[0]) : ",", start, count);
                        }
                    }
                    return null;
                case "count":
                    return list == null ? 0 : list.Count;
                case "has":
                    return null;
                case "index":
                    return null;
                case "include":
                    return null;
                case "set":
                case "sets":
                    if (args == null || args.Length == 0) return null;
                    IEnumerable<object> _list;
                    if (args.Length == 1)
                    {
                        _list = args[0] as IEnumerable<object>;
                        if (_list != null)
                        {
                            list = new List<object>(_list);
                        }
                        else
                        {
                            if (list == null) list = new List<object>();
                            list.Add(args[0]);
                        }
                        return null;
                    }
                    if (list == null) list = new List<object>(args.Length);
                    for (int i = 0; i < args.Length; i++)
                    {
                        _list = args[i] as IEnumerable<object>;
                        if (_list != null)
                        {
                            foreach (object obj in _list) list.Add(obj);
                        }
                        else
                        {
                            list.Add(args[i]);
                        }
                    }
                    return null;
            }
            return null;
        }
    }
}
