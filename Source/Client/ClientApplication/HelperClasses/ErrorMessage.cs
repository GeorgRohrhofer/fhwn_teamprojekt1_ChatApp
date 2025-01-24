using ClientApplication.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace ClientApplication
{
    /// <summary>
    /// Provides methods for displaying error messages on the login and registration pages.
    /// </summary>
    internal class ErrorMessage
    {
        /// <summary>
        /// The login page instance for displaying login-related error messages.
        /// </summary>
        private Login_Page loginPage;

        /// <summary>
        /// The registration page instance for displaying registration-related error messages.
        /// </summary>
        private Register_Page registerPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage"/> class with a login page.
        /// </summary>
        /// <param name="loginPage">The login page instance.</param>
        public ErrorMessage(Login_Page loginPage)
        {
            this.loginPage = loginPage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessage"/> class with a registration page.
        /// </summary>
        /// <param name="registerPage">The registration page instance.</param>
        public ErrorMessage(Register_Page registerPage)
        {
            this.registerPage = registerPage;
        }

        /// <summary>
        /// Displays the default username message on the login page.
        /// </summary>
        public void Login_Username_Default_Message()
        {
            loginPage.lb_input_username_default.Visibility = Visibility.Visible;
            loginPage.lb_input_username_empty_error.Visibility = Visibility.Hidden;
            loginPage.lb_input_username_wrong_username_error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the empty username error message on the login page.
        /// </summary>
        public void Login_Username_Empty_Message()
        {
            loginPage.lb_input_username_default.Visibility = Visibility.Hidden;
            loginPage.lb_input_username_empty_error.Visibility = Visibility.Visible;
            loginPage.lb_input_username_wrong_username_error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the wrong username error message on the login page.
        /// </summary>
        public void Login_Username_Wrong_Message()
        {
            loginPage.lb_input_username_default.Visibility = Visibility.Hidden;
            loginPage.lb_input_username_empty_error.Visibility = Visibility.Hidden;
            loginPage.lb_input_username_wrong_username_error.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the default password message on the login page.
        /// </summary>
        public void Login_Password_Default_Message()
        {
            loginPage.lb_input_password_default.Visibility = Visibility.Visible;
            loginPage.lb_input_password_password_wrong_error.Visibility = Visibility.Hidden;
            loginPage.lb_input_password_empty_error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the empty password error message on the login page.
        /// </summary>
        public void Login_Password_Empty_Message()
        {
            loginPage.lb_input_password_default.Visibility = Visibility.Hidden;
            loginPage.lb_input_password_password_wrong_error.Visibility = Visibility.Hidden;
            loginPage.lb_input_password_empty_error.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the wrong password error message on the login page.
        /// </summary>
        public void Login_Password_Wrong_Message()
        {
            loginPage.lb_input_password_default.Visibility = Visibility.Hidden;
            loginPage.lb_input_password_password_wrong_error.Visibility = Visibility.Visible;
            loginPage.lb_input_password_empty_error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the default IP message on the login page.
        /// </summary>
        public void Login_IP_Default_Message()
        {
            loginPage.lb_input_ip_default.Visibility = Visibility.Visible;
            loginPage.lb_input_ip_error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the IP error message on the login page.
        /// </summary>
        public void Login_IP_Error_Message()
        {
            loginPage.lb_input_ip_default.Visibility = Visibility.Hidden;
            loginPage.lb_input_ip_error.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the IP error message on the registration page.
        /// </summary>
        public void Register_IP_Error_Message()
        {
            registerPage.lb_register_ip_default.Visibility = Visibility.Hidden;
            registerPage.lb_register_ip_error.Visibility = Visibility.Visible;
            registerPage.lb_register_ip_invalid_pattern_error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the invalid IP pattern message on the registration page.
        /// </summary>
        public void Register_IP_Invalid_Pattern_Message()
        {
            registerPage.lb_register_ip_default.Visibility = Visibility.Hidden;
            registerPage.lb_register_ip_error.Visibility = Visibility.Hidden;
            registerPage.lb_register_ip_invalid_pattern_error.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the default IP message on the registration page.
        /// </summary>
        public void Register_IP_Default_Message()
        {
            registerPage.lb_register_ip_default.Visibility = Visibility.Visible;
            registerPage.lb_register_ip_error.Visibility = Visibility.Hidden;
            registerPage.lb_register_ip_invalid_pattern_error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the default username message on the registration page.
        /// </summary>
        public void Register_Username_Default_Message()
        {
            registerPage.name_lable_regular.Visibility = Visibility.Visible;
            registerPage.name_lable_error_empty.Visibility = Visibility.Hidden;
            registerPage.name_lable_error_username_taken.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the empty username error message on the registration page.
        /// </summary>
        public void Register_Username_Empty_Message()
        {
            registerPage.name_lable_regular.Visibility = Visibility.Hidden;
            registerPage.name_lable_error_empty.Visibility = Visibility.Visible;
            registerPage.name_lable_error_username_taken.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the too short username error message on the registration page.
        /// </summary>
        public void Register_Username_To_Short_Message()
        {
            registerPage.name_lable_regular.Visibility = Visibility.Hidden;
            registerPage.name_lable_error_empty.Visibility = Visibility.Visible;
            registerPage.name_lable_error_username_taken.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the username already taken error message on the registration page.
        /// </summary>
        public void Register_Username_already_taken_Message()
        {
            registerPage.name_lable_regular.Visibility = Visibility.Hidden;
            registerPage.name_lable_error_empty.Visibility = Visibility.Hidden;
            registerPage.name_lable_error_username_taken.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the default email message on the registration page.
        /// </summary>
        public void Register_Email_Default_Message()
        {
            registerPage.email_lable_error.Visibility = Visibility.Hidden;
            registerPage.email_lable_regular.Visibility = Visibility.Visible;
            registerPage.email_lable_already_taken_error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the invalid email error message on the registration page.
        /// </summary>
        public void Register_Email_Invalid_Error_Message()
        {
            registerPage.email_lable_error.Visibility = Visibility.Visible;
            registerPage.email_lable_regular.Visibility = Visibility.Hidden;
            registerPage.email_lable_already_taken_error.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the email already taken error message on the registration page.
        /// </summary>
        public void Register_Email_Already_Taken_Error_Message()
        {
            registerPage.email_lable_error.Visibility = Visibility.Hidden;
            registerPage.email_lable_regular.Visibility = Visibility.Hidden;
            registerPage.email_lable_already_taken_error.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the invalid password error message on the registration page.
        /// </summary>
        public void Register_password_Invalid_Error_Message()
        {
            registerPage.pw_lable_error.Visibility = Visibility.Visible;
            registerPage.pw_lable_regular.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the default password message on the registration page.
        /// </summary>
        public void Register_password_Default_Message()
        {
            registerPage.pw_lable_error.Visibility = Visibility.Hidden;
            registerPage.pw_lable_regular.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Displays the repeat password mismatch error message on the registration page.
        /// </summary>
        public void Register_repeat_password_Wrong_Message()
        {
            registerPage.pw_rep_lable_error.Visibility = Visibility.Visible;
            registerPage.pw_rep_lable_regular.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Displays the default repeat password message on the registration page.
        /// </summary>
        public void Register_repeat_password_Default_Message()
        {
            registerPage.pw_rep_lable_error.Visibility = Visibility.Hidden;
            registerPage.pw_rep_lable_regular.Visibility = Visibility.Visible;
        }

        internal void Register_Success_Message()
        {
            // TODO: implement!
        }
    }
}
