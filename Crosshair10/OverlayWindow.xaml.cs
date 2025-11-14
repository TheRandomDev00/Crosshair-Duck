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
        private double _lastSize = 60;
        private double _lastThickness = 2;
        private double _lastGap = 5;
        private Color _lastColor = Colors.Red;

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
        public void UpdateCrosshair(double size, double thickness, double gap, Color color)
        {
            _lastSize = size;
            _lastThickness = thickness;
            _lastGap = gap;
            _lastColor = color;

            RedrawCrosshair();
        }

        private void RedrawCrosshair()
        {
            if (CrosshairCanvas.ActualWidth == 0 || CrosshairCanvas.ActualHeight == 0)
                return;

            double centerX = CrosshairCanvas.ActualWidth / 2;
            double centerY = CrosshairCanvas.ActualHeight / 2;

            var brush = new SolidColorBrush(_lastColor);

            // LEFT line
            LineLeft.Width = _lastSize;
            LineLeft.Height = _lastThickness;
            LineLeft.Fill = brush;
            Canvas.SetLeft(LineLeft, centerX - _lastGap - _lastSize);
            Canvas.SetTop(LineLeft, centerY - _lastThickness / 2);

            // RIGHT line
            LineRight.Width = _lastSize;
            LineRight.Height = _lastThickness;
            LineRight.Fill = brush;
            Canvas.SetLeft(LineRight, centerX + _lastGap);
            Canvas.SetTop(LineRight, centerY - _lastThickness / 2);

            // TOP line
            LineTop.Width = _lastThickness;
            LineTop.Height = _lastSize;
            LineTop.Fill = brush;
            Canvas.SetLeft(LineTop, centerX - _lastThickness / 2);
            Canvas.SetTop(LineTop, centerY - _lastGap - _lastSize);

            // BOTTOM line
            LineBottom.Width = _lastThickness;
            LineBottom.Height = _lastSize;
            LineBottom.Fill = brush;
            Canvas.SetLeft(LineBottom, centerX - _lastThickness / 2);
            Canvas.SetTop(LineBottom, centerY + _lastGap);
        }
    }
}
