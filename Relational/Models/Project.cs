using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streamnote.Relational.Models
{
    /// <summary>
    /// The task class.
    /// </summary>
    public class Project
    {
        public Project()
        {
            Tasks = new List<TaskItem>(); 
            Users = new List<ApplicationUser>(); 
        }

        public virtual int Id { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual TodoStatus Status { get; set; }
        public virtual bool IsPublic { get; set; }

        public ApplicationUser CreatedBy { get; set; }

        public string OwnedByUsername { get; set; } 
        public List<TaskItem> Tasks { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
