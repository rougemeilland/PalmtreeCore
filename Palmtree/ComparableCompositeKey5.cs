/*
  ComparableCompositeKey5.cs

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
    public class ComparableCompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T>
        : CompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T>, IComparable<ComparableCompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T>>, IComparable
        where KEY1_T : IComparable
        where KEY2_T : IComparable
        where KEY3_T : IComparable
        where KEY4_T : IComparable
        where KEY5_T : IComparable
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
        public ComparableCompositeKey(KEY1_T key1, KEY2_T key2, KEY3_T key3, KEY4_T key4, KEY5_T key5)
            : base(key1, key2, key3, key4, key5)
        {
        }

        #endregion

        #region IComparable のメンバ

        /// <summary>
        /// オブジェクトの大小を比較します。
        /// </summary>
        /// <param name="o">
        /// 比較対象のオブジェクトです。
        /// </param>
        /// <returns>
        /// このオブジェクトがoより大きいならば正の値、oと等しいならば0、oより小さいならば負の値を返します。
        /// </returns>
        public int CompareTo(object o)
        {
            if (o == null)
                return (1);
            if (GetType() != o.GetType())
                throw (new ArgumentException());
            return (CompareTo((ComparableCompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T>)o));
        }

        #endregion

        #region IComparable<ComparableCompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T>> のメンバ

        /// <summary>
        /// オブジェクトの大小を比較します。
        /// </summary>
        /// <param name="o">
        /// 比較対象のオブジェクトです。
        /// </param>
        /// <returns>
        /// このオブジェクトがoより大きいならば正の値、oと等しいならば0、oより小さいならば負の値を返します。
        /// </returns>
        public int CompareTo(ComparableCompositeKey<KEY1_T, KEY2_T, KEY3_T, KEY4_T, KEY5_T> o)
        {
            if (o == null)
                return (1);
            if (GetType() != o.GetType())
                throw (new ArgumentException());
            int c;
            if ((c = Key1.CompareTo(o.Key1)) != 0)
                return (c);
            if ((c = Key2.CompareTo(o.Key2)) != 0)
                return (c);
            if ((c = Key3.CompareTo(o.Key3)) != 0)
                return (c);
            if ((c = Key4.CompareTo(o.Key4)) != 0)
                return (c);
            if ((c = Key5.CompareTo(o.Key5)) != 0)
                return (c);
            return (0);
        }

        #endregion
    }
}