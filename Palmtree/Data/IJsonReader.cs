/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

namespace Palmtree.Data
{
    /// <summary>
    /// JSONデシリアライザの入力Streamのインターフェースです。
    /// </summary>
    internal interface IJsonReader
    {
        /// <summary>
        /// 入力から1文字読み込みます。
        /// </summary>
        /// <returns>
        /// 読み込んだ文字列です。
        /// </returns>
        char ReadChar();

        /// <summary>
        /// 入力から指定文字数を文字列として読み込みます。
        /// </summary>
        /// <param name="length">
        /// 読み込む文字数です。
        /// </param>
        /// <returns>
        /// 読み込んだ文字列です。
        /// </returns>
        string ReadStr(int length);

        /// <summary>
        /// 入力からストリームを進めることなく1文字読み込みます。
        /// </summary>
        /// <returns>
        /// 読み込んだ文字です。
        /// </returns>
        char Peek();

        /// <summary>
        /// ストリームが終端に達しているかどうか調べます。
        /// </summary>
        bool IsEnd { get; }
    }
}
