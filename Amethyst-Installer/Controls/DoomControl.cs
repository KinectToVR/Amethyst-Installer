using amethyst_installer_gui.DirectX;
using ManagedDoom.SFML;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using D2DContext = SharpDX.Direct2D1.DeviceContext;

namespace amethyst_installer_gui.Controls {
    public class DoomControl : D2DControl {

        private float x = 0;
        private float y = 0;
        private float w = 10;
        private float h = 10;
        private float dx = 1;
        private float dy = 1;

        private Random rnd = new Random();

        public SfmlDoom doomGame;
        bool hasDrawEvent = false;
        byte[] texData = { };

        public DoomControl() {
            resCache.Add("RedBrush", t => new SolidColorBrush(t, new RawColor4(1.0f, 0.0f, 0.0f, 1.0f)));
            resCache.Add("GreenBrush", t => new SolidColorBrush(t, new RawColor4(0.0f, 1.0f, 0.0f, 1.0f)));
            resCache.Add("BlueBrush", t => new SolidColorBrush(t, new RawColor4(0.0f, 0.0f, 1.0f, 1.0f)));
        }
        private SharpDX.Direct2D1.Factory Factory2D;
        private RenderTarget RenderTarget2D;
        BitmapRenderTarget bmp;

        public override void Render(D2DContext target) {

            if ( doomGame == null )
                return;

            if ( !hasDrawEvent ) {
                doomGame.window.OnDraw += Draw;
                hasDrawEvent = true;
                bmp = new BitmapRenderTarget(d2DRenderTarget, CompatibleRenderTargetOptions.None);
            }

            doomGame.OnFrame();

            // target.Clear(new RawColor4(1.0f, 1.0f, 1.0f, 1.0f));
            Brush brush = null;
            switch ( rnd.Next(3) ) {
                case 0:
                    brush = resCache["RedBrush"] as Brush;
                    break;
                case 1:
                    brush = resCache["GreenBrush"] as Brush;
                    break;
                case 2:
                    brush = resCache["BlueBrush"] as Brush;
                    break;
            }
            // target.DrawRectangle(new RawRectangleF(x, y, x + w, y + h), brush);

            x = x + dx;
            y = y + dy;
            if ( x >= ActualWidth - w || x <= 0 ) {
                dx = -dx;
            }
            if ( y >= ActualHeight - h || y <= 0 ) {
                dy = -dy;
            }

            if ( texData != null && texData.Length > 0 ) {
                lock ( texData ) {
                    bmp.Bitmap.CopyFromMemory(texData, doomGame.video.renderer.Width * 4);
                }
                target.DrawBitmap(
                    bmp.Bitmap,
                    new RawRectangleF(0, 0, ( int ) ActualWidth, ( int ) ActualHeight), 1,
                    BitmapInterpolationMode.NearestNeighbor,
                    new RawRectangleF(0, 0, doomGame.video.renderer.Width, doomGame.video.renderer.Height));
            }
        }

        private void Draw(ref byte[] textureData) {
            lock ( texData ) {
                lock ( textureData ) {
                    if ( texData.Length != textureData.Length ) {
                        Array.Resize(ref texData, textureData.Length);
                    }
                    // Society:
                    // GCHandle pinnedTargetData = GCHandle.Alloc(texData, GCHandleType.Pinned);
                    // Marshal.Copy(textureData, 0, pinnedTargetData.AddrOfPinnedObject(), textureData.Length);
                    // pinnedTargetData.Free();

                    // Reality:
                    for ( int i = 0; i < textureData.Length; i += 4 ) {

                        /* The game renders in the following manner:

                                H
                        |----------------|
                        |00            01|
                        |                |
                        |                |
                        |                | W
                        |                |
                        |                |
                        |                |
                        |10____________11|
                        
                        */

                        int x = (i / 4) % doomGame.video.renderer.Height;
                        int y = (i / 4) / doomGame.video.renderer.Height;

                        int newXYIndexedCoord = (x * doomGame.video.renderer.Width + y) * 4;

                        // We need to swap bytes because of differing texture formats
                        texData[newXYIndexedCoord + 0] = textureData[i + 2];
                        texData[newXYIndexedCoord + 1] = textureData[i + 1];
                        texData[newXYIndexedCoord + 2] = textureData[i + 0];
                        texData[newXYIndexedCoord + 3] = textureData[i + 3];
                    }
                }
            }
        }
    }
}
