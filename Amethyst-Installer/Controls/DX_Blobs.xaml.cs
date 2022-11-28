using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace amethyst_installer_gui.Controls {
    /// <summary>
    /// Interaction logic for DX_Blobs.xaml
    /// </summary>
    public partial class DX_Blobs : UserControl {

        IntPtr m_handle;
        Direct3D m_d3d9;
        Surface m_surface;
        int m_width = 1920;
        int m_height = 1080;

        public DX_Blobs() {
            InitializeComponent();

            m_d3d9 = new Direct3D();
            m_handle = new WindowInteropHelper(Application.Current.MainWindow).Handle;

            //Create a device. Using standard creation param. 
            //Width and height have been set to 1 because we wont be using the backbuffer.
            //Adapter 0 = default adapter.
            PresentParameters presentationParams = new PresentParameters(1,1);
            Device d3dDevice = new Device(m_d3d9, 0, DeviceType.Hardware, m_handle, CreateFlags.HardwareVertexProcessing, presentationParams);
            
            //Create an empty offscreen surface. Use SystemMemory to allow for surface copying.
            if ( m_width > 0 && m_height > 0) {
                m_surface = Surface.CreateOffscreenPlain(d3dDevice, m_width, m_height, Format.A8R8G8B8, Pool.SystemMemory);
                //Fill the surface with the image data.
                Surface.FromFile(m_surface, @"F:\Projects\GitHub\amethyst-installer\Amethyst-Installer\icon.png", Filter.None, 0);

                //Create the surface that will act as the render target.
                //Set as lockable (required for D3DImage)
                Surface target = Surface.CreateRenderTarget(d3dDevice, m_width, m_height, Format.A8R8G8B8, MultisampleType.None, 0, true);

                //Copy the image surface contents into the target surface.
                d3dDevice.UpdateSurface(m_surface, target);

                this.wpfImageSource.Lock();
                this.wpfImageSource.SetBackBuffer(D3DResourceType.IDirect3DSurface9, target.NativePointer);
                this.wpfImageSource.AddDirtyRect(new Int32Rect(0, 0, wpfImageSource.PixelWidth, wpfImageSource.PixelHeight));
                this.wpfImageSource.Unlock();
            }

        }
    }
}
