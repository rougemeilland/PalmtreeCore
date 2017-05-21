/*
  DateTimeExtensions.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Linq;

namespace Palmtree
{
    /// <summary>
    /// 日時に関する拡張メソッドのクラスです。
    /// </summary>
    public static class DateTimeExtensions
    {
        #region プライベートフィールド

        private static DateTime _base_date_time;
        private static string[] _dayofweek_short_strings;
        private static string[] _dayofweek_long_strings;

        #endregion

        #region コンストラクタ

        static DateTimeExtensions()
        {
            _base_date_time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
            var sunday = new DateTime(2015, 5, 17, 12, 0, 0, DateTimeKind.Local);
            var query = Enumerable.Range(0, 7).Select(index => sunday.AddDays(index)).Select(day => new { short_name = day.ToString("ddd"), long_name = day.ToString("dddd") }).ToArray();
            _dayofweek_short_strings = query.Select(item => item.short_name).ToArray();
            _dayofweek_long_strings = query.Select(item => item.long_name).ToArray();
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 与えられた整数を標準時の1970年1月1日0時0分0秒からのミリ秒数とみなし、それと同じ時刻を表す<see cref="DateTime"/>オブジェクトを返します。
        /// </summary>
        /// <param name="time_stamp">
        /// 標準時の1970年1月1日0時0分0秒からのミリ秒数を表す整数です。
        /// </param>
        /// <returns>
        /// 与えられた整数から計算された<see cref="DateTime"/>オブジェクトです。
        /// </returns>
        public static DateTime FromTimeStampToDateTime(this long time_stamp)
        {
            var result = _base_date_time.AddMilliseconds(time_stamp);
#if DEBUG
            var x = (long)(result - _base_date_time).TotalMilliseconds;
            if (x != time_stamp)
                throw (new ApplicationException(string.Format("時刻の検算結果が一致しません。：x={0}, y={1}", x, time_stamp)));
#endif
            return (result);
        }

        /// <summary>
        /// 与えられた<see cref="DateTime"/>オブジェクトから、標準時の1970年1月1日0時0分0秒からのミリ秒数を返します。
        /// </summary>
        /// <param name="time">
        /// <see cref="DateTime"/>オブジェクトです。
        /// </param>
        /// <returns>
        /// 与えられた<see cref="DateTime"/>オブジェクトから計算された整数です。
        /// </returns>
        public static long FromDateTimeToTimeStamp(this DateTime time)
        {
            if (time.Kind == DateTimeKind.Unspecified)
                throw (new ArgumentException("timeのKindが指定されていません。"));
            var result = (long)(time.ToLocalTime() - _base_date_time).TotalMilliseconds;
#if DEBUG
            var x = _base_date_time.AddMilliseconds(result);
            if ((x - time.ToLocalTime()).Duration() > TimeSpan.FromMilliseconds(1))
                throw (new ApplicationException(string.Format("時刻の検算結果が一致しません。：x={0:yyyy/MM/dd HH:mm:ss.fff}, y={1:yyyy/MM/dd HH:mm:ss.fff}", x, time.ToLocalTime())));
#endif
            return (result);
        }

        /// <summary>
        /// 与えられた<see cref="DateTime"/>オブジェクトの最大値を求めます。
        /// </summary>
        /// <param name="time">
        /// <see cref="DateTime"/>オブジェクトです。
        /// </param>
        /// <param name="additional_times">
        /// 追加の<see cref="DateTime"/>オブジェクトです。
        /// </param>
        /// <returns>
        /// timeとadditional_timesで与えられた<see cref="DateTime"/>オブジェクトの最大値を求めます。
        /// </returns>
        public static DateTime Max(this DateTime time, params DateTime[] additional_times)
        {
            if (time.Kind == DateTimeKind.Unspecified)
                throw (new ArgumentException("timeのKindが指定されていません。"));
            if (!additional_times.Any())
                throw (new ArgumentException("additional_timesが空です。"));
            if (additional_times.Any(t => t.Kind == DateTimeKind.Unspecified))
                throw (new ArgumentException("additional_timesのKindが指定されていません。"));
            return (new[] { time }.Concat(additional_times)
                    .Select(t => new { t, gmt = t.ToUniversalTime() })
                    .Aggregate((item1, item2) => item1.gmt >= item2.gmt ? item1 : item2)
                    .t);
        }

        /// <summary>
        /// 与えられた<see cref="DateTime"/>オブジェクトの最小値を求めます。
        /// </summary>
        /// <param name="time">
        /// <see cref="DateTime"/>オブジェクトです。
        /// </param>
        /// <param name="additional_times">
        /// 追加の<see cref="DateTime"/>オブジェクトです。
        /// </param>
        /// <returns>
        /// timeとadditional_timesで与えられた<see cref="DateTime"/>オブジェクトの最小値を求めます。
        /// </returns>
        public static DateTime Min(this DateTime time, params DateTime[] additional_times)
        {
            if (time.Kind == DateTimeKind.Unspecified)
                throw (new ArgumentException("timeのKindが指定されていません。"));
            if (!additional_times.Any())
                throw (new ArgumentException("additional_timesが空です。"));
            if (additional_times.Any(t => t.Kind == DateTimeKind.Unspecified))
                throw (new ArgumentException("additional_timesのKindが指定されていません。"));
            return (new[] { time }.Concat(additional_times)
                    .Select(t => new { t, gmt = t.ToUniversalTime() })
                    .Aggregate((item1, item2) => item1.gmt <= item2.gmt ? item1 : item2)
                    .t);
        }

        /// <summary>
        /// 与えられた<see cref="TimeSpan"/>オブジェクトの最大値を求めます。
        /// </summary>
        /// <param name="time">
        /// <see cref="TimeSpan"/>オブジェクトです。
        /// </param>
        /// <param name="additional_times">
        /// 追加の<see cref="TimeSpan"/>オブジェクトです。
        /// </param>
        /// <returns>
        /// timeとadditional_timesで与えられた<see cref="TimeSpan"/>オブジェクトの最大値を求めます。
        /// </returns>
        public static TimeSpan Max(this TimeSpan time, params TimeSpan[] additional_times)
        {
            if (!additional_times.Any())
                throw (new ArgumentException("additional_timesが空です。"));
            return (new[] { time }.Concat(additional_times)
                    .Aggregate((t1, t2) => t1 >= t2 ? t1 : t2));
        }

        /// <summary>
        /// 与えられた<see cref="TimeSpan"/>オブジェクトの最小値を求めます。
        /// </summary>
        /// <param name="time">
        /// <see cref="TimeSpan"/>オブジェクトです。
        /// </param>
        /// <param name="additional_times">
        /// 追加の<see cref="TimeSpan"/>オブジェクトです。
        /// </param>
        /// <returns>
        /// timeとadditional_timesで与えられた<see cref="TimeSpan"/>オブジェクトの最小値を求めます。
        /// </returns>
        public static TimeSpan Min(this TimeSpan time, params TimeSpan[] additional_times)
        {
            if (!additional_times.Any())
                throw (new ArgumentException("additional_timesが空です。"));
            return (new[] { time }.Concat(additional_times)
                    .Aggregate((t1, t2) => t1 <= t2 ? t1 : t2));
        }

        /// <summary>
        /// 与えられた<see cref="TimeSpan"/>オブジェクトの、与えられた<see cref="double"/>オブジェクト倍だけの長さを持つ時間を求めます。
        /// </summary>
        /// <param name="x">
        /// <see cref="TimeSpan"/>オブジェクトです。
        /// </param>
        /// <param name="y">
        /// <see cref="TimeSpan"/>オブジェクトに掛ける<see cref="double"/>値です。
        /// </param>
        /// <returns>
        /// x の y 倍の長さを意味する<see cref="TimeSpan"/>オブジェクトです。
        /// </returns>
        public static TimeSpan Multiply(this TimeSpan x, double y)
        {
            return (TimeSpan.FromSeconds(x.TotalSeconds * y));
        }

        /// <summary>
        /// 与えられた<see cref="TimeSpan"/>オブジェクトの、(1 / 与えられた<see cref="double"/>オブジェクト) だけの長さを持つ時間を求めます。
        /// </summary>
        /// <param name="x">
        /// <see cref="TimeSpan"/>オブジェクトです。
        /// </param>
        /// <param name="y">
        /// <see cref="TimeSpan"/>オブジェクトを割る<see cref="double"/>値です。
        /// </param>
        /// <returns>
        /// x の 1 / y の長さを意味する<see cref="TimeSpan"/>オブジェクトです。
        /// </returns>
        public static TimeSpan Divide(this TimeSpan x, double y)
        {
            return (TimeSpan.FromSeconds(x.TotalSeconds / y));
        }

        #endregion
    }
}