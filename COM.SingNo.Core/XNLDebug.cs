using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
namespace COM.SingNo.XNLCore
{
   public  class XNLDebug
    {
       public static void show(string debugStr)
       {
           HttpContext.Current.Response.Write(debugStr);
       }

       public static void end()
       {
           HttpContext.Current.Response.End();
       }
    }
}
