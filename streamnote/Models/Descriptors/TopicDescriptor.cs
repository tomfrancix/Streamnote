using System;

namespace streamnote.Models.Descriptors
{
    public class TopicDescriptor
    {
        public virtual int Id { get; set; }
        public string Name { get; set; }
        public int ItemCount { get; set; }
        public bool UserFollowsTopic { get; set; }
        public string TopicElementId { get; set; }
    }
}
