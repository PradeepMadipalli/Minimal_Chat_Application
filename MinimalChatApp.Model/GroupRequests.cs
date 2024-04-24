using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Model
{
    public class GroupRequests
    {
    }
    public class UserRoomRequests
    {
        public string? UserId { get; set; }
        public Guid? GroupId { get; set; }

        public int ? Status { get; set; }
    }
}
