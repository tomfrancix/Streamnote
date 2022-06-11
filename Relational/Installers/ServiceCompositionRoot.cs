using LightInject;
using Streamnote.Relational.Interfaces.Services;
using Streamnote.Relational.Service;

namespace Streamnote.Relational.Installers
{
    public class ServiceCompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry registry)
        {
            registry.Register<IS3Service, S3Service>();
            registry.Register<IDataCullingService, DataCullingService>();
        }
    }
}
