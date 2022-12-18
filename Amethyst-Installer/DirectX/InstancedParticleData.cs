using System.Numerics;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    struct InstancedParticleData {
        // Vector4 position;
        Vector3 localPosition;
        Vector3 color;
        // X => Polar angle, Y => Scale , Z => Timing offset, W => Timing period 
        Vector4 animData;

        public InstancedParticleData(Vector3 localPosition, Vector3 color, float polarAngle, float timingOffset, float timingPeriod, float scale) {
            // this.position = position;
            this.localPosition = localPosition;
            this.color = color;
            this.animData = new Vector4(polarAngle, scale, timingOffset, timingPeriod);
        }
    }
}
