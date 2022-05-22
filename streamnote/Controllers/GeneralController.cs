using System.Linq;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using streamnote.Data;

namespace streamnote.Controllers
{
    public class GeneralController : Controller
    {
        private readonly ApplicationDbContext Context;

        public GeneralController(ApplicationDbContext context)
        {
            Context = context;
        }

        [HttpPost]
        public JsonResult IsUserNameAvailable(string userName)
        {
            var usernames = Context.Users.Where(u => u.UserName == userName).ToList();

            return Json(usernames.Any());
        }
    }
}
