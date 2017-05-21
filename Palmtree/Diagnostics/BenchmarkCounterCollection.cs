/*
  BenchmarkCounterCollection.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Collections.Generic;

namespace Palmtree.Diagnostics
{
    /// <summary>
    /// <see cref="BenchmarkCounter"/>オブジェクトのコレクションのクラスです。
    /// </summary>
    public class BenchmarkCounterCollection
    {
        #region プライベートフィールド

        private Dictionary<string, BenchmarkCounter> _counters;

        #endregion

        #region コンストラクタ

        /// <summary>
        /// デフォルトコンストラクタです。
        /// </summary>
        public BenchmarkCounterCollection()
        {
            _counters = new Dictionary<string, BenchmarkCounter>();
        }

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// 名前に対応する<see cref="BenchmarkCounter"/>オブジェクトです。
        /// もし名前に一致するオブジェクトが存在しなかった場合には新規に<see cref="BenchmarkCounter"/>オブジェクトが作成されます。
        /// </summary>
        /// <param name="location">
        /// <see cref="BenchmarkCounter"/>オブジェクトの名前です。
        /// </param>
        /// <returns>
        /// 見つかったあるいは作成された<see cref="BenchmarkCounter"/>オブジェクトです。
        /// </returns>
        /// <remarks>
        /// 測定対象のコードの区間を特定できる文字列を名前を<see cref="BenchmarkCounter"/>オブジェクトの名前として使用すると、区間毎に所要時間の累計をカウントするようにできます。
        /// </remarks>
        public BenchmarkCounter this[string location]
        {
            get
            {
                BenchmarkCounter found;
                if (!_counters.TryGetValue(location, out found))
                {
                    found = new BenchmarkCounter(location);
                    _counters.Add(location, found);
                }
                return (found);
            }
        }

        /// <summary>
        /// <see cref="BenchmarkCounter"/>オブジェクトのコレクションです。
        /// </summary>
        public ICollection<BenchmarkCounter> Items
        {
            get
            {
                return (_counters.Values);
            }
        }

        #endregion
    }
}