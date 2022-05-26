using System.Collections.Generic;
using streamnote.Models;
using streamnote.Models.Descriptors;

namespace streamnote.Mapper
{
    /// <summary>
    /// Mapper for username partial.
    /// </summary>
    public class UserMapper
    {
        /// <summary>
        /// Map a list of users.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public List<UserNameDescriptor> MapDescriptors(List<ApplicationUser> users)
        {
            var descriptor = new List<UserNameDescriptor>();

            foreach (var user in users)
            {
                descriptor.Add(MapDescriptor(user));
            }

            return descriptor;
        }

        /// <summary>
        /// Map a single user.
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public UserNameDescriptor MapDescriptor(ApplicationUser profile)
        {
            return new UserNameDescriptor
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                FullName = profile.FirstName + " " + profile.LastName,
                UserImage = profile.Image,
                UserImageContentType = profile.ImageContentType,
                UserName = profile.UserName
            };
        }
    }
}
