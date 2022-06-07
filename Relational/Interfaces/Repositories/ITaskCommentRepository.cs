using System.Linq;
using System.Threading.Tasks;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Interfaces.Repositories
{
    public interface ITaskCommentRepository
    {
        IQueryable<TaskComment> QueryAllComments();
    }
}
