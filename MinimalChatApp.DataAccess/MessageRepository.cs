using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MinimalChatApp.DataAccess.Interface;
using MinimalChatApp.Model;
using MinimalChatApplication.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.DataAccess
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatDBContext _context;
        public MessageRepository(ChatDBContext context)
        {
            _context = context;

        }

        public async Task<Message> FindMessage(string MessageId)
        {
            Guid guid;
            try
            {
                guid = Guid.Parse(MessageId);

                Message message = await _context.Messages.FindAsync(guid);
                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<object> getLogs(DateTime? startTime = null, DateTime? endTime = null)
        {
            return await _context.Logs.Where(l => l.CreatedDate >= startTime && l.CreatedDate <= endTime)
                .ToListAsync();

        }

        public async Task<bool> isExistReceiver(string Receverid)
        {
            return await _context.Users.AnyAsync(u => u.Id == Receverid);
        }

        public async Task<bool> isExistGroup(string groupid)
        {
            return await _context.Group.AnyAsync(u => u.GroupId.ToString() == groupid);
        }

        public async Task<Group> insertGruop(string groupname)
        {
            var group = new Group { GroupName = groupname };
            await _context.Group.AddAsync(group);
            await _context.SaveChangesAsync();

            return group;
        }
        public async Task<List<UserGroup>> InsertUserGroup(List<UserGroup> usergroup)
        {
            await _context.UserGroups.AddRangeAsync(usergroup);
            await _context.SaveChangesAsync();
            return usergroup;
        }
        public async Task<Group> FindByGroupName(string groupname)
        {
            return await _context.Group.Where(u => u.GroupName == groupname).SingleAsync();
        }
        public async Task<UserStatuss> UpdateUserStatus(UserStatuss userStatuss)
        {
           await _context.UserStatus.AddAsync(userStatuss);
            await _context.SaveChangesAsync();
            return userStatuss;

        }
        public async Task<List<UserGroup>> GetUserGroup(string UserId)
        {
            return await _context.UserGroups.Where(u => u.UserId == UserId).ToListAsync();
        }

    }
}
