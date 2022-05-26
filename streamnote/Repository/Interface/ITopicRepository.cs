using System.Linq;
using System.Threading.Tasks;
using streamnote.Models;

namespace streamnote.Repository.Interface
{
    public interface ITopicRepository
    {
        IQueryable<Topic> QueryAllTopics();

        Topic QueryExistingTopic(string name);

        Task IncrementItemCount(Topic topic);
        Task<Topic> CreateTopic(Topic topic);
    }
}
