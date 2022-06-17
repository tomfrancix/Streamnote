using System;
using System.Collections.Generic;

namespace Streamnote.Relational.Models
{
    /// <summary>
    /// The item class.
    /// </summary>
    public class Item
    {
        public Item()
        {
            Comments = new List<Comment>();
            Likes = new List<Like>();
            Topics = new List<Topic>();
        }

        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string ImageContentType { get; set; }
        public virtual bool IsPublic { get; set; }
        public virtual int CommentCount { get; set; }
        public virtual int ShareCount { get; set; }
        public virtual int LikeCount { get; set; }

        public ApplicationUser User { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
        public List<Topic> Topics { get; set; }

        public List<ItemImage> Images { get; set; }
    }
}