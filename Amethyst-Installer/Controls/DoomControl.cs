using amethyst_installer_gui.DirectX;
using ManagedDoom.SFML;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using D2DContext = SharpDX.Direct2D1.DeviceContext;

namespace amethyst_installer_gui.Controls
{
    public class DoomControl : D2DControl {

        public SfmlDoom doomGame;
        bool hasDrawEvent = false;
        byte[] texData = { };

        public DoomControl() {
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }
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
