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
    /// Provides methods for displaying informational messages.
    /// </summary>
    internal class InfoMessage
    {
        /// <summary>
        /// Holds a reference to the <see cref="Register_Page"/> instance.
        /// </summary>
        private Register_Page registerPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoMessage"/> class with the specified <see cref="Register_Page"/>.
        /// </summary>
        /// <param name="registerPage">The register page associated with this info message instance.</param>
        public InfoMessage(Register_Page registerPage)
        {
            this.registerPage = registerPage;
        }

        /// <summary>
        /// Displays the success message for the registration function.
        /// </summary>
        public void Register_Success_Message()
        {
            //Console.WriteLine("Register function");
            this.registerPage.info_message_register_success.Visibility = Visibility.Visible;
        }
    }
}

