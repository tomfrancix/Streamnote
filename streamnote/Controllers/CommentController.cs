using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using streamnote.Data;
using streamnote.Mapper;
using streamnote.Models;
using ActionResult = Microsoft.AspNetCore.Mvc.ActionResult;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace streamnote.Controllers
{
    /// <summary>
    /// Controller for comments.
    /// </summary>
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly CommentMapper CommentMapper;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="context"></param>
        /// <param name="commentMapper"></param>
        public CommentController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, CommentMapper commentMapper)
        {
            UserManager = userManager;
            Context = context;
            CommentMapper = commentMapper;
        }

        /// <summary>
        /// Create action.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        /// <exception cref="AccessViolationException"></exception>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<ActionResult> Create(string content, int itemId)
        {
            var user = await UserManager.GetUserAsync(User);

            var item = Context.Items
                .Include(c => c.Comments).ThenInclude(c => c.User)
                .FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                var comment = new Comment
                {
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow,
                    Content = content,
                    Image = null,
                    ImageContentType = null,
                    User = user,
                    Item = item
                };

                var existingCommentsByUser = item.Comments.Where(u => u.User.Id == user.Id).ToList();

                if (existingCommentsByUser.Any())
                {
                    var recentCommentByUser = existingCommentsByUser.OrderByDescending(c => c.Created).FirstOrDefault();
                    if (recentCommentByUser != null && recentCommentByUser.Created > DateTime.UtcNow.AddSeconds(-2))
                    {
                        throw new AccessViolationException("Subsequent comments posted to fast.");
                    }
                }

                Context.Add(comment);

                item.CommentCount = Context.Comments.Where(c => c.Item.Id == itemId).ToList().Count + 1;

                await Context.SaveChangesAsync();

                var commentDescriptor = CommentMapper.MapDescriptor(comment, user.Id);

                return PartialView("_Comment", commentDescriptor);
            }

            throw new AccessViolationException("This item does not exist.");
        }

        /*
        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
    }
}
