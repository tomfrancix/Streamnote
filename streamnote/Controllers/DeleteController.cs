using System.Linq;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Streamnote.Relational.Data;
using Streamnote.Relational.Interfaces.Services;
using Streamnote.Relational.Models;

namespace Streamnote.Web.Controllers
{
    public class DeleteController : Controller
    {
        private readonly IDataCullingService DataCullingService;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ApplicationDbContext Context;

        public DeleteController(IDataCullingService dataCullingService, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            DataCullingService = dataCullingService;
            UserManager = userManager;
            Context = context;
        }

        /// <summary>
        /// Delete a project.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public void DeleteProject(int id)
        {
            DataCullingService.DeleteProject(id);
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
        public async Task DeleteItem(int id)
        {
            await DataCullingService.DeleteItem(id);
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

        /// <summary>
        /// Delete a task.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAll(int id)
        {
            var user = await UserManager.GetUserAsync(User);
            var draftPostIds = Context.Items.Where(i => i.User.Id == user.Id && !i.IsPublic).Select(i => i.Id).ToList();

            foreach (var postId in draftPostIds)
            {
                await DeleteItem(postId);
            }

            return true;
        }
    }
}
