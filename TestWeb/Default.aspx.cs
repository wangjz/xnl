using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Com.AimUI.TagCore;
using Com.AimUI.TagEngine;
using System.Text;
namespace TestWeb
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TagContext context = new TagContext();

            var temp = @"<at:if a=""1"" b=""2"">
    <if>
        修改表单
    </if>
    <else>=====
        {$now}
        <at:for start=""1"" end=""3"">

            {@i}

        </at:for>

        ====
    </else>
</at:if>";
            
            var str = ParserEngine<TagContext>.ParseToString(temp, context);
            
            Response.Write(str);

            Response.Write("\n");

            context.GetTagResponse().buffer.Remove(0, context.GetTagResponse().buffer.Length);
            str = ParserEngine<TagContext>.ParseToString(temp, context, ParseMode.Dynamic);

            Response.Write(str);
        }
    }
}