using Microsoft.AspNetCore.Mvc;
using Streamnote.Relational.Interfaces.Services;

namespace Streamnote.Web.Controllers
{
    public class DeleteController : Controller
    {
        private readonly IDataCullingService DataCullingService;

        public DeleteController(IDataCullingService dataCullingService)
        {
            DataCullingService = dataCullingService;
        }

        /// <summary>
        /// Delete a task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public void DeleteTask(int id)
        {
            DataCullingService.DeleteTask(id);
        }

        /// <summary>
        /// Delete a task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public void DeleteTaskComment(int id)
        {
            DataCullingService.DeleteTaskComment(id);
        }

        /// <summary>
        /// Delete a task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public void DeleteStep(int id)
        {
            DataCullingService.DeleteStep(id);
        }

        /// <summary>
        /// Delete a task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public void DeleteItem(int id)
        {
            DataCullingService.DeleteItem(id);
        }

        /// <summary>
        /// Delete a task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public void DeleteComment(int id)
        {
            DataCullingService.DeleteComment(id);
        }
    }
}
