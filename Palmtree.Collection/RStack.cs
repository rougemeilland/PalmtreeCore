/*
  RStack.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Palmtree.Collection
{
    /// <summary>
    /// 指定された型のインスタンスのLIFOの可変サイズのコレクションを表すクラスです。
    /// </summary>
    /// <typeparam name="ELEMENT_T">
    /// コレクションの要素の型です。
    /// </typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class RStack<ELEMENT_T>
        : IEnumerable<ELEMENT_T>, IEnumerable, ICollection, IReadOnlyCollection<ELEMENT_T>
    {
        #region プライベートフィールド

        private LinkedList<ELEMENT_T> _collection;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// デフォルトコンストラクタです。
        /// </summary>
        public RStack()
        {
            _collection = new LinkedList<ELEMENT_T>();
        }

        /// <summary>
        /// コンストラクタです。
        /// 与えられたコレクションを初期値としてオブジェクトを初期化します。
        /// </summary>
        /// <param name="collection">
        /// 初期値となるコレクションです。
        /// </param>
        public RStack(IEnumerable<ELEMENT_T> collection)
        {
            _collection = new LinkedList<ELEMENT_T>(collection);
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// コレクションを空にします。
        /// </summary>
        public void Clear()
        {
            _collection.Clear();
        }

        /// <summary>
        /// オブジェクトがコレクションに含まれているかどうかを調べます。
        /// </summary>
        /// <param name="item">
        /// コレクションに含まれているかどうかを調べるオブジェクトです。
        /// </param>
        /// <returns>
        /// itemがコレクションに含まれていればtrue、そうではないのならfalseです。
        /// </returns>
        public bool Contains(ELEMENT_T item)
        {
            return (_collection.Contains(item));
        }

        /// <summary>
        /// コレクションの要素を配列に複写します。
        /// </summary>
        /// <param name="array">
        /// 複写先の配列です。
        /// </param>
        /// <param name="arrayIndex">
        /// array上で複写を開始する配列のインデックスです。
        /// </param>
        public void CopyTo(ELEMENT_T[] array, int arrayIndex)
        {
            CopyToImp(array, arrayIndex);
        }

        /// <summary>
        /// コレクションの要素を列挙します。
        /// コレクションの要素のうち、先にpushされた要素が先に列挙されます。
        /// </summary>
        /// <returns>
        /// コレクションの列挙子です。
        /// </returns>
        public IEnumerator<ELEMENT_T> GetEnumerator()
        {
            return (_collection.GetEnumerator());
        }

        /// <summary>
        /// コレクションに最後にpushされた要素をコレクションから削除することなく参照します。
        /// </summary>
        /// <returns>
        /// コレクションに最後にpushされた要素です。
        /// </returns>
        public ELEMENT_T Peek()
        {
            var last = _collection.Last;
            if (last == null)
                throw new InvalidOperationException();
            return (last.Value);
        }

        /// <summary>
        /// コレクションに最後にpushされた要素をコレクションから削除します。
        /// </summary>
        /// <returns>
        /// コレクションに最後にpushされた要素です。
        /// </returns>
        public ELEMENT_T Pop()
        {
            var last = _collection.Last;
            if (last == null)
                throw new InvalidOperationException();
            var value = last.Value;
            _collection.RemoveLast();
            return (value);
        }

        /// <summary>
        /// コレクションに要素を追加します。
        /// </summary>
        /// <param name="item">
        /// コレクションに追加する要素です。
        /// </param>
        public void Push(ELEMENT_T item)
        {
            _collection.AddLast(item);
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
                return (_collection.Count);
            }
        }

        #endregion

        #region プライベートメソッド

        private void CopyToImp(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();
            var index = arrayIndex;
            foreach (var item in _collection)
            {
                if (index >= array.Length)
                    throw new ArgumentException();
                array.SetValue(item, index);
                ++index;
            }
        }

        #endregion

        #region ICollection のメンバ

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            CopyToImp(array, arrayIndex);
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return (false);
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return (this);
            }
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