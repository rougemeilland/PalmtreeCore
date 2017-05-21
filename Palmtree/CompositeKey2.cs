/*
  CompositeKey2.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;

namespace Palmtree
{
    /// <summary>
    /// 2つのオブジェクトを複合キーとして一度に比較するクラスです。
    /// </summary>
    /// <typeparam name="KEY1_T">
    /// 1番目のクラスです。
    /// </typeparam>
    /// <typeparam name="KEY2_T">
    /// 2番目のクラスです。
    /// </typeparam>
    public class CompositeKey<KEY1_T, KEY2_T>
        : IEquatable<CompositeKey<KEY1_T, KEY2_T>>
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
        public CompositeKey(KEY1_T key1, KEY2_T key2)
        {
            Key1 = key1;
            Key2 = key2;
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
            return (Equals((CompositeKey<KEY1_T, KEY2_T>)o));
        }

        /// <summary>
        /// オブジェクトのハッシュ値を計算します。
        /// </summary>
        /// <returns>
        /// 計算されたハッシュ値です。
        /// </returns>
        public override int GetHashCode()
        {
            return (Key1.GetHashCode() ^ Key2.GetHashCode());
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

        #endregion

        #region IEquatable<CompositeKey2<KEY1_T, KEY2_T>> のメンバ

        /// <summary>
        /// オブジェクトが等しいかどうか調べます。
        /// </summary>
        /// <param name="o">
        /// 比較対象のオブジェクトです。
        /// </param>
        /// <returns>
        /// オブジェクトが等しければtrue、そうではないのならfalseです。
        /// </returns>
        public bool Equals(CompositeKey<KEY1_T, KEY2_T> o)
        {
            if (o == null || GetType() != o.GetType())
                return (false);
            if (!Key1.Equals(o.Key1))
                return (false);
            if (!Key2.Equals(o.Key2))
                return (false);
            return (true);
        }

        #endregion

    }
}