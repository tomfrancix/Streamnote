using System;
using System.Collections.Generic;

namespace Streamnote.Relational.Models.Descriptors
{
    public class ProjectDescriptor
    {
        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual TodoStatus Status { get; set; }
        public virtual bool IsPublic { get; set; }
        public virtual bool IsCurrentProject { get; set; }

        public ApplicationUser CreatedBy { get; set; }

        public string OwnedByUsername { get; set; }
        public List<TaskDescriptor> Tasks { get; set; }
        public List<UserNameDescriptor> Users { get; set; }

        public List<TodoStatus> StatusOptions { get; set; }
    }
}
