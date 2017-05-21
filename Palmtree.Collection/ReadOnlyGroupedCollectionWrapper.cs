/*
  ReadOnlyGroupedCollectionWrapper.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Palmtree.Collection
{
    /// <summary>
    /// 読み込み専用のグループ分けされたコレクションのラッパークラスです。
    /// </summary>
    /// <typeparam name="KEY_T"></typeparam>
    /// <typeparam name="VALUE_T"></typeparam>
    public class ReadOnlyGroupedCollectionWrapper<KEY_T, VALUE_T>
        : IGroupedCollection<KEY_T, VALUE_T>
    {
        #region プライベートフィールド

        private IGroupedCollection<KEY_T, VALUE_T> _imp;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="imp">
        /// グループ分けされたコレクションです。
        /// </param>
        public ReadOnlyGroupedCollectionWrapper(IGroupedCollection<KEY_T, VALUE_T> imp)
        {
            _imp = imp;
        }

        #endregion

        #region IGroupedCollection<KEY_T, VALUE_T> のメンバ

        IEnumerable<VALUE_T> IReadOnlyIndexer<KEY_T, IEnumerable<VALUE_T>>.this[KEY_T index]
        {
            get
            {
                return (_imp[index]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_imp.GetEnumerator());
        }

        IEnumerator<IGrouping<KEY_T, VALUE_T>> IEnumerable<IGrouping<KEY_T, VALUE_T>>.GetEnumerator()
        {
            return (_imp.GetEnumerator());
        }

        IEnumerable<KEY_T> IGroupedCollection<KEY_T, VALUE_T>.Keys
        {
            get
            {
                return (_imp.Keys);
            }
        }

        #endregion
    }
}