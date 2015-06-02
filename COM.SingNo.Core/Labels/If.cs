using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
namespace COM.SingNo.XNLCore.Labels
{
  public  class If<T>:IXNLTag<T> where T:XNLContext
   {
      public object v1;
      public object v2;
      public string test;
      public bool value;

      public object tagV1;
      public object tagV2;
      public string tagTest;
      public bool tagValue;
      bool isSetParam;
      public T xnlContext
      {
          get;
          set;
      }

      public string instanceName { get; set; }

      public string curTag { get; set; }

      public void OnInit()
      {
          test = "=";
          value = true;

          tagTest = "=";
          tagValue = true;
          isSetParam = false;
      }

      public virtual void OnStart()
      {
          tagV1 = v1;
          tagV2 = v2;
          tagTest = test;
          value = LogicTest(v1, v2, test);
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
              if (isSetParam)
              {
                  tagValue = LogicTest(tagV1, tagV2, tagTest);
                  if (tagValue) tagDelegate();
                  isSetParam = false;
              }
              else if (value) 
              {
                  tagDelegate();
              }
          }
          else if ("else" == curTag)
          {
              if(isSetParam)
              {
                  tagValue = LogicTest(tagV1, tagV2, tagTest);
                  if (!tagValue) tagDelegate();
                  isSetParam = false;
              }
              else if(!value)
              {
                  tagDelegate();
              }
          }
          else
          {
              if (value) tagDelegate();
          }
          tagValue = value;
      }

      public void SetAttribute(string paramName, object value, string tagName=null)
      {
          if ("if" == tagName || "else" == tagName)
          {
              isSetParam = true;
              if("v1"==paramName)
              {
                  tagV1 = value;
              }
              else if("v2"==paramName)
              {
                  tagV2 = value;
              }
              else if("test"==paramName)
              {
                  if (value != null) tagTest = value.ToString();
              }
          }
          else
          {
              if ("v1" == paramName)
              {
                  v1 = value;
              }
              else if ("v2" == paramName)
              {
                  v2 = value;
              }
              else if ("test" == paramName)
              {
                  if (value != null) test = value.ToString();
              }
          }
      }

      public object GetAttribute(string paramName, string tagName = null)
      {
          object v;
          TryGetAttribute(out v, paramName, tagName);
          return v;
      }
      public bool TryGetAttribute(out object outValue, string paramName, string tagName = null)
      {
          if ("if" == tagName || "else" == tagName)
          {
              if ("v1" == paramName)
              {
                  outValue = tagV1;
                  return true;
              }
              else if ("v2" == paramName)
              {
                  outValue = tagV2;
                  return true;
              }
              else if ("test" == paramName)
              {
                  outValue = tagTest;
                  return true;
              }
              else if ("value" == paramName)
              {
                  outValue = tagValue;
                  return true;
              }
          }
          else
          {
              if ("v1" == paramName)
              {
                  outValue = v1;
                  return true;
              }
              else if ("v2" == paramName)
              {
                  outValue = v2;
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
          }
          outValue = null;
          return false;
      }

     
      //创建 
      public IXNLTag<T> Create()
      {
          return new If<T>();
      }

      public bool ExistAttribute(string paramName, string tagName=null)
      {
          if (paramName == "v1" || paramName == "v2" || paramName == "test") return true;
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