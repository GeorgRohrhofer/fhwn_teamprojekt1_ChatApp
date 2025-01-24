using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public class Users
    {
        public Users(int id, string name, Socket socket) 
        {
            this.Id = id;
            this.Name = name;
            this.Socket = socket;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Socket Socket { get; set; }
    }
}
