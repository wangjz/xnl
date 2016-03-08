using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Com.AimUI.TagCore;
using System.Collections;
using Com.AimUI.TagCore.Exceptions;
namespace TestWeb
{
    public partial class test : System.Web.UI.Page
    {
        private void parse<T>(T tagContext) where T : TagContext
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            TagContext tagContext = new TagContext();
            parse(tagContext);
        }
    }
}