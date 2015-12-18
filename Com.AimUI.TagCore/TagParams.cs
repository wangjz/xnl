using System;
using System.Collections.Generic;

namespace Com.AimUI.TagCore
{
    public class TagParams : Dictionary<string, string>
    {
        public TagParams()
            : base(StringComparer.OrdinalIgnoreCase)
        {

        }
        public new string this[string key]
        {
            get
            {
                string tagParam;
                base.TryGetValue(key, out tagParam);
                return tagParam;
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
