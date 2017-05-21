/*
  CodeScope.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;

namespace Palmtree.Diagnostics
{
    /// <summary>
    /// 指定したコードの区間の開始と終了時に特定のコードを実行させるクラスの基本クラスです。
    /// </summary>
    public abstract class CodeScope
        : IDisposable
    {
        #region プライベートフィールド

        private bool _disposed;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        protected CodeScope()
        {
            _disposed = false;
        }

        #endregion

        #region デストラクタ

        /// <summary>
        /// デストラクタです。
        /// </summary>
        ~CodeScope()
        {
            Dispose(false);
        }

        #endregion

        #region プロテクテッドメソッド

        /// <summary>
        /// 区間の開始時に呼び出されるメソッドです。
        /// </summary>
        protected abstract void OnBegin();

        /// <summary>
        /// 区間の終了時に呼び出されるメソッドです。
        /// </summary>
        protected abstract void OnEnd();

        /// <summary>
        /// オブジェクトに割り当てられたリソースを解放します。
        /// </summary>
        /// <param name="disposing">
        /// <see cref="IDisposable"/>インターフェースにより明示的に解放が指示された場合にはtrue、デストラクタにより暗黙的に解放が指示された場合にはfalseです。
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    OnEnd();
                }
            }
            _disposed = true;
        }

        #endregion

        #region IDisposable のメンバ

        /// <summary>
        /// オブジェクトに割り当てられたリソースを解放します。
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}