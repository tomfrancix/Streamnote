using System.Linq;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using Streamnote.Relational.Data;
using Streamnote.Web.Mapper;

namespace Streamnote.Web.Controllers
{
    /// <summary>
    /// Controller for user actions.
    /// </summary>
    public class GeneralController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserMapper UserMapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userMapper"></param>
        public GeneralController(ApplicationDbContext context, UserMapper userMapper)
        {
            Context = context;
            UserMapper = userMapper;
        }

        /// <summary>
        /// Check if the username is available.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult IsUserNameAvailable(string userName)
        {
            var usernames = Context.Users.Where(u => u.UserName == userName).ToList();

            return Json(usernames.Any());
        }

        /// <summary>
        /// Get the users using the search function.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetUsers(string query)
        {
            var usernames = Context.Users.Where(u => u.UserName.ToLower().Contains(query.ToLower())).ToList();

            return PartialView("_UserSearchResult", UserMapper.MapDescriptors(usernames));
        }
    }
}
