using System;

namespace Com.AimUI.TagCore.Tags
{
    public class If<T> : ITag<T> where T : TagContext
    {
        public object a;
        public object b;
        public string test;
        public bool value;

        bool isChange;

        private byte _break = 0;
        public T tagContext
        {
            get;
            set;
        }

        public string instanceName { get; set; }

        public string curTag { get; set; }

        public If()
        {
            a = "";
            b = "";
            test = "=";
            value = true;
            isChange = false;
        }

        public void OnInit()
        {

        }

        public virtual void OnStart()
        {

        }

        public void OnEnd()
        {

        }

        public void OnTag(OnTagDelegate tagDelegate = null)
        {
            if (_break == 2) return;
            if (tagDelegate == null) return;
            if ("if" == curTag)
            {
                if (isChange)
                {
                    value = LogicTest(a, b, test);
                    isChange = false;
                }
                if (value)
                {
                    tagDelegate();
                    if (_break == 1) _break = 2;
                }
            }
            else if ("else" == curTag)
            {
                if (isChange)
                {
                    value = LogicTest(a, b, test);
                    isChange = false;
                }
                if (!value)
                {
                    tagDelegate();
                    if (_break == 1) _break = 2;
                }
            }
            else
            {
                if (isChange)
                {
                    value = LogicTest(a, b, test);
                    isChange = false;
                }
                if (value)
                {
                    tagDelegate();
                    if (_break == 1) _break = 2;
                }
            }
        }

        public void SetAttribute(string paramName, object value)
        {
            isChange = true;
            switch (paramName)
            {
                case "a":
                    a = (value ?? "");
                    return;
                case "b":
                    b = (value ?? "");
                    return;
                case "test":
                    if (value != null) test = value.ToString();
                    return;
                case "break":
                    if (value != null)
                    {
                        if (value is bool)
                        {
                            bool _v = Convert.ToBoolean(value);
                            _break = (byte)(_v ? 1 : 0);
                        }
                        else
                        {
                            bool _v = false;
                            string s_v = Convert.ToString(value);
                            if ("1" == s_v)
                            {
                                _v = true;
                            }
                            else if (string.IsNullOrEmpty(s_v) == false)
                            {
                                bool.TryParse(s_v, out _v);
                            }
                            _break = (byte)(_v ? 1 : 0);
                        }
                    }
                    return;
            }
        }

        public object GetAttribute(string paramName, object[] userData = null)
        {
            switch (paramName)
            {
                case "a":
                    return a;
                case "b":
                    return b;
                case "test":
                    return test;
                case "value":
                    return value;
            }
            return null;
        }

        //创建 
        public ITag<T> Create()
        {
            return new If<T>();
        }

        public bool ExistAttribute(string paramName)
        {
            if (paramName == "a" || paramName == "b" || paramName == "test" || paramName == "value") return true;
            return false;
        }

        public string subTagNames
        {
            get
            {
                return "if|else";
            }
        }

        public ParseMode parseMode
        {
            get;
            set;
        }

        public static bool LogicTest(object v1, object v2, string test)
        {
            if (v1 == null && v2 == null)
            {
                if (test == "=")
                {
                    return true;
                }
                else if (test == "!=")
                {
                    return false;
                }
            }
            if (v1 == null) return false;
            try
            {
                double v;
                string s;
                switch (test)
                {
                    case "=":
                        if (v1.GetType() == v2.GetType())
                        {
                            if (v1.Equals(v2)) return true;
                        }
                        else if (v1.ToString() == v2.ToString())
                        {
                            return true;
                        }
                        break;
                    case "!=":
                        if (v1.GetType() == v2.GetType())
                        {
                            if (!v1.Equals(v2)) return true;
                        }
                        else if (v1.ToString() != v2.ToString())
                        {
                            return true;
                        }
                        break;
                    case ">":
                    case "&gt;":
                        if (v2 == null) return true;
                        s = v1.ToString();

                        if (double.TryParse(s, out v))
                        {
                            if (v > Convert.ToDouble(v2)) return true;
                        }
                        else
                        {
                            DateTime _v;
                            if (DateTime.TryParse(s, out _v))
                            {
                                if (_v > Convert.ToDateTime(v2)) return true;
                            }
                        }
                        break;
                    case "<":
                    case "&lt;":
                        if (v2 == null) return false;
                        s = v1.ToString();
                        if (double.TryParse(s, out v))
                        {
                            if (v < Convert.ToDouble(v2)) return true;
                        }
                        else
                        {
                            DateTime _v;
                            if (DateTime.TryParse(s, out _v))
                            {
                                if (_v < Convert.ToDateTime(v2)) return true;
                            }
                        }
                        break;
                    case ">=":
                    case "&gt;=":
                        if (v2 == null) return true;
                        s = v1.ToString();
                        if (double.TryParse(s, out v))
                        {
                            if (v >= Convert.ToDouble(v2)) return true;
                        }
                        else
                        {
                            DateTime _v;
                            if (DateTime.TryParse(s, out _v))
                            {
                                if (_v >= Convert.ToDateTime(v2)) return true;
                            }
                        }
                        break;
                    case "<=":
                    case "&lt;=":
                        if (v2 == null) return false;
                        s = v1.ToString();
                        if (double.TryParse(s, out v))
                        {
                            if (v <= Convert.ToDouble(v2)) return true;
                        }
                        else
                        {
                            DateTime _v;
                            if (DateTime.TryParse(s, out _v))
                            {
                                if (_v <= Convert.ToDateTime(v2)) return true;
                            }
                        }
                        break;
                    default:
                        if (v1.Equals(v2)) return true;
                        break;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        public TagEvents events
        {
            get { return TagEvents.Tag; }
        }
    }
}