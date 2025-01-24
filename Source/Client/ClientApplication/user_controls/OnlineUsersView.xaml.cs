using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;

namespace ClientApplication.user_controls
{
    /// <summary>
    /// Interaction logic for OnlineUsersView.xaml
    /// </summary>
    public partial class OnlineUsersView : UserControl, INotifyPropertyChanged
    {
        public bool IsAdmin = true;
        private ObservableCollection<User> users;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<User> Users 
        {
            get
            {
                return users;
            }
            set
            {
                users = value;
                FireNotifyChange(nameof(Users));
            }
        }
        public OnlineUsersView()
        {
            InitializeComponent();
            Users = new ObservableCollection<User>();
            MessageHandler.userOverview = this;
            this.DataContext = this;
          
            //lb_users.ItemsSource = Users;
           
        }

        public void AddUser_TO(Dictionary<int, string> col)
        {
            this.Dispatcher.Invoke(() =>
            {
                foreach (var us in col)
                {
                    this.Users.Add(new User(us.Value));
                }
            });
        }
                        

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            foreach(var us in this.Users)
            {
                if (us.Name.Equals(this.Name))
                {
                    us.IsSelected = true;

                }

            }
        }

        private void Cb_selection_OnUnchecked(object sender, RoutedEventArgs e)
        {
            
        }

        public void Change_BanLabels()
        {
            //foreach (User user in Users)
            //{
            //    foreach (var item in )
            //}
        }
        public void FireNotifyChange(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        }
    }
}
