/*
  GroupedList.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Palmtree.Collection
{
    /// <summary>
    /// キーによりグループ分けされたコレクションです。
    /// </summary>
    /// <typeparam name="KEY_T">
    /// グループ分けのキー(グループキー)の型です。
    /// </typeparam>
    /// <typeparam name="VALUE_T">
    /// 要素の型です。
    /// </typeparam>
    public class GroupedList<KEY_T, VALUE_T>
        : IGroupedCollection<KEY_T, VALUE_T>
    {
        #region GroupingWrapper の定義

        private class GroupingWrapper
            : IGrouping<KEY_T, VALUE_T>
        {
            #region プライベートフィールド

            private KEY_T _key;
            private IEnumerable<VALUE_T> _value;

            #endregion

            #region コンストラクタ

            public GroupingWrapper(KEY_T key, IEnumerable<VALUE_T> value)
            {
                _key = key;
                _value = value;
            }

            #endregion

            #region プライベートメソッド

            private IEnumerator<VALUE_T> GetEnumerator()
            {
                return (_value.GetEnumerator());
            }

            #endregion

            #region IGrouping<KEY_T, VALUE_T> のメンバ

            KEY_T IGrouping<KEY_T, VALUE_T>.Key
            {
                get
                {
                    return (_key);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }

            IEnumerator<VALUE_T> IEnumerable<VALUE_T>.GetEnumerator()
            {
                return (GetEnumerator());
            }

            #endregion
        }

        #endregion

        #region プライベートフィールド

        private Func<VALUE_T, KEY_T> _key_selecter;
        private IDictionary<KEY_T, ICollection<VALUE_T>> _collection;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="key_selecter">
        /// 与えられた要素からグループキーを取得するためのセレクタです。
        /// </param>
        /// <param name="key_comparer">
        /// グループキーが互いに等しいかを比較するためのオブジェクトです。
        /// </param>
        public GroupedList(Func<VALUE_T, KEY_T> key_selecter, IEqualityComparer<KEY_T> key_comparer = null)
        {
            _key_selecter = key_selecter;
            _collection = new Dictionary<KEY_T, ICollection<VALUE_T>>(key_comparer);
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// コレクションに要素を追加します。
        /// </summary>
        /// <param name="value">
        /// 追加する要素です。
        /// </param>
        public void Add(VALUE_T value)
        {
            var key = _key_selecter(value);
            ICollection<VALUE_T> col;
            if (!_collection.TryGetValue(key, out col))
            {
                col = new List<VALUE_T>();
                _collection.Add(key, col);
            }
            col.Add(value);
        }

        /// <summary>
        /// コレクションの要素をすべて削除します。
        /// </summary>
        public void Clear()
        {
            _collection.Clear();
        }

        /// <summary>
        /// 与えられたグループキーのいずれかに一致する要素をすべて取得します。
        /// </summary>
        /// <param name="keys">
        /// グループキーのコレクションです。
        /// </param>
        /// <returns>
        /// 与えられたグループキーのいずれかに一致する要素のコレクションです。
        /// </returns>
        public IEnumerable<VALUE_T> Query(params KEY_T[] keys)
        {
            IEnumerable<VALUE_T> result = new VALUE_T[0];
            foreach (var key in keys)
            {
                ICollection<VALUE_T> value;
                if (_collection.TryGetValue(key, out value))
                    result = result.Concat(value);
            }
            return (result);
        }

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// コレクションのすべての要素のグループキーのコレクションです。
        /// </summary>
        public IEnumerable<KEY_T> Keys
        {
            get
            {
                return (_collection.Keys);
            }
        }

        /// <summary>
        /// 与えられたグループキーに一致する要素のコレクションを取得します。
        /// </summary>
        /// <param name="index">
        /// グループキーです。
        /// </param>
        /// <returns>
        /// グループキーに一致する要素のコレクションです。
        /// </returns>
        public IEnumerable<VALUE_T> this[KEY_T index]
        {
            get
            {
                ICollection<VALUE_T> value;
                return (_collection.TryGetValue(index, out value) ? value : new VALUE_T[0]);
            }
        }

        #endregion

        #region プライベートメソッド

        #endregion

        #region IEnumerable<IGrouping<KEY_T, VALUE_T>> のメンバ

        /// <summary>
        /// コレクションを列挙する列挙子を取得します。
        /// </summary>
        /// <returns>
        /// コレクションを列挙する列挙子です。
        /// </returns>
        public IEnumerator<IGrouping<KEY_T, VALUE_T>> GetEnumerator()
        {
            return (_collection.Select(item => new GroupingWrapper(item.Key, item.Value)).GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (GetEnumerator());
        }

        #endregion
    }
}