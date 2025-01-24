using ClientApplication.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    /// <summary>
    /// File that holds information used throughout application.
    /// </summary>
    public class ApplicationInfo
    {
        /// <summary>
        /// Currently connected client to server.
        /// </summary>
        public Client Client
        {
            get;
            set;
        }

        /// <summary>
        /// Thread that listens to server requests
        /// </summary>
        public Thread ListenThread
        {
            get;
            set;
        }

        /// <summary>
        /// Property for user id of the logged in user.
        /// </summary>
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// Property for admin status of the logged in user.
        /// </summary>
        public bool IsAdmin
        {
            get;
            set;
        }

        /// <summary>
        /// Property for all online users.
        /// </summary>
        public  Dictionary<int,string> OnlineUsers  { get; set; } =
        [

        ];

        /// <summary>
        /// Property for all users.
        /// </summary>
        public Dictionary<int, string> AllUsers { get; set; } = [];

        /// <summary>
        /// Property ChatUI that holds UI of the chat view.
        /// </summary>
        public ChatWindowPage ChatUI { get; set; }
        /// <summary>
        /// Property that holds the UI of the UserOptions view.
        /// </summary>
        public UserOptions_Page OptionsPage { get; set; }
       
        /// <summary>
        /// Property that holds the UI of the AdminPage.
        /// </summary>
        public AdminOptions_Page AdminPage { get; set; }

    }
}
