using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Model
{
    public class UserList
    {
        public string? UserId {  get; set; }
        public string? UserName { get; set; }
    }
    public class UserGroupList
    {
     
      
        public string? GroupName { get; set; }

        public List<UserGroup>? Groups { get; set; }
    }
}
