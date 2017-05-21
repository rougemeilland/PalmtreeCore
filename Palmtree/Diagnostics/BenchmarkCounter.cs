/*
  BenchmarkCounter.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Diagnostics;

namespace Palmtree.Diagnostics
{
    /// <summary>
    /// 指定されたコードの区間の所要時間を測定するオブジェクトのクラスです。
    /// </summary>
    public class BenchmarkCounter
    {
        #region プライベートフィールド

        private Stopwatch _stop_watch;
        private TimeSpan? _elapsed_cache;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="location">
        /// オブジェクトの名前です。
        /// この名前は一意である必要があります。
        /// </param>
        public BenchmarkCounter(string location)
        {
            _stop_watch = new Stopwatch();
            _elapsed_cache = null;
            Location = location;
            Count = 0;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 所要時間のカウントを開始または再開します。
        /// </summary>
        public void Start()
        {
            _elapsed_cache = null;
            _stop_watch.Start();
        }

        /// <summary>
        /// 所要時間のカウントを停止します。
        /// </summary>
        public void Stop()
        {
            _stop_watch.Stop();
            ++Count;
        }

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// オブジェクトの名前です。
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// 所要時間のカウントが行なわれた回数です。
        /// </summary>
        public int Count { get; private set; }


        /// <summary>
        /// 所要時間の累計です。
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                if (!_elapsed_cache.HasValue)
                {
                    if (_stop_watch.IsRunning)
                        throw (new InvalidOperationException());
                    _elapsed_cache = _stop_watch.Elapsed;
                }
                return (_elapsed_cache.Value);
            }
        }

        #endregion
    }
}