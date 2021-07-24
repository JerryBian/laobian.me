﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share.Extension
{
    /// <summary>
    ///     Extensions for <see cref="DateTime" />
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        ///     Display time as date and time combination
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Combination of date and time part</returns>
        public static string ToDateAndTime(this DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string ToChinaDateAndTime(this DateTime time)
        {
            return time.ToString("yyyy年MM月dd日 HH时mm分ss秒 CST");
        }

        /// <summary>
        ///     Display time as date part
        /// </summary>
        /// <param name="time">The given time</param>
        /// <returns>Date part format</returns>
        public static string ToDate(this DateTime time)
        {
            return time.ToString("yyyy年MM月dd日");
        }
    }
}
