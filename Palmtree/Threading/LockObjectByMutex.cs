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
    /// <see cref="Lock"/>�N���X�Ƌ��Ɏg�p���A<see cref="Lock"/>�I�u�W�F�N�g��Dispose����Mutex�̔r���̉����ۏ؂��邽�߂̃N���X�ł��B
    /// </summary>
    public class LockObjectByMutex
        : ILockable
    {
        #region �v���C�x�[�g�t�B�[���h

        private Mutex _mutex;

        #endregion

        #region �R���X�g���N�^

        /// <summary>
        /// �R���X�g���N�^�ł��B
        /// </summary>
        /// <param name="mutex">
        /// �r���̂��߂Ɏg�p����<see cref="Mutex"/>�I�u�W�F�N�g�ł��B
        /// </param>
        public LockObjectByMutex(Mutex mutex)
        {
            _mutex = mutex;
        }

        #endregion

        #region ILockable �����o

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
