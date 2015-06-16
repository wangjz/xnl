using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
namespace COM.SingNo.XNLCore.Tags
{
  public  class If<T>:IXNLTag<T> where T:XNLContext
   {
      public object a;
      public object b;
      public string test;
      public bool value;

      bool isChange;

      public T xnlContext
      {
          get;
          set;
      }

      public string instanceName { get; set; }

      public string curTag { get; set; }

      public void OnInit()
      {
          a = "";
          b = "";
          test = "=";
          value = true;
          isChange = false;
      }

      public virtual void OnStart()
      {
         
      }

      public void OnEnd()
      {

      }
      //子标签解析
      public void OnTag(OnTagDelegate tagDelegate=null)
      {
          if (tagDelegate == null) return;
          if ("if" == curTag)
          {
              if (isChange)
              {
                  value = LogicTest(a, b, test);
                  isChange = false;
              }
              if (value) tagDelegate();
          }
          else if ("else" == curTag)
          {
              if (isChange)
              {
                  value = LogicTest(a, b, test);
                  isChange = false;
              }
              if (!value) tagDelegate();
          }
          else
          {
              if (isChange)
              {
                  value = LogicTest(a, b, test);
                  isChange = false;
              }
              if (value) tagDelegate();
          }
      }

      public void SetAttribute(string paramName, object value)
      {
          isChange = true;
          if ("a" == paramName)
          {
              a = value;
          }
          else if ("b" == paramName)
          {
              b = value;
          }
          else if ("test" == paramName)
          {
              if (value != null) test = value.ToString();
          }
      }

      public object GetAttribute(string paramName, object userData = null)
      {
          object v;
          TryGetAttribute(out v, paramName);
          return v;
      }
      public bool TryGetAttribute(out object outValue, string paramName , object userData = null)
      {
          if ("a" == paramName)
          {
              outValue = a;
              return true;
          }
          else if ("b" == paramName)
          {
              outValue = b;
              return true;
          }
          else if ("test" == paramName)
          {
              outValue = test;
              return true;
          }
          else if ("value" == paramName)
          {
              outValue = value;
              return true;
          }

          outValue = null;
          return false;
      }

     
      //创建 
      public IXNLTag<T> Create()
      {
          return new If<T>();
      }

      public bool ExistAttribute(string paramName)
      {
          if (paramName == "a" || paramName == "b" || paramName == "test") return true;
          return false;
      }

      public string subTagNames
      {
          get{
              return "if|else";
          }
      }

      public ParseMode parseMode
      {
          get;
          set;
      }

      public static bool LogicTest(object v1, object v2, string test)
      {
          if (v1 == null && v2 == null)
          {
              if (test == "=")
              {
                  return true;
              }
              else if (test == "!=")
              {
                  return false;
              }
          }
          if (v1 == null) return false;
          try
          {
              double v;
              string s;
              switch (test)
              {
                  case "=":
                      if (v1.Equals(v2)) return true;
                      break;
                  case "!=":
                      if (!v1.Equals(v2)) return true;
                      break;
                  case "&gt;":
                      if (v2 == null) return true;
                      s = v1.ToString();

                      if (double.TryParse(s, out v)) //number
                      {
                          if (v > Convert.ToDouble(v2)) return true;
                      }
                      else
                      {
                          DateTime _v;
                          if (DateTime.TryParse(s, out _v))
                          {
                              if (_v > Convert.ToDateTime(v2)) return true;
                          }
                      }
                      break;
                  case "&lt;":
                      if (v2 == null) return false;
                      s = v1.ToString();
                      if (double.TryParse(s, out v)) //number
                      {
                          if (v < Convert.ToDouble(v2)) return true;
                      }
                      else
                      {
                          DateTime _v;
                          if (DateTime.TryParse(s, out _v))
                          {
                              if (_v < Convert.ToDateTime(v2)) return true;
                          }
                      }
                      break;
                  case "&gt;=": //>=
                      if (v2 == null) return true;
                      s = v1.ToString();
                      if (double.TryParse(s, out v)) //number
                      {
                          if (v >= Convert.ToDouble(v2)) return true;
                      }
                      else
                      {
                          DateTime _v;
                          if (DateTime.TryParse(s, out _v))
                          {
                              if (_v >= Convert.ToDateTime(v2)) return true;
                          }
                      }
                      break;
                  case "&lt;=":  //<=
                      if (v2 == null) return false;
                      s = v1.ToString();
                      if (double.TryParse(s, out v)) //number
                      {
                          if (v <= Convert.ToDouble(v2)) return true;
                      }
                      else
                      {
                          DateTime _v;
                          if (DateTime.TryParse(s, out _v))
                          {
                              if (_v <= Convert.ToDateTime(v2)) return true;
                          }
                      }
                      break;
                  default:
                      if (v1.Equals(v2)) return true;
                      break;
              }
          }
          catch (Exception)
          {
          }
          return false;
      }
   }
}