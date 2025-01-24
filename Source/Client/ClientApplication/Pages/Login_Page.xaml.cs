using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    /// Represents the interaction logic for the Login_Page.xaml.
    /// Handles user login functionality, including input validation and server communication.
    /// </summary>
    public partial class Login_Page : Page
    {
        /// <summary>
        /// The main navigation frame of the application.
        /// </summary>
        private Frame mainFrame;

        /// <summary>
        /// The main window of the application.
        /// </summary>
        private MainWindow mainWindow;

        /// <summary>
        /// Stores the IP address provided by the user.
        /// </summary>
        private string ipAddress;

        /// <summary>
        /// Stores the username provided by the user.
        /// </summary>
        private string username;

        /// <summary>
        /// Stores the password provided by the user.
        /// </summary>
        private string pw;

        /// <summary>
        /// Handles displaying error messages to the user.
        /// </summary>
        private ErrorMessage errorMessage;

        /// <summary>
        /// Handles loading and saving configuration file values.
        /// </summary>
        private ConfigFileHandler configFileHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Login_Page"/> class.
        /// Used when no MainWindow reference is passed.
        /// </summary>
        public Login_Page()
        {
            this.mainFrame = mainWindow.MainFrame;
            this.mainWindow = mainWindow;
            InitializeComponent();
            errorMessage = new ErrorMessage(this);
            configFileHandler = new ConfigFileHandler();
            Tuple<string, string> tuple = configFileHandler.LoadConfigValues();
            ipAddress = tuple.Item1;
            username = tuple.Item2;
            tb_ip.Text = ipAddress;
            tb_name.Text = username;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Login_Page"/> class.
        /// Used when a MainWindow reference is passed.
        /// </summary>
        /// <param name="mainWindow">The main window of the application.</param>
        public Login_Page(MainWindow mainWindow)
        {
            this.mainFrame = mainWindow.MainFrame;
            this.mainWindow = mainWindow;
            InitializeComponent();
            errorMessage = new ErrorMessage(this);
            configFileHandler = new ConfigFileHandler();
            Tuple<string, string> tuple = configFileHandler.LoadConfigValues();
            ipAddress = tuple.Item1;
            username = tuple.Item2;
            tb_ip.Text = ipAddress;
            tb_name.Text = username;
        }

        /// <summary>
        /// Handles the text changed event for the IP address input box.
        /// Resets the border brush color to the default state.
        /// </summary>
        private void tb_ip_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb_ip.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 120, 120, 120));
        }

        /// <summary>
        /// Handles the text changed event for the username input box.
        /// Resets the border brush color to the default state.
        /// </summary>
        private void tb_name_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb_name.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 120, 120, 120));
        }

        /// <summary>
        /// Handles the text changed event for the password input box.
        /// Resets the border brush color to the default state.
        /// </summary>
        private void tb_pw_TextChanged(object sender, TextChangedEventArgs e)
        {
            tb_pw.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 120, 120, 120));
        }

        /// <summary>
        /// Handles the click event for the login button.
        /// Validates input fields, attempts login, and navigates to the chat window upon success.
        /// </summary>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ipAddress = tb_ip.Text;
            username = tb_name.Text;

            if (this.cb_show_pw.IsChecked != true)
            {
                this.tb_pw.Text = pwb_pw.Password;
            }

            pw = tb_pw.Text;

            if (string.IsNullOrEmpty(tb_name.Text))
            {
                errorMessage.Login_Username_Empty_Message();
            }
            else
            {
                errorMessage.Login_Username_Default_Message();
            }

            if (string.IsNullOrEmpty(tb_pw.Text))
            {
                errorMessage.Login_Password_Empty_Message();
            }
            else
            {
                errorMessage.Login_Password_Default_Message();
            }

            if (string.IsNullOrEmpty(tb_pw.Text) || string.IsNullOrEmpty(tb_name.Text))
            {
                return;
            }

            try
            {
                errorMessage.Login_IP_Default_Message();
                ApplicationInfo appInfo = await LoginHandler.AttempLogin(ipAddress, username, username, pw, this.mainWindow);

                if (appInfo != null)
                {
                    configFileHandler.SaveIPAndUsernameInConfigFile(ipAddress, username);
                    mainWindow.Width = 1440;
                    mainWindow.Height = 800;
                    mainWindow.Left = 0;
                    mainWindow.Top = 0;
                    mainWindow.Title = "ChatApplication";
                    this.mainFrame.Navigate(new ChatWindowPage(this.mainFrame, appInfo));
                }
                else
                {
                    errorMessage.Login_Username_Wrong_Message();
                    errorMessage.Login_Password_Wrong_Message();
                }
            }
            catch (Exception ex)
            {
                errorMessage.Login_IP_Error_Message();
                Console.WriteLine($"An error occurred during client initialization: {ex}.");
                return;
            }
        }

        /// <summary>
        /// Validates whether the given email address matches the required format.
        /// </summary>
        /// <param name="mail">The email address to validate.</param>
        /// <returns>True if the email format is valid, otherwise false.</returns>
        private bool Validate_Email(string mail)
        {
            if (!mail.Contains('@'))
            {
                return false;
            }

            string emailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            Regex r = new Regex(emailPattern);
            return r.IsMatch(mail);
        }

        /// <summary>
        /// Handles the password visibility toggle event.
        /// Shows or hides the password based on the checkbox state.
        /// </summary>
        private void cb_show_pw_Changed(object sender, RoutedEventArgs e)
        {
            if (this.cb_show_pw.IsChecked == true)
            {
                this.tb_pw.Visibility = Visibility.Visible;
                this.tb_pw.Focusable = true;
                this.tb_pw.Text = pwb_pw.Password;
                this.pwb_pw.Visibility = Visibility.Collapsed;
                this.pwb_pw.Focusable = false;
            }
            else
            {
                this.pwb_pw.Visibility = Visibility.Visible;
                this.pwb_pw.Focusable = true;
                this.pwb_pw.Password = tb_pw.Text;
                this.tb_pw.Visibility = Visibility.Collapsed;
                this.tb_pw.Focusable = false;
            }
        }
    }
}

