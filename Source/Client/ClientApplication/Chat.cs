using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ClientApplication
{
    /// <summary>
    /// Represents a chat object containing information about its ID, name, creation time,
    /// associated messages, users, and visibility status.
    /// </summary>
    public class Chat : INotifyPropertyChanged
    {
        /// <summary>
        /// The unique identifier for the chat.
        /// </summary>
        private int chatId;

        /// <summary>
        /// The name of the chat.
        /// </summary>
        private string chatName;

        /// <summary>
        /// The creation time of the chat.
        /// </summary>
        private DateTime creationTime;

        /// <summary>
        /// The collection of messages associated with the chat.
        /// </summary>
        private ObservableCollection<Message> messages;

        /// <summary>
        /// The collection of users participating in the chat.
        /// </summary>
        private ObservableCollection<User> users;

        /// <summary>
        /// Indicates whether the chat is visible to the user.
        /// </summary>
        private bool visibility;

        /// <summary>
        /// Gets or sets the unique identifier for the chat.
        /// </summary>
        public int ChatId
        {
            get => chatId;
            set
            {
                chatId = value;
                firePropertyChanged(nameof(this.ChatId));
            }
        }

        /// <summary>
        /// Gets or sets the name of the chat.
        /// </summary>
        public string ChatName
        {
            get => chatName;
            set
            {
                chatName = value;
                firePropertyChanged(nameof(this.ChatName));
            }
        }

        /// <summary>
        /// Gets or sets the creation time of the chat.
        /// </summary>
        public DateTime CreationTime
        {
            get => creationTime;
            set
            {
                creationTime = value;
                firePropertyChanged(nameof(this.CreationTime));
            }
        }

        /// <summary>
        /// Gets or sets the collection of messages associated with the chat.
        /// </summary>
        public ObservableCollection<Message> Messages
        {
            get => messages;
            set
            {
                messages = value;
                firePropertyChanged(nameof(this.Messages));
            }
        }

        /// <summary>
        /// Gets or sets the collection of users participating in the chat.
        /// </summary>
        public ObservableCollection<User> Users
        {
            get => users;
            set
            {
                users = value;
                firePropertyChanged(nameof(this.Users));
                firePropertyChanged(nameof(this.UserCount));
            }
        }

        /// <summary>
        /// Gets the count of users in the chat.
        /// </summary>
        public int UserCount
        {
            get
            {
                return Users.Count;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the chat is visible to the user.
        /// </summary>
        public bool Visibility
        {
            get => visibility;
            set
            {
                this.visibility = value;
                this.firePropertyChanged(nameof(Visibility));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Fires the PropertyChanged event for the specified property name.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        protected void firePropertyChanged(string name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chat"/> class.
        /// Sets default values for visibility and initializes the user collection.
        /// </summary>
        public Chat()
        {
            this.Visibility = true;
            this.Users = new ObservableCollection<User>();
        }
    }


}
