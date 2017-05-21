/*
  IReadOnlyIndexer.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

namespace Palmtree.Collection
{
    /// <summary>
    /// 読み込み専用のインデクサのインターフェースです。
    /// </summary>
    /// <typeparam name="KEY_T">
    /// キーの型です。
    /// </typeparam>
    /// <typeparam name="VALUE_T">
    /// 値の型です。
    /// </typeparam>
    public interface IReadOnlyIndexer<KEY_T, out VALUE_T>
    {
        /// <summary>
        /// 与えられたキーに対応する値を取得します。
        /// </summary>
        /// <param name="key">
        /// キーとなるオブジェクトです。
        /// </param>
        /// <returns>
        /// 取得された値です。
        /// </returns>
        VALUE_T this[KEY_T key] { get; }
    }
}