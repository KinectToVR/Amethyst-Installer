using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WpfColor = System.Windows.Media.Color;

namespace amethyst_installer_gui.PInvoke {
    // Entry points and usage from
    // https://github.com/AvaloniaUI/Avalonia/issues/3950#issuecomment-631168424
    public static class UxTheme {
        #region UxTheme Entry Points
        
        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        private static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType, bool bIgnoreHighContrast, uint dwHighContrastCacheMode);
        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        private static extern uint GetImmersiveColorTypeFromName(IntPtr pName);
        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        private static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);

        [DllImport("uxtheme.dll", EntryPoint = "#132")] //Win 10 1809+ ?
        private static extern bool fnShouldAppsUseDarkMode();
        [DllImport("uxtheme.dll", EntryPoint = "#138")] //Win 10 1903+ ?
        private static extern bool fnShouldSystemUseDarkMode();
        
        #endregion

        private static WpfColor GetThemeColorRef(string h) {
            uint colorSetEx = GetImmersiveColorFromColorSetEx(
                (uint) GetImmersiveUserColorSetPreference( false, false ),
                GetImmersiveColorTypeFromName( Marshal.StringToHGlobalUni(h) ),
                false,
                0);

            uint a = 0xFFFFFF & colorSetEx >> 24;
            uint r = (0xFFFFFF & colorSetEx);
            uint g = (0xFFFFFF & colorSetEx) >> 8;
            uint b = (0xFFFFFF & colorSetEx) >> 16;

            WpfColor colour = WpfColor.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);

            return colour;
        }

#region Colours

        /// <summary>
        /// Window title bar accent colour
        /// </summary>
        public static WpfColor ThemeAccent { get { return GetThemeColorRef("ImmersiveSystemAccent"); } }
        /// <summary>
        /// Light mode accent colour 1 (brightest)
        /// </summary>
        public static WpfColor ThemeAccentLight1 { get { return GetThemeColorRef("ImmersiveSystemAccentLight1"); } }
        /// <summary>
        /// Light mode accent colour 2 (neutral)
        /// </summary>
        public static WpfColor ThemeAccentLight2 { get { return GetThemeColorRef("ImmersiveSystemAccentLight2"); } }
        /// <summary>
        /// Light mode accent colour 3 (brightest)
        /// </summary>
        public static WpfColor ThemeAccentLight3 { get { return GetThemeColorRef("ImmersiveSystemAccentLight3"); } }
        /// <summary>
        /// Dark mode accent colour 1 (brightest)
        /// </summary>
        public static WpfColor ThemeAccentDark1 { get { return GetThemeColorRef("ImmersiveSystemAccentDark1"); } }
        /// <summary>
        /// Dark mode accent colour 2 (neutral)
        /// </summary>
        public static WpfColor ThemeAccentDark2 { get { return GetThemeColorRef("ImmersiveSystemAccentDark2"); } }
        /// <summary>
        /// Dark mode accent colour 3 (darkest)
        /// </summary>
        public static WpfColor ThemeAccentDark3 { get { return GetThemeColorRef("ImmersiveSystemAccentDark3"); } }

        /// <summary>
        /// The brightest accent colour
        /// </summary>
        public static WpfColor ThemeAccent1 {
            get {
                if ( ShouldUseDarkMode )
                    return ThemeAccentDark1;
                return ThemeAccentLight1;
            }
        }

        /// <summary>
        /// Neutral accent colour
        /// </summary>
        public static WpfColor ThemeAccent2 {
            get {
                if ( ShouldUseDarkMode )
                    return ThemeAccentDark2;
                return ThemeAccentLight2;
            }
        }

        /// <summary>
        /// Closest colour to theme (ie in light mode its close to white, in dark mode close to black)
        /// </summary>
        public static WpfColor ThemeAccent3 {
            get {
                if ( ShouldUseDarkMode )
                    return ThemeAccentDark3;
                return ThemeAccentLight3;
            }
        }

        #endregion


        public static bool ShouldUseDarkMode {
            get {
                if ( WindowsUtils.GetVersion().Major > ( int ) WindowsUtils.WindowsMajorReleases.Win10_1903 ) {
                    return fnShouldSystemUseDarkMode();
                }
                return fnShouldAppsUseDarkMode();
            }
        }
    }
}
