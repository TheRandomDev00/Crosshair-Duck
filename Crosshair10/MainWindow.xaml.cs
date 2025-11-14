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
            this.Close();  // closes the window
        }

        private void BtnMinimise_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized; //  the window
        }

        private OverlayWindow _overlay;

        private void ShowOverlay_Click(object sender, RoutedEventArgs e)
        {
            if (_overlay == null)
            {
                _overlay = new OverlayWindow();
                _overlay.Closed += (_, __) => _overlay = null;
                _overlay.Show();
            }
            else
            {
                _overlay.Close();
            }
        }

    }

 
}