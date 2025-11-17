using System.Windows;

namespace NetworkTrafficMonitor
{
    public partial class MainWindow : Window
    {
        private DebugWindow? _debugWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DebugButton_Click(object sender, RoutedEventArgs e)
        {
            if (_debugWindow == null || !_debugWindow.IsLoaded)
            {
                _debugWindow = new DebugWindow();
                _debugWindow.Show();
                DebugWindow.Log("Debug window opened");
            }
            else
            {
                _debugWindow.Activate();
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow
            {
                Owner = this
            };
            aboutWindow.ShowDialog();
        }
    }
}
