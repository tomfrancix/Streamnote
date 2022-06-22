using System.Threading.Tasks;

namespace Streamnote.Relational.Interfaces.Services
{
    public interface IDataCullingService
    {
        bool DeleteProject(int id);
        bool DeleteTask(int id);
        bool DeleteTaskComment(int id);
        bool DeleteStep(int id);
        Task<bool> DeleteItem(int id);
        Task<bool> DeleteComment(int id);
    }
}
