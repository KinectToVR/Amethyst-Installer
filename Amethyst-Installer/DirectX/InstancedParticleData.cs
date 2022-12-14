using System.Numerics;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    struct InstancedParticleData {
        // Vector4 position;
        Vector3 localPosition;
        Vector4 color;
        // X => Polar angle, Y => Unused , Z => Timing offset, W => Timing period 
        Vector4 animData;

        public InstancedParticleData(Vector4 position, Vector3 localPosition, Vector4 color, float polarAngle, float timingOffset, float timingPeriod, float other = 0) {
            // this.position = position;
            this.localPosition = localPosition;
            this.color = color;
            this.animData = new Vector4(polarAngle, other, timingOffset, timingPeriod);
        }
    }
}
