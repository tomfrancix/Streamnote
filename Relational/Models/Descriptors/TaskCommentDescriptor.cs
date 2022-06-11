using System;

namespace Streamnote.Relational.Models.Descriptors
{
    public class TaskCommentDescriptor
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Content { get; set; }
        public virtual int TaskId { get; set; }
        public virtual bool CreatedByLoggedInUser { get; set; }
        public string TaskCommentIdentifier { get; set; }

        public string UserName { get; set; }
        public string UserImageContentType { get; set; }
        public byte[] UserImage { get; set; }
    }
}
