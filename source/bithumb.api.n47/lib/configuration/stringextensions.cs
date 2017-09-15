using System;
using System.Text.RegularExpressions;

namespace Bithumb.LIB.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="toSearch"></param>
        /// <param name="toFind"></param>
        /// <returns></returns>
        public static bool Like(this string toSearch, string toFind)
        { // https://stackoverflow.com/questions/5417070/c-sharp-version-of-sql-like
            return new Regex(
                                @"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z",
                                RegexOptions.Singleline
                            )
                .IsMatch(toSearch);
        }

        /// <summary>
        /// 연-월-일(yyyy-MM-dd) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateString(this DateTime d)
        {
            return d.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 연월일(yyyyMMdd) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateString2(this DateTime d)
        {
            return d.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 연-월-일 시:분:초(yyyy-MM-dd HH:mm:ss) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateTimeString(this DateTime d)
        {
            return d.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 연-월-일T시:분:초.Zone(yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateTimeZoneString(this DateTime d)
        {
            return d.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK");
        }

        /// <summary>
        /// 연-월-일 시:분(yyyy-MM-dd HH:mm) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToDateHHmmString(this DateTime d)
        {
            return d.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 시:분:초(HH:mm:ss) 형식으로 변환 합니다.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string ToTimeHHmmssString(this DateTime d)
        {
            return d.ToString("HH:mm:ss");
        }
    }
}
