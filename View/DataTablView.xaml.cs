using System;
using System.Windows.Controls;
using Beridze_5.Models;

namespace Beridze_5.View
{
    internal partial class DataTablView : UserControl
    {
        internal DataTablView()
        {
            InitializeComponent();
            DataContext = new DataTableModel();
        }

        internal void Close()
        {
            ((DataTableModel)DataContext).Close();
        }
    }
}

