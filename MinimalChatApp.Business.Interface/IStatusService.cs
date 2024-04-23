using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Business.Interface
{
    public interface IStatusService
    {
        public void SetUserStatus(UserStatus status);
    }
}
