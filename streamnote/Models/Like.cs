using System;

namespace streamnote.Models
{
    public class Like
    {
        public virtual int Id { get; set; }

        public ApplicationUser User { get; set; }
        public Item Item { get; set; }
    }
}
