using System;
using System.Collections.Generic;

namespace streamnote.Models.Descriptors
{
    public class ProfileDescriptor
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Bio { get; set; }
        public virtual DateTime Dob { get; set; }

        public string UserName { get; set; }
        public string UserImageContentType { get; set; }
        public byte[] UserImage { get; set; }

        public bool IsSignedInUser { get; set; }

        public List<ItemDescriptor> Posts { get; set; }
        public List<ImageDescriptor> Images { get; set; }
        public List<CommentDescriptor> Comments { get; set; }
    }
}
