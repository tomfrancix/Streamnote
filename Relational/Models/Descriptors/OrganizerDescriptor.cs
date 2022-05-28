using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Streamnote.Relational.Models.Descriptors
{
    public class OrganizerDescriptor
    {
        public virtual List<ProjectDescriptor> Projects { get; set; }
        public virtual List<TaskDescriptor> Tasks { get; set; }
        public virtual List<TaskDescriptor> YourTasks { get; set; }
        public virtual List<TaskDescriptor> CompletedTasks { get; set; }
        public bool IsViewingProject { get; set; }
        public int ProjectId { get; set; }
    }
}
