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

    }
}
