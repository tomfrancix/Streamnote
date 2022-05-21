using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace streamnote.Models
{
    public class Item
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Title { get; set; }
        public virtual string Content { get; set; }
        public virtual byte[] Image { get; set; }
        public virtual string ImageContentType { get; set; }
        public virtual bool IsPublic { get; set; }
        public virtual int CommentCount { get; set; }

        public ApplicationUser User { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
