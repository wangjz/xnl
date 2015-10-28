using System.Collections.Generic;

namespace Com.AimUI.TagCore
{
    public class TagParams:Dictionary<string,string>
    {
        public new string this[string key]
        {
            get
            {
                string tagParam;
                this.TryGetValue(key, out tagParam);
                return tagParam;
            }
            set
            {
                this[key]  = value;
            }
        }
    }
}
