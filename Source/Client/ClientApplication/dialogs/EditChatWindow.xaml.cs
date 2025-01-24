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
using System.Windows.Shapes;

namespace ClientApplication.dialogs
{
    /// <summary>
    /// Interaction logic for EditChatWindow.xaml
    /// </summary>
    public partial class EditChatWindow : Window
    {
        public EditChatWindow()
        {
            InitializeComponent();
        }

        private void Bn_save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void bn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
