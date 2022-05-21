using Microsoft.AspNetCore.Identity;

namespace streamnote.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string ProfilePicture { get; set;}
    }
}
