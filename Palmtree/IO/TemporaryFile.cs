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
    /// 一意の名前を持つ一時ファイル名を管理するクラスです。
    /// </summary>
    public class TemporaryFile
        : IDisposable
    {
        #region プライベートフィールド

        private static object _lockobj = new object();
        private bool _disposed;
        private string _tempfile;


        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public TemporaryFile()
        {
            _disposed = false;
            _tempfile = Path.GetTempFileName();
        }

        /// <summary>
        /// デストラクタです。
        /// </summary>
        ~TemporaryFile()
        {
            // 以下でc#の管理下にない資源を解放する(Disposeを呼び出すだけ)
            // 他のC#オブジェクトが既に先に解放されている可能性があるので、他のC#オブジェクトに関連した解放処理は行ってはならない
            Dispose(false);
        }

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// 一時ファイルのパス名を取得します。
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

        #region IDisposable のメンバ

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
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

        #region プロテクテッドメソッド

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">
        /// マネージ リソースが破棄される場合 true、破棄されない場合は false です。
        /// </param>
        protected void Dispose(bool disposing)
        {
            lock (_lockobj)
            {
                if (!this._disposed)
                {
                    if (disposing)
                    {
                        // C#の管理下にある資源を解放する
                    }
                    // 以下でc#の管理下にない資源を解放する
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