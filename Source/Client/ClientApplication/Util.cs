using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    /// <summary>
    /// Utility class for copying ObservableCollections.
    /// </summary>
    public static class Util
    {

        /// <summary>
        /// Copy ObservableCollection holding Users.
        /// </summary>
        /// <param name="source">Source collection.</param>
        /// <returns>Return newly created Collection.</returns>
        public static ObservableCollection<User> CopyObservableUserCollection(ObservableCollection<User> source)
        {
            ObservableCollection<User> dest = new();
            foreach (User user in source)
            {
                dest.Add(new User(user.Name)
                {
                    Id = user.Id,
                    IsAdmin = user.IsAdmin,
                    IsSelected = user.IsSelected,
                    IsSearched = user.IsSearched,
                });
            }

            return dest;
        }

        /// <summary>
        /// Copy ObservableCollection holding Chats.
        /// </summary>
        /// <param name="source">Source collection.</param>
        /// <returns>Return newly created Collection.</returns>
        public static ObservableCollection<Chat> CopyObservableChatCollection(ObservableCollection<Chat> source)
        {
            return new ObservableCollection<Chat>(source);
        }
    }
}
