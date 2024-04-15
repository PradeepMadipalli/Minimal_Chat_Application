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
        }
        public ICollection<Message> Messages { get; set; }
    }
}
