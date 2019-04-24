/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Palmtree.Threading
{
    // https://thomaslevesque.com/2015/06/04/async-and-cancellation-support-for-wait-handles/ より改変し転載

    /// <summary>
    /// <see cref="WaitHandle"/>の拡張メソッドのクラスです。
    /// </summary>
    public static class WaitHandleExtensions
    {
        #region パブリックメソッド

        /// <summary>
        /// <see cref="CancellationToken"/>オブジェクトを監視しつつ待機します。
        /// </summary>
        /// <param name="handle">待機対象の<see cref="WaitHandle"/>オブジェクトです。</param>
        /// <param name="cancellation_token">監視対象の<see cref="CancellationToken"/>オブジェクトです。</param>
        /// <returns>オブジェクトがシグナル状態となって復帰したならばtrue、そうではないのならfalseです。</returns>
        /// <exception cref="OperationCanceledException">cancellation_tokenによりキャンセルされました。</exception>
        public static bool WaitOne(this WaitHandle handle, CancellationToken cancellation_token)
        {
            return (handle.WaitOne(Timeout.Infinite, cancellation_token));
        }

        /// <summary>
        /// <see cref="CancellationToken"/>オブジェクトを監視しつ32ビット整数によりミリ秒単位でタイムアウト時間を指定して待機します。
        /// </summary>
        /// <param name="handle">待機対象の<see cref="WaitHandle"/>オブジェクトです。</param>
        /// <param name="timeout">タイムアウト時間を示す<see cref="TimeSpan"/>オブジェクトです。</param>
        /// <param name="cancellation_token">監視対象の<see cref="CancellationToken"/>オブジェクトです。</param>
        /// <returns>オブジェクトがシグナル状態となって復帰したならばtrue、そうではないのならfalseです。</returns>
        /// <exception cref="OperationCanceledException">cancellation_tokenによりキャンセルされました。</exception>
        public static bool WaitOne(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellation_token)
        {
            return (handle.WaitOne((int)timeout.TotalMilliseconds, cancellation_token));
        }

        /// <summary>
        /// <see cref="CancellationToken"/>オブジェクトを監視しつ<see cref="TimeSpan"/>オブジェクトによりタイムアウト時間を指定して待機します。
        /// </summary>
        /// <param name="handle">待機対象の<see cref="WaitHandle"/>オブジェクトです。</param>
        /// <param name="milliseconds_timeout">ミリ秒単位でタイムアウト時間を示す32ビット整数です。</param>
        /// <param name="cancellation_token">監視対象の<see cref="CancellationToken"/>オブジェクトです。</param>
        /// <returns>オブジェクトがシグナル状態となって復帰したならばtrue、そうではないのならfalseです。</returns>
        /// <exception cref="OperationCanceledException">cancellation_tokenによりキャンセルされました。</exception>
        public static bool WaitOne(this WaitHandle handle, int milliseconds_timeout, CancellationToken cancellation_token)
        {
            var n = WaitHandle.WaitAny(new[] { handle, cancellation_token.WaitHandle }, milliseconds_timeout);
            switch (n)
            {
                case WaitHandle.WaitTimeout:
                    return (false);
                case 0:
                    return (true);
                default:
                    cancellation_token.ThrowIfCancellationRequested();
                    return (false); // never reached
            }
        }

        /// <summary>
        /// 非同期に待機します。
        /// </summary>
        /// <param name="handle">待機対象の<see cref="WaitHandle"/>オブジェクトです。</param>
        /// <returns>オブジェクトがシグナル状態になった場合にはtrueで完了するタスク、そうではない場合にはfalseで完了するタスクです。</returns>
        public static Task<bool> WaitOneAsync(this WaitHandle handle)
        {
            return (handle.WaitOneAsync(Timeout.Infinite, CancellationToken.None));
        }

        /// <summary>
        /// <see cref="TimeSpan"/>値をタイムアウト値として非同期に待機します。
        /// </summary>
        /// <param name="handle">待機対象の<see cref="WaitHandle"/>オブジェクトです。</param>
        /// <param name="timeout">待機のタイムアウト値です。</param>
        /// <returns>オブジェクトがシグナル状態になった場合にはtrueで完了するタスク、そうではない場合にはfalseで完了するタスクです。</returns>
        public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout)
        {
            return (handle.WaitOneAsync(timeout, CancellationToken.None));
        }

        /// <summary>
        /// 32ビット整数によりミリ秒単位でタイムアウト時間を指定して非同期に待機します。
        /// </summary>
        /// <param name="handle">待機対象の<see cref="WaitHandle"/>オブジェクトです。</param>
        /// <param name="milliseconds_timeout">ミリ秒単位でタイムアウト時間を示す32ビット整数です。</param>
        /// <returns>オブジェクトがシグナル状態になった場合にはtrueで完了するタスク、そうではない場合にはfalseで完了するタスクです。</returns>
        public static Task<bool> WaitOneAsync(this WaitHandle handle, int milliseconds_timeout)
        {
            return (handle.WaitOneAsync(milliseconds_timeout, CancellationToken.None));
        }

        /// <summary>
        /// <see cref="CancellationToken"/>オブジェクトを監視しつつ非同期に待機します。
        /// </summary>
        /// <param name="handle">待機対象の<see cref="WaitHandle"/>オブジェクトです。</param>
        /// <param name="cancellation_token">監視する<see cref="CancellationToken"/>オブジェクトです。</param>
        /// <returns>オブジェクトがシグナル状態になった場合にはtrueで完了するタスク、そうではない場合にはfalseで完了するタスクです。</returns>
        /// <exception cref="OperationCanceledException">cancellation_tokenがキャンセルされました。</exception>
        public static Task<bool> WaitOneAsync(this WaitHandle handle, CancellationToken cancellation_token)
        {
            return (handle.WaitOneAsync(Timeout.Infinite, cancellation_token));
        }

        /// <summary>
        /// <see cref="CancellationToken"/>オブジェクトを監視しつつ32ビット整数によりミリ秒単位でタイムアウト時間を指定して非同期に待機します。
        /// </summary>
        /// <param name="handle">待機対象の<see cref="WaitHandle"/>オブジェクトです。</param>
        /// <param name="timeout">待機のタイムアウト値です。</param>
        /// <param name="cancellation_token">監視する<see cref="CancellationToken"/>オブジェクトです。</param>
        /// <returns>オブジェクトがシグナル状態になった場合にはtrueで完了するタスク、そうではない場合にはfalseで完了するタスクです。</returns>
        /// <exception cref="OperationCanceledException">cancellation_tokenがキャンセルされました。</exception>
        public static Task<bool> WaitOneAsync(this WaitHandle handle, TimeSpan timeout, CancellationToken cancellation_token)
        {
            return (handle.WaitOneAsync((int)timeout.TotalMilliseconds, cancellation_token));
        }

        /// <summary>
        /// <see cref="CancellationToken"/>オブジェクトを監視しつつ<see cref="TimeSpan"/>値をタイムアウト値として非同期に待機します。
        /// </summary>
        /// <param name="handle">待機対象の<see cref="WaitHandle"/>オブジェクトです。</param>
        /// <param name="milliseconds_timeout">ミリ秒単位でタイムアウト時間を示す32ビット整数です。</param>
        /// <param name="cancellation_token">監視する<see cref="CancellationToken"/>オブジェクトです。</param>
        /// <returns>オブジェクトがシグナル状態になった場合にはtrueで完了するタスク、そうではない場合にはfalseで完了するタスクです。</returns>
        /// <exception cref="OperationCanceledException">cancellation_tokenがキャンセルされました。</exception>
        public static async Task<bool> WaitOneAsync(this WaitHandle handle, int milliseconds_timeout, CancellationToken cancellation_token)
        {
            RegisteredWaitHandle registeredHandle = null;
            CancellationTokenRegistration tokenRegistration = default(CancellationTokenRegistration);
            try
            {
                var tcs = new TaskCompletionSource<bool>();
                registeredHandle = ThreadPool.RegisterWaitForSingleObject(handle, (state, timed_out) => ((TaskCompletionSource<bool>)state).TrySetResult(!timed_out), tcs, milliseconds_timeout, true);
                tokenRegistration = cancellation_token.Register(state => ((TaskCompletionSource<bool>)state).TrySetCanceled(), tcs);
                return await tcs.Task;
            }
            finally
            {
                if (registeredHandle != null)
                    registeredHandle.Unregister(null);
                tokenRegistration.Dispose();
            }
        }

        #endregion
    }
}
