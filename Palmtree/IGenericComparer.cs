/*
  IGenericComparer.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Collections.Generic;

namespace Palmtree
{
    /// <summary>
    /// <see cref="IEqualityComparer{T}"/>と<see cref="IComparer{T}"/>を含んだインターフェースです。
    /// </summary>
    /// <typeparam name="ITEM_T">
    /// 比較するオブジェクトの型です。
    /// </typeparam>
    public interface IGenericComparer<in ITEM_T>
        : IEqualityComparer<ITEM_T>, IComparer<ITEM_T>
    {
    }
}