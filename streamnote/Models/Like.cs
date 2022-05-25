namespace streamnote.Models
{
    /// <summary>
    /// The like class.
    /// </summary>
    public class Like
    {
        public virtual int Id { get; set; }

        public ApplicationUser User { get; set; }
        public Item Item { get; set; }
    }
}
