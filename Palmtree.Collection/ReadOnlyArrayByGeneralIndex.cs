/*
  ReadOnlyArrayByGeneralIndex.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Palmtree.Collection;

namespace Palmtree.Collection
{
    /// <summary>
    /// 読み込み専用の配列のラッパークラスです。
    /// </summary>
    /// <typeparam name="KEY_T">
    /// 配列の添え字の型です。
    /// </typeparam>
    /// <typeparam name="VALUE_T">
    /// 配列の要素の型です。
    /// </typeparam>
    public class ReadOnlyArrayByGeneralIndex<KEY_T, VALUE_T>
        : IReadOnlyArrayByGeneralIndex<KEY_T, VALUE_T>
    {
        #region プライベートフィールド

        private Func<KEY_T, int> _index_converter;
        private VALUE_T[] _source;
        private bool _has_default_value;
        private VALUE_T _default_value;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="source">
        /// 配列の実体となるコレクションです。
        /// </param>
        /// <param name="index_converter">
        /// 配列の添え字のオブジェクトを配列のインデックスを示す整数に変換するコンバータです。
        /// </param>
        public ReadOnlyArrayByGeneralIndex(IEnumerable<VALUE_T> source, Func<KEY_T, int> index_converter)
            : this(source, index_converter, false, default(VALUE_T))
        {
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="source">
        /// 配列の実体となるコレクションです。
        /// </param>
        /// <param name="index_converter">
        /// 配列の添え字のオブジェクトを配列のインデックスを示す整数に変換するコンバータです。
        /// </param>
        /// <param name="default_value">
        /// 添え字が範囲外である場合に返される値です。
        /// </param>
        public ReadOnlyArrayByGeneralIndex(IEnumerable<VALUE_T> source, Func<KEY_T, int> index_converter, VALUE_T default_value)
            : this(source, index_converter, true, default_value)
        {
        }

        private ReadOnlyArrayByGeneralIndex(IEnumerable<VALUE_T> source, Func<KEY_T, int> index_converter, bool has_default_value, VALUE_T default_value)
        {
            _source = source.ToArray();
            _index_converter = index_converter;
            _has_default_value = has_default_value;
            _default_value = default_value;
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
                return (_source.Length);
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
        public VALUE_T this[KEY_T index]
        {
            get
            {
                var index_int = _index_converter(index);
                if (index_int < 0 || index_int >= _source.Length)
                {
                    if (_has_default_value)
                        return (_default_value);
                    else
                        throw new IndexOutOfRangeException();
                }
                return (_source[index_int]);
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
            return (_source.AsEnumerable().GetEnumerator());
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