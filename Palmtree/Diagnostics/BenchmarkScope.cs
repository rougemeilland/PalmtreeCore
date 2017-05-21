/*
  BenchmarkScope.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;

namespace Palmtree.Diagnostics
{
    /// <summary>
    /// 指定されたコードの区間の所要時間を測定するためのヘルパクラスです。
    /// </summary>
    /// <example>
    /// BenchmarkCounterCollection counters = new BenchmarkCounterCollection();
    /// 
    /// ...
    /// 
    /// using (BenchmarkScope.Begin(counters[location]))
    /// {
    ///     (測定対象のコード)
    /// }
    /// </example>
    public class BenchmarkScope
        : CodeScope
    {
        #region DummyCounter の定義

        private class DummyCounter
            : IDisposable
        {
            #region IDisposable のメンバ

            void IDisposable.Dispose()
            {
            }

            #endregion
        }

        #endregion

        #region プライベートフィールド

        private static DummyCounter _dummy_counter;
        private BenchmarkCounter _counter;

        #endregion

        #region コンストラクタ

        static BenchmarkScope()
        {
            _dummy_counter = new DummyCounter();
        }

        private BenchmarkScope(BenchmarkCounter counter)
        {
            _counter = counter;
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 測定開始区間の最初にusingとともに呼び出すメソッドです。
        /// </summary>
        /// <param name="counter">
        /// 測定に使用する<see cref="BenchmarkCounter"/>オブジェクトです。
        /// </param>
        /// <returns>
        /// <see cref="IDisposable"/>オブジェクトです。
        /// このオブジェクトに対しDisposeが呼び出されると測定が停止します。
        /// </returns>
        public static IDisposable Begin(BenchmarkCounter counter)
        {
            if (counter == null)
                return (_dummy_counter);
            else
            {
                BenchmarkScope scope = new BenchmarkScope(counter);
                scope.OnBegin();
                return (scope);
            }
        }

        #endregion

        #region プロテクテッドメソッド

        /// <summary>
        /// 区間の開始時に呼び出されるメソッドです。
        /// </summary>
        protected override void OnBegin()
        {
            _counter.Start();
        }

        /// <summary>
        /// 区間の終了時に呼び出されるメソッドです。
        /// </summary>
        protected override void OnEnd()
        {
            _counter.Stop();
        }

        #endregion
    }
}