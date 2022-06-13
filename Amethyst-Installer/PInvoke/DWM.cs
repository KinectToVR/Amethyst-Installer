using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace amethyst_installer_gui.PInvoke {
    public static class DWM {

        [DllImport("Dwmapi.dll", SetLastError = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute, [In] ref uint pvAttribute, uint cbAttribute);

        private enum DWMWINDOWATTRIBUTE : uint {
            DWMWA_NCRENDERING_ENABLED = 1,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_PASSIVE_UPDATE_MODE,
            DWMWA_USE_HOSTBACKDROPBRUSH,
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
            DWMWA_BORDER_COLOR,
            DWMWA_CAPTION_COLOR,
            DWMWA_TEXT_COLOR,
            DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,
            DWMWA_LAST
        }

        private const int S_OK = 0x0;
        private const int S_FALSE = 0x1;

        public static bool SetWindowCorners(Window window, CornerPreference preference) {
            try {
                var windowHandle = new WindowInteropHelper(window).EnsureHandle();

                return SetWindowCorners(windowHandle, preference);
            } catch ( Exception ) {
                return false;
            }
        }

        public static bool SetWindowCorners(IntPtr windowHandle, CornerPreference preference) {
            var value = (uint)preference;

            return ( DwmSetWindowAttribute(windowHandle, ( uint ) DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, ref value, ( uint ) Marshal.SizeOf(value)) == S_OK );
        }
        public static bool SetWindowAccentColor(Window window, Color borderColor) {
            try {
                var windowHandle = new WindowInteropHelper(window).EnsureHandle();

                return SetWindowAccentColor(windowHandle, borderColor);
            } catch ( Exception ) {
                return false;
            }
        }

        public static bool SetWindowAccentColor(IntPtr windowHandle, Color borderColor) {

            // buffer for the current Accent Color ; this gives the window border a proper "tint"
            uint borderColorAsCOLORREF = 0x00FFFFFF;

            borderColorAsCOLORREF = ( ( uint ) 0x00FFFFFF & ( uint ) ( borderColor.R << 0 ) );
            borderColorAsCOLORREF |= ( ( uint ) 0x00FFFFFF & ( uint ) ( borderColor.G << 8 ) );
            borderColorAsCOLORREF |= ( ( uint ) 0x00FFFFFF & ( uint ) ( borderColor.B << 16 ) );

            return ( DwmSetWindowAttribute(windowHandle, ( uint ) DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR, ref borderColorAsCOLORREF, ( uint ) Marshal.SizeOf(borderColorAsCOLORREF)) == S_OK );
        }
    }

    public enum CornerPreference : uint {
        /// <summary>
        /// Let the system decide whether or not to round window corners.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Never round window corners.
        /// </summary>
        DoNotRound = 1,

        /// <summary>
        /// Round the corners if appropriate.
        /// </summary>
        Round = 2,

        /// <summary>
        /// Round the corners if appropriate, with a small radius.
        /// </summary>
        RoundSmall = 3
    }
}
