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
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using IOPath = System.IO.Path;

public class CrosshairPreset
{
    public string Name { get; set; } = "New";
    public double Size { get; set; }
    public double Thickness { get; set; }
    public double Gap { get; set; }
    public double OutlineThickness { get; set; }
    public string ColorHex { get; set; } = "#00FF00";
}

namespace Crosshair10
{

    /// Interaction logic for MainWindow.xaml

    public partial class MainWindow : Window
    {

        public MainWindow()

        {
            InitializeComponent();
            DataContext = this;
            LoadPresets();

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

        public ObservableCollection<CrosshairPreset> Presets { get; } = new ObservableCollection<CrosshairPreset>();

        private string PresetsPath =>
            IOPath.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Crosshair10", "presets.xml");

        private string GetSelectedColorHex()
        {
            string hex = "#00FF00"; // default lime
            if (ColorCombo.SelectedItem is ComboBoxItem cb && cb.Tag is string tagHex)
                hex = tagHex;
            return hex;
        }

        private void BtnSavePreset_Click(object sender, RoutedEventArgs e)
        {
            string name = string.IsNullOrWhiteSpace(PresetNameTextBox?.Text)
                ? $"New {Presets.Count + 1}"
                : PresetNameTextBox.Text.Trim();

            var p = new CrosshairPreset
            {
                Name = name,
                Size = SizeSlider.Value,
                Thickness = ThicknessSlider.Value,
                Gap = GapSlider.Value,
                OutlineThickness = OutlineThicknessSlider.Value,
                ColorHex = GetSelectedColorHex()
            };

            Presets.Add(p);
            SavePresets();
        }

        private void PresetApply_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is CrosshairPreset p)
            {
                // push values back to UI
                SizeSlider.Value = p.Size;
                ThicknessSlider.Value = p.Thickness;
                GapSlider.Value = p.Gap;
                OutlineThicknessSlider.Value = p.OutlineThickness;

                // select color in the ComboBox by Tag hex
                foreach (var obj in ColorCombo.Items)
                {
                    if (obj is ComboBoxItem cbi && cbi.Tag is string hex &&
                        hex.Equals(p.ColorHex, StringComparison.OrdinalIgnoreCase))
                    {
                        ColorCombo.SelectedItem = cbi;
                        break;
                    }
                }

                // update overlay if it’s open
                if (_overlay != null)
                {
                    var color = (Color)ColorConverter.ConvertFromString(p.ColorHex);
                    _overlay.UpdateCrosshair(p.Size, p.Thickness, p.Gap, p.OutlineThickness, color);
                }
            }
        }

        private void PresetDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b && b.Tag is CrosshairPreset p)
            {
                Presets.Remove(p);
                SavePresets();
            }
        }

        private void SavePresets()
        {
            try
            {
                var dir = IOPath.GetDirectoryName(PresetsPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var xs = new XmlSerializer(typeof(ObservableCollection<CrosshairPreset>));
                using var fs = File.Create(PresetsPath);
                xs.Serialize(fs, Presets);
            }
            catch { /* ignore for now */ }
        }

        private void LoadPresets()
        {
            try
            {
                var dir = IOPath.GetDirectoryName(PresetsPath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (!File.Exists(PresetsPath)) return;

                var xs = new XmlSerializer(typeof(ObservableCollection<CrosshairPreset>));
                using var fs = File.OpenRead(PresetsPath);
                if (xs.Deserialize(fs) is ObservableCollection<CrosshairPreset> list)
                {
                    Presets.Clear();
                    foreach (var p in list) Presets.Add(p);
                }
            }
            catch { /* ignore for now */ }
        }
    }


}