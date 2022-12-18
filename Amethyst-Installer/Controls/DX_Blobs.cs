using amethyst_installer_gui.DirectX;
using amethyst_installer_gui.DirectX.Renderdoc;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Numerics;
using DX11Buffer = SharpDX.Direct3D11.Buffer;
using D2DContext = SharpDX.Direct2D1.DeviceContext;

namespace amethyst_installer_gui.Controls {

    public class DX_Blobs : D2DControl, IDisposable {
        private RenderDoc rdoc;
        public bool doCapture = false;
        const int particleCount = 2048;

        private const float ANIM_OFFSET_MIN = 0;
        private const float ANIM_OFFSET_MAX = 1;
        private const float ANIM_PERIOD_MIN = 0.2f;
        private const float ANIM_PERIOD_MAX = 1;
        private const float ANIM_POLAR_ANGLE_MIN = 0;
        private const float ANIM_POLAR_ANGLE_MAX = 1;
        private const float SCALE_MIN = 0.01f;
        private const float SCALE_MAX = 0.25f;

        private DX11ShaderPair m_shaders;
        private DX11Buffer m_vertexBuffer;
        private DX11Buffer m_indexBuffer;
        private DX11Buffer m_instanceBuffer;

        private Random rng;

        public DX_Blobs() {
            rng = new Random();
            try {
                RenderDoc.Load(out rdoc);
            } catch ( Exception e ) { }
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        public override void TargetsCreated() {
            // Called whenever we must create resources
            // This includes initialization

            // Setup shaders
            if ( m_shaders == null )
                // Don't init the native stuff because we'll do it after exiting the this scope
                m_shaders = new DX11ShaderPair(ref device, "Shaders.simple_vert.cso", "Shaders.simple_frag.cso", true, false);
            m_shaders.Recreate(ref device);

            // Setup per-particle data
            var data = new InstancedParticleData[particleCount];

            for ( int i = 0; i < particleCount; i++ ) {

                // Sampling is done on a poisson disk, where the distance is sqrt(1-x), which is a good enough analogue to a
                // probability distribution function

                const float revolutions = 20.0f;

                float t = i / (float) particleCount;
                float distX = (float) Math.Sin(2.0 * Math.PI * t * revolutions);
                float distY = (float) Math.Cos(2.0 * Math.PI * t * revolutions);
                float magnitude = ( float ) Math.Sqrt(1.0 - t);

                distX *= magnitude;
                distY *= magnitude;

                data[i] = new InstancedParticleData(
                    localPosition:  new Vector3(distX, distY, 0.0f),
                    color:          new Vector3(rng.NextFloat(), rng.NextFloat(), rng.NextFloat()),

                    // Attributes
                    polarAngle:     rng.NextFloat(ANIM_POLAR_ANGLE_MIN, ANIM_POLAR_ANGLE_MAX),
                    timingOffset:   rng.NextFloat(ANIM_OFFSET_MIN, ANIM_OFFSET_MAX),
                    timingPeriod:   rng.NextFloat(ANIM_PERIOD_MIN, ANIM_PERIOD_MAX),
                    scale:          rng.NextFloat(SCALE_MIN, SCALE_MAX));
            }

            // Particle meshes (a quad lol)
            var positions = new Vector4[] {
                new Vector4(-1.0f, +1.0f, 0.5f, 0.0f),
                new Vector4(+1.0f, +1.0f, 0.5f, 0.0f),
                new Vector4(-1.0f, -1.0f, 0.5f, 0.0f),
                new Vector4(+1.0f, -1.0f, 0.5f, 0.0f),
            };
            m_vertexBuffer = DX11Buffer.Create(device, BindFlags.VertexBuffer, positions);
            m_instanceBuffer = DX11Buffer.Create(device, BindFlags.VertexBuffer, data);
            // 16-bit integers to occupy less memory
            m_indexBuffer = DX11Buffer.Create(device, BindFlags.IndexBuffer, new ushort[] { 0, 1, 2, 3, 2, 1 });

        }

        public override void Render(D2DContext target) {

            // Called every frame
            if ( rdoc != null && doCapture ) {
                if ( !rdoc.IsFrameCapturing() ) {
                    rdoc.StartFrameCapture();
                }
                doCapture = false;
            }

            // This is really odd but supposedly this renders a full screen quad
            if ( renderView != null ) {
                device.ImmediateContext.OutputMerger.SetTargets(renderView);
                device.ImmediateContext.ClearRenderTargetView(renderView, new RawColor4(0.0f, 0.0f, 0.0f, 0.0f));
            }
            // Stride = 8 elements * 4 bytes (per float) = 32 bytes
            // Bind to the device context
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0,
                new VertexBufferBinding(m_vertexBuffer, Utilities.SizeOf<Vector4>(), 0),                // Per-vertex data
                new VertexBufferBinding(m_instanceBuffer, Utilities.SizeOf<InstancedParticleData>(), 0) // Per-instance data
            );
            device.ImmediateContext.InputAssembler.SetIndexBuffer(m_indexBuffer, Format.R16_UInt, 0);
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            
            m_shaders.Bind();

            device.ImmediateContext.DrawIndexedInstanced(6, particleCount, 0, 0, 0);

            if ( rdoc != null && rdoc.IsFrameCapturing() ) {
                rdoc.EndFrameCapture();
            }
        }

        public new void Dispose() {
            m_vertexBuffer?.Dispose();
            m_indexBuffer?.Dispose();
            m_shaders?.Dispose();
            base.Dispose();
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e) {
            Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
            Dispose();
        }

        private static float DistributionFunction(float t) {
            return ( float ) ( 1.65 * Math.Sqrt(-2.0 * Math.Log(t)) * Math.Sin(2 * Math.PI * t * 0.5) );
        }
    }
}