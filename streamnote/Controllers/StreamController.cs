using Microsoft.AspNetCore.Mvc;
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
using streamnote.Repository.Interface;

namespace streamnote.Controllers
{
    /// <summary>
    /// Controller for the homepage.
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ItemMapper ItemMapper;
        private readonly TopicMapper TopicMapper;
        private readonly IItemRepository ItemRepository;
        private readonly ITopicRepository TopicRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="itemMapper"></param>
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ItemMapper itemMapper, TopicMapper topicMapper, IItemRepository itemRepository, ITopicRepository topicRepository)
        {
            Context = context;
            UserManager = userManager;
            ItemMapper = itemMapper;
            TopicMapper = topicMapper;
            ItemRepository = itemRepository;
            TopicRepository = topicRepository;
        }

        /// <summary>
        /// Get the main stream (homepage).
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index(string? topic)
        {
            var user = await UserManager.GetUserAsync(User);

            var items = ItemRepository.QueryAllItems().Where(i => i.IsPublic);

            if (topic is { Length: > 0 })
            {
                items = ItemRepository.FilterItemsByTopic(items, topic);
            }

            var model = items
                .OrderByDescending(i => i.Id)
                .ToList();

            var topics = TopicRepository.QueryAllTopics()
                .OrderBy(t => t.Name)
                .ToList();

            var streamDescriptor = new StreamDescriptor
            {
                Items = ItemMapper.MapDescriptors(model, user.Id),
                Topics = TopicMapper.MapDescriptors(topics, user.Id)
            };

            return View(streamDescriptor);
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
