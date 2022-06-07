using System.Linq;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Interfaces.Repositories
{
    public interface IStepRepository
    {
        IQueryable<Step> QueryAllSteps();
    }
}
