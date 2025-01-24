using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApplication
{
    /// <summary>
    /// Class that handles editing user data on the server.
    /// </summary>
    public class EditDataHandler
    {
        /// <summary>
        /// Sends an attempt for editing to the server.
        /// </summary>
        /// <param name="new_username">new username</param>
        /// <param name="new_Password">new password</param>
        /// <param name="appInfo">the actual sessioninformations of the client</param>
        /// <returns></returns>
        public async void Attempt_Edit(string new_username, string new_Password, ApplicationInfo appInfo)
        {
            UserData ud = new UserData(new_username, " ", new_Password);
            string userDataString = JsonSerializer.Serialize(ud);
            ConverterContainer cv = new ConverterContainer("edit", userDataString);
            string finalString = JsonSerializer.Serialize(cv);
            // Send the user, and pw to the server
            await appInfo.Client.SendMessage(finalString);
        }
    }
}
