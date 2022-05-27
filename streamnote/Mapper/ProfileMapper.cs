using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Mapper
{
    /// <summary>
    /// Mapper for the user profile.
    /// </summary>
    public class ProfileMapper
    {
        /// <summary>
        /// Map a user profile from username.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ProfileDescriptor MapDescriptor(ApplicationUser profile, string userName)
        {
            return new ProfileDescriptor
            {
                Bio = profile.Bio,
                Dob = profile.Dob,
                UserFullName = profile.FirstName + " " + profile.LastName, 
                UserImage = profile.Image,
                UserImageContentType = profile.ImageContentType,
                UserName = profile.UserName,
                IsSignedInUser = profile.UserName == userName
            };
        }
    }
}
