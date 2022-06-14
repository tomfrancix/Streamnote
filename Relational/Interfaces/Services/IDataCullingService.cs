namespace Streamnote.Relational.Interfaces.Services
{
    public interface IDataCullingService
    {
        bool DeleteProject(int id);
        bool DeleteTask(int id);
        bool DeleteTaskComment(int id);
        bool DeleteStep(int id);
        bool DeleteItem(int id);
        bool DeleteComment(int id);
    }
}
