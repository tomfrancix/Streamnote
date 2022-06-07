using LightInject;
using Streamnote.Relational.Interfaces.Repositories;
using Streamnote.Relational.Repositories;

namespace Streamnote.Relational.Installers
{
    public class RepositoryCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry registry)
        {
            registry.Register<IItemRepository, ItemRepository>();
            registry.Register<ITopicRepository, TopicRepository>();
            registry.Register<IProjectRepository, ProjectRepository>();
            registry.Register<ITaskRepository, TaskRepository>();
            registry.Register<IStepRepository, StepRepository>();
            registry.Register<ITaskCommentRepository, TaskCommentRepository>();
        }
    }
}
