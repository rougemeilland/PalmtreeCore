/*
  JasonSerializationFormatParameter.cs

  Copyright (c) 2017-2019 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

namespace Palmtree.Data
{
    /// <summary>
    /// オブジェクトのシリアライズの書式を指定するオブジェクトのクラスです。
    /// </summary>
    public class JsonSerializationFormatParameter
    {
        #region コンストラクタ

        /// <summary>
        ///  デフォルトコンストラクタです。
        /// </summary>
        public JsonSerializationFormatParameter()
        {

            DelimiterArrayLeft = "";
            DelimiterArrayComma = "";
            DelimiterArrayRight = "";
            DelimiterObjectLeft = "";
            DelimiterObjecColon = "";
            DelimiterObjecComma = "";
            DelimiterObjectRight = "";
        }

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// 配列の開きカッコの内側に挿入される文字列です。
        /// </summary>
        public string DelimiterArrayLeft { get; set; }


        /// <summary>
        /// 配列の要素の区切りのカンマの後に挿入される文字列です。
        /// </summary>
        public string DelimiterArrayComma { get; set; }

        /// <summary>
        /// 配列の閉じカッコの内側に挿入される文字列です。
        /// </summary>
        public string DelimiterArrayRight { get; set; }

        /// <summary>
        /// オブジェクトの開きカッコの内側に挿入される文字列です。
        /// </summary>
        public string DelimiterObjectLeft { get; set; }

        /// <summary>
        /// オブジェクトの名前と値の区切りのコロンの後に挿入される文字列です。
        /// </summary>
        public string DelimiterObjecColon { get; set; }

        /// <summary>
        /// オブジェクトの値の後のカンマの後に挿入される文字列です。
        /// </summary>
        public string DelimiterObjecComma { get; set; }

        /// <summary>
        /// オブジェクトの閉じカッコの内側に挿入される文字列です。
        /// </summary>
        public string DelimiterObjectRight { get; set; }

        #endregion
    }
}