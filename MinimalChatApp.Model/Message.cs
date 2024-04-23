
using Microsoft.AspNetCore.Identity;
using MinimalChatApp.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalChatApplication.Model
{
    public class Message
    {

        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MessageId { get; set; }

        [Required]
        public string? SenderId { get; set; }

        public string? ReceiverId { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public string? groupId { get; set; }
        public virtual Group Group
        { get; set; }
        public virtual AppUser sender { get; set; }


    }
    public class MessageGroup
    {
        public int Id { get; set; }
        public string Content { get; set; }

    }

    public class Groups
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class UserStatus
    {
        public string UserId { get; set; }
        public string StatusMessage { get; set; }
    }

    public class Insertmessage
    {
        public Guid messageId { get; set; }


        public string? senderId { get; set; }

        public string? receiverId { get; set; }
        [Required]
        public string? content { get; set; }
        [Required]
        public DateTime timestamp { get; set; }
    }
}
