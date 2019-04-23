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

        #endregion
    }
}