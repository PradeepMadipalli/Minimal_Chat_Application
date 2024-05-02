using Microsoft.AspNetCore.Identity;
using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Model
{
    public class AppUser :IdentityUser
    {
        public AppUser()
        {
            Messages = new HashSet<Message>();
            Groups = new HashSet<UserGroup>();
        }
        public int OnlineStatus { get; set; }
        public ICollection<Message> Messages { get; set; }

        public ICollection<UserGroup> Groups { get; set; }
    }
}
