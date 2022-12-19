using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using Device = SharpDX.Direct3D11.Device;
using DeviceContext = SharpDX.Direct2D1.DeviceContext;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace amethyst_installer_gui.DirectX {
    public static class Texture2DLoader {
        private static readonly ImagingFactory Imgfactory = new ImagingFactory();

        public static Texture2D LoadFromResource(ref Device device, string resourcePath) {
            var bSource = LoadBitmap(resourcePath);
            return LoadFromBitmap(ref device, bSource);
        }

        private static BitmapSource LoadBitmap(string resourcePath) {
            var d = new BitmapDecoder(Imgfactory, Util.ExtractResourceAsStream(resourcePath), DecodeOptions.CacheOnDemand);

            var frame = d.GetFrame(0);
            var fconv = new FormatConverter(Imgfactory);

            fconv.Initialize(
                frame,
                PixelFormat.Format32bppPRGBA,
                BitmapDitherType.None, null,
                0.0, BitmapPaletteType.Custom);
            return fconv;
        }

        private static Texture2D LoadFromBitmap(ref Device device, BitmapSource bSource) {

            Texture2DDescription desc;
            desc.Width                      = bSource.Size.Width;
            desc.Height                     = bSource.Size.Height;
            desc.ArraySize                  = 1;
            desc.BindFlags                  = BindFlags.ShaderResource;
            desc.Usage                      = ResourceUsage.Default;
            desc.CpuAccessFlags             = CpuAccessFlags.None;
            desc.Format                     = Format.R8G8B8A8_UNorm;
            desc.MipLevels                  = 1;
            desc.OptionFlags                = ResourceOptionFlags.None;
            desc.SampleDescription.Count    = 1;
            desc.SampleDescription.Quality  = 0;

            var dataStream = new DataStream(bSource.Size.Height * bSource.Size.Width * 4, true, true);
            bSource.CopyPixels(bSource.Size.Width * 4, dataStream);
            
            var rect = new DataRectangle(dataStream.DataPointer, bSource.Size.Width * 4);

            Texture2D tex = new Texture2D(device, desc, rect);

            return tex;
        }
    }
}
