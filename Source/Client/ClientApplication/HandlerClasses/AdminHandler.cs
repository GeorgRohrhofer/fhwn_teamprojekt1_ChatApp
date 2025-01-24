using ClientApplication.Pages;
using ClientApplication.user_controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ClientApplication
{
    /// <summary>
    /// Class that handles all functions for admin.
    /// </summary>
    public class AdminHandler
    {
        /// <summary>
        /// Holds the collection of the selected user for the functions.
        /// </summary>
        private ObservableCollection<User> selected_User { get; set; }

        /// <summary>
        /// Property that holds the applicationinfo
        /// </summary>
        ApplicationInfo applicationInfo { get; set; }

        /// <summary>
        /// Property that holds the UI of the AdminPage.
        /// </summary>
        private AdminOptions_Page adminpage { get; set; }

        /// <summary>
        /// Constructor for a new AdminHandler instance.
        /// </summary>
        /// <param name="users">Collection of the selected user.</param>
        /// <param name="appinfo">sets the </param>
        public AdminHandler(ObservableCollection<User> users, ApplicationInfo appinfo, AdminOptions_Page adminoptions)
        {
            selected_User = users;
            this.applicationInfo = appinfo;
            this.adminpage = adminoptions;      
        }
        /// <summary>
        /// Method that sends an ban request to the server
        /// </summary>
        public async void Ban() 
        {
            List<int> result = new List<int>();
            foreach (User user in selected_User) 
            {
                if (user.IsSelected)
                {
                    result.Add(user.Id);
                }
            }
            string u = JsonSerializer.Serialize(result);
            ConverterContainer cv = new ConverterContainer("ban_user", u);
            await applicationInfo.Client.SendMessage(JsonSerializer.Serialize(cv));
        }
        /// <summary>
        /// Method that sends an unban request to the server
        /// </summary>
        public async void UnBan() 
        {
            List<int> result = new List<int>();
            foreach (User user in selected_User)
            {
                if (user.IsSelected)
                {
                    result.Add(user.Id);
                }
            }
            string u = JsonSerializer.Serialize(result);
            ConverterContainer cv = new ConverterContainer("unban_user", u);
            await applicationInfo.Client.SendMessage(JsonSerializer.Serialize(cv));
        }
        /// <summary>
        /// Method that sends an kick request to the server
        /// </summary>
        public async void Kick() 
        {
            List<int> result = new List<int>();
            foreach (User user in selected_User)
            {
                if (user.IsSelected)
                {
                    result.Add(user.Id);
                }
            }
            string u = JsonSerializer.Serialize(result);
            ConverterContainer cv = new ConverterContainer("kick_user", u);
            await applicationInfo.Client.SendMessage(JsonSerializer.Serialize(cv));
        }
        /// <summary>
        /// Method that sends a reset password request to the server
        /// </summary>
        public async void ResetPassword() 
        {
            List<int> result = new List<int>();
            foreach (User user in selected_User)
            {
                if (user.IsSelected)
                {
                    result.Add(user.Id);
                }
            }
            string u = JsonSerializer.Serialize(result);
            ConverterContainer cv = new ConverterContainer("reset_password", u);
            await applicationInfo.Client.SendMessage(JsonSerializer.Serialize(cv));
        }
        /// <summary>
        /// Method that sends a promote request to the server
        /// </summary>
        public async void Promote() 
        {
            List<int> result = new List<int>();
            foreach (User user in selected_User)
            {
                if (user.IsSelected)
                {
                    result.Add(user.Id);
                }
            }
            string u = JsonSerializer.Serialize(result);
            ConverterContainer cv = new ConverterContainer("promote_user", u);
            await applicationInfo.Client.SendMessage(JsonSerializer.Serialize(cv));
        }
        /// <summary>
        /// Method that sends a demote request to the server
        /// </summary>
        public async void Demote() 
        {
            List<int> result = new List<int>();
            foreach (User user in selected_User) 
            {
                if (user.IsSelected)
                {
                    result.Add(user.Id);
                }
            }
            string u = JsonSerializer.Serialize(result);
            ConverterContainer cv = new ConverterContainer("demote_user", u);
            await applicationInfo.Client.SendMessage(JsonSerializer.Serialize(cv));
        }
    }
}
