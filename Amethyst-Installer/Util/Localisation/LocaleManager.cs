using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui {
    public static class LocaleManager {
        
        private static Dictionary<string, string> m_loadedLocale;

        static LocaleManager() {
            m_loadedLocale = new Dictionary<string, string>();

            LoadLocale("en");
            LoadLocale(FetchSystemLocale());
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
        /// <param name="localeCode"></param>
        public static void LoadLocale(string localeCode) {
            Console.WriteLine(localeCode);

            string localeJson = string.Empty;
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"amethyst_installer_gui.Resources.Lang.{localeCode}.json") ) {
                if ( resource != null ) {
                    using ( StreamReader reader = new StreamReader(resource) ) {
                        localeJson = reader.ReadToEnd();
                    }
                }
            }

            m_loadedLocale = JsonConvert.DeserializeObject<Dictionary<string,string>>(localeJson);

        }

        static string FetchSystemLocale() {

            string windowsLocale = CultureInfo.CurrentUICulture.Name;

            return windowsLocale.Substring(0, windowsLocale.IndexOf('-'));
        }

        // TODO: Filewatcher

    }
}
