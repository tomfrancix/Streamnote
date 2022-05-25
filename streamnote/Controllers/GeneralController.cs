using System.Linq;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using streamnote.Data;
using streamnote.Mapper;

namespace streamnote.Controllers
{
    public class GeneralController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserMapper UserMapper;

        public GeneralController(ApplicationDbContext context, UserMapper userMapper)
        {
            Context = context;
            UserMapper = userMapper;
        }

        [HttpPost]
        public JsonResult IsUserNameAvailable(string userName)
        {
            var usernames = Context.Users.Where(u => u.UserName == userName).ToList();

            return Json(usernames.Any());
        }

        [HttpPost]
        public ActionResult GetUsers(string query)
        {
            var usernames = Context.Users.Where(u => u.UserName.ToLower().Contains(query.ToLower())).ToList();

            return PartialView("_UserSearchResult", UserMapper.MapDescriptors(usernames));
        }
    }
}
