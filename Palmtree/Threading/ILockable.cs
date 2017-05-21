/*
  ILockable.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

namespace Palmtree.Threading
{
    /// <summary>
    /// �I�u�W�F�N�g�ɑ΂����b�N������A�����b�N���邱�Ƃ�ۏ؂��邽�߂̔ėp�C���^�[�t�F�[�X�ł��B
    /// </summary>
    public interface ILockable
    {
        /// <summary>
        /// �I�u�W�F�N�g�����b�N��Ԃɂ��܂��B
        /// </summary>
        void Lock();

        /// <summary>
        /// �I�u�W�F�N�g���A�����b�N��Ԃɂ��܂��B
        /// </summary>
        void Unlock();
    }
}
