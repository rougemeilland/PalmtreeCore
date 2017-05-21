/*
  TemporaryFile.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.IO;

namespace Palmtree.IO
{
    /// <summary>
    /// ��ӂ̖��O�����ꎞ�t�@�C�������Ǘ�����N���X�ł��B
    /// </summary>
    public class TemporaryFile
        : IDisposable
    {
        #region �v���C�x�[�g�t�B�[���h

        private static object _lockobj = new object();
        private bool _disposed;
        private string _tempfile;


        #endregion

        #region �R���X�g���N�^

        /// <summary>
        /// �R���X�g���N�^�ł��B
        /// </summary>
        public TemporaryFile()
        {
            _disposed = false;
            _tempfile = Path.GetTempFileName();
        }

        /// <summary>
        /// �f�X�g���N�^�ł��B
        /// </summary>
        ~TemporaryFile()
        {
            // �ȉ���c#�̊Ǘ����ɂȂ��������������(Dispose���Ăяo������)
            // ����C#�I�u�W�F�N�g�����ɐ�ɉ������Ă���\��������̂ŁA����C#�I�u�W�F�N�g�Ɋ֘A������������͍s���Ă͂Ȃ�Ȃ�
            Dispose(false);
        }

        #endregion

        #region �p�u���b�N�v���p�e�B

        /// <summary>
        /// �ꎞ�t�@�C���̃p�X�����擾���܂��B
        /// </summary>
        public string FilePath
        {
            get
            {
                if (this._disposed)
                    throw (new ObjectDisposedException(GetType().ToString()));
                return (_tempfile);
            }
        }

        #endregion

        #region IDisposable �̃����o

        /// <summary> 
        /// �g�p���̃��\�[�X�����ׂăN���[���A�b�v���܂��B
        /// </summary>
        public virtual void Dispose()
        {
            lock (_lockobj)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region �v���e�N�e�b�h���\�b�h

        /// <summary> 
        /// �g�p���̃��\�[�X�����ׂăN���[���A�b�v���܂��B
        /// </summary>
        /// <param name="disposing">
        /// �}�l�[�W ���\�[�X���j�������ꍇ true�A�j������Ȃ��ꍇ�� false �ł��B
        /// </param>
        protected void Dispose(bool disposing)
        {
            lock (_lockobj)
            {
                if (!this._disposed)
                {
                    if (disposing)
                    {
                        // C#�̊Ǘ����ɂ��鎑�����������
                    }
                    // �ȉ���c#�̊Ǘ����ɂȂ��������������
                    try
                    {
                        File.Delete(_tempfile);
                    }
                    catch (Exception)
                    {
                    }
                    _tempfile = null;
                }
                //base.Dispose(disposing);
                _disposed = true;
            }
        }

        #endregion
    }
}