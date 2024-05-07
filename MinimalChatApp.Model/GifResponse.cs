using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalChatApp.Model
{
    public class GifResponse
    {
        public List<Gif>? Data { get; set; }
    }
    public class Gif 
    {
        public string? Url { get; set; }
    }
}
