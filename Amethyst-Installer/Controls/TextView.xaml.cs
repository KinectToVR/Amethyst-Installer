using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace amethyst_installer_gui.Controls {
    /// <summary>
    /// Interaction logic for TextView.xaml
    /// </summary>
    public partial class TextView : UserControl {

        public string Text {
            get { return m_text; }
            set { m_text = value; m_isDirty = true; }
        }
        public double ComputedHeight {
            get { return m_height; }
            set { m_height = value; m_isDirty = true; }
        }
        public Typeface TypeFace {
            get { return m_typeFace; }
            set { m_typeFace = value; m_isDirty = true; }
        }
        public List<FontSizeFormatting> FontSizes {
            get { return m_fontSizes; }
            set { m_fontSizes = value; m_isDirty = true; }
        }
        public List<FontColorFormatting> FontColors {
            get { return m_fontColors; }
            set { m_fontColors = value; m_isDirty = true; }
        }
        public double HeightOffset = 0;

        public EventHandler OnRecomputedValues;

        private string m_text = string.Empty;
        private Typeface m_typeFace = new Typeface("Consolas");
        private List<FontSizeFormatting> m_fontSizes = new List<FontSizeFormatting>();
        private List<FontColorFormatting> m_fontColors = new List<FontColorFormatting>();

        private bool m_isDirty = true;
        private double m_height = 0.0;
        private double m_dpi = 0.0;
        private FormattedText m_computedTextFormatting;

        public TextView() {
            InitializeComponent();
            DefaultStyleKey = typeof(TextView);
        }

        protected override void OnRender(DrawingContext drawingContext) {

            if ( m_isDirty )
                RecomputeTextInternal();

            // TODO: Rewrite the text rendering using this sample as a base
            // https://github.com/microsoft/WPF-Samples/blob/master/PerMonitorDPI/TextFormatting/

            // Draw the formatted text string to the DrawingContext of the control.
            // TODO: Render a small portion of the text, we know 90% of it is out of view anyway so why bother rendering the rest of the massive string
            drawingContext.DrawText(m_computedTextFormatting, new Point(0, HeightOffset));
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            // Compute the full text
            if ( m_isDirty )
                RecomputeTextInternal();
        }

        private void RecomputeTextInternal() {

            m_dpi = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            // Create the initial formatted text string.
            m_computedTextFormatting = new FormattedText(Text, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, TypeFace, FontSize, Brushes.White, m_dpi);

            // Set a maximum width and height. If the text overflows these values, an ellipsis "..." appears.
            m_computedTextFormatting.MaxTextWidth = ActualWidth;
            m_computedTextFormatting.MaxTextHeight = double.PositiveInfinity;

            for ( int i = 0; i < FontSizes.Count; i++ ) {
                m_computedTextFormatting.SetFontSize(FontSizes[i].TargetFontSize, FontSizes[i].Start, FontSizes[i].Length);
            }
            for ( int i = 0; i < FontColors.Count; i++ ) {
                m_computedTextFormatting.SetForegroundBrush(FontColors[i].TargetBrush, FontColors[i].Start, FontColors[i].Length);
            }

            m_height = (m_computedTextFormatting?.Height ?? 0) * m_dpi;

            OnRecomputedValues?.Invoke(this, null);
        }
    }

    public struct FontSizeFormatting {
        public int Start;
        public int Length;
        public double TargetFontSize;
    }

    public struct FontColorFormatting {
        public int Start;
        public int Length;
        public SolidColorBrush TargetBrush;
    }
}
