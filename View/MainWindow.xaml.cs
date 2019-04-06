using Beridze_5.Data;
using System.ComponentModel;
using System.Windows;

namespace Beridze_5.View
{
    internal partial class MainWindow : Window
    {
        private DataTablView _dataTablView;

        public MainWindow()
        {
            InitializeComponent();
            ShowProcessesListView();
        }

        private void ShowProcessesListView()
        {
            MainGrid.Children.Clear();
            if (_dataTablView == null)
                _dataTablView = new DataTablView();
            MainGrid.Children.Add(_dataTablView);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _dataTablView?.Close();
            SystemProcessDB.Close();
            base.OnClosing(e);
        }
       
    }
}
