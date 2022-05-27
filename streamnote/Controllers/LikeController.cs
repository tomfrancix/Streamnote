using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streamnote.Relational.Data;
using Streamnote.Relational.Models;

namespace Streamnote.Web.Controllers
{
    /// <summary>
    /// Controller for like actions.
    /// </summary>
    public class LikeController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;

        public LikeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            UserManager = userManager;
            Context = context;
        }

        /// <summary>
        /// Create a like.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Create(int itemId)
        {
            var user = await UserManager.GetUserAsync(User);

            var item = Context.Items
                .Include(c => c.Likes).ThenInclude(l => l.User)
                .FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                var like = new Like
                {
                    User = user,
                    Item = item
                };

                item.LikeCount = Context.Likes.Where(c => c.Item.Id == itemId).ToList().Count;

                var exists = item.Likes.Any(l => l.User.Id == user.Id);
                                                           
                if (!exists)
                {
                    Context.Add(like);
                    item.LikeCount++;
                }


                await Context.SaveChangesAsync();

                return new JsonResult(item.LikeCount);
            }

            return new JsonResult(0);
        }

        /// <summary>
        /// Delete a like.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Delete(int itemId)
        {
            var user = await UserManager.GetUserAsync(User);

            var item = Context.Items
                .Include(c => c.Likes)
                .FirstOrDefault(i => i.Id == itemId);

            if (item != null)
            {
                var like = item.Likes.FirstOrDefault(c => c.Item.Id == itemId);

                if (like != null)
                {
                    Context.Remove(like);

                    item.LikeCount = Context.Likes.Where(c => c.Item.Id == itemId).ToList().Count - 1;

                    await Context.SaveChangesAsync();
                }
                                               
                return new JsonResult(item.LikeCount);
            }
                      
            return new JsonResult(0);
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
