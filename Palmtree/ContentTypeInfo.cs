/*
  ContentTypeInfo.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System.Collections.Generic;
using System.Linq;

namespace Palmtree
{
    /// <summary>
    /// MIMEタイプに関する情報を持つクラスです。
    /// </summary>
    public class ContentTypeInfo
    {
        #region プライベートフィールド

        private static object _lock_obj = new object();
        private static Dictionary<string, ContentTypeInfo> _content_type_infos_by_mime = null;
        private static Dictionary<string, ContentTypeInfo> _content_type_infos_by_ext = null;

        #endregion

        #region パブリックプロパティ

        /// <summary>
        /// MIMEタイプの文字列です。
        /// </summary>
        public string MIMEMediaType { get; private set; }

        /// <summary>
        /// ファイルの拡張子です。
        /// </summary>
        public string[] FileExtension { get; private set; }

        #endregion

        #region パブリックメソッド

        /// <summary>
        /// MIMEタイプに対応する<see cref="ContentTypeInfo"/>オブジェクトを取得します。
        /// </summary>
        /// <param name="mime_media_type">
        /// MIMEタイプです。
        /// </param>
        /// <returns>
        /// 見つかった<see cref="ContentTypeInfo"/>オブジェクトです。
        /// </returns>
        public static ContentTypeInfo FromMIMEMediaType(string mime_media_type)
        {
            InitializeStaticFields();
            ContentTypeInfo value;
            if (!_content_type_infos_by_mime.TryGetValue(mime_media_type, out value))
                value = _content_type_infos_by_ext[""];
            return (value);
        }

        /// <summary>
        /// ファイルの拡張子に対応する<see cref="ContentTypeInfo"/>オブジェクトを取得します。
        /// </summary>
        /// <param name="ext">
        /// ファイルの拡張子です。
        /// </param>
        /// <returns>
        /// 見つかった<see cref="ContentTypeInfo"/>オブジェクトです。
        /// </returns>
        public static ContentTypeInfo FromExtension(string ext)
        {
            InitializeStaticFields();
            ContentTypeInfo value;
            if (!_content_type_infos_by_ext.TryGetValue(ext, out value))
                value = _content_type_infos_by_ext[""];
            return (value);
        }

        #endregion

        #region プライベートメソッド

        private static void InitializeStaticFields()
        {
            lock (_lock_obj)
            {
                if (_content_type_infos_by_ext == null || _content_type_infos_by_mime == null)
                {
                    string[][] source = new string[][]
                    {
                        new string[] { ".xml", "application/xml"},
                        new string[] { ".xsd", "application/xml"},
                        new string[] { ".css", "text/css"},
                        new string[] { ".htm", "text/html"},
                        new string[] { ".html", "text/html"},
                        new string[] { ".js", "text/javascript"},
                        new string[] { ".txt", "text/plain"},
                        new string[] { ".gif", "image/gif"},
                        new string[] { ".bmp", "image/bmp"},
                        new string[] { ".jpg", "image/jpeg"},
                        new string[] { ".jpeg", "image/jpeg"},
                        new string[] { ".png", "image/png"},
                        new string[] { "", "application/octet-stream"},
                    };
                    var infos = (from item in source
                                 group item[0] by item[1] into g
                                 select new ContentTypeInfo
                                 {
                                     MIMEMediaType = g.Key,
                                     FileExtension = g.ToArray(),
                                 });

                    _content_type_infos_by_mime = infos.ToDictionary(k => k.MIMEMediaType);
                    _content_type_infos_by_ext = (from row in source
                                                  select new
                                                  {
                                                      Key = row[0],
                                                      Value = _content_type_infos_by_mime[row[1]],
                                                  }).ToDictionary(k => k.Key, v => v.Value);
                }
            }
        }

        #endregion
    }
}
