using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace amethyst_installer_gui.PInvoke {
    public static class DWM {

        [DllImport("Dwmapi.dll", SetLastError = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute, [In] ref uint pvAttribute, uint cbAttribute);

        [DllImport("Dwmapi.dll", SetLastError = true)]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData attribute);

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
            DWMWA_SYSTEMBACKDROP_TYPE = 38,
            DWMWA_LAST,
            DWMWA_MICA_EFFECT = 1029,
        }

        [Flags]
        enum DWM_SYSTEMBACKDROP_TYPE : uint {
            DWMSBT_MAINWINDOW = 2,      // Mica
            DWMSBT_TRANSIENTWINDOW = 3, // Acrylic
            DWMSBT_TABBEDWINDOW = 4,    // Tabbed
            
            DWMSBT_MICAWINDOW = 2,      // Mica
            DWMSBT_ACRYLLICWINDOW = 3,  // Acryllic
        }

        private enum AccentState {
            ACCENT_DISABLED,
            ACCENT_ENABLE_GRADIENT,
            ACCENT_ENABLE_TRANSPARENTGRADIENT,
            ACCENT_ENABLE_BLURBEHIND,
            ACCENT_INVALID_STATE,
        }

        private enum WindowCompositionAttribute {
            WCA_ACCENT_POLICY = 19,
        }

        private const int S_OK = 0x0;
        private const int S_FALSE = 0x1;

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS {
            public int cxLeftWidth;      // width of left border that retains its size
            public int cxRightWidth;     // width of right border that retains its size
            public int cyTopHeight;      // height of top border that retains its size
            public int cyBottomHeight;   // height of bottom border that retains its size
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct AccentPolicy {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WindowCompositionAttributeData {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        /// <summary>
        /// Enables Acryllic on Windows 10, Mica on Windows 11
        /// </summary>
        public static bool EnableBackdropBlur(Window window) {
            try {
                Version windowsVersion = WindowsUtils.GetVersion();
                if ( windowsVersion.Major > 9 ) {

                    uint value;
                    IntPtr windowHandle = new WindowInteropHelper(window).EnsureHandle();

                    if ( windowsVersion.Build >= ( int ) WindowsUtils.WindowsMajorReleases.Win11_21H2 ) {
                        value = ( uint ) DWM_SYSTEMBACKDROP_TYPE.DWMSBT_MICAWINDOW;
                        if (windowsVersion.Build > 22522) {
                            // Win 11 on builds newer than 22523 (this will probably end up becoming 22H2)
                            return DwmSetWindowAttribute(windowHandle, ( uint ) DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE, ref value, ( uint ) Marshal.SizeOf(value)) == S_OK;
                        } else {
                            // On older version of Windows 11: we use this
                            value = 0x01;
                            return DwmSetWindowAttribute(windowHandle, ( uint ) DWMWINDOWATTRIBUTE.DWMWA_MICA_EFFECT, ref value, ( uint ) Marshal.SizeOf(value)) == S_OK;
                        }
                    } else {
                        // Win 10
                        AccentPolicy accent = new AccentPolicy {
                            AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND
                        };
                        int accentSize = Marshal.SizeOf(accent);
                        IntPtr accentPtr = Marshal.AllocHGlobal(accentSize);
                        Marshal.StructureToPtr(accent, accentPtr, false);
                        WindowCompositionAttributeData data = new WindowCompositionAttributeData{
                            Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                            SizeOfData = accentSize,
                            Data = accentPtr
                        };

                        int result = SetWindowCompositionAttribute(windowHandle, ref data);
                        Marshal.FreeHGlobal(accentPtr);
                        return false; // @TODO: Temporary since we need to rework the theme to work better with acrylic on win 10
                        return result == 1;
                    }

                }
            } catch { }

            return false;
        }

        public static bool SetWindowCorners(Window window, CornerPreference preference) {
            try {
                var windowHandle = new WindowInteropHelper(window).EnsureHandle();

                return SetWindowCorners(windowHandle, preference);
            } catch ( Exception ) {
                return false;
            }
        }

        public static bool SetDarkMode(Window window, bool darkMode) {
            try {
                var windowHandle = new WindowInteropHelper(window).EnsureHandle();

                uint value = darkMode ? 1U : 0U;
                return DwmSetWindowAttribute(windowHandle, (uint) DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, ( uint ) Marshal.SizeOf(value)) == S_OK;
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

        public static bool ExtendWindowChrome(Window window) {
            try {
                IntPtr mainWindowPtr = new WindowInteropHelper(window).EnsureHandle();
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                MARGINS margins = new MARGINS();
                margins.cxLeftWidth = -1;
                margins.cxRightWidth = -1;
                margins.cyTopHeight = -1;
                margins.cyBottomHeight = -1;

                return DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins) == S_OK;
            } catch { }
            return false;
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
