/*
  Lock.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;

namespace Palmtree.Threading
{
    /// <summary>
    /// DisposeされるときにILockable.Unlockを呼び出す、汎用ロックオブジェクトです。
    /// </summary>
    public sealed class Lock
        : IDisposable
    {
        #region プライベートフィールド

        private ILockable _obj;

        #endregion

        #region コンストラクタ

        private Lock(ILockable obj)
        {
            _obj = obj;
            _obj.Lock();
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// usingステートメントで使用する<see cref="IDisposable"/>オブジェクトを生成します。
        /// </summary>
        /// <param name="obj">
        /// DisposeされるときにUnlockを呼び出す<see cref="ILockable"/>オブジェクトです。
        /// </param>
        /// <returns>
        /// usingステートメントで使用する<see cref="IDisposable"/>オブジェクトです。
        /// </returns>
        public static IDisposable Create(ILockable obj)
        {
            return (new Lock(obj));
        }

        #endregion

        #region IDisposable メンバ

        void IDisposable.Dispose()
        {
            _obj.Unlock();
        }

        #endregion
    }

}
