using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Model;
using System.Linq;
using System.Reflection.Emit;


namespace MinimalChatApplication.Model
{
    public class ChatDBContext : IdentityDbContext
    {
        public ChatDBContext(DbContextOptions<ChatDBContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Message>()
                .HasOne(a => a.sender)
                .WithMany(d => d.Messages)
                .HasForeignKey(d => d.SenderId);
            builder.Entity<UserGroup>().
             HasOne(a => a.User)
                 .WithMany(d => d.Groups)
                 .HasForeignKey(d => d.UserId);
            builder.Entity<UserGroup>().
                HasOne(a => a.Group)
                .WithMany(a => a.Groups)
                .HasForeignKey(a => a.GroupId);
            builder.Entity<UserGroup>()
            .Property(f => f.GId)
            .ValueGeneratedOnAdd();
            builder.Entity<UserStatuss>().HasKey(u => u.Id);
            builder.Entity<ProfilePhoto>().HasKey(p => p.Id);

        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Logs> Logs { get; set; }

        public DbSet<ErrorLogger> ErrorLogs { get; set; }

        public DbSet<Group> Group { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserStatuss> UserStatus { get; set; }

        public DbSet<OnlineStatus> OnlineStatus { get; set; }
        public DbSet<ProfilePhoto> ProfilePhoto { get; set; }
    }
}
