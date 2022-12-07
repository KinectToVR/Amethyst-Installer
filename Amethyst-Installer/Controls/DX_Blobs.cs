using amethyst_installer_gui.DirectX;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using DX11Buffer = SharpDX.Direct3D11.Buffer;
using D2DContext = SharpDX.Direct2D1.DeviceContext;

namespace amethyst_installer_gui.Controls {

    public class DX_Blobs : D2DControl, IDisposable {

        private DX11ShaderPair m_shaders;
        private DX11Buffer m_vertexBuffer;
        private DX11Buffer m_indexBuffer;

        private Random rnd = new Random();

        public DX_Blobs() {
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        public override void TargetsCreated() {
            // Called whenever we must create resources
            // This includes initialization

            // Setup shaders
            if ( m_shaders == null)
                // Don't init the native stuff because we'll do it after exiting the this scope
                m_shaders = new DX11ShaderPair(ref device, "Shaders.simple_vert.cso", "Shaders.simple_frag.cso", false);
            m_shaders.Recreate(ref device);

            // Construct a full screen quad in screenspace
            m_vertexBuffer = DX11Buffer.Create(device, BindFlags.VertexBuffer, new VertexData[]
            {
                // POSITION                                         COLOR
                new VertexData(new Vector3(-1.0f,  1.0f, 0.5f),     new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                new VertexData(new Vector3( 1.0f,  1.0f, 0.5f),     new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                new VertexData(new Vector3(-1.0f, -1.0f, 0.5f),     new Vector4(0.0f, 0.0f, 1.0f, 1.0f)),

                new VertexData(new Vector3( 1.0f, -1.0f, 0.5f),     new Vector4(0.5f, 0.0f, 0.5f, 1.0f))
            });
            // 16-bit integers to occupy less memory
            m_indexBuffer = DX11Buffer.Create(device, BindFlags.IndexBuffer,  new ushort[] { 0, 1, 2, 3, 2, 1 });
            
            // Stride = 8 elements * 4 bytes (per float) = 32 bytes
            // Bind to the device context
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(m_vertexBuffer, Utilities.SizeOf<VertexData>(), 0));
            device.ImmediateContext.InputAssembler.SetIndexBuffer(m_indexBuffer, Format.R16_UInt, 0);
            m_shaders.Bind();
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            device.ImmediateContext.OutputMerger.SetTargets(renderView);
        }

        public override void Render(D2DContext target) {

            // Called every frame

            // This is really odd but supposedly this renders a full screen quad
            if ( renderView != null ) {
                device.ImmediateContext.ClearRenderTargetView(renderView, new RawColor4(1.0f, 0.2f, 0.3f, 0.0f));
            }
            // device.ImmediateContext.Draw(3, 0);
            device.ImmediateContext.DrawIndexed(6, 0, 0);

            // DX11Context.DrawIndexed(0, 0, 0);

            // Instancing!
            // DX11Context.DrawIndexedInstanced();

            // target.DrawBitmap();
        }

        public new void Dispose() {
            m_vertexBuffer.Dispose();
            m_indexBuffer.Dispose();
            m_shaders.Dispose();
            base.Dispose();
        }
    }
}
