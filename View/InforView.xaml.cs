using System.Windows.Controls;
using Beridze_5.ViewModels;

namespace Beridze_5.View
{
    internal partial class InforView : UserControl
    {
        internal InforView(System.Diagnostics.Process process)
        {
            InitializeComponent();
            DataContext = new InformViewModel(process.Modules, process.Threads);
        }
    }
}
