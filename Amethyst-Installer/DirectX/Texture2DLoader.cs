using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.WIC;
using System.IO;
using System.Text;
using Device = SharpDX.Direct3D11.Device;
using DeviceContext = SharpDX.Direct2D1.DeviceContext;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace amethyst_installer_gui.DirectX {
    public static class Texture2DLoader {
        public static Texture2D LoadFromResource(ref Device device, string resourcePath) {
            var tempPath = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, Path.GetFileName(resourcePath)));
            var bSource = LoadBitmap(resourcePath, ref tempPath);
            return LoadFromBitmap(ref device, bSource);
        }

        private static BitmapSource LoadBitmap(string resourcePath, ref string tempPath) {
            if (!File.Exists(tempPath)) {
                Util.ExtractResourceToFile(resourcePath, tempPath);
            }
            var imgFactory = new ImagingFactory();
            var decoder = new BitmapDecoder(imgFactory, tempPath, DecodeOptions.CacheOnDemand);
            var fconv = new FormatConverter(imgFactory);

            fconv.Initialize(
                decoder.GetFrame(0),
                PixelFormat.Format32bppPRGBA,
                BitmapDitherType.None,
                null,
                0.0,
                BitmapPaletteType.Custom);

            return fconv;
        }

        private static Texture2D LoadFromBitmap(ref Device device, BitmapSource bSource) {

            int stride = bSource.Size.Width * 4;

            using ( var buffer = new DataStream(bSource.Size.Height * stride, true, true) ) {

                Texture2DDescription desc = new Texture2DDescription() {
                    Width                       = bSource.Size.Width,
                    Height                      = bSource.Size.Height,
                    ArraySize                   = 1,
                    BindFlags                   = BindFlags.ShaderResource,
                    Usage                       = ResourceUsage.Immutable,
                    CpuAccessFlags              = CpuAccessFlags.None,
                    Format                      = Format.R8G8B8A8_UNorm,
                    MipLevels                   = 1,
                    OptionFlags                 = ResourceOptionFlags.None,
                    SampleDescription           = new SampleDescription(1, 0),
                };
                var rect = new DataRectangle(buffer.DataPointer, stride);

                bSource.CopyPixels(stride, buffer);
                return new Texture2D(device, desc, rect);
            }
        }
    }
}
