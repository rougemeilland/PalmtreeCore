/*
  FileExtensions.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.IO;

namespace Palmtree.IO
{
    /// <summary>
    /// <see cref="FileInfo"/>の拡張メソッドのクラスです。
    /// </summary>
    public static class FileExtensions
    {
        #region パブリックメソッド

        /// <summary>
        /// 与えられた<see cref="FileInfo"/>オブジェクトが示すファイルの内容のハッシュ値を計算します。
        /// </summary>
        /// <param name="info">
        /// ハッシュ値を計算するファイルです。
        /// </param>
        /// <returns>
        /// 計算されたハッシュ値です。
        /// </returns>
        public static string ComputeHash(this FileInfo info)
        {
            return (File.ReadAllBytes(info.FullName).ComputeHashString());
        }

        #endregion
    }
}