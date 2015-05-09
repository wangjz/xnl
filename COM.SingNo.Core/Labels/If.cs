using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
namespace COM.SingNo.XNLCore.Labels
{
  public  class If<T>:IXNLTag<T> where T:XNLContext
   {
      public T xnlContext
      {
          get;
          set;
      }

      public string instanceName { get; set; }

      public string curTag { get; set; }

      public void OnInit() //T xnlContext, string instanceName
      {
          //this.xnlContext = xnlContext;
          //this.instanceName = instanceName;
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
          }
          else if ("else" == curTag)
          {

          }
          tagDelegate();
      }

      public void SetAttribute(string paramName, object value, string tagName=null)
      {

      }
      public object GetAttribute(string paramName, string tagName = null)
      {
          return null;
      }
      public bool TryGetAttribute(out object outValue, string paramName, string tagName = null)
      {
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
   }
}

/*
       #region IXNLBase 成员
     public string main(XNLTagStruct tagStruct, T XNLPage)
     {
           if (string.IsNullOrEmpty(tagStruct.bodyContent))
           {
               return "";
           }
           else if (tagStruct.subTagStruct != null)
           {
               StringBuilder sb = new StringBuilder();
               string tagObjName = tagStruct.instanceName;
               XNLParam resultparam=null;
               XNLParam testParam=null;
               string operatorString = "=";
               if (tagStruct.tagParams != null)
               {
                   resultparam = tagStruct.tagParams["value"];
                   testParam = tagStruct.tagParams["test"];
                   XNLParam operaParam = tagStruct.tagParams["operator"];
                   if (operaParam != null) operatorString = operaParam.value.ToString();
               } 
               bool resultVar = logicAction(operatorString, resultparam, testParam);
               foreach (XNLTagStruct t in tagStruct.subTagStruct)
               {
                   if (string.IsNullOrEmpty(t.tagName))
                   {
                       sb.Append(t.allContent);
                   }
                   else if (string.Compare(t.tagName, "if", true) == 0 && resultVar)
                   {
                       sb.Append(ParserEngine<T>.xnlParser.replaceAttribleVariable(t.bodyContent, t.tagParams, XNLPage, tagStruct.instanceName));
                   }
                   else if (string.Compare(t.tagName, "else", true) == 0 && resultVar==false)
                   {
                       sb.Append(ParserEngine<T>.xnlParser.replaceAttribleVariable(t.bodyContent, t.tagParams, XNLPage, tagStruct.instanceName));
                   }
               }
               return ParserEngine<T>.xnlParser.replaceAttribleVariable(sb.ToString(), tagStruct.tagParams, XNLPage, tagStruct.instanceName);
           }
           return "";
       }

       #endregion

       private bool logicAction(string operatorStr, XNLParam ifVar, XNLParam resultVar)
       {
           if (ifVar == null || resultVar == null) return false;
             bool boolResult=false;
             object ifVarValue = ifVar.value;
             object resultVarValue = resultVar.value;
             switch (operatorStr)
             {
                 case "=":
                    boolResult= testEquals(resultVar, ifVar);
                     break;
                 case "&gt;":  //>
                     boolResult =testDaYu(resultVar, ifVar);
                     break;
                 case "&lt;":  //<
                     boolResult = testXiaoYu(resultVar, ifVar);
                     break;
                 case "&gt;=": //>=
                     boolResult = testDaYuOrEquals(resultVar, ifVar);
                     break;
                 case "&lt;=":  //<=
                     boolResult = testXiaoYuOrEquals(resultVar, ifVar);
                     break;
                 case "!=":
                     boolResult = testNoEquals(resultVar, ifVar);
                     break;
                 default:
                     boolResult =testEquals(resultVar, ifVar);
                     break;
             }
             return boolResult;
       }

       private Boolean testEquals( XNLParam resultVar,XNLParam testVar)
       {
           bool boolResult = false;
           object ifVarValue = testVar.value;
           object resultVarValue = resultVar.value;
           string TypeFrist1 =XNLParam.getTypeName(resultVar.type).Substring(0, 1).ToUpper();
           string TypeFrist2 = XNLParam.getTypeName(testVar.type).Substring(0, 1).ToUpper();
           try
           {
               if (TypeFrist1 == "I" || TypeFrist2 == "I")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (ifVarValue.Equals(resultVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else
                   {
                       return boolResult;
                   }
               }
               else
               {
                   if (Convert.ToString(ifVarValue).Equals(Convert.ToString(resultVarValue))) boolResult = true;
                   return boolResult;
               }              
           }
           catch
           {
               boolResult = false;
               return boolResult;
           }
       }

       private Boolean testDaYu(XNLParam resultVar, XNLParam testVar)
       {
           bool boolResult = false;
           object ifVarValue = testVar.value;
           object resultVarValue = resultVar.value;
           string typeName1=XNLParam.getTypeName(resultVar.type);
           string typeName2=XNLParam.getTypeName(testVar.type);
           string TypeFrist1 =typeName1.Substring(0, 1).ToUpper();
           string TypeFrist2 = typeName2.Substring(0, 1).ToUpper();
           try
           {
               if(TypeFrist1=="I"||TypeFrist2=="I")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (resultVar.type == XNLType.Int32 || testVar.type == XNLType.Int32)
                       {
                           if (Convert.ToInt32(resultVarValue) > Convert.ToInt32(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.Int16 || testVar.type == XNLType.Int16)
                       {
                           if (Convert.ToInt16(resultVarValue) > Convert.ToInt16(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.Int64 || testVar.type == XNLType.Int64)
                       {
                           if (Convert.ToInt64(resultVarValue) > Convert.ToInt64(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                   }
                   else
                   {
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "D" || TypeFrist2 == "D")
               {
                   if (typeName1.Trim().IndexOf("data") == 0 || typeName2.Trim().IndexOf("data") == 0)
                       {
                           DateTime resultDateTime;
                           DateTime testDateTime;
                           if (DateTime.TryParse(resultVarValue.ToString(), out resultDateTime) && DateTime.TryParse(ifVarValue.ToString(), out testDateTime))
                           {
                               if (DateTime.Compare(resultDateTime, testDateTime) > 0)
                               {
                                   boolResult = true;
                               }
                               return boolResult;
                           }
                           else
                           {
                               return boolResult;
                           }
                       }
                       else if (resultVar.type == XNLType.Double|| testVar.type == XNLType.Double)
                       {
                           if (Convert.ToDouble(resultVarValue) > Convert.ToDouble(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.Decimal || testVar.type == XNLType.Decimal)
                       {
                           if (Convert.ToDecimal(resultVarValue) > Convert.ToDecimal(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                        
                                           
               }
               else if (TypeFrist1 == "U" || TypeFrist2 == "U")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (resultVar.type == XNLType.UInt32 || testVar.type == XNLType.UInt32)
                       {
                           if (Convert.ToUInt32(resultVarValue) > Convert.ToUInt32(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.UInt16 || testVar.type == XNLType.UInt16)
                       {
                           if (Convert.ToUInt16(resultVarValue) > Convert.ToUInt16(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.UInt64 || testVar.type == XNLType.UInt64)
                       {
                           if (Convert.ToUInt64(resultVarValue) > Convert.ToUInt64(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                   }
                   else
                   {
                       return boolResult;
                   }
                    
               }
               else if (TypeFrist1 == "C" || TypeFrist2 == "C")
               {
                   if (resultVar.type == XNLType.Currency|| testVar.type == XNLType.Currency)
                   {
                       if (Convert.ToDecimal(resultVarValue) > Convert.ToDecimal(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "T" || TypeFrist2 == "T")
               {
                   if (resultVar.type == XNLType.Time || testVar.type == XNLType.Time)
                   {
                       if (Convert.ToDateTime(resultVarValue) > Convert.ToDateTime(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "S" || TypeFrist2 == "S")
               {
                   if (resultVar.type == XNLType.SByte || testVar.type == XNLType.SByte)
                   {
                       if (Convert.ToSByte(resultVarValue) > Convert.ToSByte(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if(resultVar.type == XNLType.Single|| testVar.type == XNLType.Single)
                   {
                       if (Convert.ToSingle(resultVarValue) > Convert.ToSingle(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.String || testVar.type == XNLType.String)
                   {
                       if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                       {
                           if (Convert.ToDouble(resultVarValue) > Convert.ToDouble(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else
                       {
                           return boolResult;
                       }
                   }
               }
               else if (TypeFrist1 == "B" || TypeFrist2 == "B")
               {
                   if (Convert.ToByte(resultVarValue) > Convert.ToByte(ifVarValue)) boolResult = true;
                   return boolResult;
               }
               else
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                        if (Convert.ToDouble(resultVarValue) > Convert.ToDouble(ifVarValue)) boolResult = true;
                        return boolResult;                    
                   }
                   else
                   {
                       return boolResult;
                   }
               }
           }
           catch
           {
               boolResult = false;
               return boolResult;

           }
           return boolResult;
       }

       private Boolean testXiaoYu(XNLParam resultVar, XNLParam testVar)
       {
           bool boolResult = false;
           object ifVarValue = testVar.value;
           object resultVarValue = resultVar.value;
           string typeName1 = XNLParam.getTypeName(resultVar.type);
           string typeName2 = XNLParam.getTypeName(testVar.type);
           string TypeFrist1 = typeName1.Substring(0, 1).ToUpper();
           string TypeFrist2 = typeName2.Substring(0, 1).ToUpper();
           try
           {
               if (TypeFrist1 == "I" || TypeFrist2 == "I")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (resultVar.type == XNLType.Int32 || testVar.type == XNLType.Int32)
                       {
                           if (Convert.ToInt32(resultVarValue) < Convert.ToInt32(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.Int16 || testVar.type == XNLType.Int16)
                       {
                           if (Convert.ToInt16(resultVarValue) < Convert.ToInt16(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.Int64 || testVar.type == XNLType.Int64)
                       {
                           if (Convert.ToInt64(resultVarValue) < Convert.ToInt64(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                   }
                   else
                   {
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "D" || TypeFrist2 == "D")
               {

                   if (typeName1.Trim().IndexOf("data") == 0 || typeName2.Trim().IndexOf("data") == 0)
                   {
                       DateTime resultDateTime;
                       DateTime testDateTime;
                       if (DateTime.TryParse(resultVarValue.ToString(), out resultDateTime) && DateTime.TryParse(ifVarValue.ToString(), out testDateTime))
                       {
                           if (DateTime.Compare(resultDateTime, testDateTime) < 0)
                           {
                               boolResult = true;
                           }
                           return boolResult;
                       }
                       else
                       {
                           return boolResult;
                       }
                   }
                   else if (resultVar.type == XNLType.Double || testVar.type == XNLType.Double)
                   {
                       if (Convert.ToDouble(resultVarValue) < Convert.ToDouble(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.Decimal || testVar.type == XNLType.Decimal)
                   {
                       if (Convert.ToDecimal(resultVarValue) < Convert.ToDecimal(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   

               }
               else if (TypeFrist1 == "U" || TypeFrist2 == "U")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (resultVar.type == XNLType.UInt32 || testVar.type == XNLType.UInt32)
                       {
                           if (Convert.ToUInt32(resultVarValue) < Convert.ToUInt32(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.UInt16 || testVar.type == XNLType.UInt16)
                       {
                           if (Convert.ToUInt16(resultVarValue) < Convert.ToUInt16(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.UInt64 || testVar.type == XNLType.UInt64)
                       {
                           if (Convert.ToUInt64(resultVarValue) < Convert.ToUInt64(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                   }
                   else
                   {
                       return boolResult;
                   }

               }
               else if (TypeFrist1 == "C" || TypeFrist2 == "C")
               {
                   if (resultVar.type == XNLType.Currency || testVar.type == XNLType.Currency)
                   {
                       if (Convert.ToDecimal(resultVarValue) < Convert.ToDecimal(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "T" || TypeFrist2 == "T")
               {
                   if (resultVar.type == XNLType.Time || testVar.type == XNLType.Time)
                   {
                       if (Convert.ToDateTime(resultVarValue) < Convert.ToDateTime(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "S" || TypeFrist2 == "S")
               {
                   if (resultVar.type == XNLType.SByte || testVar.type == XNLType.SByte)
                   {
                       if (Convert.ToSByte(resultVarValue) < Convert.ToSByte(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.Single || testVar.type == XNLType.Single)
                   {
                       if (Convert.ToSingle(resultVarValue) < Convert.ToSingle(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.String || testVar.type == XNLType.String)
                   {
                       if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                       {
                           if (Convert.ToDouble(resultVarValue) < Convert.ToDouble(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else
                       {
                           return boolResult;
                       }
                   }
               }
               else if (TypeFrist1 == "B" || TypeFrist2 == "B")
               {
                   if (Convert.ToByte(resultVarValue) < Convert.ToByte(ifVarValue)) boolResult = true;
                   return boolResult;
               }
               else
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (Convert.ToDouble(resultVarValue) < Convert.ToDouble(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else
                   {
                       return boolResult;
                   }
               }
           }
           catch
           {
               boolResult = false;
               return boolResult;

           }
           return boolResult;
       }

       private Boolean testDaYuOrEquals(XNLParam resultVar, XNLParam testVar)
       {
           bool boolResult = false;
           object ifVarValue = testVar.value;
           object resultVarValue = resultVar.value;
           string typeName1 = XNLParam.getTypeName(resultVar.type);
           string typeName2 = XNLParam.getTypeName(testVar.type);
           string TypeFrist1 = typeName1.Substring(0, 1).ToUpper();
           string TypeFrist2 = typeName2.Substring(0, 1).ToUpper();
           try
           {
               if (TypeFrist1 == "I" || TypeFrist2 == "I")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (resultVar.type == XNLType.Int32 || testVar.type == XNLType.Int32)
                       {
                           if (Convert.ToInt32(resultVarValue) >= Convert.ToInt32(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.Int16 || testVar.type == XNLType.Int16)
                       {
                           if (Convert.ToInt16(resultVarValue) >= Convert.ToInt16(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.Int64 || testVar.type == XNLType.Int64)
                       {
                           if (Convert.ToInt64(resultVarValue) >= Convert.ToInt64(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                   }
                   else
                   {
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "D" || TypeFrist2 == "D")
               {
                   if (typeName1.Trim().IndexOf("data") == 0 || typeName2.Trim().IndexOf("data") == 0)
                   {
                       DateTime resultDateTime;
                       DateTime testDateTime;
                       if (DateTime.TryParse(resultVarValue.ToString(), out resultDateTime) && DateTime.TryParse(ifVarValue.ToString(), out testDateTime))
                       {
                           if (DateTime.Compare(resultDateTime, testDateTime) > 0 || DateTime.Compare(resultDateTime, testDateTime) == 0)
                           {
                               boolResult = true;
                           }
                           return boolResult;
                       }
                       else
                       {
                           return boolResult;
                       }
                   }
                   else if (resultVar.type == XNLType.Double || testVar.type == XNLType.Double)
                   {
                       if (Convert.ToDouble(resultVarValue) >= Convert.ToDouble(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.Decimal || testVar.type == XNLType.Decimal)
                   {
                       if (Convert.ToDecimal(resultVarValue) >= Convert.ToDecimal(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                    

               }
               else if (TypeFrist1 == "U" || TypeFrist2 == "U")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (resultVar.type == XNLType.UInt32 || testVar.type == XNLType.UInt32)
                       {
                           if (Convert.ToUInt32(resultVarValue) >= Convert.ToUInt32(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.UInt16 || testVar.type == XNLType.UInt16)
                       {
                           if (Convert.ToUInt16(resultVarValue) >= Convert.ToUInt16(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.UInt64 || testVar.type == XNLType.UInt64)
                       {
                           if (Convert.ToUInt64(resultVarValue) >= Convert.ToUInt64(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                   }
                   else
                   {
                       return boolResult;
                   }

               }
               else if (TypeFrist1 == "C" || TypeFrist2 == "C")
               {
                   if (resultVar.type == XNLType.Currency || testVar.type == XNLType.Currency)
                   {
                       if (Convert.ToDecimal(resultVarValue) >= Convert.ToDecimal(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "T" || TypeFrist2 == "T")
               {
                   if (resultVar.type == XNLType.Time || testVar.type == XNLType.Time)
                   {
                       if (Convert.ToDateTime(resultVarValue) >= Convert.ToDateTime(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "S" || TypeFrist2 == "S")
               {
                   if (resultVar.type == XNLType.SByte || testVar.type == XNLType.SByte)
                   {
                       if (Convert.ToSByte(resultVarValue) >= Convert.ToSByte(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.Single || testVar.type == XNLType.Single)
                   {
                       if (Convert.ToSingle(resultVarValue) >= Convert.ToSingle(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.String || testVar.type == XNLType.String)
                   {
                       if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                       {
                           if (Convert.ToDouble(resultVarValue) >= Convert.ToDouble(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else
                       {
                           return boolResult;
                       }
                   }
               }
               else if (TypeFrist1 == "B" || TypeFrist2 == "B")
               {
                   if (Convert.ToByte(resultVarValue) >= Convert.ToByte(ifVarValue)) boolResult = true;
                   return boolResult;
               }
               else
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (Convert.ToDouble(resultVarValue) >= Convert.ToDouble(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else
                   {
                       return boolResult;
                   }
               }
           }
           catch
           {
               boolResult = false;
               return boolResult;

           }
           return boolResult;
       }

       private Boolean testXiaoYuOrEquals(XNLParam resultVar, XNLParam testVar)
       {

           bool boolResult = false;
           object ifVarValue = testVar.value;
           object resultVarValue = resultVar.value;
           string typeName1 = XNLParam.getTypeName(resultVar.type);
           string typeName2 = XNLParam.getTypeName(testVar.type);
           string TypeFrist1 = typeName1.Substring(0, 1).ToUpper();
           string TypeFrist2 = typeName2.Substring(0, 1).ToUpper();
           try
           {
               if (TypeFrist1 == "I" || TypeFrist2 == "I")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (resultVar.type == XNLType.Int32 || testVar.type == XNLType.Int32)
                       {
                           if (Convert.ToInt32(resultVarValue) <= Convert.ToInt32(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.Int16 || testVar.type == XNLType.Int16)
                       {
                           if (Convert.ToInt16(resultVarValue) <= Convert.ToInt16(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.Int64 || testVar.type == XNLType.Int64)
                       {
                           if (Convert.ToInt64(resultVarValue) <= Convert.ToInt64(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                   }
                   else
                   {
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "D" || TypeFrist2 == "D")
               {

                   if (typeName1.Trim().IndexOf("data") == 0 || typeName2.Trim().IndexOf("data") == 0)
                   {
                       DateTime resultDateTime;
                       DateTime testDateTime;
                       if (DateTime.TryParse(resultVarValue.ToString(), out resultDateTime) && DateTime.TryParse(ifVarValue.ToString(), out testDateTime))
                       {
                           if (DateTime.Compare(resultDateTime, testDateTime) < 0 || DateTime.Compare(resultDateTime, testDateTime) == 0)
                           {
                               boolResult = true;
                           }
                           return boolResult;
                       }
                       else
                       {
                           return boolResult;
                       }
                   }
                   else if (resultVar.type == XNLType.Double || testVar.type == XNLType.Double)
                   {
                       if (Convert.ToDouble(resultVarValue) <= Convert.ToDouble(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.Decimal || testVar.type == XNLType.Decimal)
                   {
                       if (Convert.ToDecimal(resultVarValue) <= Convert.ToDecimal(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "U" || TypeFrist2 == "U")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (resultVar.type == XNLType.UInt32 || testVar.type == XNLType.UInt32)
                       {
                           if (Convert.ToUInt32(resultVarValue) <= Convert.ToUInt32(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.UInt16 || testVar.type == XNLType.UInt16)
                       {
                           if (Convert.ToUInt16(resultVarValue) <= Convert.ToUInt16(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else if (resultVar.type == XNLType.UInt64 || testVar.type == XNLType.UInt64)
                       {
                           if (Convert.ToUInt64(resultVarValue) <= Convert.ToUInt64(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                   }
                   else
                   {
                       return boolResult;
                   }

               }
               else if (TypeFrist1 == "C" || TypeFrist2 == "C")
               {
                   if (resultVar.type == XNLType.Currency || testVar.type == XNLType.Currency)
                   {
                       if (Convert.ToDecimal(resultVarValue) <= Convert.ToDecimal(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "T" || TypeFrist2 == "T")
               {
                   if (resultVar.type == XNLType.Time || testVar.type == XNLType.Time)
                   {
                       if (Convert.ToDateTime(resultVarValue) <= Convert.ToDateTime(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
               }
               else if (TypeFrist1 == "S" || TypeFrist2 == "S")
               {
                   if (resultVar.type == XNLType.SByte || testVar.type == XNLType.SByte)
                   {
                       if (Convert.ToSByte(resultVarValue) <= Convert.ToSByte(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.Single || testVar.type == XNLType.Single)
                   {
                       if (Convert.ToSingle(resultVarValue) <= Convert.ToSingle(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else if (resultVar.type == XNLType.String || testVar.type == XNLType.String)
                   {
                       if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                       {
                           if (Convert.ToDouble(resultVarValue) <= Convert.ToDouble(ifVarValue)) boolResult = true;
                           return boolResult;
                       }
                       else
                       {
                           return boolResult;
                       }
                   }
               }
               else if (TypeFrist1 == "B" || TypeFrist2 == "B")
               {
                   if (Convert.ToByte(resultVarValue) <= Convert.ToByte(ifVarValue)) boolResult = true;
                   return boolResult;
               }
               else
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (Convert.ToDouble(resultVarValue) <= Convert.ToDouble(ifVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else
                   {
                       return boolResult;
                   }
               }
           }
           catch
           {
               boolResult = false;
               return boolResult;

           }
           return boolResult;
       }

       private Boolean testNoEquals(XNLParam resultVar, XNLParam testVar)
       {

           bool boolResult = false;
           object ifVarValue = testVar.value;
           object resultVarValue = resultVar.value;
           string typeName1 = XNLParam.getTypeName(resultVar.type);
           string typeName2 = XNLParam.getTypeName(testVar.type);
           string TypeFrist1 = typeName1.Substring(0, 1).ToUpper();
           string TypeFrist2 = typeName2.Substring(0, 1).ToUpper();
           try
           {
               if (TypeFrist1 == "I" || TypeFrist2 == "I")
               {
                   if (UtilsCode.IsNumeric(Convert.ToString(resultVarValue)) && UtilsCode.IsNumeric(Convert.ToString(ifVarValue)))
                   {
                       if (!ifVarValue.Equals(resultVarValue)) boolResult = true;
                       return boolResult;
                   }
                   else
                   {
                       return boolResult;
                   }
               }
               else
               {
                   if (!Convert.ToString(ifVarValue).Equals(Convert.ToString(resultVarValue))) boolResult = true;
                   return boolResult;
               }
           }
           catch
           {
               boolResult = false;
               return boolResult;
           }
       }
      */