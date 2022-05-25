using System;

namespace streamnote.Models
{
    /// <summary>
    /// The message class.
    /// </summary>
    public class Message
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Text { get; set; }
        public virtual string Content { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string ImageContentType { get; set; }
        public virtual bool IsPublic { get; set; }
        public virtual bool MessageSeen { get; set; }

        public ApplicationUser SentBy { get; set; }

        public ApplicationUser SentTo { get; set; }
    }
}
