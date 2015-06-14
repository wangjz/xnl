using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using COM.SingNo.XNLCore;
using COM.SingNo.XNLEngine;
using COM.SingNo.XNLParser;
namespace TestWeb
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            ParserEngine<XNLContext>.Initialize(RegexParser<XNLContext>.GetInstance(), new List<XNLLib<XNLContext>> { new XNLLib<XNLContext>("xnl", true) });
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}