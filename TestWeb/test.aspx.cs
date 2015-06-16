using System;
using System.Collections.Generic;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using COM.SingNo.XNLCore;
using System.Collections;
using COM.SingNo.XNLCore.Exceptions;
namespace TestWeb
{
    public partial class test : System.Web.UI.Page
    {
        private void parse<T>(T xnlContext) where T : XNLContext
        {

            IXNLTag<T> i2 = null;
            IXNLTag<T> t__2 = null;
            IXNLTag<T> t__1 = null;
            IXNLTag<T> t__0 = null;
            IXNLTag<T> exp_xnl_expression = null;
            System.Text.StringBuilder buffer = xnlContext.response.buffer;
            try
            {
              
                buffer.Append(@"hgjg ");


                exp_xnl_expression = XNLLib<T>.GetTagInstance("xnl", "expression").Create();
                exp_xnl_expression.xnlContext = xnlContext;
                exp_xnl_expression.instanceName = "exp_xnl_expression";
                exp_xnl_expression.OnInit();

                buffer.Append(exp_xnl_expression.GetAttribute("now"));
                buffer.Append(@"  ");


                buffer.Append(exp_xnl_expression.GetAttribute("lower", new ArrayList() { @"HELLO" }));
                buffer.Append(@" ");


                buffer.Append(exp_xnl_expression.GetAttribute("iif", new ArrayList() { @"1>2", @"1", @"2" }));
                buffer.Append(@" 111
"" ""
");

                t__0 = XNLLib<T>.GetTagInstance("xnl", "set").Create();
                t__0.xnlContext = xnlContext;
                t__0.instanceName = "t__0";
                t__0.OnInit();
                t__0.SetAttribute("a", @"test a");
                try
                {

                    t__0.OnStart();
                    t__0.curTag = null;
                    OnTagDelegate t__0_delegate = delegate()
                    {

                        buffer.Append(@"set body");
                    };
                    t__0.OnTag(t__0_delegate);
                    t__0.OnEnd();
                }
                catch (TagStopException)
                {

                    t__0.OnEnd();
                }

                buffer.Append(@"
222

");


                buffer.Append(t__0.GetAttribute("a"));

                buffer.Append(@"  ");


                buffer.Append(t__0.GetAttribute("body"));

                buffer.Append(@"

333
");


                buffer.Append(exp_xnl_expression.GetAttribute("_", new ArrayList() { @"2+3" }));
                buffer.Append(@" ");


                buffer.Append(exp_xnl_expression.GetAttribute("test", new ArrayList() { exp_xnl_expression.GetAttribute("go", new ArrayList() { exp_xnl_expression.GetAttribute("url"), "" }), "", @"123", @" 456 ", @"789", @"true", @"false" }));
                buffer.Append(@" ");


                buffer.Append(@"{@test}");

                t__1 = XNLLib<T>.GetTagInstance("xnl", "for").Create();
                t__1.xnlContext = xnlContext;
                t__1.instanceName = "t__1";
                t__1.OnInit();
                int t__1_v_inx_0 = buffer.Length;

                buffer.Append(@"0");

                buffer.Append(exp_xnl_expression.GetAttribute("lower", new ArrayList() { @"" }));
                t__1.SetAttribute("start", buffer.ToString(t__1_v_inx_0, buffer.Length - t__1_v_inx_0));
                buffer.Remove(t__1_v_inx_0, buffer.Length - t__1_v_inx_0);
                t__1.SetAttribute("end", @"5");
                t__1.SetAttribute("step", @"2");
                t__1.SetAttribute("str", @"a,b,c");
                try
                {

                    t__1.OnStart();
                    t__1.curTag = null;
                    OnTagDelegate t__1_delegate = delegate()
                    {

                        t__2 = XNLLib<T>.GetTagInstance("xnl", "if").Create();
                        t__2.xnlContext = xnlContext;
                        t__2.instanceName = "t__2";
                        t__2.OnInit();
                        t__2.SetAttribute("a", @"1");
                        t__2.SetAttribute("b", @"2");
                        try
                        {

                            t__2.OnStart();

                            buffer.Append(t__1.GetAttribute("i", ""));
                            buffer.Append(@"-");

                            buffer.Append(t__1.GetAttribute("pos", ""));
                            buffer.Append(@"-");

                            buffer.Append(t__1.GetAttribute("str", ""));
                            buffer.Append(@"<br/>");

                            buffer.Append(t__2.GetAttribute("test", ""));
                            buffer.Append(@" ");

                            buffer.Append(t__2.GetAttribute("test2", ""));
                            buffer.Append(@" if测试");
                            t__2.curTag = @"if";
                            OnTagDelegate t__2_if_delegate = delegate()
                            {

                                buffer.Append(@" 这是if项");
                            };
                            t__2.OnTag(t__2_if_delegate);
                            t__2.curTag = @"else";
                            OnTagDelegate t__2_else_delegate = delegate()
                            {

                                buffer.Append(@"这是else项  ");

                                i2 = t__2.Create();
                                i2.xnlContext = xnlContext;
                                i2.instanceName = "i2";
                                i2.OnInit();
                                try
                                {

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
                                }
                                catch (TagStopException)
                                {

                                    i2.OnEnd();
                                }

                                buffer.Append(@" ");
                            };
                            t__2.OnTag(t__2_else_delegate);
                            t__2.OnEnd();
                        }
                        catch (TagStopException)
                        {

                            t__2.OnEnd();
                        }
                    };
                    t__1.OnTag(t__1_delegate);
                    t__1.OnEnd();
                }
                catch (TagStopException)
                {

                    t__1.OnEnd();
                }

                buffer.Append(@"  ;ljlkhlh ");
                if (t__2 == null)
                {
                    t__2 = XNLLib<T>.GetTagInstance("xnl", "if").Create();
                    t__2.xnlContext = xnlContext;
                    t__2.instanceName = "t__2";
                    t__2.OnInit();
                }
                try
                {

                    t__2.OnStart();
                    t__2.curTag = null;
                    OnTagDelegate t__2_delegate = delegate()
                    {

                        buffer.Append(exp_xnl_expression.GetAttribute("test", new ArrayList() { exp_xnl_expression.GetAttribute("go", new ArrayList() { exp_xnl_expression.GetAttribute("url"), t__2.GetAttribute("a", "") }), t__2.GetAttribute("b", ""), @"123", @" 456 ", @"789", @"true", @"false" }));
                        buffer.Append(@"gogogo");
                    };
                    t__2.OnTag(t__2_delegate);
                    t__2.OnEnd();
                }
                catch (TagStopException)
                {

                    t__2.OnEnd();
                }

                buffer.Append(@" jhk 222");

            }
            catch (ResponseEndException) { }

            Response.Write(buffer.ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            XNLContext xnlContext = new XNLContext();
            parse(xnlContext);
        }
    }
}