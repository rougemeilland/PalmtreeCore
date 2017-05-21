/*
  ReadOnlyDictionaryWrapper.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Collections;
using System.Collections.Generic;

namespace Palmtree.Collection
{
    /// <summary>
    /// <see cref="IReadOnlyDictionary{KEY_T, VALUE_T}"/>を実装するラッパークラスです。
    /// </summary>
    /// <typeparam name="KEY_T">
    /// キーの型です。
    /// </typeparam>
    /// <typeparam name="VALUE_T">
    /// 値の型です。
    /// </typeparam>
    public class ReadOnlyDictionaryWrapper<KEY_T, VALUE_T>
        : IReadOnlyDictionary<KEY_T, VALUE_T>
    {
        #region プライベートフィールド

        private IDictionary<KEY_T, VALUE_T> _source;
        private bool _enabled_default_value;
        private VALUE_T _default_value;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// <see cref="IDictionary{KEY_T, VALUE_T}"/>オブジェクトを初期値として使用するコンストラクタです。
        /// </summary>
        /// <param name="source">
        /// コレクションの初期値として使用されるオブジェクトです。
        /// </param>
        public ReadOnlyDictionaryWrapper(IDictionary<KEY_T, VALUE_T> source)
            : this(source, false, default(VALUE_T))
        {
        }

        /// <summary>
        /// <see cref="IDictionary{KEY_T, VALUE_T}"/>オブジェクトを初期値として使用するコンストラクタです。
        /// </summary>
        /// <param name="source">
        /// コレクションの初期値として使用されるオブジェクトです。
        /// </param>
        /// <param name="default_value">
        /// 値の既定値として使用される値です。
        /// </param>
        public ReadOnlyDictionaryWrapper(IDictionary<KEY_T, VALUE_T> source, VALUE_T default_value)
            : this(source, true, default_value)
        {
        }

        private ReadOnlyDictionaryWrapper(IDictionary<KEY_T, VALUE_T> source, bool enabled_default_value, VALUE_T default_value)
        {
            _source = source;
            _enabled_default_value = enabled_default_value;
            _default_value = default_value;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// コレクションに含まれる要素のうちキーが一致する要素が存在するかどうかを調べます。
        /// </summary>
        /// <param name="key">
        /// キーの値です。
        /// </param>
        /// <returns>
        /// コレクションに含まれる要素のうちキーが与えられたキーと一致する要素が存在すればtrue、そうではないのならfalseです。
        /// </returns>
        public bool ContainsKey(KEY_T key)
        {
            return (_source.ContainsKey(key));
        }

        /// <summary>
        /// コレクションに含まれる要素のうちキーが一致する要素を取得します。
        /// </summary>
        /// <param name="key">
        /// 取得する要素のキーです。
        /// </param>
        /// <param name="value">
        /// 取得した値です。
        /// </param>
        /// <returns>
        /// 値が取得できたならtrue、そうではないのならfalseです。
        /// </returns>
        public bool TryGetValue(KEY_T key, out VALUE_T value)
        {
            return (_source.TryGetValue(key, out value));
        }

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// コレクションに含まれる要素の数です。
        /// </summary>
        public int Count
        {
            get
            {
                return (_source.Count);
            }
        }

        /// <summary>
        /// キー値が与えられたキーと一致する要素を取得します。
        /// </summary>
        /// <param name="index">
        /// キー値です。
        /// </param>
        /// <returns>
        /// 取得した値です。
        /// </returns>
        public VALUE_T this[KEY_T index]
        {
            get
            {
                if (_enabled_default_value)
                {
                    VALUE_T value;
                    return (_source.TryGetValue(index, out value) ? value : _default_value);
                }
                else
                    return (_source[index]);
            }
        }

        /// <summary>
        /// コレクションに含まれるすべての要素のキー値のコレクションです。
        /// </summary>
        public IEnumerable<KEY_T> Keys
        {
            get
            {
                return (_source.Keys);
            }
        }

        /// <summary>
        /// コレクションに含まれるすべての要素のコレクションです。
        /// </summary>
        public IEnumerable<VALUE_T> Values
        {
            get
            {
                return (_source.Values);
            }
        }

        /// <summary>
        /// コレクションの要素を列挙する列挙子を取得します。
        /// </summary>
        /// <returns>
        /// コレクションの要素を列挙する列挙子です。
        /// </returns>
        public IEnumerator<KeyValuePair<KEY_T, VALUE_T>> GetEnumerator()
        {
            return (_source.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_source.GetEnumerator());
        }

        #endregion
    }
}