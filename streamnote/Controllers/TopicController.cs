using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational.Data;
using Streamnote.Web.Mapper;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Controllers
{
    /// <summary>
    /// Controller for topic actions.
    /// </summary>
    public class TopicController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly TopicMapper TopicMapper;

        /// <summary>
        /// Constructor for topics.
        /// </summary>
        /// <param name="context"></param>
        public TopicController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, TopicMapper topicMapper)
        {
            Context = context;
            UserManager = userManager;
            TopicMapper = topicMapper;
        }

        /// <summary>
        /// Get topics from query string.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTopics(string query)
        {
            var topics = Context.Topics.Include(t => t.Users).Where(t => t.Name.Contains(query)).ToList();

            if (topics.Count < 1)
            {
                topics = new List<Topic>()
                {
                    new()
                    {
                        Name = query,
                        ItemCount = 0,
                        Users = new List<ApplicationUser>()
                    }
                };
            }

            return PartialView("_Topic", TopicMapper.MapDescriptors(topics, UserManager.GetUserId(User)));
        }

        /// <summary>
        /// Get topics from query string.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SearchTopics(string query)
        {
            var topics = Context.Topics.Include(t => t.Users).Where(t => t.Name.Contains(query)).ToList();

            var loggedInUser = await UserManager.GetUserAsync(User);

            if (topics.Count < 1)
            {
                topics = new List<Topic>()
                {
                    new ()
                    {
                        Name = query,
                        ItemCount = 0
                    }
                };
            }

            var topicDescriptor = TopicMapper.MapDescriptors(topics, loggedInUser.Id);

            return PartialView("_TopicStream", topicDescriptor);
        }

        /// <summary>
        /// Follow a topic.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> FollowTopic(string topicName)
        {
            var topic = Context.Topics
                .Include(t => t.Users)
                .FirstOrDefault(t => t.Name == topicName.ToLower());

            var topicDescriptor = new List<TopicDescriptor>();

            if (topic != null)
            {
                var loggedInUser = await UserManager.GetUserAsync(User);

                topic.Users.Add(loggedInUser);

                Context.Update(topic);
                await Context.SaveChangesAsync();

                topicDescriptor = TopicMapper.MapDescriptors(
                    Context.Topics
                        .Include(t => t.Users)
                        .Where(t => t.Users.Any(u => u.Id == loggedInUser.Id))
                        .ToList(), loggedInUser.Id);
            }

            return PartialView("_TopicStream", topicDescriptor);
        }

        /// <summary>
        /// Follow a topic.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> UnFollowTopic(string topicName)
        {
            var topic = Context.Topics
                .Include(t => t.Users)
                .FirstOrDefault(t => t.Name == topicName.ToLower());

            var topicDescriptor = new List<TopicDescriptor>();

            if (topic != null)
            {
                var loggedInUser = await UserManager.GetUserAsync(User);

                topic.Users.Remove(loggedInUser);

                Context.Update(topic);
                await Context.SaveChangesAsync();
                
                topicDescriptor = TopicMapper.MapDescriptors(
                    Context.Topics
                        .Include(t => t.Users)
                        .Where(t => t.Users.Any(u => u.Id == loggedInUser.Id))
                        .ToList(), loggedInUser.Id);
            }

            return PartialView("_TopicStream", topicDescriptor);
        }

        /*// POST: TopicController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
           
        }*/
    }
}
