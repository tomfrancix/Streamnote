using System;

namespace Streamnote.Relational.Models.Descriptors
{
    public class StepDescriptor
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Content { get; set; }
        public virtual bool IsCompleted { get; set; }
        public virtual int TaskId { get; set; }
        public string StepIdentifier { get; set; }
    }
}
