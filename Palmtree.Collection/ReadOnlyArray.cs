/*
  ReadOnlyArray.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Palmtree.Collection;

namespace Palmtree.Collection
{
    /// <summary>
    /// 読み込み専用の配列のラッパークラスです。
    /// </summary>
    /// <typeparam name="VALUE_T">
    /// 配列の要素の型です。
    /// </typeparam>
    public class ReadOnlyArray<VALUE_T>
        : IReadOnlyArray<VALUE_T>
    {
        #region プライベートフィールド

        private VALUE_T[] _imp;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="source">
        /// 配列の実体となるコレクションです。
        /// </param>
        public ReadOnlyArray(IEnumerable<VALUE_T> source)
        {
            _imp = source.ToArray();
        }

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// 配列の大きさです。
        /// </summary>
        public int Length
        {
            get
            {
                return (_imp.Length);
            }
        }

        /// <summary>
        /// 配列の要素です。
        /// </summary>
        /// <param name="index">
        /// 配列のインデックスです。
        /// </param>
        /// <returns>
        /// インデックスに対応する配列の要素です。
        /// </returns>
        public VALUE_T this[int index]
        {
            get
            {
                return (_imp[index]);
            }
        }

        #endregion

        #region IEnumerable<VALUE_T> のメンバ

        /// <summary>
        /// コレクションを列挙するための列挙子を取得します。
        /// </summary>
        /// <returns>
        /// 取得した列挙子です。
        /// </returns>
        public IEnumerator<VALUE_T> GetEnumerator()
        {
            return (_imp.AsEnumerable().GetEnumerator());
        }

        #endregion

        #region IEnumerable のメンバ

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (GetEnumerator());
        }

        #endregion
    }
}