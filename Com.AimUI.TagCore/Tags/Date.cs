using System;
using System.Collections.Generic;
using System.Text;

namespace Com.AimUI.TagCore.Tags
{
    public class Date<T> : TagBase<T> where T : TagContext
    {
        public override ITag<T> Create()
        {
            return new Date<T>();
        }

        public override object GetAttribute(string paramName, object[] args = null)
        {
            switch (paramName)
            {
                case "now":
                    if (args == null) return DateTime.Now.ToLocalTime();
                    DateTime dt;
                    DateTime.TryParse(Convert.ToString(args[0]), out dt);
                    return dt;
                case "year":
                    if (args == null) return DateTime.Now.Year;
                    DateTime.TryParse(Convert.ToString(args[0]), out dt);
                    return dt.Year;
                case "month":
                    if (args == null) return DateTime.Now.Month;
                    DateTime.TryParse(Convert.ToString(args[0]), out dt);
                    return dt.Month;
                case "day":
                    if (args == null) return DateTime.Now.Day;
                    DateTime.TryParse(Convert.ToString(args[0]), out dt);
                    return dt.Day;
                case "today":
                    return DateTime.Today;
                case "week":
                    if (args == null) return DateTime.Now.DayOfWeek;
                    DateTime.TryParse(Convert.ToString(args[0]), out dt);
                    return dt.DayOfWeek;
                case "hour":
                    return DateTime.Now.Hour;
                case "minute":
                    return DateTime.Now.Minute;
                case "second":
                    return DateTime.Now.Second;
                case "millisecond":
                    return DateTime.Now.Millisecond;
                case "format":
                    return null;
            }
            return null;
        }

        public override TagEvents events
        {
            get
            {
                return TagEvents.None;
            }
        }
    }
}
