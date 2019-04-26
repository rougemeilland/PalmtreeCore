/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Palmtree.Data
{

    /// <summary>
    /// JSON形式のテキストをオブジェクトに変換するクラスです。
    /// </summary>
    /// <remarks>
    /// <seealso cref="SimpleJsonSerializer"/>で直接デシリアライズ可能なのは以下の種類のオブジェクトのみです。
    /// <list type="bullet">
    ///     <item>
    ///         <description><seealso cref="sbyte"/>/<seealso cref="byte"/>/<seealso cref="short"/>/<seealso cref="ushort"/>/<seealso cref="int"/>/<seealso cref="uint"/>/<seealso cref="long"/>/<seealso cref="ulong"/>/<seealso cref="double"/>/<seealso cref="decimal"/>(数値として扱われます)</description>
    ///     </item>
    ///     <item>
    ///         <description><seealso cref="string"/>/<seealso cref="char"/>(文字列として扱われます)</description>
    ///     </item>
    ///     <item>
    ///         <description>true/false/null</description>
    ///     </item>
    ///     <item>
    ///         <description><seealso cref="string"/>以外の<seealso cref="IEnumerable"/> を実装するクラスのオブジェクト(配列として扱われます)</description>
    ///     </item>
    ///     <item>
    ///         <description>IDictionary&lt;string,object&gt; を実装するクラスのオブジェクト(名前付きのメンバーを持つオブジェクトとして扱われます)</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public static class SimpleJsonDeserializer
    {

        /// <summary>
        /// JSON形式のテキストをオブジェクトにデシリアライズします。
        /// </summary>
        /// <param name="source_text">
        /// シリアライズするテキストです。
        /// </param>
        /// <returns>
        /// デシリアライズされたオブジェクトです。
        /// </returns>
        public static object Deserialize(string source_text)
        {
            return (Deserialize(new JsonStringReader(source_text)));
        }

        /// <summary>
        /// <see cref="TextReader"/>からの入力をJSON形式としてオブジェクトへデシリアライズします。
        /// </summary>
        /// <param name="source_stream">
        /// デシリアライズの入力元の<see cref="TextReader"/>オブジェクトです。
        /// </param>
        /// <returns>
        /// デシリアライズされたオブジェクトです。
        /// </returns>
        public static object Deserialize(TextReader source_stream)
        {
            return (Deserialize(new JsonTextStreamReader(source_stream)));
        }

        private static object Deserialize(JsonReaderBase reader)
        {
            reader.SkipSpace();
            switch (reader.Peek())
            {
                case '[':
                    return (DeserializeArray(reader));
                case '{':
                    return (DeserializeObject(reader));
                case '"':
                    return (DeserializeString(reader));
                case 't': // true
                    if (reader.ReadStr(4) != "true")
                        throw new FormatException();
                    return (true);
                case 'f': // false
                    if (reader.ReadStr(5) != "false")
                        throw new FormatException();
                    return (false);
                case 'n': // null
                    if (reader.ReadStr(4) != "null")
                        throw new FormatException();
                    return (null);
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return (DeserializeNumber(1, reader));
                case '-':
                    reader.ReadChar();
                    return (DeserializeNumber(-1, reader));
                default:
                    throw new FormatException();
            }
        }

        private static object DeserializeArray(JsonReaderBase reader)
        {
            if (reader.ReadChar() != '[')
                throw new FormatException();
            var collection = new List<object>();
            var first = true;
            while (true)
            {
                reader.SkipSpace();
                if (reader.IsEnd)
                    throw new FormatException("配列の閉じ括弧が見つかりません。");
                if (reader.Peek() == ']')
                {
                    reader.ReadChar();
                    break;
                }
                if (!first)
                {
                    if (reader.Peek() != ',')
                        throw new FormatException("配列の区切り文字が見つかりません。");
                    reader.ReadChar();
                }
                reader.SkipSpace();
                collection.Add(Deserialize(reader));
                first = false;
            }
            return (collection.ToArray());
        }

        private static object DeserializeObject(JsonReaderBase reader)
        {
            if (reader.ReadChar() != '{')
                throw new FormatException();
            var dic = new Dictionary<string, object>();
            var first = true;
            while (true)
            {
                reader.SkipSpace();
                if (reader.IsEnd)
                    throw new FormatException("オブジェクトの閉じ括弧が見つかりません。");
                if (reader.Peek() == '}')
                {
                    reader.ReadChar();
                    break;
                }
                if (!first)
                {
                    if (reader.Peek() != ',')
                        throw new FormatException("オブジェクトの区切り文字','が見つかりません。");
                    reader.ReadChar();
                }
                reader.SkipSpace();
                var key = DeserializeString(reader);
                reader.SkipSpace();
                if (reader.Peek() != ':')
                    throw new FormatException("オブジェクトの区切り文字':'が見つかりません。");
                reader.ReadChar();
                reader.SkipSpace();
                var value = Deserialize(reader);
                dic[key] = value;
                first = false;
            }
            return (dic);
        }

        private static string DeserializeString(JsonReaderBase reader)
        {
            if (reader.ReadChar() != '"')
                throw new FormatException();
            var sb = new StringBuilder();
            while (!reader.IsEnd)
            {
                char c = reader.ReadChar();
                if (c == '"')
                    break;
                if (c == '\\')
                {
                    char c2 = reader.ReadChar();
                    switch (c2)
                    {
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'u':
                            {
                                var s = reader.ReadStr(4);
                                var ch = char.ConvertFromUtf32(Int32.Parse(s, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
                                sb.Append(ch);
                            }
                            break;
                        case '"':
                        case '\\':
                        case '/':
                            sb.Append(c2);
                            break;
                        default:
                            throw new FormatException("不正なエスケープシーケンスが見つかりました。");
                    }
                }
                else
                    sb.Append(c);
            }
            return (sb.ToString());
        }

        private static object DeserializeNumber(int sign, JsonReaderBase reader)
        {
            var sb = new StringBuilder();
            var is_end_of_numeric = false;
            while (!is_end_of_numeric && !reader.IsEnd)
            {
                char c = reader.Peek();
                switch (c)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '.':
                    case 'e':
                    case 'E':
                        sb.Append(reader.ReadChar());
                        break;
                    default:
                        is_end_of_numeric = true;
                        break;
                }
            }
            var text = sb.ToString();

            Int32 value_int32;
            if (Int32.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture.NumberFormat, out value_int32))
                return (sign * value_int32);

            Int64 value_int64;
            if (Int64.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture.NumberFormat, out value_int64))
                return (sign * value_int64);

            if (sign > 0)
            {
                UInt32 value_uint32;
                if (UInt32.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture.NumberFormat, out value_uint32))
                    return (value_int32);

                UInt64 value_uint64;
                if (UInt64.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture.NumberFormat, out value_uint64))
                    return (value_int64);
            }

            Decimal value_decimal;
            var success_decimal = Decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture.NumberFormat, out value_decimal);
            Double value_double;
            var success_double = Double.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture.NumberFormat, out value_double);
            if (success_decimal)
            {
                if (success_double)
                    return (sign * value_double);
                else
                    return (sign * value_decimal);
            }
            else
            {
                if (success_double)
                    return (sign * value_double);
                else
                    throw new FormatException("不正な形式の数値が見つかりました。");
            }
        }

    }
}
