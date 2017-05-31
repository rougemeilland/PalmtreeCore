/*
  BASE32Extensions.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Palmtree
{
    /// <summary>
    /// データの変換を行う拡張メソッドのクラスです。
    /// </summary>
    public static class BASE32Extensions
    {
        #region プライベートフィールド

        private static char[] _digits;
        private static Dictionary<char, int> _keyed_digits;

        #endregion

        #region コンストラクタ

        static BASE32Extensions()
        {
            _digits = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', };
            Debug.Assert(_digits.Length == 32);
            _keyed_digits = new Dictionary<char, int>();
            for (int index = 0; index < _digits.Length; ++index)
            {
                char c = _digits[index];
                _keyed_digits.Add(c, index);
                char upper_c = char.ToUpperInvariant(c);
                if (upper_c != c)
                    _keyed_digits.Add(upper_c, index);
            }
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// バイト配列をBASE32形式の文字列に変換します。
        /// </summary>
        /// <param name="data">
        /// 変換するバイト配列です。
        /// </param>
        /// <returns>
        /// 変換されたBASE32形式の文字列です。
        /// </returns>
        public static string ToBase32String(this byte[] data)
        {
            byte[] in_data = data;
            char[] out_data = new char[(in_data.Length + 4) / 5 * 8];
            for (int in_index = 0, out_index = 0; in_index < in_data.Length; in_index += 5, out_index += 8)
                ConvertWordToBase32(in_data, in_index, out_data, out_index);
            return (new string(out_data));
        }

        /// <summary>
        /// BASE32形式の文字列をバイト配列に変換します。
        /// </summary>
        /// <param name="s">
        /// 変換するBASE32形式の文字列です。
        /// </param>
        /// <returns>
        /// 変換されたバイト配列です。
        /// </returns>
        public static byte[] FromBase32String(this string s)
        {
            char[] in_data = s.ToCharArray();
            byte[] out_data = new byte[(in_data.Length + 7) / 8 * 5];
            int length = 0;
            for (int in_index = 0, out_index = 0; in_index < in_data.Length; in_index += 8, out_index += 5)
                length = ConvertWordFromBase32(in_data, in_index, out_data, out_index);
            Array.Resize<byte>(ref out_data, length);
            return (out_data);
        }

        #endregion

        #region プライベートメソッド

        private static void ConvertWordToBase32(byte[] in_data, int in_index, char[] out_data, int out_index)
        {
            int copy_length = in_data.Length - in_index;
            Debug.Assert(copy_length > 0);
            if (copy_length >= 5)
            {
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 3)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 2) | (in_data[in_index] >> 6)) & 0x1f];
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 1)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 4) | (in_data[in_index] >> 4)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 1) | (in_data[in_index] >> 7)) & 0x1f];
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 2)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 3) | (in_data[in_index] >> 5)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++]) /*                          */ ) & 0x1f];
            }
            else if (copy_length == 4)
            {
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 3)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 2) | (in_data[in_index] >> 6)) & 0x1f];
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 1)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 4) | (in_data[in_index] >> 4)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 1) | (in_data[in_index] >> 7)) & 0x1f];
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 2)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 3) /*                      */) & 0x1f];
                out_data[out_index++] = '0';
            }
            else if (copy_length == 3)
            {
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 3)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 2) | (in_data[in_index] >> 6)) & 0x1f];
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 1)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 4) | (in_data[in_index] >> 4)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 1) /*                      */) & 0x1f];
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
            }
            else if (copy_length == 2)
            {
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 3)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 2) | (in_data[in_index] >> 6)) & 0x1f];
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 1)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 4) /*                      */) & 0x1f];
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
            }
            else if (copy_length == 1)
            {
                out_data[out_index++] = _digits[( /*                       */ (in_data[in_index] >> 3)) & 0x1f];
                out_data[out_index++] = _digits[((in_data[in_index++] << 2) /*                      */) & 0x1f];
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
                out_data[out_index++] = '0';
            }
        }

        private static int ConvertWordFromBase32(char[] in_data, int in_index, byte[] out_data, int out_index)
        {
            int in_data_0 = GetValueFromBase32(in_data[in_index++], true);
            int in_data_1 = GetValueFromBase32(in_data[in_index++], true);
            out_data[out_index++] = (byte)((in_data_0 << 3) | (in_data_1 >> 2));
            int in_data_2 = GetValueFromBase32(in_data[in_index++], false);
            if (in_data_2 < 0)
                return (out_index);
            int in_data_3 = GetValueFromBase32(in_data[in_index++], true);
            out_data[out_index++] = (byte)((in_data_1 << 6) | (in_data_2 << 1) | (in_data_3 >> 4));
            int in_data_4 = GetValueFromBase32(in_data[in_index++], false);
            if (in_data_4 < 0)
                return (out_index);
            out_data[out_index++] = (byte)((in_data_3 << 4) | (in_data_4 >> 1));
            int in_data_5 = GetValueFromBase32(in_data[in_index++], false);
            if (in_data_5 < 0)
                return (out_index);
            int in_data_6 = GetValueFromBase32(in_data[in_index++], true);
            out_data[out_index++] = (byte)((in_data_4 << 7) | (in_data_5 << 2) | (in_data_6 >> 3));
            int in_data_7 = GetValueFromBase32(in_data[in_index++], false);
            if (in_data_7 < 0)
                return (out_index);
            out_data[out_index++] = (byte)((in_data_6 << 5) | in_data_7);
            return (out_index);
        }

        private static int GetValueFromBase32(char in_data, bool exception_on_zero)
        {
            if (in_data == '0')
            {
                if (exception_on_zero)
                    throw (new FormatException());
                else
                    return (-1);
            }
            if (!_keyed_digits.ContainsKey(in_data))
                throw (new FormatException());
            return (_keyed_digits[in_data]);
        }

        #endregion
    }
}