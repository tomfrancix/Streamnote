namespace Streamnote.Relational.Interfaces.Services
{
    public interface IDataCullingService
    {
        public bool DeleteTask(int id);
        public bool DeleteTaskComment(int id);
        public bool DeleteStep(int id);
        public bool DeleteItem(int id);
        public bool DeleteComment(int id);
    }
}
