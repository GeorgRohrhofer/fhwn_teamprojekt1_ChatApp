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
using System.Windows.Xps;

namespace ClientApplication.dialogs
{
    /// <summary>
    /// Interaction logic for CreateNewChatDialog.xaml
    /// </summary>
    public partial class CreateNewChatDialog : Window
    {
        private string placeHolderText = "";

        public CreateNewChatDialog()
        {
            InitializeComponent();
            this.tb_chat_name.Text = this.placeHolderText;

            this.uc_online_users.lb_users.BorderThickness = new Thickness(0);
        }

        private void bn_save_Click(object sender, RoutedEventArgs e)
        {
            if (this.uc_online_users.Users.Where(user => user.IsSelected).Count() == 0)
            {
                MessageBox.Show("You need to select someone.");
                return;
            }
            
            if (this.uc_online_users.Users.Where(user => user.IsSelected).Count() > 1)
            {
                MessageBox.Show("No Groups supported yet, you need to select exactly 1 person.");
                return;
            }

            /*if (this.tb_chat_name.Text == this.placeHolderText)
            {
                MessageBox.Show("Enter a group name.");
                return;
            }
            */

            this.DialogResult = true;
        }

        private void bn_discard_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void tb_chat_name_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
           
            if (tb.Text == this.placeHolderText)
            {
                tb.Text = "";
            }
        }
    }
}
