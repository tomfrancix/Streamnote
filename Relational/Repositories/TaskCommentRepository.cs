using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational.Data;
using Streamnote.Relational.Interfaces;
using Streamnote.Relational.Interfaces.Repositories;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Repositories
{
    /// <summary>
    /// Repository class for database calls.
    /// </summary>
    public class TaskCommentRepository : ITaskCommentRepository
    {
        private readonly ApplicationDbContext Context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        public TaskCommentRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Get all the items.
        /// </summary>                 
        /// <returns></returns>
        public IQueryable<TaskComment> QueryAllComments()
        {
            return Context.TaskComments;
        }
    }
}
