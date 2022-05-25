using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using streamnote.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using streamnote.Data;
using streamnote.Mapper;
using streamnote.Models.Descriptors;

namespace streamnote.Controllers
{
    /// <summary>
    /// Controller for the homepage.
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> Logger;
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ItemMapper ItemMapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="itemMapper"></param>
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, ItemMapper itemMapper)
        {
            Context = context;
            UserManager = userManager;
            ItemMapper = itemMapper;
            Logger = logger;
        }

        /// <summary>
        /// Get the main stream (homepage).
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var user = await UserManager.GetUserAsync(User);

            var model = Context.Items
                .Where(i => i.IsPublic)
                .Include(b => b.User)
                .Include(b => b.Likes).ThenInclude(l => l.User)
                .Where(u => u.User != null)
                .OrderByDescending(i => i.Id)
                .ToList();

            var items = ItemMapper.MapDescriptors(model, user.Id);

            var descriptor = new StreamDescriptor()
            {
                Items = items
            };
            return View(descriptor);
        }

        /// <summary>
        /// Get the privacy information.
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Error page...
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
