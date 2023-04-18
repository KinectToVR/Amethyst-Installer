using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace amethyst_installer_gui {
    public static class LocaleManager {
        
        private static Dictionary<string, string> 
            m_loadedLocale = new Dictionary<string, string>();

        public static string CurrentLocale { get; private set; }

        static LocaleManager() {
            CurrentLocale = FetchSystemLocale();
            ReloadLocale();
        }

        public static void ReloadLocale() {
            // I'm not sure why but on exit this function gets called again...
            // I'm not sure as to why this is happening either
            // Because of this, error sounds have been omitted

            // Load defaults
            UnloadLocales();
            LoadLocale("en");

            if ( MainWindow.DebugMode ) {
                var langDir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lang"));

                if ( !Directory.Exists(langDir) ) {
                    LoadLocale(CurrentLocale);
                    return;
                }

                var localeFile = Path.Combine(langDir, $"locale.json");
                if ( File.Exists(localeFile) ) {
                    try {
                        using ( var reader = new StreamReader(File.Open(localeFile,
                                   FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) ) {

                            string localeJson = reader.ReadToEnd();
                            if ( localeJson.Length < 1 ) {
                                LoadLocale(CurrentLocale);
                                Logger.Error(
                                    $"File \"Lang\\locale.json\" is invalid! Defaulting to built-in locale...");
                                return;
                            }

                            LoadStringsFromJson(localeJson);
                        }
                    } catch ( Exception e ) {
                        LoadLocale(CurrentLocale);
                        Logger.Error("Failed to read locale.json! Defaulting to built-in locale...");
                        Console.Error.WriteLine(Util.FormatException(e));
                    }
                } else {
                    LoadLocale(CurrentLocale);
                    Logger.Error("Directory \"Lang\" exists, but couldn't find file \"locale.json\"! Defaulting to built-in locale...");
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
                        LoadStringsFromJson(localeJson);
                    }
                }
            }
        }

        /// <summary>
        /// Clears locale keymap from memory
        /// </summary>
        public static void UnloadLocales() {
            m_loadedLocale.Clear();
        }

        private static string FetchSystemLocale() {

            string windowsLocale = CultureInfo.CurrentUICulture.Name;
            return windowsLocale.Substring(0, windowsLocale.IndexOf('-'));
        }

        private static void LoadStringsFromJson(string jsonString) {
            var localisationFile = JsonConvert.DeserializeObject<LocalisationFileJSON>(jsonString);
            foreach ( var messageInfo in localisationFile.Messages ) {
                if ( m_loadedLocale.ContainsKey(messageInfo.Id) ) {
                    m_loadedLocale[messageInfo.Id] = messageInfo.Translation;
                } else {
                    m_loadedLocale.Add(messageInfo.Id, messageInfo.Translation);
                }
            }
        }
    }
}
