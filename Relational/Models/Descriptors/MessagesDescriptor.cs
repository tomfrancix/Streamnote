using System;
using System.Collections.Generic;

namespace Streamnote.Relational.Models.Descriptors
{
    public class MessagesDescriptor
    {
        public virtual int Id { get; set; }
        public List<Message> Messages { get; set; }
        public List<UserNameDescriptor> Users { get; set; }
    }
}
