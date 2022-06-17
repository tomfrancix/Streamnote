using System;
using System.Collections.Generic;

namespace Streamnote.Relational.Models
{
    /// <summary>
    /// The item class.
    /// </summary>
    public class ItemImage
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Name { get; set; }
        public virtual string FullS3Location { get; set; }
        public virtual byte[] Bytes { get; set; }
        public virtual string ImageContentType { get; set; }

        public Item Item { get; set; }
    }
}
