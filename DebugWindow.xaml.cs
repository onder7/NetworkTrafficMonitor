using System;
using System.Windows;

namespace NetworkTrafficMonitor
{
    public partial class DebugWindow : Window
    {
        private static DebugWindow? _instance;

        public DebugWindow()
        {
            InitializeComponent();
            _instance = this;
        }

        public static void Log(string message)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (_instance != null && _instance.IsLoaded)
                {
                    _instance.DebugTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
                    _instance.DebugTextBox.ScrollToEnd();
                }
            });
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DebugTextBox.Clear();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance = null;
        }
    }
}
