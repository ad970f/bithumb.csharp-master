//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Bithumb.LIB.Types
//{
//    /// <summary>
//    /// 
//    /// </summary>
//    public class OrderTypeConverter
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="s"></param>
//        /// <returns></returns>
//        public static OrderType FromString(string s)
//        {
//            switch (s)
//            {
//                case "sell":
//                    return OrderType.sell;
 
//                case "buy":
//                    return OrderType.buy;

//                default:
//                    throw new ArgumentException();
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="v"></param>
//        /// <returns></returns>
//        public static string ToString(OrderType v)
//        {
//            return Enum.GetName(typeof(OrderType), v).ToLowerInvariant();
//        }
//    }
//}