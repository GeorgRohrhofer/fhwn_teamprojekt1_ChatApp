using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int user_id { get; set; }

        [Required]
        public string username { get; set; }

        [Required]
        public byte[] password { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public bool is_banned { get; set; }

        [Required]
        public bool reset_pw { get; set; } = false;

        [Required]
        public byte[] salt { get; set; }

        public int? privilege_id { get; set; }
        [ForeignKey("privilege_id")]
        public Privilege privilege { get; set; }

        //public ICollection<UserGroupMember> usergroupmembers { get; set; }
        public ICollection<Message> messages { get; set; }

        // ADDITION
        public ICollection<UserGroup> groups { get; set; }
    }
}
