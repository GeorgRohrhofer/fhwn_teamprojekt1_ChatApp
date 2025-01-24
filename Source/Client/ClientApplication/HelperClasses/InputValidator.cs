using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClientApplication
{
    /// <summary>
    /// Class to help validate user input.
    /// </summary>
    public class InputValidator
    {
        /// <summary>
        /// Function to validate email.
        /// </summary>
        /// <param name="email">Email string to validate.</param>
        /// <returns>True if input is valide email, false if not.</returns>
        public bool ValidateEmail(string email)
        {
            if (!email.Contains('@') || string.IsNullOrEmpty(email))
            {
                return false;
            }

            string emailPattern = @"^([-A-Za-z0-9!#$%&'*+/=?^_`{|}~]+(?:\.[-A-Za-z0-9!#$%&'*+/=?^_`{|}~]+)*@(?:[A-Za-z0-9](?:[-A-Za-z0-9]*[A-Za-z0-9])?\.)+[A-Za-z0-9](?:[-A-Za-z0-9]*[A-Za-z0-9])?)$";
            Regex r = new Regex(emailPattern);
            return r.IsMatch(email);          
        }

        /// <summary>
        /// Function to validate ip address.
        /// </summary>
        /// <param name="ip">IP string to validate.</param>
        /// <returns>True if input is valide ip, false if not.</returns>
        public bool ValidateIP(string ip) 
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }
            string ip_pattern = @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
            Regex r = new Regex(ip_pattern);
            return r.IsMatch(ip);
        }


        /// <summary>
        /// Function to validate username.
        /// </summary>
        /// <param name="username">Username string to validate.</param>
        /// <returns>True if input is valide username, false if not.</returns>
        public bool ValidateUsername(string username) 
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Function to validate password.
        /// </summary>
        /// <param name="password">Password string to validate.</param>
        /// <returns>True if input is valide password, false if not.</returns>
        public bool ValidatePassword(string password) 
        {
            if (password.Length < 8 || string.IsNullOrEmpty(password))
            {
                return false;
            }
            return true;
        }
    }
}
