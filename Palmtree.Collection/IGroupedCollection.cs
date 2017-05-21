/*
  IGroupedCollection.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Collections.Generic;
using System.Linq;

namespace Palmtree.Collection
{
    /// <summary>
    /// キーによってグループ分けされたコレクションのクラスです。
    /// </summary>
    /// <typeparam name="KEY_T">
    /// キーの型です。
    /// </typeparam>
    /// <typeparam name="VALUE_T">
    /// コレクションの要素の型です。
    /// </typeparam>
    public interface IGroupedCollection<KEY_T, out VALUE_T>
        : IEnumerable<IGrouping<KEY_T, VALUE_T>>, IReadOnlyIndexer<KEY_T, IEnumerable<VALUE_T>>
    {
        /// <summary>
        /// すべてのキーを列挙する列挙子です。
        /// </summary>
        IEnumerable<KEY_T> Keys { get; }
    }
}