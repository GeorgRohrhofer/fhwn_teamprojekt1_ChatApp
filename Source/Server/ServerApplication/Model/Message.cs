using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication.Model
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int message_id { get; set; }
        
        [Required]
        public string message_text { get; set; }

        [Required]
        public DateTime message_timestamp { get; set; }

        public int? user_id { get; set; }
        [ForeignKey("user_id")]
        public User user { get; set; }

        public int? group_id { get; set; }
        [ForeignKey("group_id")]
        public UserGroup group { get; set; }
    }
}
