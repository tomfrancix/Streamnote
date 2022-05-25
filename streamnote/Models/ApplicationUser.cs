using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace streamnote.Models
{
    /// <summary>
    /// The application user class.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public virtual byte[] Image { get; set; }
        public virtual string ImageContentType { get; set; }
        public virtual bool DarkMode { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Dob { get; set; }
        public virtual string Bio { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string ConnectionId { get; set; }


        public List<Item> Items { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
    }
}
