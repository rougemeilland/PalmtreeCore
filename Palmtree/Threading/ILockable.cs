/*
  ILockable.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

namespace Palmtree.Threading
{
    /// <summary>
    /// オブジェクトに対しロックした後アンロックすることを保証するための汎用インターフェースです。
    /// </summary>
    public interface ILockable
    {
        /// <summary>
        /// オブジェクトをロック状態にします。
        /// </summary>
        void Lock();

        /// <summary>
        /// オブジェクトをアンロック状態にします。
        /// </summary>
        void Unlock();
    }
}
