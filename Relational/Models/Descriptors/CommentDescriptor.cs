using System;

namespace Streamnote.Relational.Models.Descriptors
{
    public class CommentDescriptor
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string ImageContentType { get; set; }
        public virtual bool CreatedByLoggedInUser { get; set; }
        public string CommentIdentifier { get; set; }

        public string UserName { get; set; }
        public string UserImageContentType { get; set; }
        public byte[] UserImage { get; set; }
    }
}
