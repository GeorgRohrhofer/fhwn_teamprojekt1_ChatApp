using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ClientApplication.Pages;

namespace ClientApplication.user_controls
{
    /// <summary>
    /// Interaction logic for SearchBar.xaml
    /// </summary>
    public partial class SearchBar : UserControl
    {
        private string placeHolderText = "Search for Users or Groups";
        public ChatWindowPage ChatWindowPage { get; set; }
        public SearchBar()
        {
            InitializeComponent();
            this.tb_searchbar.Text = this.placeHolderText;
        }

        private void img_search_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //clear usernotshown list
            //var usersNotShown = new ObservableCollection<User>(UserInfo.notShownUsers);

            //foreach (User user in usersNotShown)
            //{
            //    UserInfo.notShownUsers.Remove(user);
            //    UserInfo.Users.Add(user);
            //}
            
            //search users and add to usersnotshown
            //IEnumerable<User> foundUsers = UserInfo.Users.Where(user => user.Name == tb_searchbar.Text);

            //var currentUsers = new ObservableCollection<User>(UserInfo.Users);

            //foreach (User user in currentUsers)
            //{
            //    if (!foundUsers.Contains(user))
            //    {
            //        UserInfo.notShownUsers.Add(user);
            //        UserInfo.Users.Remove(user);
            //    }
            //}
        }

        private void Control_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.tb_searchbar.Text) || this.tb_searchbar.Text == this.placeHolderText)
            {
                return;
            }

            this.ChatWindowPage.Chats
                .Where(chat => !chat.ChatName.Contains(this.tb_searchbar.Text))
                .ToList().ForEach(c => c.Visibility = false);
        }

        private void Searchfield_MouseDown(object sender, RoutedEventArgs e)
        {
            TextBox box = (TextBox)sender;
            box.Text = "";
        }

        private void Img_clear_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.tb_searchbar.Text = this.placeHolderText;
            this.ChatWindowPage.Chats.ToList().ForEach(chat => chat.Visibility = true);
        }
    }
}
