/*
  IReadOnlyArrayByGeneralIndex.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Collections.Generic;

namespace Palmtree.Collection
{
    /// <summary>
    /// 読み取り専用のVALUE_T型の配列のインターフェースです。
    /// </summary>
    /// <typeparam name="KEY_T">
    /// 配列の添え字の型です。
    /// </typeparam>
    /// <typeparam name="VALUE_T">
    /// 配列の値として使用される型です。
    /// </typeparam>
    public interface IReadOnlyArrayByGeneralIndex<KEY_T, VALUE_T>
        : IReadOnlyIndexer<KEY_T, VALUE_T>, IEnumerable<VALUE_T>
    {
        /// <summary>
        /// 配列の長さです。
        /// </summary>
        int Length { get; }
    }
}