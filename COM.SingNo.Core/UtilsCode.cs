using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
//using System.Drawing.Text;
//using System.Drawing;
namespace COM.SingNo.XNLCore
{
   public class UtilsCode
   {
       public const RegexOptions regexOptions = (((RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline) | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
       public static bool IsNumeric(string value)
       {
           return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$", regexOptions);
       }
       public static bool IsInt(string value)
       {
           return Regex.IsMatch(value, @"^[+-]?\d*$", regexOptions);
       }
       public static bool IsUnsign(string value)
       {
           return Regex.IsMatch(value, @"^\d*[.]?\d*$", regexOptions);
       }
       public static bool IsDate(string value)
        {
            return Regex.IsMatch(value, @"^(?ni:(?=\d)((?'year'((1[6-9])|([2-9]\d))\d\d)(?'sep'[/.-])(?'month'0?[1-9]|1[012])\2(?'day'((?<!(\2((0?[2469])|11)\2))31)|(?<!\2(0?2)\2)(29|30)|((?<=((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(16|[2468][048]|[3579][26])00)\2\3\2)29)|((0?[1-9])|(1\d)|(2[0-8])))(?:(?=\x20\d)\x20|$))?((?<time>((0?[1-9]|1[012])(:[0-5]\d){0,2}(\x20[AP]M))|([01]\d|2[0-3])(:[0-5]\d){1,2}))?)$", regexOptions);
        }
       public static bool IsEMail(string value)
        {
            return Regex.IsMatch(value, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", regexOptions);
        }
       public static bool IsFlash(string value)
        {
            return Regex.IsMatch(value, @"\.swf$", regexOptions);
        }
       public static bool IsIdCard(string value)
        {
            return Regex.IsMatch(value, @"\d{17}[x]|\d{18}|\d{15}|\d{14}[x]$", regexOptions);
        }
       public static bool IsImage(string value)
        {
            return Regex.IsMatch(value, @"\.gif$|\.jpg$|\.bmp$|\.ico$|\.png$", regexOptions);
        }
       public static bool IsFileType(string value,string fileType)
        {
            return Regex.IsMatch(value, @"\." + fileType + "$", regexOptions);
        }
       public static bool IsUrl(string value)
        {
            return Regex.IsMatch(value, @"^http\:\/\/[a-zA-Z0-9\-\._]+(\.[a-zA-Z0-9\-\._]+){2,}(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$", regexOptions);
        }
       public static bool IsZipNo(string value)
        {
            return Regex.IsMatch(value, @"^\d{6}$", regexOptions);
        }
       /// <summary>
       /// 是否自然数
       /// </summary>
       /// <param name="value"></param>
       /// <returns></returns>
       public static bool IsGreateThanZero(string value)
        {
            return Regex.IsMatch(value, "^([1-9]|d*", regexOptions);
        }
       public static bool IsHanZi(string value)
        {
            return Regex.IsMatch(value, "[\u4e00-\u9fa5]", regexOptions);
        }
       public static bool IsTelNo(string value)
        {
            return Regex.IsMatch(value, @"(\(\d{3}\)|\d{3}-)?\d{8}", regexOptions);
        }
       public static bool IsCellPhotoNo(string value)
        {
            return Regex.IsMatch(value, @"/^[+]{0,1}(\d){1,3}[   ]?([-]?((\d)|[   ]){1,12})+$/", regexOptions);
        } 
       public static bool IsMatch(string input,string pattern)
       {
           Regex regex = new Regex(pattern, regexOptions);
           return regex.IsMatch(input);
       }
       public static string  getMatchGroup(string input, string pattern,int groupNum)
       {
           string returnStr = "";
           Regex regex = new Regex(pattern, regexOptions);
          Match m= regex.Match(input);
           if (m.Success)
           {
               if (m.Groups.Count > groupNum&&groupNum>=0)
               {
                   returnStr=m.Groups[groupNum].Value;
               }
           }
           return returnStr;
       }
       public static string getMatchCount(string input, string pattern, int groupNum)
       {
           string returnStr = "";
           Regex regex = new Regex(pattern, XNLBaseCommon.XNL_RegexOptions);
           Match m = regex.Match(input);
           if (m.Success)
           {
               if (m.Groups.Count > groupNum && groupNum >= 0)
               {
                   returnStr = m.Groups[groupNum].Value;
               }
           }
           return returnStr;
       }
       public static string getMatchs(string input, string pattern,int groupNum)
       {
           string returnStr = "";
           Regex regex = new Regex(pattern, XNLBaseCommon.XNL_RegexOptions);
           MatchCollection matchColl = regex.Matches(input);
           if (matchColl.Count>0)
           {
               string splitStr = "";
               for (var i = 0; i < matchColl.Count; i++)
               {
                   if (i > 0) splitStr = ",";
                   returnStr += splitStr + matchColl[i].Groups[groupNum];
               }
           }
           return returnStr;
       }
       public static string encodeHtml(string input)
       {
           return HttpContext.Current.Server.HtmlEncode(input).Replace("\"", "&quot;");
       }
       public static string decodeHtml(string input)
       {
           return HttpContext.Current.Server.HtmlDecode(input);
       }
       public static string decodeHtmlAndXnl(string input)
       {
           input = UtilsCode.decodeHtml(input);
           input = UtilsCode.decodeXNL(input);
           return input.Replace("&quot;","\"");
       }
       public static string encodeHtmlAndXnl(string input)
       {
           input = UtilsCode.encodeHtml(input);
           input = UtilsCode.encodeXNL(input);
           return input;
       }
       public static string encodeXNL(string input)
       {
           input = input.Replace("{", "&xnlal;");
           input = input.Replace("}", "&xnlar;");
           return input;
       }

       public static string replace(string input,string oldValue,string newValue)
       {
           return input.Replace(oldValue, newValue);
       }
       public static string decodeXNL(string input)
       {
           input = input.Replace("&xnlal;", "{");
           input = input.Replace("&xnlar;", "}");
           return input;
       }

       public static string IndexOf(string input,string indexStr,int startIndex,int count)
       {
           if (count == -1) count = input.Length;
          return Convert.ToString( input.IndexOf( indexStr, startIndex, count));
       }

       public static string Insert(string input,int startIndex,string value)
       {
           return input.Insert(startIndex,value);
       }

       public static string Remove(string input,int startIndex,int count)
       {
           if (count == -1) count = input.Length;
           return input.Remove(startIndex,count);
       }

       public static string ToLower(string input)
       {
           return input.ToLower();
       }

       public static string ToUpper(string input)
       {
           return input.ToUpper();
       }

       public static string TrimStart(string input)
       {
           return input.TrimStart();
       }

       public static string TrimEnd(string input)
       {
           return input.TrimEnd();
       }

       public static string formatDate(string input,string formatString)
       {
           return input;
       }

       public static string formatTime(string input, string formatString)
       {
           return input;
       }

       public static string formatNumber(string input, string formatString)
       {
           return input;
       }

/*日期函数*/
       public static string now()
       {
           return DateTime.Now.ToString();
       }

       public static string Year()
       {
           return DateTime.Now.Year.ToString();
       }

       public static string Today()
       {
           return DateTime.Today.ToString();
       }

       public static string Day()
       {
           return DateTime.Today.Day.ToString();
       }

       public static string Time()
       {
           return DateTime.Now.TimeOfDay.ToString();
       }

       public static string Week()
       {
           return DateTime.Today.DayOfWeek.ToString();
       }

       public static string Date()
       {
           return DateTime.Today.Date.ToString();
       }

       public static string Month()
       {
           return DateTime.Today.Month.ToString();
       }

       public static string Hour()
       {
           return DateTime.Today.Hour.ToString();
       }

       public static string Minute()
       {
           return DateTime.Now.Minute.ToString();
       }

       public static string Second()
       {
           return DateTime.Now.Second.ToString();
       }

       public static string Millisecond()
       {
           return DateTime.Now.Millisecond.ToString();
       }

    //   ToLongDateString  将当前 DateTime 对象的值转换为其等效的长日期字符串表示形式。 
    //ToLongTimeString  将当前 DateTime 对象的值转换为其等效的长时间字符串表示形式。 

    //ToShortDateString  将当前 DateTime 对象的值转换为其等效的短日期字符串表示形式。 
    //ToShortTimeString  
       public static string ToLongDate(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.ToLongDateString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string ToLongTime(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.ToLongTimeString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string ToShortDate(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.ToShortDateString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string ToShortTime(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.ToShortTimeString();
           }
           else
           {
               return dateTimeStr;
           }
       }

/*日期函数完*/
       public static string GetYear(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.Year.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string GetDay(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.Day.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string GetTime(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.TimeOfDay.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string GetWeek(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.DayOfWeek.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string GetDate(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.Date.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string GetMonth(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.Month.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string GetHour(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.Hour.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string GetMinute(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.Minute.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string GetSecond(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.Second.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }

       public static string GetMillisecond(string dateTimeStr)
       {
           DateTime resultValue;
           if (DateTime.TryParse(dateTimeStr, out resultValue))
           {
               return resultValue.Millisecond.ToString();
           }
           else
           {
               return dateTimeStr;
           }
       }
       /// <summary>
       /// 将指定中文字符串转换为拼音形式。
       /// </summary>
       /// <param name="chs">要转换的中文字符串。</param>
       /// <param name="separator">连接拼音之间的分隔符。</param>
       /// <param name="initialCap">指定是否将首字母大写。</param>
       /// <returns>包含中文字符串的拼音的字符串。</returns>
       public static string CHS2PinYin(string chs, string separator, bool initialCap)
       {
           return COM.SingNo.XNLCore.CHS2PinYin.Convert(chs, separator, initialCap);
       }
       /// <summary>
       /// 将指定中文字符串转换为拼音缩写形式。
       /// </summary>
       /// <param name="chs">要转换的中文字符串。</param>
       /// <param name="separator">连接拼音之间的分隔符。</param>
       /// <param name="initialCap">指定是否将首字母大写。</param>
       /// <returns>包含中文字符串的拼音缩写的字符串。</returns>
       public static string CHS2PY(string chs, string separator, bool initialCap)
       {
           string returnStr="";
           string pinyinStr=COM.SingNo.XNLCore.CHS2PinYin.Convert(chs, "**||**", initialCap);
           string []pyStr_arr=pinyinStr.Split(new string []{"**||**"},StringSplitOptions.RemoveEmptyEntries);
           for(int i=0;i<pyStr_arr.Length;i++)
           {
               if(i==0)
               {
                   returnStr += pyStr_arr[i].Length > 0 ? pyStr_arr[i].Substring(0, 1) : "";
               }
               else
               {
                   returnStr += separator + (pyStr_arr[i].Length > 0 ? pyStr_arr[i].Substring(0, 1) : "");
               }
             
           } 
        return returnStr;
       }
       /// <summary>
       /// 只替换第一个匹配
       /// </summary>
       /// <param name="srcStr"></param>
       /// <param name="oldStr"></param>
       /// <param name="newStr"></param>
       /// <returns></returns>
       public static string onceReplace(string srcStr, string oldStr, string newStr)
       {
           int startPos = srcStr.IndexOf(oldStr);
           if (startPos == -1 || oldStr.Trim().Equals(string.Empty)) return srcStr;
           int endPos = startPos + oldStr.Length + "<xnlReplace>".Length;
           srcStr = srcStr.Insert(startPos, "<xnlReplace>");
           srcStr = srcStr.Insert(endPos, "</xnlReplace>");
           return srcStr.Replace("<xnlReplace>" + oldStr + "</xnlReplace>", newStr);
       }
       /// <summary>
       /// json格式字符串是否有相应属性
       /// </summary>
       /// <param name="jsonData"></param>
       /// <param name="attriableName"></param>
       /// <returns></returns>
       public static bool jsonHasAttriable(string jsonData, string attriableName)
       {
           if (jsonData.IndexOf("\"" + attriableName + "\":") >= 0)
           {
               return true;
           }
           return false;
       }

       public static void writeFile(string filePath, string charset, string responseStr)
       {
           Encoding encoder = Encoding.GetEncoding(charset);
           using (FileStream fs = File.Create(filePath))
           {
               byte[] b_info = encoder.GetBytes(responseStr);
               fs.Write(b_info, 0, b_info.Length);
           }
       }
    }
}
