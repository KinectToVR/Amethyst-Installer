using amethyst_installer_gui.DirectX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace amethyst_installer_gui.Controls {
    public class DX_Blobs : D2DControl {

        private float x = 0;
        private float y = 0;
        private float w = 10;
        private float h = 10;
        private float dx = 1;
        private float dy = 1;

        private Random rnd = new Random();

        public DX_Blobs() {
            resCache.Add("RedBrush", t => new SolidColorBrush(t, new RawColor4(1.0f, 0.0f, 0.0f, 1.0f)));
            resCache.Add("GreenBrush", t => new SolidColorBrush(t, new RawColor4(0.0f, 1.0f, 0.0f, 1.0f)));
            resCache.Add("BlueBrush", t => new SolidColorBrush(t, new RawColor4(0.0f, 0.0f, 1.0f, 1.0f)));
        }

        public override void Render(DeviceContext target) {
            Application.Current.Shutdown();
            target.Clear(new RawColor4(1.0f, 1.0f, 1.0f, 1.0f));
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
            target.DrawRectangle(new RawRectangleF(x, y, x + w, y + h), brush);

            x = x + dx;
            y = y + dy;
            if ( x >= ActualWidth - w || x <= 0 ) {
                dx = -dx;
            }
            if ( y >= ActualHeight - h || y <= 0 ) {
                dy = -dy;
            }
        }
    }
}
