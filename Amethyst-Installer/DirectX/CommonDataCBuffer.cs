using System.Numerics;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.DirectX {
    [StructLayout(LayoutKind.Sequential)]
    public struct CommonDataCBuffer {
        public Vector2 Time; // X => Elapsed Time ; Y => DeltaTime
        public Vector2 ScreenResolution; // Screen resolution in pixels
    };
}
