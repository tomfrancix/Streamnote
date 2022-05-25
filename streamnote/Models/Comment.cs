using System;

namespace streamnote.Models
{
    /// <summary>
    /// The comment class.
    /// </summary>
    public class Comment
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Content { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string ImageContentType { get; set; }

        public ApplicationUser User { get; set; }
        public Item Item { get; set; }
    }
}
