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
    /// Dispose�����Ƃ���ILockable.Unlock���Ăяo���A�ėp���b�N�I�u�W�F�N�g�ł��B
    /// </summary>
    public sealed class Lock
        : IDisposable
    {
        #region �v���C�x�[�g�t�B�[���h

        private ILockable _obj;

        #endregion

        #region �R���X�g���N�^

        private Lock(ILockable obj)
        {
            _obj = obj;
            _obj.Lock();
        }

        #endregion

        #region �p�u���b�N���\�b�h

        /// <summary>
        /// using�X�e�[�g�����g�Ŏg�p����<see cref="IDisposable"/>�I�u�W�F�N�g�𐶐����܂��B
        /// </summary>
        /// <param name="obj">
        /// Dispose�����Ƃ���Unlock���Ăяo��<see cref="ILockable"/>�I�u�W�F�N�g�ł��B
        /// </param>
        /// <returns>
        /// using�X�e�[�g�����g�Ŏg�p����<see cref="IDisposable"/>�I�u�W�F�N�g�ł��B
        /// </returns>
        public static IDisposable Create(ILockable obj)
        {
            return (new Lock(obj));
        }

        #endregion

        #region IDisposable �����o

        void IDisposable.Dispose()
        {
            _obj.Unlock();
        }

        #endregion
    }

}
