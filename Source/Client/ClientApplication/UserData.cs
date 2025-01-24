namespace ClientApplication
{
    /// <summary>
    /// Class that holds user data to send to the server in the correct format.
    /// </summary>
    public class UserData
    {
        /// <summary>
        /// Constructor for the user data instance.
        /// </summary>
        /// <param name="name">Username of the User.</param>
        /// <param name="email">Email of the User.</param>
        /// <param name="password">Password of the User.</param>
        public UserData(string name="",string email="",string password = "")
        {
            this.Email = email;
            this.Name = name;
            this.Password = password;
        }

        /// <summary>
        /// Name property for the user.
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// Email property for the user.
        /// </summary>
        public string Email
        {
            get;
            set;
        }
        /// <summary>
        /// Password property for the user.
        /// </summary>
        public string Password
        {
            get;
            set;
        }
    }
}