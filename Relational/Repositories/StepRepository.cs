using System.Linq;
using System.Threading.Tasks;
using Streamnote.Relational.Data;
using Streamnote.Relational.Interfaces.Repositories;
using Streamnote.Relational.Models;

namespace Streamnote.Relational.Repositories
{
    /// <summary>
    /// Repository class for database calls.
    /// </summary>
    public class StepRepository : IStepRepository
    {
        private readonly ApplicationDbContext Context;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        public StepRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        /// <summary>
        /// Get all the items.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Step> QueryAllSteps()
        {
            return Context.Steps;
        }
    }
}
