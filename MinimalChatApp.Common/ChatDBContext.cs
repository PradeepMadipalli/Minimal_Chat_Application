using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Model;


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
                .HasForeignKey(d => d.UserId);
        }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Logs> Logs { get; set; }

        public DbSet<ErrorLogger> ErrorLogs { get; set; }
    }
}
