using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.Model
{
    public class UserGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int group_id { get; set; }

        [Required]
        public string group_name { get; set; }

        //public ICollection<UserGroupMember> usergroupmembers { get; set; }
        public ICollection<Message> messages { get; set; }

        // ADDITION
        public ICollection<User> users { get; set; }
    }
}