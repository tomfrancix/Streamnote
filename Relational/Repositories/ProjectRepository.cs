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
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext Context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        public ProjectRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Get all the items.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Project> QueryAllProjects(ApplicationUser user)
        {
            var projects = Context.Projects
                .Include(t => t.Tasks)
                .Include(p => p.Users)
                .Include(p => p.CreatedBy)
                .Where(p => 
                    p.Users.Select(u => u.Id).Contains(user.Id) || 
                    p.CreatedBy.Id == user.Id);

            return projects;
        }
    }
}
