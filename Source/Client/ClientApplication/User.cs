using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClientApplication
{
    /// <summary>
    /// Class to hold user information in the client.
    /// </summary>
    public class User: INotifyPropertyChanged
    {
        private int id;
        private string name;
        private bool isSelected;
        private bool isAdmin;
        private bool isSearched;

        /// <summary>
        /// Holds user id;
        /// </summary>
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                this.OnPropertyChanged(nameof(Id));
            }
        }

        /// <summary>
        /// Holds username of the user.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Constructor for a new User instance.
        /// </summary>
        /// <param name="name">Username of the user.</param>
        public User(string name)
        {
            this.Name = name;
        }
        
        /// <summary>
        /// Property to indicate whether a user is an admin or not.
        /// </summary>
        public bool IsAdmin
        {
            get
            {
                return this.isAdmin;
            }
            set
            {
                this.isAdmin = value;
                this.OnPropertyChanged(nameof(IsAdmin));
            }
        }

        /// <summary>
        /// A property to check if the user is selected in the UI.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;
                this.OnPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// A orioerty to check if the user is currently searched in the searchbar.
        /// </summary>
        public bool IsSearched
        {
            get
            {
                return this.isSearched;
            }
            set
            {
                this.isSearched = value;
                this.OnPropertyChanged(nameof(IsSearched));
            }
        }

        /// <summary>
        /// Event to notify UI if some properties change.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Event to fire if property changes.
        /// </summary>
        /// <param name="name">Name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
