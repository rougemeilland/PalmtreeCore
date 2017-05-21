/*
  SimpleJson.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace Palmtree
{
    /// <summary>
    /// オブジェクトとJSON形式のテキストの相互変換を行うクラスです。
    /// </summary>
    /// <remarks>
    /// <seealso cref="SimpleJson"/>で直接シリアライズ/デシリアライズ可能なのは以下の種類のオブジェクトのみです。
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
    public static class SimpleJson
    {
        #region SourceStringReader の定義

        private class SourceStringReader
        {
            #region プライベートフィールド

            private int _position;
            private string _json_string;

            #endregion

            #region コンストラクタ

            public SourceStringReader(string source_string)
            {
                _position = 0;
                _json_string = source_string;
            }

            #endregion

            #region パブリックメソッド

            public char ReadChar()
            {
                if (!IsRead)
                    return ('\0');
                var c = _json_string[_position];
                _position++;
                return (c);
            }

            public string ReadStr(int length)
            {
                if (Remain < length)
                    throw new ArgumentException();
                var s = _json_string.Substring(_position, length);
                _position += length;
                return (s);
            }

            public char Peek()
            {
                if (!IsRead)
                    return ('\0');
                var c = _json_string[_position];
                return (c);
            }

            public void Back()
            {
                if (_position <= 0)
                    throw new InvalidOperationException();
                --_position;
            }

            #endregion

            #region パブリックプロパティ

            public bool IsRead
            {
                get
                {
                    return (_position < _json_string.Length);
                }
            }

            public int Remain
            {
                get
                {
                    return (_json_string.Length - _position);
                }
            }

            #endregion
        }


        #endregion

        #region SerializationFormatParameter

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// 与えられた文字列をJSON形式のテキストとみなして、デシリアライズを行います。
        /// </summary>
        /// <param name="source_string">
        /// JSON形式のテキストです。
        /// </param>
        /// <returns>
        /// デシリアライズされたオブジェクトです。
        /// </returns>
        public static object Deserialize(string source_string)
        {
            var reader = new SourceStringReader(source_string);
            return (Parse(reader));
        }

        /// <summary>
        /// オブジェクトをJSON形式のテキストにシリアライズします。
        /// </summary>
        /// <param name="source_object">
        /// シリアライズするオブジェクトです。
        /// </param>
        /// <param name="param">
        /// シリアライズの書式のパラメタです。
        /// </param>
        /// <returns>
        /// JSON形式のテキストです。
        /// </returns>
        public static string Serialize(object source_object, SerializationFormatParameter param = null)
        {
            return (SerializeValue(source_object, param ?? new SerializationFormatParameter()));
        }

        /// <summary>
        /// 与えられたオブジェクトをlong型とみなして値を変換します。
        /// </summary>
        /// <param name="source_object">
        /// 変換元のオブジェクトです。
        /// </param>
        /// <param name="default_value">
        /// 与えられたオブジェクトをlong型に変換できなかった場合に返す既定の値です。
        /// nullが与えられた場合は、変換できなかった時に例外を通知します。
        /// </param>
        /// <returns>
        /// 変換されたlong値です。
        /// </returns>
        public static long CastObjectAsLong(object source_object, long? default_value = null)
        {
            if (source_object is int)
                return ((long)(int)source_object);
            else if (source_object is uint)
                return ((long)(uint)source_object);
            else if (source_object is long)
                return ((long)source_object);
            else if (default_value.HasValue)
                return (default_value.Value);
            else
                throw (new ArgumentException());
        }

        /// <summary>
        /// 与えられたオブジェクトをdouble型とみなして値を変換します。
        /// </summary>
        /// <param name="source_object">
        /// 変換元のオブジェクトです。
        /// </param>
        /// <param name="default_value">
        /// 与えられたオブジェクトをdouble型に変換できなかった場合に返す既定の値です。
        /// nullが与えられた場合は、変換できなかった時に例外を通知します。
        /// </param>
        /// <returns>
        /// 変換されたdouble値です。
        /// </returns>
        public static double CastObjectAsDouble(object source_object, double? default_value = null)
        {
            if (source_object is int)
                return ((double)(int)source_object);
            else if (source_object is uint)
                return ((double)(uint)source_object);
            else if (source_object is long)
                return ((double)(long)source_object);
            else if (source_object is ulong)
                return ((double)(ulong)source_object);
            else if (source_object is decimal)
                return ((double)(decimal)source_object);
            else if (source_object is double)
                return ((double)source_object);
            else if (default_value.HasValue)
                return (default_value.Value);
            else
                throw (new ArgumentException());
        }

        /// <summary>
        /// 与えられたオブジェクトをdecimal型とみなして値を変換します。
        /// </summary>
        /// <param name="source_object">
        /// 変換元のオブジェクトです。
        /// </param>
        /// <param name="default_value">
        /// 与えられたオブジェクトをdecimal型に変換できなかった場合に返す既定の値です。
        /// nullが与えられた場合は、変換できなかった時に例外を通知します。
        /// </param>
        /// <returns>
        /// 変換されたdecimal値です。
        /// </returns>
        public static decimal CastObjectAsDecimal(object source_object, decimal? default_value = null)
        {
            if (source_object is int)
                return ((decimal)(int)source_object);
            else if (source_object is uint)
                return ((decimal)(uint)source_object);
            else if (source_object is long)
                return ((decimal)(long)source_object);
            else if (source_object is ulong)
                return ((decimal)(ulong)source_object);
            else if (source_object is decimal)
                return ((decimal)source_object);
            else if (source_object is double)
                return ((decimal)(double)source_object);
            else if (default_value.HasValue)
                return (default_value.Value);
            else
                throw (new ArgumentException());
        }

        #endregion

        #region プライベートメソッド

        private static object Parse(SourceStringReader reader)
        {
            while (reader.IsRead)
            {
                char c1 = reader.ReadChar();

                if (c1 == '[')
                    return (ParseArray(reader));
                else if (c1 == '{')
                    return (ParseObject(reader));
            }
            return ("");
        }

        private static object ParseArray(SourceStringReader reader)
        {
            var collection = new List<object>();
            while (reader.IsRead)
            {
                char c1 = reader.ReadChar();
                if (c1 == ']')
                    break;
                else if (c1 == ' ' || c1 == '\n' || c1 == '\r' || c1 == '\t' || c1 == ',')
                    continue;
                collection.Add(ParseValue(reader, c1));
            }
            return (collection.ToArray());
        }

        private static object ParseNumber(SourceStringReader reader)
        {
            reader.Back();
            var sb = new StringBuilder();
            while (reader.IsRead)
            {
                char c1 = reader.ReadChar();
                if (c1.IsAnyOf( ',',  ':',  '}',  ']'))
                {
                    reader.Back();
                    break;
                }
                sb.Append(c1);
            }
            return (ToNumeric(sb.ToString()));
        }

        private static object ParseObject(SourceStringReader reader)
        {
            var dic = new Dictionary<string, object>();
            string key = null;
            while (reader.IsRead)
            {
                char c1 = reader.ReadChar();
                if (c1 == '}')
                    break;
                else if (c1 == ' ' || c1 == '\n' || c1 == '\r' || c1 == '\t' || c1 == ',' || c1 == ':')
                    continue;
                else if (key == null)
                {
                    key = ParseString(reader);
                    continue;
                }
                dic[key] = ParseValue(reader, c1);
                key = null;
            }
            return (dic);
        }

        private static string ParseString(SourceStringReader reader)
        {
            var sb = new StringBuilder();
            while (reader.IsRead)
            {
                char c1 = reader.ReadChar();
                if (c1 == '"')
                    break;
                if (c1 == '\\')
                {
                    char c2 = reader.ReadChar();
                    switch (c2)
                    {
                        case 'b':
                        case 'f':
                        case 'n':
                        case 'r':
                        case 't':
                            sb.AppendFormat("{0}{1}", c1, c2);
                            break;
                        case 'u':
                            if (reader.Remain >= 4)
                            {
                                var s = reader.ReadStr(4);
                                var c = Convert.ToChar(Convert.ToInt32(s, 16), CultureInfo.InvariantCulture);
                                sb.Append(c);
                            }
                            break;
                        case '"':
                        case '\\':
                        case '/':
                        default:
                            sb.Append(c2);
                            break;
                    }
                }
                else
                    sb.Append(c1);
            }
            return (sb.ToString());
        }

        private static object ParseValue(SourceStringReader reader, char header_char)
        {
            switch (header_char)
            {
                case '[':
                    return (ParseArray(reader));
                case '{':
                    return (ParseObject(reader));
                case '"':
                    return (ParseString(reader));
                case 't': // true
                    if (reader.ReadStr(3) != "rue")
                        throw new FormatException();
                    return (true);
                case 'f': // false
                    if (reader.ReadStr(4) != "alse")
                        throw new FormatException();
                    return (false);
                case 'n': // null
                    if (reader.ReadStr(3) != "ull")
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
                    return (ParseNumber(reader));
                case '-':
                    char c2 = (reader.Remain > 0) ? reader.Peek() : default(char);
                    if (c2 >= '0' && c2 <= '9')
                        return (ParseNumber(reader));
                    break;
            }
            return (null);
        }

        private static string SerializeArray(IEnumerable source_collection, SerializationFormatParameter param)
        {
            int count = 0;
            var sb = new StringBuilder();
            sb.Append("[" + param.DelimiterArrayLeft);
            foreach (var item in source_collection)
            {
                if (count != 0)
                    sb.Append("," + param.DelimiterArrayComma);
                sb.Append(SerializeValue(item, param));
                count++;
            }
            sb.Append(param.DelimiterArrayRight + "]");
            return (sb.ToString());
        }

        private static string SerializeNumber(string source_number_string, SerializationFormatParameter param)
        {
            return (ToNumericString(source_number_string));
        }

        private static string SerializeObject(IDictionary<string, object> source_dictionary, SerializationFormatParameter param)
        {
            int count = 0;
            var sb = new StringBuilder();
            sb.Append("{" + param.DelimiterObjectLeft);
            foreach (var element in source_dictionary)
            {
                if (count != 0)
                    sb.Append("," + param.DelimiterObjecComma);
                sb.AppendFormat("{0}:{1}{2}", SerializeString(element.Key, param), param.DelimiterObjecColon, SerializeValue(element.Value, param));
                count++;
            }
            sb.Append(param.DelimiterObjectRight+ "}");
            return (sb.ToString());
        }

        private static string SerializeObject(NameValueCollection source_namevalue_collection, SerializationFormatParameter param)
        {
            int count = 0;
            var sb = new StringBuilder();
            sb.Append("{" + param.DelimiterObjectLeft);
            foreach (string key in source_namevalue_collection.Keys)
            {
                if (count != 0)
                    sb.Append(", ");
                sb.AppendFormat("{0}:{1}{2}", SerializeString(key, param), param.DelimiterObjecColon, SerializeValue(source_namevalue_collection[key], param));
                count++;
            }
            sb.Append(param.DelimiterObjectRight + "}");
            return (sb.ToString());
        }

        private static string SerializeString(string source_string, SerializationFormatParameter param)
        {
            var sb = new StringBuilder();
            sb.Append("\"");
            for (int i = 0; i < source_string.Length; i++)
            {
                char c1 = source_string[i];
                switch (c1)
                {
                    case '"':
                    case '\\':
                    case '/':
                        sb.AppendFormat("\\{0}", c1);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
#if false
                        if (c1 != 0x3C && c1 != 0x3E && 0x20 <= c1 && c1 <= 0x7E)
                            sb.Append(c1);
                        else
                            sb.Append("\\u" + System.Convert.ToString(c1, 16).PadLeft(4, '0'));
#else
                        if (!char.IsControl(c1))
                            sb.Append(c1);
                        else
                            sb.Append("\\u" + System.Convert.ToString(c1, 16).PadLeft(4, '0'));
#endif
                        break;
                }
            }
            sb.Append("\"");
            return (sb.ToString());
        }

        private static string SerializeValue(object source_object, SerializationFormatParameter param)
        {
            if (source_object == null)
                return ("null");
            var type_code = Type.GetTypeCode(source_object.GetType());
            switch (type_code)
            {
                case TypeCode.Byte:
                    return (SerializeNumber(((byte)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.SByte:
                    return (SerializeNumber(((sbyte)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.Int16:
                    return (SerializeNumber(((short)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.Int32:
                    return (SerializeNumber(((int)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.Int64:
                    return (SerializeNumber(((long)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.UInt16:
                    return (SerializeNumber(((uint)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.UInt32:
                    return (SerializeNumber(((ushort)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.UInt64:
                    return (SerializeNumber(((ulong)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.Single:
                    if (float.IsNaN((float)source_object) || float.IsInfinity((float)source_object))
                        return ("null");
                    else
                        return (SerializeNumber(((float)source_object).ToString("r", CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.Double:
                    if (double.IsNaN((double)source_object) || double.IsInfinity((double)source_object))
                        return ("null");
                    else
                        return (SerializeNumber(((double)source_object).ToString("r", CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.Decimal:
                    return (SerializeNumber(((decimal)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat), param));
                case TypeCode.Boolean:
                    return (((bool)source_object).ToString(CultureInfo.InvariantCulture).ToLower());
                case TypeCode.Char:
                    return (SerializeString(((char)source_object).ToString(), param));
                case TypeCode.String:
                    return (SerializeString(source_object as string, param));
                default:
                    if (source_object is IDictionary<string, object>)
                        return (SerializeObject(source_object as IDictionary<string, object>, param));
                    else if (source_object is NameValueCollection)
                        return (SerializeObject(source_object as NameValueCollection, param));
                    else if (source_object is IEnumerable)
                        return (SerializeArray(source_object as IEnumerable, param));
                    else
                        return (SerializeString(source_object.ToString(), param));
            }
        }

        private static object ToNumeric(string source_string)
        {
            source_string = source_string.Trim();
            int value_int;
            if (int.TryParse(source_string, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_int))
                return (value_int);
            uint value_uint;
            if (uint.TryParse(source_string, NumberStyles.None, CultureInfo.InvariantCulture, out value_uint))
                return (value_uint);
            long value_long;
            if (long.TryParse(source_string, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_long))
                return (value_long);
            ulong value_ulong;
            if (ulong.TryParse(source_string, NumberStyles.None, CultureInfo.InvariantCulture, out value_ulong))
                return (value_ulong);
            decimal value_decimal;
            if (decimal.TryParse(source_string, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_decimal))
            {
                double value_double;
                if (double.TryParse(source_string, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_double))
                {
                    // doubleとdecimalのTryParseの際に桁落ちが起きていないかどうかを調べ、桁落ちのない方を採用する
                    var str_double = value_double.ToString("r", CultureInfo.InvariantCulture.NumberFormat);
                    if (str_double == source_string)
                        return (value_double);
                    var str_decimal = value_decimal.ToString(CultureInfo.InvariantCulture.NumberFormat);
                    if (str_decimal == source_string)
                        return (value_decimal);
                    // ここに到達するのは、double形式とdecimal形式の両方で桁落ちが起きてしまっている場合。
                    // 仕方がないのでとりあえずdouble形式で返す。
                    return (value_double);
                }
                else
                    return (value_decimal);
            }
            else
            {
                double value_double;
                if (double.TryParse(source_string, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_double))
                    return (value_double);
                else
                {
                    // nop
                }
            }
            return ((int)0);
        }

        private static string ToNumericString(string source_string)
        {
            int value_int;
            if (int.TryParse(source_string, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_int))
                return (value_int.ToString(CultureInfo.InvariantCulture.NumberFormat));
            uint value_uint;
            if (uint.TryParse(source_string, NumberStyles.None, CultureInfo.InvariantCulture, out value_uint))
                return (value_uint.ToString(CultureInfo.InvariantCulture.NumberFormat));
            long value_long;
            if (long.TryParse(source_string, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_long))
                return (value_long.ToString(CultureInfo.InvariantCulture.NumberFormat));
            ulong value_ulong;
            if (ulong.TryParse(source_string, NumberStyles.None, CultureInfo.InvariantCulture, out value_ulong))
                return (value_ulong.ToString(CultureInfo.InvariantCulture.NumberFormat));
            double value_double;
            if (double.TryParse(source_string, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_double))
            {
                decimal value_decimal;
                if (decimal.TryParse(source_string, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_decimal))
                {
                    // 文字列として表現するのにdouble.ToString()とdecimal.ToString()のどちらを使用するか判別する
                    // 順次ToString()を発行してみて元の文字列と一致する方を採用する。
                    var str_double = value_double.ToString("r", CultureInfo.InvariantCulture.NumberFormat);
                    if (str_double == source_string)
                        return (str_double);
                    var str_decimal = value_decimal.ToString(CultureInfo.InvariantCulture.NumberFormat);
                    if (str_decimal == source_string)
                        return (str_decimal);
                    // ここに到達するのは、double形式とdecimal形式の両方でToString()の結果が一致しない場合。
                    // 仕方がないのでとりあえずdouble形式で返す。
                    return (str_double);
                }
                else
                    return (value_double.ToString("r", CultureInfo.InvariantCulture.NumberFormat));
            }
            else
            {
                decimal value_decimal;
                if (decimal.TryParse(source_string, NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value_decimal))
                    return (value_decimal.ToString(CultureInfo.InvariantCulture.NumberFormat));
                else
                {
                    // nop
                }
            }
            return ("0");
        }

        #endregion
    }
}