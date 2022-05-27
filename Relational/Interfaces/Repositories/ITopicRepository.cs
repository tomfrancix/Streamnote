using System.Linq;
using System.Threading.Tasks;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Interfaces.Repositories
{
    public interface ITopicRepository
    {
        IQueryable<Topic> QueryAllTopics();

        Topic QueryExistingTopic(string name);

        Task IncrementItemCount(Topic topic);
        Task<Topic> CreateTopic(Topic topic);
    }
}
