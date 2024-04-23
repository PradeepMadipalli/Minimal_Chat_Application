using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Model
{
    public class UserGroupConnection
    {
        public string? User { get; set; }
        public string? Group { get; set; }

        public string? senderId { get; set; }
        
    }
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? GroupId { get; set; }
        public string? GroupName { get; set;}

        public ICollection<UserGroup> Groups { get; set;}
        public ICollection<Message> messages { get; set;}
    }


    public class UserGroup
    {
        public string? Id { get; set;}
        public virtual AppUser User { get; set; }

        public Guid? GroupId { get; set; }

        public virtual Group Group { get; set; }

    }
}
