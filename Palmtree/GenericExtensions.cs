/*
  GenericExtensions.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Linq;

namespace Palmtree
{
    /// <summary>
    /// 一般的な型に関する拡張メソッドのクラスです。
    /// </summary>
    public static class GenericExtensions
    {
        #region パブリックメソッド

        /// <summary>
        /// 与えられた値が、ある範囲内にあるかどうかを調べます。
        /// </summary>
        /// <typeparam name="VALUE_T">
        /// 値の型です。この型は<see cref="IComparable"/>インターフェースを実装している必要があります。
        /// </typeparam>
        /// <param name="value">
        /// 調べる値です。
        /// </param>
        /// <param name="low_value">
        /// 調べる範囲の下限です。
        /// </param>
        /// <param name="high_value">
        /// 調べる範囲の上限です。
        /// </param>
        /// <returns>
        /// value が low_value 以上かつ high_value 以下であれば true、そうではないのならfalseです。
        /// </returns>
        public static bool Between<VALUE_T>(this VALUE_T value, VALUE_T low_value, VALUE_T high_value)
            where VALUE_T : IComparable
        {
            if (value == null)
                return (low_value == null);
            else
                return (value.CompareTo(low_value) >= 0 && value.CompareTo(high_value) <= 0);
        }

        /// <summary>
        /// 与えられた配列の中に与えられた値と等しい要素が含まれているかどうかを調べます。
        /// </summary>
        /// <typeparam name="VALUE_T">
        /// 値の型です。
        /// </typeparam>
        /// <param name="x">
        /// 比較する値です。
        /// </param>
        /// <param name="y_array">
        /// 比較する配列です。
        /// </param>
        /// <returns>
        /// y_arrayの中にxとEqualsによる比較で一致する要素が存在すればtrue、そうではないのならfalseです。
        /// </returns>
        public static bool IsAnyOf<VALUE_T>(this VALUE_T x, params VALUE_T[] y_array)
        {
            return (y_array.Any(y => object.Equals(x, y)));
        }

        #endregion
    }
}