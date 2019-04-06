using System;
using System.Windows;

namespace Beridze_5.View
{
    internal partial class InformWindoView : Window
    {
            private InforView _infoView;

            internal InformWindoView(System.Diagnostics.Process process)
            {
                InitializeComponent();
                Title = $"{process.ProcessName} Info";
                ShowInfoView(process);
            }

            private void ShowInfoView(System.Diagnostics.Process process)
            {
                MainGrid.Children.Clear();
                if (_infoView == null)
                    _infoView = new InforView(process);
                MainGrid.Children.Add(_infoView);
            }
    }
}