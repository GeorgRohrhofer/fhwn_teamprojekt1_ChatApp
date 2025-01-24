using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public class UserData
    {
        public UserData(string name, string email, string password) 
        { 
            this.Name = name;
            this.Email = email;
            this.Password = password;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
