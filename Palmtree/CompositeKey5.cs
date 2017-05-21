/*
  CompositeKey5.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;

namespace Palmtree
{
    /// <summary>
    /// 5つのオブジェクトを複合キーとして一度に比較するクラスです。
    /// </summary>
    /// <typeparam name="KEY1_T">
    /// 1番目のクラスです。
    /// </typeparam>
    /// <typeparam name="KEY2_T">
    /// 2番目のクラスです。
    /// </typeparam>
    /// <typeparam name="KEY3_T">
    /// 3番目のクラスです。
    /// </typeparam>
    /// <typeparam name="KEY4_T">
    /// 4番目のクラスです。
    /// </typeparam>
    /// <typeparam name="KEY5_T">
    /// 5番目のクラスです。
    /// </typeparam>
    public class CompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T>
        : IEquatable<CompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T>>
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="key1">
        /// 1番目のクラスのオブジェクトです。
        /// </param>
        /// <param name="key2">
        /// 2番目のクラスのオブジェクトです。
        /// </param>
        /// <param name="key3">
        /// 3番目のクラスのオブジェクトです。
        /// </param>
        /// <param name="key4">
        /// 4番目のクラスのオブジェクトです。
        /// </param>
        /// <param name="key5">
        /// 5番目のクラスのオブジェクトです。
        /// </param>
        public CompositeKey(KEY1_T key1, KEY2_T key2, KEY3_T key3, KEY4_T key4, KEY5_T key5)
        {
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
            Key4 = key4;
            Key5 = key5;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// オブジェクトが等しいかどうか調べます。
        /// </summary>
        /// <param name="o">
        /// 比較対象のオブジェクトです。
        /// </param>
        /// <returns>
        /// オブジェクトが等しければtrue、そうではないのならfalseです。
        /// </returns>
        public override bool Equals(object o)
        {
            if (o == null || GetType() != o.GetType())
                return (false);
            return (Equals((CompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T>)o));
        }

        /// <summary>
        /// オブジェクトのハッシュ値を計算します。
        /// </summary>
        /// <returns>
        /// 計算されたハッシュ値です。
        /// </returns>
        public override int GetHashCode()
        {
            return (Key1.GetHashCode() ^ Key2.GetHashCode() ^ Key3.GetHashCode() ^ Key4.GetHashCode() ^ Key5.GetHashCode());
        }

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// 1番目のクラスのオブジェクトです。
        /// </summary>
        public KEY1_T Key1 { get; private set; }

        /// <summary>
        /// 2番目のクラスのオブジェクトです。
        /// </summary>
        public KEY2_T Key2 { get; private set; }

        /// <summary>
        /// 3番目のクラスのオブジェクトです。
        /// </summary>
        public KEY3_T Key3 { get; private set; }

        /// <summary>
        /// 4番目のクラスのオブジェクトです。
        /// </summary>
        public KEY4_T Key4 { get; private set; }

        /// <summary>
        /// 5番目のクラスのオブジェクトです。
        /// </summary>
        public KEY5_T Key5 { get; private set; }

        #endregion

        #region IEquatable<CompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T>> のメンバ

        /// <summary>
        /// オブジェクトが等しいかどうか調べます。
        /// </summary>
        /// <param name="o">
        /// 比較対象のオブジェクトです。
        /// </param>
        /// <returns>
        /// オブジェクトが等しければtrue、そうではないのならfalseです。
        /// </returns>
        public bool Equals(CompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T> o)
        {
            if (o == null || GetType() != o.GetType())
                return (false);
            if (!Key1.Equals(o.Key1))
                return (false);
            if (!Key2.Equals(o.Key2))
                return (false);
            if (!Key3.Equals(o.Key3))
                return (false);
            if (!Key4.Equals(o.Key4))
                return (false);
            if (!Key5.Equals(o.Key5))
                return (false);
            return (true);
        }

        #endregion
    }
}