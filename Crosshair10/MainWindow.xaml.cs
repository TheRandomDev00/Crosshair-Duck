using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Crosshair10
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()

        {
            InitializeComponent();

        }
        


        private void HeaderBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // ⬇️ Add this method BELOW the constructor
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();  // Fully closes the window
        }

        private void BtnMinimise_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized; //  the window
        }

        private OverlayWindow _overlay;

        private void ApplyCurrentCrosshairSettings()
        {
            if (_overlay == null)
                return; // overlay isn't open, nothing to update

            double size = SizeSlider.Value;
            double thickness = ThicknessSlider.Value;
            double gap = GapSlider.Value;

            // if outline is off, send 0 thickness
            double outlineThickness =
                (OutlineEnabledCheckBox?.IsChecked == true)
                    ? OutlineThicknessSlider.Value
                    : 0;

            Color color = Colors.Red;

            if (ColorCombo.SelectedItem is ComboBoxItem item)
            {
                var name = (string)item.Tag;
                color = (Color)ColorConverter.ConvertFromString(name);
            }

            _overlay.UpdateCrosshair(size, thickness, gap, outlineThickness, color);
        }

        // called whenever any slider or color changes
        private void CrosshairSettingsChanged(object sender, RoutedEventArgs e)
        {
            ApplyCurrentCrosshairSettings();
        }

        private void ShowOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (_overlay == null)
            {
                _overlay = new OverlayWindow();
                _overlay.Closed += (_, __) => _overlay = null;
                _overlay.Show();

                // apply current settings when overlay opens
                ApplyCurrentCrosshairSettings();
            }
            else
            {
                _overlay.Close();
            }
        }


    }


}