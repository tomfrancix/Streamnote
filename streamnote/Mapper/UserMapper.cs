using System.Collections.Generic;
using streamnote.Models;
using streamnote.Models.Descriptors;

namespace streamnote.Mapper
{
    public class UserMapper
    {
        public List<UserNameDescriptor> MapDescriptors(List<ApplicationUser> users)
        {
            var descriptor = new List<UserNameDescriptor>();

            foreach (var user in users)
            {
                descriptor.Add(MapDescriptor(user));
            }

            return descriptor;
        }
        public UserNameDescriptor MapDescriptor(ApplicationUser profile)
        {
            return new UserNameDescriptor
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                UserImage = profile.Image,
                UserImageContentType = profile.ImageContentType,
                UserName = profile.UserName
            };
        }
    }
}
