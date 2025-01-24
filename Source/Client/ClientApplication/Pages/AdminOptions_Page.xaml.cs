using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml.Schema;

namespace ClientApplication.Pages
{
    /// <summary>
    /// Interaktionslogik für AdminOptions_Page.xaml
    /// </summary>
    public partial class AdminOptions_Page : Page
    {
        AdminHandler adminHandler;

        ApplicationInfo AppInfo { get; set; }
        public AdminOptions_Page(ApplicationInfo appinfo)
        {
            InitializeComponent();
            this.AppInfo = appinfo;
            uc_online_users.Users.Clear();
            foreach(int id in appinfo.AllUsers.Keys)
            {
                User u = new User(appinfo.AllUsers.GetValueOrDefault(id));
                u.Id = id;
                if(id != appinfo.ID)
                {
                    uc_online_users.Users.Add(u);
                }
            }
            adminHandler = new AdminHandler(uc_online_users.Users, AppInfo, this);

            this.uc_online_users.Width = this.Width;
            this.uc_online_users.lb_users.Width = this.Width;
            this.AppInfo.AdminPage = this;
        }

        private void btn_ban_Click(object sender, RoutedEventArgs e)
        {     
            adminHandler.Ban();

            foreach (User item in uc_online_users.Users.Where(x => x.IsSelected))
            {
                item.Name += " ban";              
            }
        }

        public void Change_BanLabels()
        {
            foreach (User user in uc_online_users.Users.Where(x => x.IsSelected))
            {
                foreach (var item in uc_online_users.lb_users.Items)
                {
                    if(item is ListBoxItem listBoxItem)
                    {
                        //uc_online_users.lb_users.Items.
                    }
                }
            }
        }

        private void btn_unban_Click(object sender, RoutedEventArgs e)
        {
            adminHandler.UnBan();
        }

        private void btn_kick_Click(object sender, RoutedEventArgs e)
        {
            adminHandler.Kick();
        }

        private void btn_reset_pw_Click(object sender, RoutedEventArgs e)
        {
            adminHandler.ResetPassword();
        }

        private void btn_promote_Click(object sender, RoutedEventArgs e)
        {
            adminHandler.Promote();
        }

        private void btn_demote_Click(object sender, RoutedEventArgs e)
        {
            adminHandler.Demote();
        }

        /// <summary>
        /// Set all checkboxes to unselect. Happens mostly after successful admin functions. 
        /// </summary>
        public void ResetSelect()
        {
            uc_online_users.lb_users.UnselectAll();          
        }
    }
}
