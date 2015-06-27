﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace Com.AimUI.TagCore.Tags
{
  public  class If<T>:ITag<T> where T:TagContext
   {
      public object a;
      public object b;
      public string test;
      public bool value;

      bool isChange;

      public T tagContext
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
              a = (value??"");
          }
          else if ("b" == paramName)
          {
              b = (value??"");
          }
          else if ("test" == paramName)
          {
              if (value != null) test = value.ToString();
          }
      }

      public object GetAttribute(string paramName, object[] userData = null)  //, bool byRef = false
      {
          if ("a" == paramName)
          {
              return a;
          }
          else if ("b" == paramName)
          {
              return b;
          }
          else if ("test" == paramName)
          {
              return  test;
          }
          else if ("value" == paramName)
          {
              return value;
          }
          return null;
      }
     
      //创建 
      public ITag<T> Create()
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
                  case ">":
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
                  case "<":
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
                  case ">=":
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
                  case "<=":
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