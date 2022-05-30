using System;
using System.Collections.Generic;

namespace Streamnote.Relational.Models.Descriptors
{
    public class TaskDescriptor
    {
        public TaskDescriptor()
        {                                 
            Steps = new List<StepDescriptor>();
            Comments = new List<TaskCommentDescriptor>();
        }

        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual TodoStatus Status { get; set; }
        public virtual bool IsPublic { get; set; }

        public ApplicationUser CreatedBy { get; set; }
        public string TaskIdentifier { get; set; }
        public string TaskBoxIdentifier { get; set; }
        public string StepsIdentifier { get; set; }
        public string CommentsIdentifier { get; set; }
        public string EditDescriptionIdentifier { get; set; }
        public string TaskTabIdentifier { get; set; }
        public int Rank { get; set; }
        public string Color { get; set; }

        public string OwnedByUsername { get; set; } 
        public List<StepDescriptor> Steps { get; set; }
        public List<TaskCommentDescriptor> Comments { get; set; }
        public int ProjectId { get; set; }
    }
}
