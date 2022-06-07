using System.Linq;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        IQueryable<TaskItem> QueryAllTasks(ApplicationUser user, int projectId);
    }
}
