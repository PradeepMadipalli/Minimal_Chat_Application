
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
      
        public string? UserId { get; set;}

        public virtual AppUser sender { get; set; }

    }
}
