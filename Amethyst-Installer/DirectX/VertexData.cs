using System.Numerics;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    struct VertexData {
        Vector3 position;
        Vector4 color;

        public VertexData(Vector3 position, Vector4 color) {
            this.position = position;
            this.color = color;
        }
    }
}
