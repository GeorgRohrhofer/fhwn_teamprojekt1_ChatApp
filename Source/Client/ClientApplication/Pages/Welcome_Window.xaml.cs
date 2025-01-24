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

namespace ClientApplication.Pages
{
    /// <summary>
    /// Interaktionslogik für Welcome_Window.xaml
    /// </summary>
    public partial class Welcome_Window : Page
    {
        private MainWindow mainWindow;
        public Welcome_Window(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            InitializeComponent();
            LoginFrame.Navigate(new Login_Page(mainWindow));
            RegisterFrame.Navigate(new Register_Page(mainWindow));
        }
        private void tc_login_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TabItem tabitem)
            {
            }
        }
    }
}
