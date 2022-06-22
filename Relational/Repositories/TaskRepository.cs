using System.Linq;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational.Data;
using Streamnote.Relational.Interfaces.Repositories;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Repositories
{
    /// <summary>
    /// Repository class for database calls.
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext Context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        public TaskRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Get all the items.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TaskItem> QueryAllTasks(ApplicationUser user, int projectId)
        {
            return Context.Tasks
                .Include(t => t.Project)
                .Include(t => t.Steps)
                .Include(t => t.Comments).ThenInclude(c => c.User)
                .Include(t => t.CreatedBy)
                .Where(t => t.Project.Id == projectId);
        }
    }
}
