/*
  StringExtensions.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Palmtree
{
    /// <summary>
    /// ひらがな/カタカナ変換で指定されるオプション動作の列挙体です。
    /// </summary>
    [Flags]
    public enum HiraganaKatakanaConversionSpesicication
    {
        /// <summary>
        /// 指定なし。
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 「ゔ」をカタカナ「ヴ」のひらがなとみなします。
        /// </summary>
        ConvertLetterNotInCommonUse = 0x01,
    }

    /// <summary>
    /// 文字列の拡張メソッドのクラスです。
    /// </summary>
    public static class StringExtensions
    {
        #region プライベートフィールド

        private const string _hash_encoding_table = "0123456789abcdefghijklmnopqrstuv";
        private static IDictionary<string, string> _hiragana_string_by_katakana;
        private static IDictionary<string, string> _katakana_string_by_hiragana;
        private static IDictionary<string, string> _additional_hiragana_string_by_katakana;
        private static IDictionary<string, string> _additional_katakana_string_by_hiragana;

        #endregion

        #region コンストラクタ

        static StringExtensions()
        {
            _hiragana_string_by_katakana = new Dictionary<string, string>();
            _katakana_string_by_hiragana = new Dictionary<string, string>();
            _additional_hiragana_string_by_katakana = new Dictionary<string, string>();
            _additional_katakana_string_by_hiragana = new Dictionary<string, string>();
            var raw_data = new[]
            {
                new { hiragana = "あ", katakana = "ア" }, new { hiragana = "い", katakana = "イ" }, new { hiragana = "う", katakana = "ウ" }, new { hiragana = "え", katakana = "エ" }, new { hiragana = "お", katakana = "オ" },
                new { hiragana = "ぁ", katakana = "ァ" }, new { hiragana = "ぃ", katakana = "ィ" }, new { hiragana = "ぅ", katakana = "ゥ" }, new { hiragana = "ぇ", katakana = "ェ" }, new { hiragana = "ぉ", katakana = "ォ" },
                new { hiragana = "か", katakana = "カ" }, new { hiragana = "き", katakana = "キ" }, new { hiragana = "く", katakana = "ク" }, new { hiragana = "け", katakana = "ケ" }, new { hiragana = "こ", katakana = "コ" },
                new { hiragana = "が", katakana = "ガ" }, new { hiragana = "ぎ", katakana = "ギ" }, new { hiragana = "ぐ", katakana = "グ" }, new { hiragana = "げ", katakana = "ゲ" }, new { hiragana = "ご", katakana = "ゴ" },
                new { hiragana = "さ", katakana = "サ" }, new { hiragana = "し", katakana = "シ" }, new { hiragana = "す", katakana = "ス" }, new { hiragana = "せ", katakana = "セ" }, new { hiragana = "そ", katakana = "ソ" },
                new { hiragana = "ざ", katakana = "ザ" }, new { hiragana = "じ", katakana = "ジ" }, new { hiragana = "ず", katakana = "ズ" }, new { hiragana = "ぜ", katakana = "ゼ" }, new { hiragana = "ぞ", katakana = "ゾ" },
                new { hiragana = "た", katakana = "タ" }, new { hiragana = "ち", katakana = "チ" }, new { hiragana = "つ", katakana = "ツ" }, new { hiragana = "て", katakana = "テ" }, new { hiragana = "と", katakana = "ト" },
                new { hiragana = "だ", katakana = "ダ" }, new { hiragana = "ぢ", katakana = "ヂ" }, new { hiragana = "づ", katakana = "ヅ" }, new { hiragana = "で", katakana = "デ" }, new { hiragana = "ど", katakana = "ド" },
                new { hiragana = "っ", katakana = "ッ" },
                new { hiragana = "な", katakana = "ナ" }, new { hiragana = "に", katakana = "ニ" }, new { hiragana = "ぬ", katakana = "ヌ" }, new { hiragana = "ね", katakana = "ネ" }, new { hiragana = "の", katakana = "ノ" },
                new { hiragana = "は", katakana = "ハ" }, new { hiragana = "ひ", katakana = "ヒ" }, new { hiragana = "ふ", katakana = "フ" }, new { hiragana = "へ", katakana = "ヘ" }, new { hiragana = "ほ", katakana = "ホ" },
                new { hiragana = "ば", katakana = "バ" }, new { hiragana = "び", katakana = "ビ" }, new { hiragana = "ぶ", katakana = "ブ" }, new { hiragana = "べ", katakana = "ベ" }, new { hiragana = "ぼ", katakana = "ボ" },
                new { hiragana = "ぱ", katakana = "パ" }, new { hiragana = "ぴ", katakana = "ピ" }, new { hiragana = "ぷ", katakana = "プ" }, new { hiragana = "ぺ", katakana = "ペ" }, new { hiragana = "ぽ", katakana = "ポ" },
                new { hiragana = "ま", katakana = "マ" }, new { hiragana = "み", katakana = "ミ" }, new { hiragana = "む", katakana = "ム" }, new { hiragana = "め", katakana = "メ" }, new { hiragana = "も", katakana = "モ" },
                new { hiragana = "や", katakana = "ヤ" }, new { hiragana = "ゆ", katakana = "ユ" }, new { hiragana = "よ", katakana = "ヨ" },
                new { hiragana = "ゃ", katakana = "ャ" }, new { hiragana = "ゅ", katakana = "ュ" }, new { hiragana = "ょ", katakana = "ョ" },
                new { hiragana = "ら", katakana = "ラ" }, new { hiragana = "り", katakana = "リ" }, new { hiragana = "る", katakana = "ル" }, new { hiragana = "れ", katakana = "レ" }, new { hiragana = "ろ", katakana = "ロ" },
                new { hiragana = "わ", katakana = "ワ" }, new { hiragana = "ゐ", katakana = "ヰ" }, new { hiragana = "ゑ", katakana = "ヱ" }, new { hiragana = "を", katakana = "ヲ" },
                new { hiragana = "ん", katakana = "ン" },
                new { hiragana = "ゝ", katakana = "ヽ" }, new { hiragana = "ゞ", katakana = "ヾ" },
            };
            foreach (var item in raw_data)
            {
                _hiragana_string_by_katakana.Add(item.katakana, item.hiragana);
                _katakana_string_by_hiragana.Add(item.hiragana, item.katakana);
            }
            _additional_hiragana_string_by_katakana.Add("ヴ", "ゔ");
            _additional_katakana_string_by_hiragana.Add("ゔ", "ヴ");
#if DEBUG
            if (_hiragana_string_by_katakana.Keys.Where(c => c.Length != 1 || c[0] < 0x30a1 || c[0] > 0x30ff).Any())
                throw new ApplicationException("_hiragana_string_by_katakana.Keysにカタカナでない文字があります。");
            if (_hiragana_string_by_katakana.Values.Where(c => c.Length != 1 || c[0] < 0x3041 || c[0] > 0x309f).Any())
                throw new ApplicationException("_hiragana_string_by_katakana.Valuesにひらがなでない文字があります。");
            if (_katakana_string_by_hiragana.Keys.Where(c => c.Length != 1 || c[0] < 0x3041 || c[0] > 0x309f).Any())
                throw new ApplicationException("_katakana_string_by_hiragana.Keysにひらがなでない文字があります。");
            if (_katakana_string_by_hiragana.Values.Where(c => c.Length != 1 || c[0] < 0x30a1 || c[0] > 0x30ff).Any())
                throw new ApplicationException("_katakana_string_by_hiragana.Valuesにカタカナでない文字があります。");
            if (_additional_hiragana_string_by_katakana.Keys.Where(c => c.Length != 1 || c[0] < 0x30a1 || c[0] > 0x30ff).Any())
                throw new ApplicationException("_additional_hiragana_string_by_katakana.Keysにカタカナでない文字があります。");
            if (_additional_hiragana_string_by_katakana.Values.Where(c => c.Length != 1 || c[0] < 0x3041 || c[0] > 0x309f).Any())
                throw new ApplicationException("_additional_hiragana_string_by_katakana.Valuesにひらがなでない文字があります。");
            if (_additional_katakana_string_by_hiragana.Keys.Where(c => c.Length != 1 || c[0] < 0x3041 || c[0] > 0x309f).Any())
                throw new ApplicationException("_additional_katakana_string_by_hiragana.Keysにひらがなでない文字があります。");
            if (_additional_katakana_string_by_hiragana.Values.Where(c => c.Length != 1 || c[0] < 0x30a1 || c[0] > 0x30ff).Any())
                throw new ApplicationException("_additional_katakana_string_by_hiragana.Valuesにカタカナでない文字があります。");
            var dic = new Dictionary<string, string>();
            try
            {
                foreach (var item in _hiragana_string_by_katakana)
                {
                    dic.Add(item.Key, item.Key);
                    dic.Add(item.Value, item.Value);
                }
                foreach (var item in _additional_hiragana_string_by_katakana)
                {
                    dic.Add(item.Key, item.Key);
                    dic.Add(item.Value, item.Value);
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("_hiragana_string_by_katakanaまたは_additional_hiragana_string_by_katakanaの文字が重複しています。");
            }
            dic.Clear();
            try
            {
                foreach (var item in _katakana_string_by_hiragana)
                {
                    dic.Add(item.Key, item.Key);
                    dic.Add(item.Value, item.Value);
                }
                foreach (var item in _additional_katakana_string_by_hiragana)
                {
                    dic.Add(item.Key, item.Key);
                    dic.Add(item.Value, item.Value);
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("_katakana_string_by_hiraganaまたは_additional_katakana_string_by_hiraganaの文字が重複しています。");
            }
#endif
        }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 文字列をCSVに埋め込める形式に変換します。
        /// ダブルクォートやカンマ、その他の特殊文字はHTMLエンコードされます。
        /// </summary>
        /// <param name="text">変換元の文字列です。
        /// </param>
        /// <returns>
        /// 変換先の文字列です。
        /// </returns>
        public static string CsvEncode(this string text)
        {
            if (text == null)
                return (null);
            return (WebUtility.HtmlEncode(text).Replace(",", "&#x2c;").Replace("//", "&#x2f;&#x2f;"));
        }

        /// <summary>
        /// CSVに埋め込まれていた文字を復元します。
        /// HTMLエスケープ文字列も復元されます。
        /// </summary>
        /// <param name="text">
        /// 変換元の文字列です。
        /// </param>
        /// <returns>
        /// 変換先の文字列です。
        /// </returns>
        public static string CsvDecode(this string text)
        {
            if (text == null)
                return (null);
            return (WebUtility.HtmlDecode(text));
        }

        /// <summary>
        /// 与えられたバイト配列のハッシュ(SHA512)を計算します。
        /// </summary>
        /// <param name="data">
        /// ハッシュを計算するバイト配列です。
        /// </param>
        /// <returns>
        /// 計算されたハッシュ値を表すバイト配列です。
        /// </returns>
        public static byte[] ComputeHashBytes(this byte[] data)
        {
            using (var engine = SHA512.Create())
            {
                return (engine.ComputeHash(data));
            }
        }

        /// <summary>
        /// ハッシュを示す与えられたバイト配列を与えられた長さに圧縮します。
        /// </summary>
        /// <param name="data">
        /// ハッシュを示すバイト配列です。
        /// </param>
        /// <param name="length">
        /// 圧縮する長さです。
        /// </param>
        /// <returns>
        /// 圧縮されたバイト配列です。この配列の長さは必ずlengthになります。
        /// </returns>
        public static byte[] CompressHashBytes(this byte[] data, int length)
        {
            if (length <= 0)
                throw new ArgumentException();
            byte[] result = new byte[length];
            foreach (var item in data.Select((b, index) => new { b, index }))
                result[item.index % length] ^= item.b;
            return (result);
        }

        /// <summary>
        /// 与えられたバイト列を文字列にエンコードします。
        /// このエンコード方式では5bitが英小文字または数字の1文字に変換されます。
        /// </summary>
        /// <param name="data">
        /// エンコードするバイト列です。
        /// </param>
        /// <returns>
        /// エンコードされた文字列です。
        /// </returns>
        public static string EncodeFromHashBytes(this byte[] data)
        {
            var result = new List<string>();
            int index = 0;
            while (index + 5 <= data.Length)
            {
                result.Add(EncodeHashBytesImp(data, index));
                index += 5;
            }
            if (index < data.Length)
                result.Add(EncodeHashBytesImp(data
                                              .Skip(index)
                                              .Concat(Enumerable.Repeat((byte)0, 4))
                                              .Take(5)
                                              .ToArray(),
                                              0));
            return (string.Concat(result));
        }

        /// <summary>
        /// EncodeFromHashBytesでエンコードされた文字列をデコードします。
        /// </summary>
        /// <param name="data">
        /// エンコードされた文字列です。
        /// </param>
        /// <returns>
        /// デコードされたバイト列です。
        /// </returns>
        public static byte[] DecodeToHashBytes(this string data)
        {
            if (data.Any(c => !_hash_encoding_table.Contains(c)))
                throw new ArgumentException();
            var result = new List<byte>();
            int index = 0;
            while (index + 8 <= data.Length)
            {
                result.AddRange(DecodeHashBytesImp(data, index));
                index += 8;
            }
            if (index < data.Length)
                result.AddRange(DecodeHashBytesImp((data.Substring(index) + new string('0', 7)).Substring(0, 8), 0));
            return (result.ToArray());
        }

        /// <summary>
        /// 与えられたバイト配列のハッシュ(SHA512)を計算します。
        /// </summary>
        /// <param name="data">
        /// ハッシュを計算するバイト配列です。
        /// </param>
        /// <returns>
        /// 計算されたハッシュ値を表す文字列です。
        /// </returns>
        public static string ComputeHashString(this byte[] data)
        {
            return (string.Concat(data.ComputeHashBytes().Select(b => b.ToString("X2"))));
        }

        /// <summary>
        /// 与えられた文字列のハッシュ(SHA512)を計算します。
        /// </summary>
        /// <param name="text">
        /// ハッシュを計算する文字列です。文字コードはUTF8として扱われます。
        /// </param>
        /// <returns>
        /// 計算されたハッシュ値を表す文字列です。
        /// </returns>
        public static string ComputeHashString(this string text)
        {
            return (Encoding.UTF8.GetBytes(text).ComputeHashString());
        }

        /// <summary>
        /// 与えられた文字列に含まれているカタカナをひらがなに置換した文字列を返します。
        /// </summary>
        /// <param name="s">
        /// 変換元の文字列です。
        /// </param>
        /// <param name="spec">
        /// 変換時に適用されるオプション動作のフラグです。
        /// </param>
        /// <returns>
        /// 変換された文字列です。
        /// </returns>
        public static string ToHiragana(this string s, HiraganaKatakanaConversionSpesicication spec = HiraganaKatakanaConversionSpesicication.None)
        {
            return (string.Concat(s.Select(c => ToHiragana(c, spec))));
        }

        /// <summary>
        /// 与えられた文字列に含まれているひらがなをカタカナに置換した文字列を返します。
        /// </summary>
        /// <param name="s">
        /// 変換元の文字列です。
        /// </param>
        /// <param name="spec">
        /// 変換時に適用されるオプション動作のフラグです。
        /// </param>
        /// <returns>
        /// 変換された文字列です。
        /// </returns>
        public static string ToKatakana(this string s, HiraganaKatakanaConversionSpesicication spec = HiraganaKatakanaConversionSpesicication.None)
        {
            return (string.Concat(s.Select(c => ToKatakana(c, spec))));
        }

        /// <summary>
        /// 与えられた文字列がすべてひらがなから構成されているかどうかを調べます。
        /// </summary>
        /// <param name="s">
        /// 調べる対象の文字列です。
        /// </param>
        /// <param name="spec">
        /// 調べる際に適用されるオプション動作のフラグです。
        /// </param>
        /// <returns>
        /// 与えられた文字列に含まれる文字がすべてひらがなであればtrue、そうではないのならfalseです。
        /// </returns>
        public static bool IsHiragana(this string s, HiraganaKatakanaConversionSpesicication spec = HiraganaKatakanaConversionSpesicication.None)
        {
            return (s.All(c =>
            {
                if (_katakana_string_by_hiragana.ContainsKey(s))
                    return (true);
                if ((spec & HiraganaKatakanaConversionSpesicication.ConvertLetterNotInCommonUse) != HiraganaKatakanaConversionSpesicication.None && _additional_katakana_string_by_hiragana.ContainsKey(s))
                    return (true);
                return (false);
            }));
        }

        /// <summary>
        /// 与えられた文字列がすべてカタカナから構成されているかどうかを調べます。
        /// </summary>
        /// <param name="s">
        /// 調べる対象の文字列です。
        /// </param>
        /// <param name="spec">
        /// 調べる際に適用されるオプション動作のフラグです。
        /// </param>
        /// <returns>
        /// 与えられた文字列に含まれる文字がすべてカタカナであればtrue、そうではないのならfalseです。
        /// </returns>
        public static bool IsKatakana(this string s, HiraganaKatakanaConversionSpesicication spec = HiraganaKatakanaConversionSpesicication.None)
        {
            return (s.All(c =>
            {
                if (_hiragana_string_by_katakana.ContainsKey(s))
                    return (true);
                if ((spec & HiraganaKatakanaConversionSpesicication.ConvertLetterNotInCommonUse) != HiraganaKatakanaConversionSpesicication.None && _additional_hiragana_string_by_katakana.ContainsKey(s))
                    return (true);
                return (false);
            }));
        }

        #endregion

        #region プライベートメソッド

        private static string ToHiragana(char c, HiraganaKatakanaConversionSpesicication spec)
        {
            string result;
            if (_hiragana_string_by_katakana.TryGetValue(c.ToString(), out result))
                return (result);
            else if ((spec & HiraganaKatakanaConversionSpesicication.ConvertLetterNotInCommonUse) != HiraganaKatakanaConversionSpesicication.None && _additional_hiragana_string_by_katakana.TryGetValue(c.ToString(), out result))
                return (result);
            else
                return (c.ToString());
        }

        private static string ToKatakana(char c, HiraganaKatakanaConversionSpesicication spec)
        {
            string result;
            if (_katakana_string_by_hiragana.TryGetValue(c.ToString(), out result))
                return (result);
            else if ((spec & HiraganaKatakanaConversionSpesicication.ConvertLetterNotInCommonUse) != HiraganaKatakanaConversionSpesicication.None && _additional_katakana_string_by_hiragana.TryGetValue(c.ToString(), out result))
                return (result);
            else
                return (c.ToString());
        }

        private static string EncodeHashBytesImp(byte[] data, int index)
        {
            if (index + 4 >= data.Length)
                throw new ArgumentException();
            var d1 = (data[index + 0] >> 3) & 0x1f;                         // data0の上位5ビット(ooooo***)
            var d2 = (data[index + 0] << 2 | data[index + 1] >> 6) & 0x1f;  // data0の下位3ビット(*****ooo)とdata1の上位2ビット(oo******)
            var d3 = (data[index + 1] >> 1) & 0x1f;                         // data1の1-5ビット(**ooooo*)
            var d4 = (data[index + 1] << 4 | data[index + 2] >> 4) & 0x1f;  // data1の下位1ビット(*******o)とdata2の上位4ビット(oooo****)
            var d5 = (data[index + 2] << 1 | data[index + 3] >> 7) & 0x1f;  // data2の下位4ビット(****oooo)とdata3の上位1ビット(o*******)
            var d6 = (data[index + 3] >> 2) & 0x1f;                         // data3の2-6ビット(*ooooo**)
            var d7 = (data[index + 3] << 3 | data[index + 4] >> 5) & 0x1f;  // data3の下位2ビット(******oo)とdata4の上位3ビット(ooo*****)
            var d8 = data[index + 4] & 0x1f;                                // data4の下位5ビット(***ooooo)
            return (string.Concat(new[] { (byte)d1, (byte)d2, (byte)d3, (byte)d4, (byte)d5, (byte)d6, (byte)d7, (byte)d8 }.Select(d => _hash_encoding_table[d])));
        }

        private static byte[] DecodeHashBytesImp(string s, int index)
        {
            if (index + 7 >= s.Length)
                throw new ArgumentException();
            var data = new[] { s[index + 0], s[index + 1], s[index + 2], s[index + 3], s[index + 4], s[index + 5], s[index + 6], s[index + 7] }
                       .Select(c => DecodeChar(c))
                       .ToArray();
            if (data.Any(d => d >= 32))
                throw new ArgumentException();
            var d1 = data[0] << 3 | data[1] >> 2;                   // d1 (00000111)
            var d2 = data[1] << 6 | data[2] << 1 | data[3] >> 4;    // d2 (11222223)
            var d3 = data[3] << 4 | data[4] >> 1;                   // d3 (33334444)
            var d4 = data[4] << 7 | data[5] << 2 | data[6] >> 3;    // d4 (45555566)
            var d5 = data[6] << 5 | data[7];                        // d5 (66677777)
            return (new[] { (byte)d1, (byte)d2, (byte)d3, (byte)d4, (byte)d5 });
        }

        private static byte DecodeChar(char c)
        {
            switch (c)
            {
                case '0':
                    return (0);
                case '1':
                    return (1);
                case '2':
                    return (2);
                case '3':
                    return (3);
                case '4':
                    return (4);
                case '5':
                    return (5);
                case '6':
                    return (6);
                case '7':
                    return (7);
                case '8':
                    return (8);
                case '9':
                    return (9);
                case 'a':
                case 'A':
                    return (10);
                case 'b':
                case 'B':
                    return (11);
                case 'c':
                case 'C':
                    return (12);
                case 'd':
                case 'D':
                    return (13);
                case 'e':
                case 'E':
                    return (14);
                case 'f':
                case 'F':
                    return (15);
                case 'g':
                case 'G':
                    return (16);
                case 'h':
                case 'H':
                    return (17);
                case 'i':
                case 'I':
                    return (18);
                case 'j':
                case 'J':
                    return (19);
                case 'k':
                case 'K':
                    return (20);
                case 'l':
                case 'L':
                    return (21);
                case 'm':
                case 'M':
                    return (22);
                case 'n':
                case 'N':
                    return (23);
                case 'o':
                case 'O':
                    return (24);
                case 'p':
                case 'P':
                    return (25);
                case 'q':
                case 'Q':
                    return (26);
                case 'r':
                case 'R':
                    return (27);
                case 's':
                case 'S':
                    return (28);
                case 't':
                case 'T':
                    return (29);
                case 'u':
                case 'U':
                    return (30);
                case 'v':
                case 'V':
                    return (31);
                default:
                    throw new ArgumentException();
            }
        }

        #endregion
    }
}