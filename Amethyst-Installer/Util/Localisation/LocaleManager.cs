using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace amethyst_installer_gui {
    public static class LocaleManager {
        
        private static Dictionary<string, string> m_loadedLocale;
        private static string m_debugLocalePath;

        public static string CurrentLocale { get; private set; }

        static LocaleManager() {
            m_loadedLocale = new Dictionary<string, string>();
            CurrentLocale = FetchSystemLocale();

            LoadLocale("en");
            if ( MainWindow.DebugMode ) {
                var langDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lang"));
                if ( !Directory.Exists(langDir) ) {
                    LoadLocale(CurrentLocale);
                    return;
                }
                m_debugLocalePath = Path.Combine(langDir, "locale.json");
                
                if ( File.Exists(m_debugLocalePath) ) {
                    try {
                        string localeJson = string.Empty;
                        using ( FileStream file = new FileStream(m_debugLocalePath, FileMode.Open, FileAccess.Read) ) {
                            localeJson = File.ReadAllText(m_debugLocalePath);
                        }
                        if ( localeJson.Length == 0 ) {
                            SoundPlayer.PlaySound(SoundEffect.Error);
                            Util.ShowMessageBox("Invalid locale.json!", "LocaleNotFoundException");
                            LoadLocale(CurrentLocale);
                            return;
                        }

                        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(localeJson);
                        foreach ( var key in dictionary.Keys ) {
                            if ( m_loadedLocale.ContainsKey(key) ) {
                                m_loadedLocale[key] = dictionary[key];
                            } else {
                                m_loadedLocale.Add(key, dictionary[key]);
                            }
                        }
                    } catch ( Exception e ) {
                        Logger.Error("Failed to read locale.json");
                        Logger.Error(Util.FormatException(e));
                        LoadLocale(CurrentLocale);
                    }
                } else {
                    SoundPlayer.PlaySound(SoundEffect.Error);
                    Util.ShowMessageBox("Failed to find a valid Locale! Does a file at \"Lang/locale.json\" exist?", "LocaleNotFoundException");
                    LoadLocale(CurrentLocale);
                }

            } else {
                LoadLocale(CurrentLocale);
            }
        }

        /// <summary>
        /// Returns a localized string
        /// </summary>
        /// <param name="key">The key of the string</param>
        public static string GetString(string key) {
            if ( m_loadedLocale.ContainsKey(key) ) {
                return m_loadedLocale[key];
            }
            return key;
        }

        /// <summary>
        /// Load locale
        /// </summary>
        /// <param name="localeCode">Locale code to load</param>
        public static void LoadLocale(string localeCode) {
            string localeJson = string.Empty;
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"amethyst_installer_gui.Resources.Lang.{localeCode}.json") ) {
                if ( resource != null ) {
                    using ( StreamReader reader = new StreamReader(resource) ) {
                        localeJson = reader.ReadToEnd();
                        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(localeJson);
                        foreach (var key in dictionary.Keys) {
                            if (m_loadedLocale.ContainsKey(key)) {
                                m_loadedLocale[key] = dictionary[key];
                            } else {
                                m_loadedLocale.Add(key, dictionary[key]);
                            }
                        }
                    }
                }
            }
        }

        private static string FetchSystemLocale() {

            string windowsLocale = CultureInfo.CurrentUICulture.Name;
            return windowsLocale.Substring(0, windowsLocale.IndexOf('-'));
        }
    }
}
