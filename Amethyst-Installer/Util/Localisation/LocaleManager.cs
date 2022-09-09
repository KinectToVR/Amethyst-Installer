using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace amethyst_installer_gui {
    public static class LocaleManager {
        
        private static Dictionary<string, string> m_loadedLocale;
        private static string m_debugLocalePath;

        public static string CurrentLocale { get; private set; }

        static LocaleManager() {
            // I'm not sure why but on exit this function gets called again...
            // I'm not sure as to why this is happening either
            // Because of this, error sounds have been omitted

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
                        if ( localeJson.Length < 1 ) {
                            LoadLocale(CurrentLocale);
                            Console.Error.WriteLine("File \"locale.json\" is invalid! Defaulting to built-in locale...");
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
                        LoadLocale(CurrentLocale);
                        Console.Error.WriteLine("Failed to read locale.json! Defaulting to built-in locale...");
                        Console.Error.WriteLine(Util.FormatException(e));
                    }
                } else {
                    LoadLocale(CurrentLocale);
                    Console.Error.WriteLine("Directory \"Lang\" exists, but couldn't find file \"locale.json\"! Defaulting to built-in locale...");
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
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"amethyst_installer_gui.Resources.Lang.{localeCode}.json") ) {
                if ( resource != null ) {
                    using ( StreamReader reader = new StreamReader(resource) ) {
                        string localeJson = reader.ReadToEnd();
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
