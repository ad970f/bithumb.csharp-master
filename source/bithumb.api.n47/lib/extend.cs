using System;
using Bithumb.LIB.Configuration;
using System.Reflection;

namespace Bithumb.LIB
{
    /// <summary>
    /// 
    /// </summary>
    public static class CExtend
    {
        public static string enumToString<T>(this T enumType)
        {
            return Enum.GetName(typeof(T), enumType).ToUpperInvariant();
        }

        public static string objToString<T>(this T obj)
        {//Reflection
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            string strObject = string.Empty;

            foreach (PropertyInfo property in properties)
            {
                strObject += "Name: " + property.Name + ", Value: " + property.GetValue(obj, null) + "\n";
            }

            return strObject;

        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="dateTime"></param>
        ///// <param name="hours"></param>
        ///// <param name="minutes"></param>
        ///// <param name="seconds"></param>
        ///// <param name="milliseconds"></param>
        ///// <returns></returns>
        //public static DateTime ChangeTime(this DateTime dateTime, decimal hours, decimal minutes, decimal seconds, decimal milliseconds)
        //{
        //    return new DateTime(
        //        dateTime.Year,
        //        dateTime.Month,
        //        dateTime.Day,
        //        (int)hours,
        //        (int)minutes,
        //        (int)seconds,
        //        (int)milliseconds,
        //        dateTime.Kind);
        //}



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string ToLogDateTimeString()
        {
            return DateTime.Now.ToLogDateTimeString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public static string ToLogDateTimeString(this DateTime datetime)
        {
            return String.Format("{0:yyyy-MM-dd-HH:mm:ss}", datetime);
        }

        ///// <summary>
        ///// Where 함수는 부분집합을 추출한다
        ///// Select 함수는 모든 원소를 순회하여 재가공한다.
        ///// </summary>
        ///// <param name="self"></param>
        ///// <returns></returns>
        //public static string RemoveWhiteSpace(this string self)
        //{
        //    return new string(self.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="sourceString"></param>
        ///// <param name="chunkLength"></param>
        ///// <returns></returns>
        //public static string[] SplitByLength(this string sourceString, int chunkLength)
        //{
        //    var _result = new List<string>();

        //    for (int i = 0; i < sourceString.Length; i += chunkLength)
        //    {
        //        if (chunkLength + i > sourceString.Length)
        //            chunkLength = sourceString.Length - i;

        //        _result.Add(sourceString.Substring(i, chunkLength));
        //    }

        //    return _result.ToArray();
        //}
    }
}