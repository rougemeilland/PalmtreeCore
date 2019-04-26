/*
  Copyright (c) 2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;

namespace Palmtree.Data
{

    /// <summary>
    /// オブジェクトをJSON形式のテキストに変換するクラスです。
    /// </summary>
    /// <remarks>
    /// <seealso cref="SimpleJsonSerializer"/>で直接シリアライズ可能なのは以下の種類のオブジェクトのみです。
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
    public static class SimpleJsonSerializer
    {

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
        public static string Serialize(object source_object, JsonSerializationFormatParameter param = null)
        {
            var sb = new StringBuilder();
            Serialize(source_object, new JsonStringWriter(sb), param ?? new JsonSerializationFormatParameter());
            return (sb.ToString());
        }

        /// <summary>
        /// オブジェクトをJSON形式で<see cref="TextWriter"/>に出力します。
        /// </summary>
        /// <param name="source_object">
        /// シリアライズするオブジェクトです。
        /// </param>
        /// <param name="writer">
        /// 出力先の<see cref="TextWriter"/>オブジェクトです。
        /// </param>
        /// <param name="param">
        /// シリアライズの書式のパラメタです。
        /// </param>
        public static void Serialize(object source_object, TextWriter writer, JsonSerializationFormatParameter param = null)
        {
            Serialize(source_object, new JsonTextStreamWriter(writer), param ?? new JsonSerializationFormatParameter());
        }

        private static void Serialize(object source_object, IJsonWriter writer, JsonSerializationFormatParameter param)
        {
            if (source_object == null)
                writer.Write("null");
            else
            {
                var type_code = Type.GetTypeCode(source_object.GetType());
                switch (type_code)
                {
                    case TypeCode.Byte:
                        writer.Write(((Byte)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.SByte:
                        writer.Write(((SByte)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.UInt16:
                        writer.Write(((UInt16)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.Int16:
                        writer.Write(((Int16)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.UInt32:
                        writer.Write(((UInt32)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.Int32:
                        writer.Write(((Int32)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.UInt64:
                        writer.Write(((UInt64)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.Int64:
                        writer.Write(((Int64)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.Single:
                        if (Single.IsNaN((Single)source_object) || Single.IsInfinity((Single)source_object))
                        {
                            // NaNとInfinityはnullとして出力する
                            writer.Write("null");
                        }
                        else
                            writer.Write(((Single)source_object).ToString("G9", CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.Double:
                        if (Double.IsNaN((Double)source_object) || Double.IsInfinity((Double)source_object))
                        {
                            // NaNとInfinityはnullとして出力する
                            writer.Write("null");
                        }
                        else
                            writer.Write(((Double)source_object).ToString("G17", CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.Decimal:
                        writer.Write(((Decimal)source_object).ToString(CultureInfo.InvariantCulture.NumberFormat));
                        break;
                    case TypeCode.Boolean:
                        writer.Write(((Boolean)source_object).ToString(CultureInfo.InvariantCulture).ToLower());
                        break;
                    case TypeCode.Char:
                        SerializeString(((char)source_object).ToString(), writer);
                        break;
                    case TypeCode.String:
                        SerializeString((string)source_object, writer);
                        break;
                    default:
                        if (source_object is IDictionary<string, object>)
                            SerializeObject((IDictionary<string, object>)source_object, writer, param);
                        else if (source_object is NameValueCollection)
                            SerializeObject((NameValueCollection)source_object, writer, param);
                        else if (source_object is IEnumerable)
                            SerializeArray((IEnumerable)source_object, writer, param);
                        else
                        {
                            // 不明な種類のオブジェクトはとりあえず文字列形式でシリアライズしておく
                            SerializeString(source_object.ToString(), writer);
                        }
                        break;
                }
            }

        }

        private static void SerializeString(string source_string, IJsonWriter writer)
        {
            writer.Write('"');
            foreach (var c in source_string)
            {
                switch (c)
                {
                    case '"':
                    case '\\':
                    case '/':
                        writer.Write('\\');
                        writer.Write(c);
                        break;
                    case '\b':
                        writer.Write("\\b");
                        break;
                    case '\f':
                        writer.Write("\\f");
                        break;
                    case '\n':
                        writer.Write("\\n");
                        break;
                    case '\r':
                        writer.Write("\\r");
                        break;
                    case '\t':
                        writer.Write("\\t");
                        break;
                    default:
                        if (c != 0x3c && c != 0x3e && c >= 0x20 && c <= 0x7e)
                            writer.Write(c);
                        else
                            writer.Write(string.Format("\\u{0:x4}", (int)c));
                        break;
                }
            }
            writer.Write('"');
        }

        private static void SerializeObject(IDictionary<string, object> source_dictionary, IJsonWriter writer, JsonSerializationFormatParameter param)
        {
            writer.Write('{');
            writer.Write(param.DelimiterObjectLeft);
            var is_first = true;
            foreach (var element in source_dictionary)
            {
                if (!is_first)
                {
                    writer.Write(',');
                    writer.Write(param.DelimiterObjecComma);
                }
                SerializeString(element.Key, writer);
                writer.Write(':');
                writer.Write(param.DelimiterObjecColon);
                Serialize(element.Value, writer, param);
                is_first = false;
            }
            writer.Write(param.DelimiterObjectRight);
            writer.Write('}');
        }

        private static void SerializeObject(NameValueCollection source_namevalue_collection, IJsonWriter writer, JsonSerializationFormatParameter param)
        {
            writer.Write('{');
            writer.Write(param.DelimiterObjectLeft);
            var is_first = true;
            foreach (string key in source_namevalue_collection.Keys)
            {
                if (!is_first)
                {
                    writer.Write(',');
                    writer.Write(param.DelimiterObjecComma);
                }
                SerializeString(key.ToString(), writer);
                writer.Write(':');
                writer.Write(param.DelimiterObjecColon);
                Serialize(source_namevalue_collection[key], writer, param);
                is_first = false;
            }
            writer.Write(param.DelimiterObjectRight);
            writer.Write('}');
        }

        private static void SerializeArray(IEnumerable source_collection, IJsonWriter writer, JsonSerializationFormatParameter param)
        {
            writer.Write('[');
            writer.Write(param.DelimiterArrayLeft);
            var is_first = true;
            foreach (var item in source_collection)
            {
                if (!is_first)
                {
                    writer.Write(',');
                    writer.Write(param.DelimiterArrayComma);
                }
                Serialize(item, writer, param);
                is_first = false;
            }
            writer.Write(param.DelimiterArrayRight);
            writer.Write(']');
        }
    }
}
