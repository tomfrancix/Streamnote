using System.Collections.Generic;

namespace Streamnote.Relational.Models
{
    /// <summary>
    /// The topic class.
    /// </summary>
    public class Topic
    {
        public virtual int Id { get; set; } 
        public string Name { get; set; }
        public int ItemCount { get; set; }
        public virtual List<Item> Items { get; set; }
        public virtual List<ApplicationUser> Users { get; set; }
    }
}
