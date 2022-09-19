using amethyst_installer_gui.PInvoke;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace amethyst_installer_gui {
    /// <summary>
    /// Utility class to fetch accent colors from Windows without having access to modern features
    /// </summary>
    public static class WindowsColorHelpers {

        private static readonly Color s_defaultColor = Color.FromRgb(24, 131, 215);

        public static Brush BorderAccent { get { return SystemParameters.WindowGlassBrush; } }
        public static Brush Accent { get { return new SolidColorBrush(GetAccentColor()); } }
        public static Brush AccentLight { get { return new SolidColorBrush(Lighten(GetAccentColor(), 0.1f)); } }
        public static Brush AccentText { get { return new SolidColorBrush(GetContrastingColor(GetAccentColor())); } }
        public static Brush AccentHighlightBackground { get { return new SolidColorBrush(GetAccentColor()); } }
        public static Brush AccentHighlightForeground { get { return new SolidColorBrush(GetContrastingColor(GetAccentColor())); } }

        // Extended from
        // https://stackoverflow.com/a/50848113
        public static Color GetAccentColor() {

            const string DWM_KEY = @"Software\Microsoft\Windows\DWM";
            using ( RegistryKey dwmKey = Registry.CurrentUser.OpenSubKey(DWM_KEY, RegistryKeyPermissionCheck.ReadSubTree) ) {
                if ( dwmKey is null ) {
                    // const string KEY_EX_MSG = "The \"HKCU\\" + DWM_KEY + "\" registry key does not exist.";
                    // throw new InvalidOperationException(KEY_EX_MSG);

                    // Fallback to default accent color: teal blue
                    return s_defaultColor;
                }

                object accentColorObj = dwmKey.GetValue("AccentColor");
                if ( accentColorObj is int accentColorDword ) {
                    Color col = ParseDWordColor(accentColorDword);

                    // Ensure the colour meets minimum contrast requirements
                    float luminosity = Luminosity(col); // 0.277641565
                    float bgColorLuminosity = 0.203921556f;

                    var brightest = Math.Max(luminosity, bgColorLuminosity);
                    var darkest = Math.Min(luminosity, bgColorLuminosity);
                    float contrast = ( brightest + 0.05f ) / ( darkest + 0.05f );

                    if ( contrast < 3.1f) {
                        // Calcualte the amount of additional luminosity required to achieve minimum contrast

                        /*
                        The formula for contrast assuming x is our colour and more luminous than bgColor is defined as:

                        y = (x + 0.05) / (0.203921556 + 0.5)

                        The recommended minimum contrast values for a colour are 3 or 4.5. Here, I target a contrast of 3.1 to be closer to 
                        the user's chosen colour.

                         */

                        float deltaLuminosity = 0.4f - luminosity;
                        ColorToHSL(col, out float H, out float S, out float L);
                        L = Math.Max(0.02f, Math.Min(0.98f, L + deltaLuminosity)); // clamp between 0.02 and 0.98
                        return ColorFromHSL(H, S, L);
                    }
                    return col;
                } else {
                    // const string VALUE_EX_MSG = "The \"HKCU\\" + DWM_KEY + "\\AccentColor\" registry key value could not be parsed as an ABGR color.";
                    // throw new InvalidOperationException(VALUE_EX_MSG);
                }
            }

            // Fallback to default accent color: teal blue
            return s_defaultColor;
        }

        public static Color Lighten(Color color, float amount = 0f) {
            ColorToHSL(color, out float H, out float S, out float L);
            S -= .3f;
            L += .04f;
            S = Math.Max(0.02f, Math.Min(0.98f, S)); // clamp between 0.02 and 0.98
            L = Math.Max(0.02f, Math.Min(0.98f, L)); // clamp between 0.02 and 0.98
            return ColorFromHSL(H, S, L);
        }

        /// <summary>
        /// Returns a color which maintains contrast with the given color. Useful for getting text colors
        /// </summary>
        public static Color GetContrastingColor(Color color) {
            return ( Luminosity(color) >= 165 ) ? Color.FromRgb(0, 0, 0) : Color.FromRgb(255, 255, 255);
        }

        /// <summary>
        /// Returns the luminosity of a color according to BT.709-1
        /// </summary>
        public static float Luminosity(Color color) {
            // Convert to 0 to 1 like a shader
            float colorR = color.R / 255f;
            float colorG = color.G / 255f;
            float colorB = color.B / 255f;

            // dot product with luma co-efficients for BT.709-1
            colorR *= 0.2125f;
            colorG *= 0.7154f;
            colorB *= 0.0721f;
            return colorR + colorG + colorB;
        }

        private static Color ParseDWordColor(int color) {
            byte
                // a = (byte)((color >> 24) & 0xFF),
                b = (byte)((color >> 16) & 0xFF),
                g = (byte)((color >> 8) & 0xFF),
                r = (byte)((color >> 0) & 0xFF);

            return Color.FromRgb(r, g, b);
        }

        /// <summary>
        /// Converts a color from RGB space to HSL space
        /// </summary>
        private static void ColorToHSL(Color color, out float H, out float S, out float L) {
            // normalize the RGB range to 0 - 1
            float normR = color.R / 255f;
            float normG = color.G / 255f;
            float normB = color.B / 255f;

            float min = Math.Min(normR, Math.Min(normG, normB));
            float max = Math.Max(normR, Math.Max(normG, normB));
            float delta = max - min;

            // min and max are equal if the colour is a shade of gray
            if ( min == max ) {
                H = 0;
                S = 0;
                L = max;
                return;
            }

            // thus colour isn't gray
            L = ( max + min ) / 2f;
            if ( L < 0.5f )
                S = delta / ( max + min );
            else
                S = delta / ( 2f - max - min );

            H = 0;
            if ( normR == max )
                H = ( normG - normB ) / delta;
            if ( normG == max )
                H = 2f + ( normB - normR ) / delta;
            if ( normB == max )
                H = 4f + ( normR - normG ) / delta;
            H *= 60f;
            if ( H < 0 )
                H += 360f;
        }

        /// <summary>
        /// Converts a color from HSL space to RGB space
        /// </summary>
        private static Color ColorFromHSL(float H, float S, float L) {
            // gray is a special case
            if ( S == 0 ) {
                byte lightnessAsByte = (byte)(L * 255f);
                return Color.FromRgb(lightnessAsByte, lightnessAsByte, lightnessAsByte);
            }

            float t1, t2;
            if ( L < 0.5f )
                t1 = L * ( 1f + S );
            else
                t1 = L + S - ( L * S );
            t2 = 2f * L - t1;
            float normH = H / 360f;

            float tR = normH + (1f/3f);
            float tB = normH - (1f/3f);

            return Color.FromRgb(EvalHSLTuple(t1, t2, tR), EvalHSLTuple(t1, t2, normH), EvalHSLTuple(t1, t2, tB));
        }

        private static byte EvalHSLTuple(float t1, float t2, float t3) {
            if ( t3 < 0f )
                t3 += 1f;
            if ( t3 > 1f )
                t3 -= 1f;

            float col;
            if ( 6f * t3 < 1f )
                col = t2 + ( t1 - t2 ) * 6f * t3;
            else if ( 2f * t3 < 1f )
                col = t1;
            else if ( 3f * t3 < 2f )
                col = t2 + ( t1 - t2 ) * 6f * ( 2f / 3f - t3 );
            else
                col = t2;

            // convert to byte and return
            return ( byte ) ( col * 255f );
        }
    }
}
