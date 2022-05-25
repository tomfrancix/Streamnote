using System;
using System.Collections.Generic;

namespace streamnote.Models.Descriptors
{
    public class StreamDescriptor
    {
        public List<ItemDescriptor> Items { get; set; }
        public List<TopicDescriptor> Topics { get; set; }
    }
}
