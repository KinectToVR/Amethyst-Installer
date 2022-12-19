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
using System.Diagnostics;

namespace amethyst_installer_gui.Controls {

    public class DX_Blobs : D2DControl, IDisposable {
        private RenderDoc rdoc;
        public bool doCapture = false;
        const int particleCount = 2048;

        private const float ANIM_OFFSET_MIN         = 0;
        private const float ANIM_OFFSET_MAX         = 1;
        private const float ANIM_PERIOD_MIN         = 0.2f;
        private const float ANIM_PERIOD_MAX         = 1;
        private const float ANIM_POLAR_ANGLE_MIN    = 0;
        private const float ANIM_POLAR_ANGLE_MAX    = 1;
        private const float SCALE_MIN               = 0.01f;
        private const float SCALE_MAX               = 0.25f;
        
        private static readonly Vector3 colorStart      = new Vector3(141.0f / 255.0f, 44.0f / 254.0f, 176.0f / 255.0f);
        private static readonly Vector3 colorMiddle     = new Vector3(59.0f / 255.0f, 192.0f / 255.0f, 254.0f / 255.0f);
        private static readonly Vector3 colorEnd        = new Vector3(185.0f / 255.0f, 70.0f / 255.0f, 209.0f / 255.0f);

        private DX11ShaderPair m_shaders;
        private DX11Buffer m_vertexBuffer;
        private DX11Buffer m_indexBuffer;
        private DX11Buffer m_instanceBuffer;
        private BlendState m_blendState;
        private DX11Buffer m_cbufferCommonData;

        private Random rng;

        private Texture2D m_gradientTexture;
        private ShaderResourceView m_gradientTextureView;

        private Stopwatch m_stopwatch;
        private double m_lastTime = 0;

        private float m_elapsedTime = 0.0f, m_deltaTime = 0.0f;
        private CommonDataCBuffer CommonData = new CommonDataCBuffer() {
            Time                = new Vector2(0, 1.0f / 30.0f),
            ScreenResolution    = new Vector2(1, 1)
        };

        public DX_Blobs() {
            rng = new Random();
            m_stopwatch = new Stopwatch();
            try {
                RenderDoc.Load(out rdoc);
            } catch ( Exception e ) { }
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        public override void TargetsCreated() {
            // Called whenever we must create resources
            // This includes initialization

            // Edge case on init
            if ( ActualHeight == 0 || ActualWidth == 0 ) {
                return;
            }

            // Setup shaders
            if ( m_shaders == null )
                // Don't init the native stuff because we'll do it after exiting the this scope
                m_shaders = new DX11ShaderPair(ref device, "Shaders.simple_vert.cso", "Shaders.simple_frag.cso", true, false);
            m_shaders.Recreate(ref device);

            // Setup per-particle data
            var data = new InstancedParticleData[particleCount];

            for ( int i = 0; i < particleCount; i++ ) {

                // Sampling is done on a disk, where the distance is sqrt(1-x), which is a good enough analogue to a
                // probability distribution function

                const float revolutions = 20.0f;

                float t = i / (float) particleCount;
                float distX = (float) Math.Sin(2.0 * Math.PI * t * revolutions);
                float distY = (float) (Math.Cos(2.0 * Math.PI * t * revolutions) * (ActualHeight / ActualWidth) - 0.05f);
                float magnitude = ( float ) Math.Sqrt(1.0 - t);

                distX *= magnitude;
                distY *= magnitude;

                float t1 = MathExtensions.Saturate(MathExtensions.Remap(0.0f, 0.543f, 0.0f, 1.0f, distX * 0.5f + 0.5f));
                float t2 = MathExtensions.Saturate(MathExtensions.Remap(0.543f, 1.0f, 0.0f, 1.0f, distX * 0.5f + 0.5f));

                data[i] = new InstancedParticleData(
                    localPosition:  new Vector3(distX, distY, rng.NextFloat(-0.1f, 0.1f)),
                    color:          MathExtensions.Lerp(colorStart, MathExtensions.Lerp(colorMiddle, colorEnd, t2), t1),

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
            m_vertexBuffer      = DX11Buffer.Create(device, BindFlags.VertexBuffer, positions);
            m_instanceBuffer    = DX11Buffer.Create(device, BindFlags.VertexBuffer, data);
            // 16-bit integers to occupy less memory
            m_indexBuffer       = DX11Buffer.Create(device, BindFlags.IndexBuffer, new ushort[] { 0, 1, 2, 3, 2, 1 });

            m_gradientTexture   = Texture2DLoader.LoadFromResource(ref device, "upgradeColorRamp.png");
            m_gradientTextureView = new ShaderResourceView(device, m_gradientTexture);

            // Prepare blend state for alpha blending
            BlendStateDescription blendStateDescription = new BlendStateDescription() {
                AlphaToCoverageEnable = false, // So far we don't need Alpha To Coverage
                IndependentBlendEnable = false,
            };
            blendStateDescription.RenderTarget[0].IsBlendEnabled            = true;
            blendStateDescription.RenderTarget[0].SourceBlend               = BlendOption.One;
            blendStateDescription.RenderTarget[0].DestinationBlend          = BlendOption.InverseSourceAlpha;
            blendStateDescription.RenderTarget[0].BlendOperation            = BlendOperation.Add;
            blendStateDescription.RenderTarget[0].SourceAlphaBlend          = BlendOption.One;
            blendStateDescription.RenderTarget[0].DestinationAlphaBlend     = BlendOption.InverseSourceAlpha;
            blendStateDescription.RenderTarget[0].AlphaBlendOperation       = BlendOperation.Add;
            blendStateDescription.RenderTarget[0].RenderTargetWriteMask     = ColorWriteMaskFlags.All;

            m_blendState                = new BlendState(device, blendStateDescription);

            // Init cbuffer data
            CommonData.Time             = new Vector2(m_elapsedTime, m_deltaTime);
            CommonData.ScreenResolution = new Vector2(( float ) ActualWidth, ( float ) ActualHeight);

            // cbuffer
            m_cbufferCommonData = DX11Buffer.Create(device,
                BindFlags.ConstantBuffer,
                ref CommonData,
                Utilities.SizeOf<CommonDataCBuffer>(),
                ResourceUsage.Dynamic,
                CpuAccessFlags.Write,
                ResourceOptionFlags.None,
                0);

            // Bind cbuffer
            device.ImmediateContext.VertexShader.SetConstantBuffer(0, m_cbufferCommonData);
            device.ImmediateContext.PixelShader.SetConstantBuffer(0, m_cbufferCommonData);

            m_stopwatch.Reset();
            m_stopwatch.Start();
        }

        public override void Render(D2DContext target) {

            // Called every frame
            if ( rdoc != null && doCapture ) {
                if ( !rdoc.IsFrameCapturing() ) {
                    rdoc.StartFrameCapture();
                }
                doCapture = false;
            }
            TimeSpan currentTimeMs  = m_stopwatch.Elapsed;
            m_deltaTime             = (float)( currentTimeMs.TotalSeconds - m_lastTime );
            m_elapsedTime           += m_deltaTime;

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
            device.ImmediateContext.OutputMerger.SetBlendState(m_blendState, new RawColor4(0,0,0,0), ~0);

            device.ImmediateContext.PixelShader.SetShaderResource(0, m_gradientTextureView);

            m_shaders.Bind();

            // Update common data cbuffer
            CommonData.Time                 = new Vector2(m_elapsedTime, m_deltaTime);
            CommonData.ScreenResolution     = new Vector2(( float ) ActualWidth, ( float ) ActualHeight);

            // Upload cbuffer to GPU
            DataStream stream;
            DataBox dataBox = device.ImmediateContext.MapSubresource(m_cbufferCommonData, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out stream);
            stream.WriteRange(new[] { CommonData });
            device.ImmediateContext.UnmapSubresource(m_cbufferCommonData, 0);
            stream.Dispose();

            // Actually issue the draw call
            device.ImmediateContext.DrawIndexedInstanced(6, particleCount, 0, 0, 0);

            m_lastTime = currentTimeMs.TotalSeconds;
            if ( rdoc != null && rdoc.IsFrameCapturing() ) {
                rdoc.EndFrameCapture();
                if (!rdoc.IsTargetControlConnected()) {
                    rdoc.LaunchReplayUI();
                }
            }
        }

        public new void Dispose() {
            m_gradientTexture?.Dispose();
            m_blendState?.Dispose();
            m_vertexBuffer?.Dispose();
            m_indexBuffer?.Dispose();
            m_shaders?.Dispose();
            m_stopwatch?.Stop();
            base.Dispose();
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e) {
            Dispatcher.ShutdownStarted -= Dispatcher_ShutdownStarted;
            Dispose();
        }
    }
}