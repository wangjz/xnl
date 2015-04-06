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
            ParserEngine<XNLContext>.xnlParser = RegexParser<XNLContext>.getInstance();
            XNLLib<XNLContext>.initialize(new List<XNLLib<XNLContext>> { new XNLLib<XNLContext>("xnl", true, new string[] { "page" }, new Dictionary<string, string> { { "channel|content|site|system|page", "xnl:extexp" } }) });
            RegxpEngineCommon<XNLContext>.updateXNLConfig();
            XNLContext context = new XNLContext();
            context.parseMode=ParseMode.Dynamic;
            //Response.Write(
            ParserEngine<XNLContext>.parse("hgjg {$app.test} <xnl:if>{@test} {@if.test2} if测试<if> 这是if项</if> <else>这是else项  <xnl:if::i2>if2测试<if::i2> 这是if2项</if::i2> <else::i2>这是else2项</else::i2></xnl:if::i2> </else></xnl:if>  ;ljlkhlh <xnl:if>gogogo</xnl:if> jhk 222", context);
            //);
            object itemObj = XNLContext.getItem(context,"__codeBuffer");
            if(itemObj!=null)
            {
                Response.Write(((StringBuilder)(itemObj)).ToString());
            }
        }
    }
}