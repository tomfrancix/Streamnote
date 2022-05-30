using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streamnote.Relational.Models
{
    /// <summary>
    /// The task class.
    /// </summary>
    public class TaskItem
    {
        public TaskItem()
        {
            Comments = new List<TaskComment>();
            Steps = new List<Step>();
        }

        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual TodoStatus Status { get; set; }
        public virtual bool IsPublic { get; set; }
        public virtual int Rank { get; set; }
        public virtual Project Project { get; set; }

        public ApplicationUser CreatedBy { get; set; }

        public string OwnedByUsername { get; set; } 
        public List<TaskComment> Comments { get; set; }
        public List<Step> Steps { get; set; }
    }
}
