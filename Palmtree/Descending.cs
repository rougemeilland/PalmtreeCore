/*
  Descending.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;

namespace Palmtree
{
    /// <summary>
    /// 元のオブジェクトの大小関係を反転するラッパークラスです。
    /// </summary>
    /// <typeparam name="VALUE_T">
    /// 元のオブジェクトの型です。
    /// </typeparam>
    public class Descending<VALUE_T>
        : IComparable<Descending<VALUE_T>>, IComparable
        where VALUE_T: IComparable
    {
        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="value">
        /// 初期化に使用される元のオブジェクトです。
        /// </param>
        public Descending(VALUE_T value)
        {
            Value = value;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// VALUE_Tオブジェクトを暗黙的にDescending{VALUE_T}オブジェクトに型変換する演算子です。
        /// </summary>
        /// <param name="o">
        /// 変換するVALUE_Tオブジェクトです。
        /// </param>
        public static  implicit operator Descending<VALUE_T>(VALUE_T o)
        {
            return (new Descending<VALUE_T>(o));
        }

        /// <summary>
        /// Descending{VALUE_T}オブジェクトを暗黙的にVALUE_Tオブジェクトに型変換する演算子です。
        /// </summary>
        /// <param name="o">
        /// 変換するDescending{VALUE_T}オブジェクトです。
        /// </param>
        public static implicit operator VALUE_T(Descending<VALUE_T> o)
        {
            if (o == null)
                throw (new ArgumentNullException());
            return (o.Value);
        }

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// 元のオブジェクトです。
        /// </summary>
        public VALUE_T Value { get; private set; }

        #endregion

        #region object から継承されたメンバ

        /// <summary>
        /// 現在のオブジェクトを表す文字列を返します。
        /// </summary>
        /// <returns>
        /// 現在のオブジェクトを表す文字列です。
        /// </returns>
        public override string ToString()
        {
            return (Value == null ? "" : Value.ToString());
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
                return (-1);
            if (GetType() != o.GetType())
                throw (new ArgumentException());
            return (CompareTo((Descending<VALUE_T>)o));
        }

        #endregion

        #region IComparable<Descending<VALUE_T>> のメンバ

        /// <summary>
        /// オブジェクトの大小を比較します。
        /// </summary>
        /// <param name="o">
        /// 比較対象のオブジェクトです。
        /// </param>
        /// <returns>
        /// このオブジェクトがoより大きいならば正の値、oと等しいならば0、oより小さいならば負の値を返します。
        /// </returns>
        public int CompareTo(Descending<VALUE_T> o)
        {
            if (o == null)
                return (-1);
            if (GetType() != o.GetType())
                throw (new ArgumentException());
            int c;
            if ((c = Value.CompareTo(o.Value)) != 0)
                return (-c);
            return (0);
        }

        #endregion
    }
}