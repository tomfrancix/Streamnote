using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational.Data;
using Streamnote.Web.Mapper;
using Streamnote.Relational.Models;
using Streamnote.Relational.Models.Descriptors;

namespace Streamnote.Web.Controllers
{
    /// <summary>
    /// Controller for profile actions.
    /// </summary>
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly ApplicationDbContext Context;
        private readonly ProfileMapper ProfileMapper;
        private readonly ItemMapper ItemMapper;
        private readonly CommentMapper CommentMapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="context"></param>
        /// <param name="profileMapper"></param>
        /// <param name="itemMapper"></param>
        public ProfileController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ProfileMapper profileMapper, ItemMapper itemMapper, CommentMapper commentMapper)
        {
            UserManager = userManager;
            Context = context;
            ProfileMapper = profileMapper;
            ItemMapper = itemMapper;
            CommentMapper = commentMapper;
        }

        /// <summary>
        /// Get the users profile by username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<IActionResult> View(string username)
        {
            var user = await UserManager.GetUserAsync(User);

            var userInfo = Context.Users.FirstOrDefault(u => u.UserName == username);

            var profile = ProfileMapper.MapDescriptor(userInfo, user.UserName);

            profile.Posts = ItemMapper.MapDescriptors(Context.Items
                .Include(u => u.User)
                .Include(l => l.Likes).ThenInclude(l => l.User)
                .Include(i => i.Topics).ThenInclude(t => t.Users)
                .Where(i => i.User.UserName == username)
                .OrderByDescending(i => i.Id)
                .ToList(), user.Id);

            profile.Images = new List<ImageDescriptor>();

            foreach (var post in profile.Posts)
            {
                if (post.Image != null && post.ImageContentType != null)
                {
                    profile.Images.Add(new ImageDescriptor()
                    {
                        UserImage = post.Image,
                        UserImageContentType = post.ImageContentType
                    });
                }
            }

            profile.Comments = CommentMapper.MapDescriptors(Context.Comments.Where(u => u.User.UserName == username).OrderByDescending(c => c.Id).Take(5).ToList(), user.Id);

            return View(profile);
        }
    }
}
