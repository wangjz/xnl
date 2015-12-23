
namespace Com.AimUI.TagCore.Tags
{
    public class Pre<T> : ITag<T> where T : TagContext
    {
        public string subTagNames
        {
            get { return null; }
        }

        public string instanceName
        {
            get;
            set;
        }

        public string curTag
        {
            get;
            set;
        }

        public T tagContext
        {
            get;
            set;
        }

        public void OnInit()
        {

        }

        public void OnStart()
        {

        }

        public void OnTag(OnTagDelegate tagDelegate = null)
        {
        }

        public void OnEnd()
        {
        }

        public void SetAttribute(string paramName, object value)
        {
        }

        public object GetAttribute(string paramName, object[] userData = null)
        {
            return null;
        }

        public ITag<T> Create()
        {
            return new Pre<T>();
        }

        public TagEvents events
        {
            get { return TagEvents.Tag; }
        }
    }
}
