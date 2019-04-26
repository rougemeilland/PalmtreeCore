/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

namespace Palmtree.Data
{
    /// <summary>
    /// JSONシリアライザの出力Streamのインターフェースです。
    /// </summary>
    internal interface IJsonWriter
    {
        /// <summary>
        /// 文字を出力します。
        /// </summary>
        /// <param name="c">
        /// 出力する文字です。
        /// </param>
        void Write(char c);

        /// <summary>
        /// 文字列を出力します。
        /// </summary>
        /// <param name="s">
        /// 出力する文字列です。
        /// </param>
        void Write(string s);
    }
}
