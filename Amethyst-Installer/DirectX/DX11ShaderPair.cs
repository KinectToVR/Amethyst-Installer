using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Device = SharpDX.Direct3D11.Device;
using DeviceContext = SharpDX.Direct3D11.DeviceContext;
using D2DContext = SharpDX.Direct2D1.DeviceContext;
using FeatureLevel = SharpDX.Direct3D.FeatureLevel;
using InputElement = SharpDX.Direct3D11.InputElement;

namespace amethyst_installer_gui.DirectX
{
    /// <summary>
    /// A thin wrapper around a DX11 Shader object
    /// </summary>
    public class DX11ShaderPair : IDisposable
    {
        // Used for caching purposes
        private string m_vertexShaderPath;
        private string m_pixelShaderPath;
        private byte[] m_vertexShaderBytes;
        private byte[] m_pixelShaderBytes;

        private Device m_device;
        private PixelShader m_pixelShaderProgram;
        private VertexShader m_vertexShaderProgram;
        private InputLayout m_inputLayout;

        public DX11ShaderPair(ref Device device, string vertexProgramPath, string pixelProgramPath, bool init = true) {
            m_device = device;
            m_vertexShaderPath = vertexProgramPath;
            m_pixelShaderPath = pixelProgramPath;

            if ( init )
                Recreate(ref device);
        }

        public void Bind() {
            // Set input layout
            m_device.ImmediateContext.InputAssembler.InputLayout = m_inputLayout;
            // Set vertex shader
            m_device.ImmediateContext.VertexShader.Set(m_vertexShaderProgram);
            // Set pixel shader
            m_device.ImmediateContext.PixelShader.Set(m_pixelShaderProgram);
        }

        public void Recreate(ref Device device) {
            Utilities.Dispose(ref m_pixelShaderProgram);
            Utilities.Dispose(ref m_vertexShaderProgram);
            m_device = device;

            // Create vertex program
            m_vertexShaderBytes = Util.ExtractResourceAsBytes(m_vertexShaderPath);
            m_vertexShaderProgram = new VertexShader(m_device, m_vertexShaderBytes);

            // Use reflection to construct the vertex layout dynamically
            // This approach is better than maintaining two vertex layouts
            // (one in the shader, and one here) as it's less error prone
            // and easier to maintain
            // 
            // Shockingly, we can use this in prod because we dont need the compiler
            List<InputElement> dynamicVertexLayout = new List<InputElement>();

            var reflectionData = new SharpDX.D3DCompiler.ShaderReflection(m_vertexShaderBytes);
            int offset = 0;
            for (int i = 0; i < reflectionData.Description.InputParameters; i++) {
                var inputParam = reflectionData.GetInputParameterDescription(i);
                Format elementFormat = Format.Unknown;

                // Count components
                int elementComponentCount = 0;
                if ( ( inputParam.UsageMask & SharpDX.D3DCompiler.RegisterComponentMaskFlags.ComponentX ) == SharpDX.D3DCompiler.RegisterComponentMaskFlags.ComponentX ) {
                    elementComponentCount++;
                }
                if ( ( inputParam.UsageMask & SharpDX.D3DCompiler.RegisterComponentMaskFlags.ComponentY ) == SharpDX.D3DCompiler.RegisterComponentMaskFlags.ComponentY ) {
                    elementComponentCount++;
                }
                if ( ( inputParam.UsageMask & SharpDX.D3DCompiler.RegisterComponentMaskFlags.ComponentZ ) == SharpDX.D3DCompiler.RegisterComponentMaskFlags.ComponentZ ) {
                    elementComponentCount++;
                }
                if ( ( inputParam.UsageMask & SharpDX.D3DCompiler.RegisterComponentMaskFlags.ComponentW ) == SharpDX.D3DCompiler.RegisterComponentMaskFlags.ComponentW ) {
                    elementComponentCount++;
                }

                // Get size in bytes of element, and select the target format
                int sizeOfElement = 0;
                switch ( inputParam.ComponentType ) {
                    case SharpDX.D3DCompiler.RegisterComponentType.Float32:
                        sizeOfElement = 4;
                        switch ( elementComponentCount ) {
                            case 1:
                                elementFormat = Format.R32_Float;
                                break;
                            case 2:
                                elementFormat = Format.R32G32_Float;
                                break;
                            case 3:
                                elementFormat = Format.R32G32B32_Float;
                                break;
                            case 4:
                                elementFormat = Format.R32G32B32A32_Float;
                                break;
                        }
                        break;
                    case SharpDX.D3DCompiler.RegisterComponentType.SInt32:
                        sizeOfElement = 4;
                        switch ( elementComponentCount ) {
                            case 1:
                                elementFormat = Format.R32_SInt;
                                break;
                            case 2:
                                elementFormat = Format.R32G32_SInt;
                                break;
                            case 3:
                                elementFormat = Format.R32G32B32_SInt;
                                break;
                            case 4:
                                elementFormat = Format.R32G32B32A32_SInt;
                                break;
                        }
                        break;
                    case SharpDX.D3DCompiler.RegisterComponentType.UInt32:
                        sizeOfElement = 4;
                        switch ( elementComponentCount ) {
                            case 1:
                                elementFormat = Format.R32_UInt;
                                break;
                            case 2:
                                elementFormat = Format.R32G32_UInt;
                                break;
                            case 3:
                                elementFormat = Format.R32G32B32_UInt;
                                break;
                            case 4:
                                elementFormat = Format.R32G32B32A32_UInt;
                                break;
                        }
                        break;
                }

                var elem = new InputElement(inputParam.SemanticName, inputParam.SemanticIndex, elementFormat, offset, 0, InputClassification.PerVertexData, 0);
                offset += sizeOfElement * elementComponentCount;
                dynamicVertexLayout.Add(elem);
            }

            m_inputLayout = new InputLayout(m_device, m_vertexShaderBytes, dynamicVertexLayout.ToArray());

            // Bind the vertex struct layout
            // var vertexLayout = new[]
            // {
            //     new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
            //     new InputElement("COLOR", 0, Format.R32G32B32_Float, 12, 0, InputClassification.PerVertexData, 0),
            // };
            // m_inputLayout = new InputLayout(m_device, m_vertexShaderBytes, vertexLayout);

            // Create pixel program
            m_pixelShaderBytes = Util.ExtractResourceAsBytes(m_pixelShaderPath);
            m_pixelShaderProgram = new PixelShader(m_device, m_pixelShaderBytes);
        }

        public void Dispose()
        {
            Utilities.Dispose(ref m_vertexShaderProgram);
            Utilities.Dispose(ref m_inputLayout);
            Utilities.Dispose(ref m_pixelShaderProgram);
        }
    }
}
