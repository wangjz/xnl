using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using COM.SingNo.XNLCore;
using System.Collections;
namespace TestWeb
{
    public partial class test : System.Web.UI.Page
    {
        private void parse<T>(T xnlContext) where T : XNLContext
        {

            IXNLTag<T> i2 = null;
            IXNLTag<T> t__1 = null;
            IXNLTag<T> t__0 = null;
            IXNLTag<T> exp_xnl_expression = null;
            System.Text.StringBuilder buffer = xnlContext.response.buffer;

            buffer.Append(@"hgjg ");


            exp_xnl_expression = XNLLib<T>.GetTagInstance("xnl", "expression").Create();
            exp_xnl_expression.xnlContext = xnlContext;
            exp_xnl_expression.instanceName = "exp_xnl_expression";
            exp_xnl_expression.OnInit();

            buffer.Append(exp_xnl_expression.GetAttribute("lower", null, new ArrayList() { "HELLO" }));
            buffer.Append(@" ");


            buffer.Append(exp_xnl_expression.GetAttribute("iif", null, new ArrayList() { "1>2", "1", "2" }));
            buffer.Append(@" 111
"" ""

222

333
");


            buffer.Append(exp_xnl_expression.GetAttribute("_", null, new ArrayList() { "2+3" }));
            buffer.Append(@" ");


            buffer.Append(exp_xnl_expression.GetAttribute("test", null, new ArrayList() { exp_xnl_expression.GetAttribute("go", null, new ArrayList() { exp_xnl_expression.GetAttribute("url"), "" }), "", "123", " 456 ", "789", "true", "false" }));
            buffer.Append(@" ");


            buffer.Append(@"{@test}");

            t__0 = XNLLib<T>.GetTagInstance("xnl", "for").Create();
            t__0.xnlContext = xnlContext;
            t__0.instanceName = "t__0";
            t__0.OnInit();
            int t__0_v_inx_0 = buffer.Length;

            buffer.Append(@"0");

            buffer.Append(exp_xnl_expression.GetAttribute("lower", null, new ArrayList() { "" }));
            t__0.SetAttribute("start", buffer.ToString(t__0_v_inx_0, buffer.Length - t__0_v_inx_0));
            buffer.Remove(t__0_v_inx_0, buffer.Length - t__0_v_inx_0);
            t__0.SetAttribute("end", @"5");
            t__0.SetAttribute("step", @"2");
            t__0.SetAttribute("str", @"a,b,c");
            t__0.OnStart();
            t__0.curTag = null;
            OnTagDelegate t__0_delegate = delegate()
            {

                t__1 = XNLLib<T>.GetTagInstance("xnl", "if").Create();
                t__1.xnlContext = xnlContext;
                t__1.instanceName = "t__1";
                t__1.OnInit();
                t__1.SetAttribute("a", @"1");
                t__1.SetAttribute("b", @"2");
                t__1.OnStart();

                buffer.Append(t__0.GetAttribute("i", ""));
                buffer.Append(@"-");

                buffer.Append(t__0.GetAttribute("pos", ""));
                buffer.Append(@"-");

                buffer.Append(t__0.GetAttribute("str", ""));
                buffer.Append(@"<br/>");

                buffer.Append(t__1.GetAttribute("test", ""));
                buffer.Append(@" ");

                buffer.Append(t__1.GetAttribute("test2", ""));
                buffer.Append(@" if测试");
                t__1.curTag = @"if";
                OnTagDelegate t__1_if_delegate = delegate()
                {

                    buffer.Append(@" 这是if项");
                };
                t__1.OnTag(t__1_if_delegate);
                t__1.curTag = @"else";
                OnTagDelegate t__1_else_delegate = delegate()
                {

                    buffer.Append(@"这是else项  ");

                    i2 = t__1.Create();
                    i2.xnlContext = xnlContext;
                    i2.instanceName = "i2";
                    i2.OnInit();
                    i2.OnStart();

                    buffer.Append(@"if2测试");
                    i2.curTag = @"if";
                    OnTagDelegate i2_if_delegate = delegate()
                    {

                        buffer.Append(@" 这是if2项");
                    };
                    i2.OnTag(i2_if_delegate);
                    i2.curTag = @"else";
                    OnTagDelegate i2_else_delegate = delegate()
                    {

                        buffer.Append(@"这是else2项");
                    };
                    i2.OnTag(i2_else_delegate);
                    i2.OnEnd();

                    buffer.Append(@" ");
                };
                t__1.OnTag(t__1_else_delegate);
                t__1.OnEnd();
            };
            t__0.OnTag(t__0_delegate);
            t__0.OnEnd();

            buffer.Append(@"  ;ljlkhlh ");
            if (t__1 == null)
            {
                t__1 = XNLLib<T>.GetTagInstance("xnl", "if").Create();
                t__1.xnlContext = xnlContext;
                t__1.instanceName = "t__1";
                t__1.OnInit();
            }
            t__1.OnStart();
            t__1.curTag = null;
            OnTagDelegate t__1_delegate = delegate()
            {

                buffer.Append(exp_xnl_expression.GetAttribute("test", null, new ArrayList() { exp_xnl_expression.GetAttribute("go", null, new ArrayList() { exp_xnl_expression.GetAttribute("url"), t__1.GetAttribute("a", "") }), t__1.GetAttribute("b", ""), "123", " 456 ", "789", "true", "false" }));
                buffer.Append(@"gogogo");
            };
            t__1.OnTag(t__1_delegate);
            t__1.OnEnd();

            buffer.Append(@" jhk 222");




            Response.Write(buffer.ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            XNLContext xnlContext = new XNLContext();
            parse(xnlContext);
        }
    }
}