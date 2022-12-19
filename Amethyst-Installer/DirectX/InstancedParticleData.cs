using System.Numerics;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    struct InstancedParticleData {
        // Vector4 position;
        Vector3 localPosition;
        Vector3 color;
        // X => Polar angle, Y => Scale , Z => Timing offset, W => Lifespan 
        Vector4 animData;
        // X => Speed ; Y => Unused ; Z => Unused ; W => Unused
        Vector4 animData2;

        public InstancedParticleData(Vector3 localPosition, Vector3 color, float polarAngle, float timingOffset, float lifespan, float scale, float speed) {
            // this.position = position;
            this.localPosition = localPosition;
            this.color = color;
            this.animData = new Vector4(polarAngle, scale, timingOffset, lifespan);
            this.animData2 = new Vector4(speed, 0, 0, 0);
        }
    }
}
