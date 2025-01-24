using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApplication.user_controls
{
    /// <summary>
    /// Interaktionslogik für AdminOptions.xaml
    /// </summary>
    public partial class AdminOptions : UserControl
    {
        public AdminOptions()
        {
            InitializeComponent();
        }

        private void cmAdminOptionsButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            ContextMenu cm = this.FindResource("cmAdminOptions") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
            */
        }
    }
}
