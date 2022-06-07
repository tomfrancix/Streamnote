using System.Linq;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        IQueryable<Project> QueryAllProjects(ApplicationUser user);
    }
}
