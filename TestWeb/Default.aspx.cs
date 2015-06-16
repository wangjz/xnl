using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using COM.SingNo.XNLCore;
using COM.SingNo.XNLEngine;
using System.Text;
namespace TestWeb
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            XNLContext context = new XNLContext();

            var temp = @"<xnl:if a=""{$get(action)}"">
    {@a}
    <if b=""add"">
        添加表单
        {$end}
    </if>
    {@b}
    <if b=""modify"">
        {@b}
        修改表单
        {$end}
    </if>
    {@a} {@b}
</xnl:if>";
            
            var str = ParserEngine<XNLContext>.Parse(temp, context);
            
            Response.Write(str);

            Response.Write("\n");

            context.response.buffer.Remove(0, context.response.buffer.Length);
            str = ParserEngine<XNLContext>.Parse(temp, context, ParseMode.Dynamic);

            Response.Write(str);
        }
    }
}