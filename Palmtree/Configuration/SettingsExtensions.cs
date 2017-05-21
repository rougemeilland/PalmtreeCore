/*
  SettingsExtensions.cs

  Copyright (c) 2017 Palmtree Software

  This software is released under the MIT License.
  https://opensource.org/licenses/MIT
*/

using System;
using System.Configuration;

namespace Palmtree.Configuration
{
    /// <summary>
    /// アプリケーションの設定に関する拡張メソッドのクラスです。
    /// </summary>
    public static class SettingsExtensions
    {
        /// <summary>
        /// 前回実行時からアセンブリのバージョンが上がっているかどうかを調べ、上がっていれば設定を前のバージョンから引き継ぎます。
        /// </summary>
        /// <typeparam name="SETTING_T">
        /// 設定オブジェクトの型です。
        /// </typeparam>
        /// <param name="settings">
        /// 設定オブジェクトです。
        /// </param>
        /// <param name="version_getter">
        /// 設定オブジェクトからバージョン文字列を取得するゲッターです。
        /// </param>
        /// <param name="version_setter">
        /// 設定オブジェクトにバージョン文字列を設定するセッターです。
        /// </param>
        public static void UpgradeOnVersionUp<SETTING_T>(this SETTING_T settings, Func<SETTING_T, string> version_getter, Action<SETTING_T, string> version_setter)
            where SETTING_T : ApplicationSettingsBase
        {
            var current_version = settings.GetType().Assembly.GetName().Version;
            var setting_version_string = version_getter(settings);
            if (!string.IsNullOrEmpty(setting_version_string))
            {

                Version settings_version;
                try
                {
                    settings_version = new Version(setting_version_string);
                }
                catch
                {
                    settings_version = null;
                }
                if (settings_version != null && settings_version >= current_version)
                    return;
            }
            settings.Upgrade();
            version_setter(settings, current_version.ToString());
            settings.Save();
        }
    }
}