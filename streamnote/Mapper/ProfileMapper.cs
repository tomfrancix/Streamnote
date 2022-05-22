using System.Collections.Generic;
using streamnote.Models;
using streamnote.Models.Descriptors;

namespace streamnote.Mapper
{
    public class ProfileMapper
    {
        public ProfileDescriptor MapDescriptor(ApplicationUser profile, string userName)
        {
            return new ProfileDescriptor
            {
                Bio = profile.Bio,
                Dob = profile.Dob,
                UserImage = profile.Image,
                UserImageContentType = profile.ImageContentType,
                UserName = profile.UserName,
                IsSignedInUser = profile.UserName == userName
            };
        }
    }
}
