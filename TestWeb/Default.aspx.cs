﻿using System;
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
            //context.parseMode=ParseMode.Dynamic;
            //Response.Write(
            var temp = "hgjg {$test($go($url,@a),@b,'123',\" 456 \",789,true,false)} {@test}<xnl:for start=\"0\" end=\"5\" step=\"2\" str=\"a,b,c\"><xnl:if a=\"1\" b=\"2\" >{@i}-{@pos}-{@str}<br/>{@test} {@if.test2} if测试<if> 这是if项</if> <else>这是else项  <xnl:if::i2>if2测试<if::i2> 这是if2项</if::i2> <else::i2>这是else2项</else::i2></xnl:if::i2> </else></xnl:if></xnl:for>  ;ljlkhlh <xnl:if>{$test($go($url,@a),@b,'123',\" 456 \",789,true,false)}gogogo</xnl:if> jhk 222";
            var str = ParserEngine<XNLContext>.Parse(temp, context);
            //);
            Response.Write(str);

            Response.Write("\n");

            context.response.buffer.Remove(0, context.response.buffer.Length);
            str = ParserEngine<XNLContext>.Parse(temp, context, ParseMode.Dynamic);

            Response.Write(str);

            //object itemObj = XNLContext.getItem(context,"$__codeBuffer");
            //if(itemObj!=null)
            //{
            //    Response.Write("\n");
            //    Response.Write(((StringBuilder)(itemObj)).ToString());
            //}
            //{ @Site.url}  onEval()
        }
    }
}