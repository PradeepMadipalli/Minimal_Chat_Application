using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Business.Interface
{
    public interface IGroupServices
    {
        Task<Group> CreateGroup(RequestGroup groupp);
        Task<List<GetGroups>> GetGetGroups();
        Task<Group> JoinGroup(RequestGroup groupp);
        Task<List<UserGroup>> GetGroupOfUsers(GroupUserRequest groupId);
        //Task<AppUser> GetUser(string request);
        Task<List<OnlineStatus>> GetStsatus();
        Task<AppUser> UpdateStatuss(UpdateStatus status, string userid);
        Task<List<UserGroup>> DeleteUserfromGroup(DeleteUsersFromGroup request);
        Task<Group> EdituserGroup(EditGroupName request);

    }
}
