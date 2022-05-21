using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using streamnote.Data;
using streamnote.Models;

namespace streamnote.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;

        public CommentController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            UserManager = userManager;
            Context = context;
        }

        // POST: CommentController/Create
        [HttpPost]
        public async Task<JsonResult> Create(string content, int itemId)
        {
            var user = await UserManager.GetUserAsync(User);

            var item = Context.Items
                .Include(c => c.Comments)
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

                Context.Add(comment);

                item.CommentCount = Context.Comments.Where(c => c.Item.Id == itemId).ToList().Count + 1;

                await Context.SaveChangesAsync();
            }

            return new JsonResult(true);
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
