using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    /// <summary>
    /// Class to hold message data.
    /// </summary>
    public class Message : INotifyPropertyChanged
    {
        private bool ownMessage;
        private string messageText;
        private bool usename;
        private DateTime timeStamp;
        private string username;

        /// <summary>
        /// Constructor for a new message instance.
        /// </summary>
        /// <param name="own">Flag if message is your own or not.</param>
        /// <param name="message">Message text.</param>
        /// <param name="username">Username of the sender.</param>
        /// <param name="timestamp">Timestamp of the sent message.</param>
        public Message(bool own, string message, string username, DateTime timestamp)
        {
            this.OwnMessage = own;
            this.MessageText = message;
            this.Username = username;
            this.TimeStamp = timestamp;
        }

        /// <summary>
        /// Property to check if user is owner of the message.
        /// </summary>
        public bool OwnMessage
        {
            get { return ownMessage; }
            set
            {
                this.ownMessage = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OwnMessage)));
            }
        }

        /// <summary>
        /// Property of the message text.
        /// </summary>

        public string MessageText
        {
            get
            {
                return this.messageText;
            }
            set
            {
                this.messageText = value;
            }
        }

        /// <summary>
        /// Property for the username
        /// </summary>
        public string Username { get => username; set => username = value; }

        /// <summary>
        /// Property for the timestamp of the message.
        /// </summary>
        public DateTime TimeStamp { get => timeStamp; set => timeStamp = value; }
        
        /// <summary>
        /// Property to get a string representation of the timestamp.
        /// </summary>
        public string TimeStampString
        { 
            get 
            {  
                return TimeStamp.ToLocalTime().ToString(new CultureInfo("de-DE")); 
            } 
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
