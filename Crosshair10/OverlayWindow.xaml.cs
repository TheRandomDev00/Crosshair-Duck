using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Crosshair10
{
    public partial class OverlayWindow : Window
    {
        // remember last settings (used if window resizes)
        private double _lastSize;
        private double _lastThickness;
        private double _lastGap;
        private double _lastOutlineThickness;   // ✅ new
        private Color _lastColor;

        public OverlayWindow()
        {
            InitializeComponent();

            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Left = 0;
            Top = 0;

            // re-draw if the canvas changes size
            CrosshairCanvas.SizeChanged += (_, __) => RedrawCrosshair();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // your click-through style (if you already added it)
            var hwnd = new WindowInteropHelper(this).Handle;
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            exStyle |= WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW;
            SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
        }

        const int GWL_EXSTYLE = -20;
        const int WS_EX_TRANSPARENT = 0x20;
        const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        // 🔹 called from MainWindow whenever user changes settings
        public void UpdateCrosshair(double size, double thickness, double gap, double outlineThickness, Color color)
        {
            _lastSize = size;
            _lastThickness = thickness;
            _lastGap = gap;
            _lastOutlineThickness = outlineThickness;
            _lastColor = color;

            RedrawCrosshair();
        }

        private void RedrawCrosshair()
        {
            if (CrosshairCanvas.ActualWidth == 0 || CrosshairCanvas.ActualHeight == 0)
                return;

            double centerX = CrosshairCanvas.ActualWidth / 2;
            double centerY = CrosshairCanvas.ActualHeight / 2;

            var innerBrush = new SolidColorBrush(_lastColor);
            var outlineBrush = new SolidColorBrush(Colors.Black);

            bool outlineOn = _lastOutlineThickness > 0.1;

            // ===== OUTLINE ARMS (slightly bigger rectangles under the color) =====
            if (outlineOn)
            {
                // LEFT outline
                OutlineLeft.Width = _lastSize + 2 * _lastOutlineThickness;
                OutlineLeft.Height = _lastThickness + 2 * _lastOutlineThickness;
                OutlineLeft.Fill = outlineBrush;
                Canvas.SetLeft(OutlineLeft, centerX - _lastGap - _lastSize - _lastOutlineThickness);
                Canvas.SetTop(OutlineLeft, centerY - (_lastThickness / 2) - _lastOutlineThickness);

                // RIGHT outline
                OutlineRight.Width = _lastSize + 2 * _lastOutlineThickness;
                OutlineRight.Height = _lastThickness + 2 * _lastOutlineThickness;
                OutlineRight.Fill = outlineBrush;
                Canvas.SetLeft(OutlineRight, centerX + _lastGap - _lastOutlineThickness);
                Canvas.SetTop(OutlineRight, centerY - (_lastThickness / 2) - _lastOutlineThickness);

                // TOP outline
                OutlineTop.Width = _lastThickness + 2 * _lastOutlineThickness;
                OutlineTop.Height = _lastSize + 2 * _lastOutlineThickness;
                OutlineTop.Fill = outlineBrush;
                Canvas.SetLeft(OutlineTop, centerX - (_lastThickness / 2) - _lastOutlineThickness);
                Canvas.SetTop(OutlineTop, centerY - _lastGap - _lastSize - _lastOutlineThickness);

                // BOTTOM outline
                OutlineBottom.Width = _lastThickness + 2 * _lastOutlineThickness;
                OutlineBottom.Height = _lastSize + 2 * _lastOutlineThickness;
                OutlineBottom.Fill = outlineBrush;
                Canvas.SetLeft(OutlineBottom, centerX - (_lastThickness / 2) - _lastOutlineThickness);
                Canvas.SetTop(OutlineBottom, centerY + _lastGap - _lastOutlineThickness);

                OutlineLeft.Visibility = Visibility.Visible;
                OutlineRight.Visibility = Visibility.Visible;
                OutlineTop.Visibility = Visibility.Visible;
                OutlineBottom.Visibility = Visibility.Visible;
            }
            else
            {
                OutlineLeft.Visibility = Visibility.Collapsed;
                OutlineRight.Visibility = Visibility.Collapsed;
                OutlineTop.Visibility = Visibility.Collapsed;
                OutlineBottom.Visibility = Visibility.Collapsed;
            }

            // ===== INNER COLORED ARMS (your original code) =====

            // LEFT line
            LineLeft.Width = _lastSize;
            LineLeft.Height = _lastThickness;
            LineLeft.Fill = innerBrush;
            Canvas.SetLeft(LineLeft, centerX - _lastGap - _lastSize);
            Canvas.SetTop(LineLeft, centerY - _lastThickness / 2);

            // RIGHT line
            LineRight.Width = _lastSize;
            LineRight.Height = _lastThickness;
            LineRight.Fill = innerBrush;
            Canvas.SetLeft(LineRight, centerX + _lastGap);
            Canvas.SetTop(LineRight, centerY - _lastThickness / 2);

            // TOP line
            LineTop.Width = _lastThickness;
            LineTop.Height = _lastSize;
            LineTop.Fill = innerBrush;
            Canvas.SetLeft(LineTop, centerX - _lastThickness / 2);
            Canvas.SetTop(LineTop, centerY - _lastGap - _lastSize);

            // BOTTOM line
            LineBottom.Width = _lastThickness;
            LineBottom.Height = _lastSize;
            LineBottom.Fill = innerBrush;
            Canvas.SetLeft(LineBottom, centerX - _lastThickness / 2);
            Canvas.SetTop(LineBottom, centerY + _lastGap);
        }

    }
}
