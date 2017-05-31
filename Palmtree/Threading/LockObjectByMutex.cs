/*
  LockObjectByMutex.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Threading;

namespace Palmtree.Threading
{
    /// <summary>
    /// <see cref="Lock"/>クラスと共に使用し、<see cref="Lock"/>オブジェクトのDispose時にMutexの排他の解放を保証するためのクラスです。
    /// </summary>
    public class LockObjectByMutex
        : ILockable
    {
        #region プライベートフィールド

        private Mutex _mutex;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="mutex">
        /// 排他のために使用する<see cref="Mutex"/>オブジェクトです。
        /// </param>
        public LockObjectByMutex(Mutex mutex)
        {
            _mutex = mutex;
        }

        #endregion

        #region ILockable メンバ

        void ILockable.Lock()
        {
            _mutex.WaitOne();
        }

        void ILockable.Unlock()
        {
            _mutex.ReleaseMutex();
        }

        #endregion
    }
}
