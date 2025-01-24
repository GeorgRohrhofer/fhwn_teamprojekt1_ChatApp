using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// Interaktionslogik für Options_Page.xaml
    /// </summary>
    public partial class Options_Page : Page
    {
        private Frame mainFrame;
        private ChatWindowPage chatWindowPage;
        private ApplicationInfo appinfo;

        public Options_Page(Frame mainFrame, ChatWindowPage chatWindow, ApplicationInfo appinfo)
        {
            InitializeComponent();

            this.mainFrame = mainFrame;
            this.chatWindowPage = chatWindow;
            this.appinfo = appinfo;

            if (!this.appinfo.IsAdmin)
            {
                this.tabitem_admin.Visibility = Visibility.Hidden;
            }
            else
            {
                this.tabitem_admin.Visibility = Visibility.Visible;
            }

            UserOptions_Page user_options_page = new UserOptions_Page(appinfo);

            this.UserOptionsFrame.Navigate(user_options_page);

            // TODO: Check for admin rights

            this.AdminOptionsFrame.Navigate(new AdminOptions_Page(appinfo));

            
        }

        private void img_close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainFrame.Navigate(chatWindowPage);
        }
    }
}
