using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// Interaktionslogik für UserOptions_Page.xaml
    /// </summary>
    public partial class UserOptions_Page : Page, INotifyPropertyChanged
    {
        private string _pwFirst;
        private string _pwSecond;
        private string _Username;
        private string _intitialUsername = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        private EditDataHandler handler;
        private InputValidator validator;
        private ConfigFileHandler configFileHandler;

        public ApplicationInfo AppInfo { get; set; }

        public string pwFirst
        {
            get
            {
                return this._pwFirst;
            }
            set
            {
                this._pwFirst = value;
                this.firePropertyChanged("pwFirst");
            }
        }
        public string pwSecond
        {
            get
            {
                return this._pwSecond;
            }
            set
            {
                this._pwSecond = value;
                this.firePropertyChanged("pwSecond");
            }
        }

        public string Username
        {
            get
            {
                return this._Username;
            }
            set
            {
                if (this._intitialUsername.Equals(string.Empty))
                {
                    this._intitialUsername = value;
                }
                this._Username = value;
                this.firePropertyChanged("Username");
            }
        }

        public UserOptions_Page(ApplicationInfo appinfo)
        {
            InitializeComponent();
            this.DataContext = this;
            this.AppInfo = appinfo;
            handler = new EditDataHandler();
            validator = new InputValidator();
            configFileHandler = new ConfigFileHandler();
            //string user = UserInfo.Client.GetUsername_from_Config();
            //this.Username = AppInfo.Client.GetUsername_from_Config();
            this.Username = configFileHandler.LoadConfigValues().Item2;
            tb_username.Text = this.Username;
            AppInfo.OptionsPage = this;
        }

        protected void firePropertyChanged(string? name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Enable and Disable "Show Password" for first Password box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_show_pw_first_Changed(object sender, RoutedEventArgs e)
        {
            if (this.cb_show_pw_first.IsChecked == true)
            {
                this.tb_pw_first.Visibility = Visibility.Collapsed;
                this.pwFirst = this.tb_pw_first.Password;
                this.tb_pw_txt_first.Visibility = Visibility.Visible;
            }
            else
            {
                this.tb_pw_txt_first.Visibility = Visibility.Collapsed;
                this.tb_pw_first.Password = this.pwFirst;
                this.tb_pw_first.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Enable and Disable "Show Password" for second Password box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_show_pw_second_Changed(object sender, RoutedEventArgs e)
        {
            if (this.cb_show_pw_second.IsChecked == true)
            {
                this.tb_pw_second.Visibility = Visibility.Collapsed;
                this.pwSecond = this.tb_pw_second.Password;
                this.tb_pw_txt_second.Visibility = Visibility.Visible;
            }
            else
            {
                this.tb_pw_txt_second.Visibility = Visibility.Collapsed;
                this.tb_pw_second.Password = this.pwSecond;
                this.tb_pw_second.Visibility = Visibility.Visible;
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (this.cb_show_pw_first.IsChecked == false)
            {
                this.pwFirst = tb_pw_first.Password;
            }
            if (this.cb_show_pw_second.IsChecked == false)
            {
                this.pwSecond = tb_pw_second.Password;
            }

            if (!validator.ValidatePassword(tb_pw_txt_first.Text) || !validator.ValidatePassword(tb_pw_txt_second.Text))
            {
                MessageBox.Show($"Please type in passwords", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!this.pwFirst.Equals(pwSecond))
            {
                MessageBox.Show($"Not matching", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                if (!validator.ValidatePassword(pwFirst))
                {
                    //TODO: Show error Message
                    MessageBox.Show("Invalid Password", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    Clear_Pw_Input();
                    return;
                }
                handler.Attempt_Edit(this.Username, this.pwFirst, this.AppInfo);
            }
        }

        private void btn_discard_Click(object sender, RoutedEventArgs e)
        {
            this.Username = _intitialUsername;
            Clear_Pw_Input();
        }

        /// <summary>
        /// Function that clears the password input in the wpf and in the properties. Will be called after successful edit
        /// </summary>
        public void Clear_Pw_Input()
        {
            this.Dispatcher.Invoke(() =>
            {
                pwFirst = "";
                pwSecond = "";
                tb_pw_first.Clear();
                tb_pw_second.Clear();
            });
        }

 
        private void tb_pw_first_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tb_pw_first.Password.Length >= 8)
            {
                lb_lengthpsw.Foreground = Brushes.Green;
            }
            else
            {
                lb_lengthpsw.Foreground = Brushes.Red;
            }
        }

        private void tb_pw_first_LostFocus(object sender, RoutedEventArgs e)
        {
            if(tb_pw_first.Password.Length >= 8)
            {
                lb_lengthpsw.Foreground = Brushes.Green;
            }
            else
            {
                lb_lengthpsw.Foreground= Brushes.Red;
            }

        }

        private void tb_pw_first_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (tb_pw_first.Password.Length >= 8)
            {
                lb_lengthpsw.Foreground = Brushes.Green;
            }
            else
            {
                lb_lengthpsw.Foreground = Brushes.Red;
            }
        }

        private void tb_pw_txt_first_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tb_pw_txt_first.Text.Length >= 8)
            {
                lb_lengthpsw.Foreground = Brushes.Green;
            }
            else
            {
                lb_lengthpsw.Foreground = Brushes.Red;
            }
        }

        private void tb_pw_txt_first_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tb_pw_txt_first.Text.Length >= 8)
            {
                lb_lengthpsw.Foreground = Brushes.Green;
            }
            else
            {
                lb_lengthpsw.Foreground = Brushes.Red;
            }
        }

        private void tb_pw_txt_first_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tb_pw_txt_first.Text.Length >= 8)
            {
                lb_lengthpsw.Foreground = Brushes.Green;
            }
            else
            {
                lb_lengthpsw.Foreground = Brushes.Red;
            }
        }


    }
}
