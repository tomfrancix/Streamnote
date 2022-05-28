using System;
using Amazon.S3.Model;

namespace Streamnote.Relational.Models
{
    /// <summary>
    /// The step class.
    /// </summary>
    public class TaskComment
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Content { get; set; }
        public ApplicationUser User { get; set; }

        public virtual TaskItem Task { get; set; }
    }
}
