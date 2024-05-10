using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Model
{
    public class UpdateStatus
    {
        public string Status { get; set; }
    }
    public class UpdateShowOptions
    {
        public string? noofdays { get; set; }
        public string? messageId { get; set; }
    }
}

