using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using streamnote.Data;

using streamnote.Models;

namespace streamnote.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext Context;
        private readonly UserManager<ApplicationUser> UserManager;

        public MessagesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            Context = context;
            UserManager = userManager;
        }

        // GET: Messages
        public async Task<IActionResult> Index(string username)
        {
            var loggedInUser = await UserManager.GetUserAsync(User);

            ViewBag.MessageToUserName = username;

            return View(await Context.Messages
                .Include(m => m.SentBy)
                .Include(m => m.SentTo)
                .Where(m => (m.SentBy.UserName == username && m.SentTo.UserName == loggedInUser.UserName) ||
                            (m.SentTo.UserName == username && m.SentBy.UserName == loggedInUser.UserName))
                .ToListAsync());
        }

        // GET: Messages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(string username, string text)
        {
            var model = new Message()
            {
                Text = text,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                SentBy = await UserManager.GetUserAsync(User),
                SentTo = Context.Users.FirstOrDefault(u => u.UserName == username)
            };

            if (ModelState.IsValid)
            {

                Context.Add(model);
                await Context.SaveChangesAsync();
                return PartialView("_Message", model);
            }

            throw new Exception("Model state was not valid!");
        }

        // GET: Messages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await Context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Created,Modified,Text,Content,Image,ImageContentType,IsPublic")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Context.Update(message);
                    await Context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(message);
        }

        // GET: Messages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await Context.Messages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var message = await Context.Messages.FindAsync(id);
            Context.Messages.Remove(message);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
            return Context.Messages.Any(e => e.Id == id);
        }
    }
}
