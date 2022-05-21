using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace streamnote.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual byte[] Image { get; set; }
        public virtual string ImageContentType { get; set; }
        public virtual bool DarkMode { get; set; }


        public List<Item> Items { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
