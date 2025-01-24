using System;
using System.Collections.Generic;
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
    /// Interaktionslogik für Register_Page.xaml
    /// </summary>
    public partial class Register_Page : Page
    {
        private Frame mainFrame;
        private MainWindow mainWindow;
        private ErrorMessage errorMessage;
        private InfoMessage infoMessage;
        private InputValidator iV;
        private ConfigFileHandler configFileHandler;
        public Register_Page(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.mainFrame = mainWindow.MainFrame;
            errorMessage = new ErrorMessage(this);
            infoMessage = new InfoMessage(this);
            iV = new InputValidator();
            configFileHandler = new ConfigFileHandler();
            InitializeComponent();
        }

        private async void RegisterButtonCallback(object sender, RoutedEventArgs e)
        {
            //check for checkbox 
            if (cb_show_pw.IsChecked != true)
                tb_pw.Text = pwb_pw.Password;
            if (cb_show_pw_rep.IsChecked != true)
                tb_pw_rep.Text = pwb_pw_rep.Password;

            bool errorflag = false;
            string ip = tb_ip.Text;
            string username = tb_name.Text;
            string email = tb_email.Text;
            string password = tb_pw.Text;
            string repeat_password = tb_pw_rep.Text;

            //set register button to default
            this.bn_reg.Content = "Register";

            // if invalid IP pattern - get error message
            if (!iV.ValidateIP(ip))
            {
                errorMessage.Register_IP_Invalid_Pattern_Message();
                errorflag = true;
            }
            else
            {
                errorMessage.Register_IP_Default_Message();
            }


            // if username empty - get error message
            if (!iV.ValidateUsername(username))
            {
                errorMessage.Register_Username_Empty_Message();
                errorflag = true;
            }
            else
            {
                errorMessage.Register_Username_Default_Message();
            }

            // if username is less than 4 token  - get error message
            if (username.Length < 4)
            {
                errorMessage.Register_Username_To_Short_Message();
                errorflag = true;
            }
            else
            {
                errorMessage.Register_Username_Default_Message();
            }

            // if invalid email pattern - get error message
            if (!iV.ValidateEmail(email))
            {
                errorMessage.Register_Email_Invalid_Error_Message();
                errorflag = true;
            }
            else
            {
                errorMessage.Register_Email_Default_Message();
            }

            // if invalid password pattern - get error message 
            if (!iV.ValidatePassword(password))
            {
                errorMessage.Register_password_Invalid_Error_Message();
                errorflag = true;
            }
            else
            {
                errorMessage.Register_password_Default_Message();
            }

            // if repeat password invalid - get error message
            if (string.IsNullOrEmpty(repeat_password) || repeat_password != password)
            {
                errorMessage.Register_repeat_password_Wrong_Message();
                errorflag = true;
            }
            else
            {
                errorMessage.Register_repeat_password_Default_Message();
            }

            
            if (errorflag)
            {
                return;
            }
           
            this.bn_reg.Content = "Registering...";

            try
            {
                RegisterReturnCodes code = await RegisterHandler.AttemptRegister(ip, email, username, password);

                switch (code)
                {
                    case RegisterReturnCodes.SUCCESS:
                        infoMessage.Register_Success_Message(); 
                        configFileHandler.SaveIPAndUsernameInConfigFile(ip, username);
                        await Task.Delay(5000);
                        mainFrame.Navigate(new Login_Page(this.mainWindow));
                        break;

                    case RegisterReturnCodes.EMAIL_TAKEN:
                        errorMessage.Register_Email_Already_Taken_Error_Message();
                        break;

                    case RegisterReturnCodes.USERNAME_TAKEN:
                        errorMessage.Register_Username_already_taken_Message();
                        break;

                    default:
                        throw new Exception("Unrecognized result code returned by server!");

                }
            }
            catch (ConnectionException)
            {
                errorMessage.Register_IP_Error_Message();
            }

        }

        /// <summary>
        /// Change the displayed box depending on if the Checkbox to show is checked or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_show_pw_Changed(object sender, RoutedEventArgs e)
        {
            if (this.cb_show_pw.IsChecked == true)
            {
                this.tb_pw.Visibility = Visibility.Visible;
                this.tb_pw.Focusable = true;
                this.tb_pw.Text = pwb_pw.Password;
                this.pwb_pw.Visibility = Visibility.Collapsed;
                this.pwb_pw.Focusable = true;
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


        /// <summary>
        /// Change the displayed box depending on if the Checkbox to show is checked or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_show_pw_rep_Changed(object sender, RoutedEventArgs e)
        {
            if (this.cb_show_pw_rep.IsChecked == true)
            {
                this.tb_pw_rep.Visibility = Visibility.Visible;
                this.tb_pw_rep.Focusable = true;
                this.tb_pw_rep.Text = pwb_pw_rep.Password;
                this.pwb_pw_rep.Visibility = Visibility.Collapsed;
                this.pwb_pw_rep.Focusable = false;
            }
            else
            {
                this.pwb_pw_rep.Visibility = Visibility.Visible;
                this.pwb_pw_rep.Focusable = true;
                this.pwb_pw_rep.Password = tb_pw_rep.Text;
                this.tb_pw_rep.Visibility = Visibility.Collapsed;
                this.tb_pw_rep.Focusable = false;
            }
        }
    }
}
